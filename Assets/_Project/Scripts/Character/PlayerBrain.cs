using GreedIsland.Aura;
using GreedIsland.Camera;
using GreedIsland.Core;
using GreedIsland.Stats;
using UnityEngine;

namespace GreedIsland.Character
{
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(PlayerGroundChecker))]
    [RequireComponent(typeof(PlayerJumpController))]
    [RequireComponent(typeof(PlayerDashController))]
    [RequireComponent(typeof(PlayerRotationController))]
    [RequireComponent(typeof(PlayerStatsProvider))]
    public sealed class PlayerBrain : MonoBehaviour, IAbilityUser
    {
        [SerializeField] private PlayerMotor motor;
        [SerializeField] private PlayerGroundChecker groundChecker;
        [SerializeField] private PlayerJumpController jumpController;
        [SerializeField] private PlayerDashController dashController;
        [SerializeField] private PlayerRotationController rotationController;
        [SerializeField] private PlayerAnimatorController animatorController;
        [SerializeField] private PlayerStatsProvider statsProvider;
        [SerializeField] private AuraController auraController;
        [SerializeField] private LockOnTargetResolver lockOnResolver;

        private readonly PlayerMovementStateMachine movementStateMachine = new();

        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool sprintHeld;

        private bool jumpRequested;
        private bool dashRequested;
        private ITargetable lockOnTarget;

        public Transform Origin => transform;
        public AuraController AuraController => auraController;
        public StatBlock Stats => statsProvider != null ? statsProvider.RuntimeStats : null;

        public PlayerMoveState CurrentMoveState => movementStateMachine.CurrentState;
        public bool IsGrounded => groundChecker != null && groundChecker.IsGrounded;

        private void Reset()
        {
            motor = GetComponent<PlayerMotor>();
            groundChecker = GetComponent<PlayerGroundChecker>();
            jumpController = GetComponent<PlayerJumpController>();
            dashController = GetComponent<PlayerDashController>();
            rotationController = GetComponent<PlayerRotationController>();
            statsProvider = GetComponent<PlayerStatsProvider>();
            auraController = GetComponent<AuraController>();
            animatorController = GetComponent<PlayerAnimatorController>();
            lockOnResolver = FindAnyObjectByType<LockOnTargetResolver>();
        }

        private void Awake()
        {
            if (motor == null)
            {
                motor = GetComponent<PlayerMotor>();
            }

            if (groundChecker == null)
            {
                groundChecker = GetComponent<PlayerGroundChecker>();
            }

            if (jumpController == null)
            {
                jumpController = GetComponent<PlayerJumpController>();
            }

            if (dashController == null)
            {
                dashController = GetComponent<PlayerDashController>();
            }

            if (rotationController == null)
            {
                rotationController = GetComponent<PlayerRotationController>();
            }

            if (animatorController == null)
            {
                animatorController = GetComponent<PlayerAnimatorController>();
            }

            if (statsProvider == null)
            {
                statsProvider = GetComponent<PlayerStatsProvider>();
            }

            if (auraController == null)
            {
                auraController = GetComponent<AuraController>();
            }

            if (lockOnResolver == null)
            {
                lockOnResolver = GetComponent<LockOnTargetResolver>();
                if (lockOnResolver == null)
                {
                    lockOnResolver = FindAnyObjectByType<LockOnTargetResolver>();
                }
            }
        }

        private void Update()
        {
            var stats = Stats;
            if (stats == null)
            {
                return;
            }

            groundChecker.Tick();

            var cameraRelative = ResolveCameraRelativeMove(moveInput);
            var inputMagnitude = Mathf.Clamp01(moveInput.magnitude);
            var moveDirection = cameraRelative.sqrMagnitude > 0.0001f ? cameraRelative.normalized : transform.forward;

            var auraMovementMultiplier = auraController != null ? auraController.MovementMultiplier : 1f;
            var moveSpeed = stats.MoveSpeed * auraMovementMultiplier;
            var sprintSpeed = stats.SprintSpeed * auraMovementMultiplier;

            var forwardAlignment = Vector3.Dot(moveDirection, ResolveCameraForward());
            var canSprint = sprintHeld && inputMagnitude > 0.15f && forwardAlignment > 0.25f && groundChecker.IsGrounded;
            var targetSpeed = canSprint ? sprintSpeed : moveSpeed;

            if (dashRequested)
            {
                dashController.Configure(stats.DashSpeed * auraMovementMultiplier, stats.DashDuration, stats.DashCooldown);
                dashController.TryStartDash(moveDirection);
                dashRequested = false;
            }

            if (jumpRequested)
            {
                jumpController.QueueJump();
                jumpRequested = false;
            }

            var verticalVelocity = jumpController.Tick(groundChecker.IsGrounded, stats.Gravity, stats.JumpHeight, !dashController.IsDashing);
            var dashVelocity = dashController.GetDashVelocity();

            motor.TickMotor(
                moveDirection,
                groundChecker.GroundNormal,
                groundChecker.IsGrounded,
                targetSpeed * inputMagnitude,
                stats.Acceleration,
                stats.Deceleration,
                stats.AirControlPercent,
                verticalVelocity,
                dashVelocity);

            var isSprinting = canSprint && !dashController.IsDashing;
            movementStateMachine.Tick(groundChecker.IsGrounded, isSprinting, dashController.IsDashing, motor.HorizontalSpeed, verticalVelocity);

            var rotationTarget = lockOnTarget != null && lockOnTarget.IsTargetable
                ? lockOnTarget.TargetTransform.position - transform.position
                : moveDirection;

            rotationController.Tick(rotationTarget, !dashController.IsDashing);
            animatorController?.Tick(movementStateMachine.CurrentState, motor.HorizontalSpeed, groundChecker.IsGrounded, verticalVelocity, isSprinting);
        }

        public void SetMoveIntent(Vector2 move, Vector2 look, bool sprint)
        {
            moveInput = Vector2.ClampMagnitude(move, 1f);
            lookInput = look;
            sprintHeld = sprint;
        }

        public void RequestJump()
        {
            jumpRequested = true;
        }

        public void RequestDash()
        {
            dashRequested = true;
        }

        public void ToggleLockOn()
        {
            if (lockOnResolver == null)
            {
                return;
            }

            if (lockOnTarget != null)
            {
                lockOnTarget = null;
                return;
            }

            lockOnTarget = lockOnResolver.ResolveBestTarget(transform.position, transform.forward);
        }

        public Vector2 CurrentMoveInput => moveInput;
        public Vector2 CurrentLookInput => lookInput;
        public float CurrentSpeed => motor != null ? motor.HorizontalSpeed : 0f;
        public float VerticalVelocity => jumpController != null ? jumpController.VerticalVelocity : 0f;
        public ITargetable CurrentLockTarget => lockOnTarget;

        private Vector3 ResolveCameraRelativeMove(Vector2 input)
        {
            var cameraTransform = UnityEngine.Camera.main != null ? UnityEngine.Camera.main.transform : null;
            if (cameraTransform == null)
            {
                return new Vector3(input.x, 0f, input.y);
            }

            var forward = cameraTransform.forward;
            var right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            return forward * input.y + right * input.x;
        }

        private Vector3 ResolveCameraForward()
        {
            var cameraTransform = UnityEngine.Camera.main != null ? UnityEngine.Camera.main.transform : null;
            if (cameraTransform == null)
            {
                return transform.forward;
            }

            var forward = cameraTransform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.001f)
            {
                return transform.forward;
            }

            return forward.normalized;
        }
    }
}

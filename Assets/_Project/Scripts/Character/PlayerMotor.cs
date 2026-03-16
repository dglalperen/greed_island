using UnityEngine;

namespace GreedIsland.Character
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private bool pushRigidbodiesOnContact = true;
        [SerializeField, Min(0f)] private float rigidbodyPushImpulse = 1.2f;
        [SerializeField, Min(1f)] private float dashPushMultiplier = 1.6f;

        private Vector3 planarVelocity;
        private Vector3 lastMotion;
        private Vector3 lastExternalVelocity;

        public Vector3 PlanarVelocity => planarVelocity;
        public float HorizontalSpeed => new Vector3(planarVelocity.x, 0f, planarVelocity.z).magnitude;
        public Vector3 LastMotion => lastMotion;
        public CollisionFlags LastCollisionFlags { get; private set; }
        public bool IsTouchingCeiling => (LastCollisionFlags & CollisionFlags.Above) != 0;
        public bool IsTouchingGround => (LastCollisionFlags & CollisionFlags.Below) != 0;

        private void Reset()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Awake()
        {
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }
        }

        public void TickMotor(
            Vector3 desiredMoveDirection,
            Vector3 groundNormal,
            bool isGrounded,
            float targetSpeed,
            float acceleration,
            float deceleration,
            float airControlPercent,
            float verticalVelocity,
            Vector3 externalVelocity)
        {
            if (characterController == null)
            {
                return;
            }

            var projectedDirection = desiredMoveDirection;
            if (isGrounded)
            {
                projectedDirection = Vector3.ProjectOnPlane(desiredMoveDirection, groundNormal).normalized;
            }

            var targetPlanarVelocity = projectedDirection * targetSpeed;
            var controlMultiplier = isGrounded ? 1f : Mathf.Clamp01(airControlPercent);

            var currentPlanar = new Vector3(planarVelocity.x, 0f, planarVelocity.z);
            var hasInput = desiredMoveDirection.sqrMagnitude > 0.0001f;
            var lerpRate = (hasInput ? acceleration : deceleration) * controlMultiplier;
            currentPlanar = Vector3.MoveTowards(currentPlanar, targetPlanarVelocity, lerpRate * Time.deltaTime);
            planarVelocity = currentPlanar;

            var horizontalVelocity = currentPlanar + new Vector3(externalVelocity.x, 0f, externalVelocity.z);
            var horizontalMotion = horizontalVelocity * Time.deltaTime;
            var verticalMotion = Vector3.up * verticalVelocity * Time.deltaTime;

            var horizontalFlags = characterController.Move(horizontalMotion);
            var verticalFlags = characterController.Move(verticalMotion);

            LastCollisionFlags = horizontalFlags | verticalFlags;
            lastExternalVelocity = externalVelocity;
            lastMotion = horizontalVelocity + Vector3.up * verticalVelocity;
        }

        public void StopPlanarMotion()
        {
            planarVelocity = Vector3.zero;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!pushRigidbodiesOnContact || hit.rigidbody == null || hit.rigidbody.isKinematic)
            {
                return;
            }

            if (hit.moveDirection.y < -0.25f)
            {
                return;
            }

            var pushDirection = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
            if (pushDirection.sqrMagnitude < 0.0001f)
            {
                return;
            }

            var impulse = rigidbodyPushImpulse;
            if (new Vector3(lastExternalVelocity.x, 0f, lastExternalVelocity.z).sqrMagnitude > 0.01f)
            {
                impulse *= dashPushMultiplier;
            }

            hit.rigidbody.AddForce(pushDirection.normalized * impulse, ForceMode.Impulse);
        }
    }
}

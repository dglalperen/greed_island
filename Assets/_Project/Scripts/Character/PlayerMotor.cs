using UnityEngine;

namespace GreedIsland.Character
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;

        private Vector3 planarVelocity;
        private Vector3 lastMotion;

        public Vector3 PlanarVelocity => planarVelocity;
        public float HorizontalSpeed => new Vector3(planarVelocity.x, 0f, planarVelocity.z).magnitude;
        public Vector3 LastMotion => lastMotion;

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

            var motion = currentPlanar + Vector3.up * verticalVelocity + externalVelocity;
            lastMotion = motion;
            characterController.Move(motion * Time.deltaTime);
        }

        public void StopPlanarMotion()
        {
            planarVelocity = Vector3.zero;
        }
    }
}

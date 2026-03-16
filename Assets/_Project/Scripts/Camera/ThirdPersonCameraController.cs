using GreedIsland.Input;
using UnityEngine;

namespace GreedIsland.Camera
{
    public sealed class ThirdPersonCameraController : MonoBehaviour
    {
        [SerializeField] private CameraTargetProvider targetProvider;
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField, Min(0.5f)] private float distance = 5f;
        [SerializeField, Min(0f)] private float shoulderOffset = 0.6f;
        [SerializeField, Min(0f)] private float heightOffset = 1.35f;
        [SerializeField, Min(0f)] private float yawSpeed = 0.12f;
        [SerializeField, Min(0f)] private float pitchSpeed = 0.12f;
        [SerializeField] private float minPitch = -30f;
        [SerializeField] private float maxPitch = 70f;
        [SerializeField, Min(0f)] private float smoothing = 15f;
        [SerializeField] private LayerMask collisionMask = ~0;

        private float yaw;
        private float pitch = 15f;

        public CameraTargetProvider TargetProvider => targetProvider;

        private void Start()
        {
            if (inputReader == null)
            {
                inputReader = FindAnyObjectByType<PlayerInputReader>();
            }

            var euler = transform.eulerAngles;
            yaw = euler.y;
            pitch = euler.x;
        }

        private void LateUpdate()
        {
            if (targetProvider == null)
            {
                return;
            }

            var lookInput = inputReader != null ? inputReader.Look : Vector2.zero;
            yaw += lookInput.x * yawSpeed;
            pitch -= lookInput.y * pitchSpeed;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            var pivot = targetProvider.CameraRoot != null ? targetProvider.CameraRoot.position : targetProvider.transform.position;
            pivot += Vector3.up * heightOffset;

            var rotation = Quaternion.Euler(pitch, yaw, 0f);
            var shoulder = rotation * Vector3.right * shoulderOffset;
            var desiredPosition = pivot + shoulder - (rotation * Vector3.forward * distance);

            var finalPosition = ResolveCollision(pivot + shoulder, desiredPosition);
            transform.position = Vector3.Lerp(transform.position, finalPosition, 1f - Mathf.Exp(-smoothing * Time.deltaTime));

            var lookTarget = targetProvider.CameraLookTarget != null ? targetProvider.CameraLookTarget.position : pivot;
            transform.rotation = Quaternion.LookRotation((lookTarget - transform.position).normalized, Vector3.up);
        }

        public void SetTarget(CameraTargetProvider provider)
        {
            targetProvider = provider;
        }

        private Vector3 ResolveCollision(Vector3 pivot, Vector3 desiredPosition)
        {
            var direction = desiredPosition - pivot;
            var distanceToTarget = direction.magnitude;
            if (distanceToTarget <= 0.001f)
            {
                return desiredPosition;
            }

            direction /= distanceToTarget;

            if (Physics.SphereCast(pivot, 0.2f, direction, out var hit, distanceToTarget, collisionMask, QueryTriggerInteraction.Ignore))
            {
                return hit.point - direction * 0.15f;
            }

            return desiredPosition;
        }
    }
}

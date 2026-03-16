using GreedIsland.Core;
using UnityEngine;

namespace GreedIsland.Character
{
    public sealed class PlayerGroundChecker : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField, Min(0.01f)] private float castRadius = 0.28f;
        [SerializeField, Min(0.05f)] private float castDistance = 0.35f;
        [SerializeField] private LayerMask groundMask = ~0;

        public bool IsGrounded { get; private set; }
        public Vector3 GroundNormal { get; private set; } = Vector3.up;
        public float SlopeAngle { get; private set; }

        private void Awake()
        {
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }

            if (groundMask == ~0)
            {
                groundMask = LayerMaskConfig.GroundMask;
            }
        }

        private void Reset()
        {
            characterController = GetComponent<CharacterController>();
            groundMask = LayerMaskConfig.GroundMask;
        }

        public void Tick()
        {
            if (characterController == null)
            {
                return;
            }

            var centerWorld = transform.position + characterController.center;
            var feet = centerWorld + Vector3.down * (characterController.height * 0.5f - characterController.radius + 0.03f);
            var origin = feet + Vector3.up * 0.08f;

            if (Physics.SphereCast(origin, castRadius, Vector3.down, out var hit, castDistance, groundMask, QueryTriggerInteraction.Ignore))
            {
                IsGrounded = true;
                GroundNormal = hit.normal;
                SlopeAngle = Vector3.Angle(Vector3.up, hit.normal);
                return;
            }

            IsGrounded = characterController.isGrounded;
            GroundNormal = Vector3.up;
            SlopeAngle = 0f;
        }

        private void OnDrawGizmosSelected()
        {
            if (characterController == null)
            {
                return;
            }

            var centerWorld = transform.position + characterController.center;
            var feet = centerWorld + Vector3.down * (characterController.height * 0.5f - characterController.radius + 0.03f);
            var origin = feet + Vector3.up * 0.08f;
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(origin + Vector3.down * castDistance, castRadius);
        }
    }
}

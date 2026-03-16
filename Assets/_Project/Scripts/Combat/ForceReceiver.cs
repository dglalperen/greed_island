using UnityEngine;

namespace GreedIsland.Combat
{
    public sealed class ForceReceiver : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField, Min(0f)] private float damping = 5f;

        private Vector3 pendingVelocity;

        private void Reset()
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (rigidBody != null)
            {
                return;
            }

            if (pendingVelocity.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            transform.position += pendingVelocity * Time.deltaTime;
            pendingVelocity = Vector3.Lerp(pendingVelocity, Vector3.zero, 1f - Mathf.Exp(-damping * Time.deltaTime));
        }

        public void ApplyImpulse(Vector3 impulse)
        {
            if (impulse.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            if (rigidBody != null)
            {
                rigidBody.AddForce(impulse, ForceMode.Impulse);
                return;
            }

            pendingVelocity += impulse;
        }
    }
}

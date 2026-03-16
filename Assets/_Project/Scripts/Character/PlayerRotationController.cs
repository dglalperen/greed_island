using UnityEngine;

namespace GreedIsland.Character
{
    public sealed class PlayerRotationController : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float rotationSpeed = 720f;

        public void Tick(Vector3 desiredForward, bool allowRotation)
        {
            if (!allowRotation)
            {
                return;
            }

            desiredForward.y = 0f;
            if (desiredForward.sqrMagnitude < 0.001f)
            {
                return;
            }

            var targetRotation = Quaternion.LookRotation(desiredForward.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}

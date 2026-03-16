using UnityEngine;

namespace GreedIsland.Character
{
    public sealed class PlayerJumpController : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float coyoteTime = 0.12f;
        [SerializeField, Min(0f)] private float jumpBufferTime = 0.12f;

        private float verticalVelocity;
        private float lastGroundedAt = -999f;
        private float jumpBufferedUntil = -999f;

        public float VerticalVelocity => verticalVelocity;

        public void QueueJump()
        {
            jumpBufferedUntil = Time.time + jumpBufferTime;
        }

        public float Tick(bool isGrounded, float gravity, float jumpHeight, bool canJump)
        {
            if (isGrounded)
            {
                lastGroundedAt = Time.time;
                if (verticalVelocity < -1f)
                {
                    verticalVelocity = -1f;
                }
            }

            var hasBufferedJump = Time.time <= jumpBufferedUntil;
            var hasCoyoteWindow = Time.time <= lastGroundedAt + coyoteTime;

            if (canJump && hasBufferedJump && hasCoyoteWindow)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpBufferedUntil = -999f;
            }

            verticalVelocity += gravity * Time.deltaTime;
            return verticalVelocity;
        }

        public void ForceVerticalVelocity(float value)
        {
            verticalVelocity = value;
        }
    }
}

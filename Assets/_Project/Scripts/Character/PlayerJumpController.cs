using GreedIsland.Core;
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
                if (verticalVelocity < -2f)
                {
                    verticalVelocity = -2f;
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
            if (verticalVelocity < GameConstants.TerminalVelocity)
            {
                verticalVelocity = GameConstants.TerminalVelocity;
            }

            return verticalVelocity;
        }

        public void ForceVerticalVelocity(float value)
        {
            verticalVelocity = value;
        }
    }
}

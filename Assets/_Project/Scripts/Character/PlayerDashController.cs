using GreedIsland.Core;
using UnityEngine;

namespace GreedIsland.Character
{
    public sealed class PlayerDashController : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float dashSpeed = GameConstants.DefaultDashSpeed;
        [SerializeField, Min(0.01f)] private float dashDuration = GameConstants.DefaultDashDuration;
        [SerializeField, Min(0f)] private float dashCooldown = GameConstants.DefaultDashCooldown;

        private CooldownTimer cooldown;
        private float dashEndsAt;
        private Vector3 dashDirection = Vector3.forward;

        public bool IsDashing => Time.time < dashEndsAt;
        public float DashCooldownRemaining => cooldown.RemainingSeconds;

        public void Configure(float speed, float duration, float cooldownSeconds)
        {
            dashSpeed = Mathf.Max(0f, speed);
            dashDuration = Mathf.Max(0.01f, duration);
            dashCooldown = Mathf.Max(0f, cooldownSeconds);
        }

        public bool TryStartDash(Vector3 worldDirection)
        {
            if (!cooldown.IsReady())
            {
                return false;
            }

            if (worldDirection.sqrMagnitude < 0.0001f)
            {
                worldDirection = transform.forward;
            }

            dashDirection = worldDirection.normalized;
            dashEndsAt = Time.time + dashDuration;
            cooldown.Start(dashCooldown);
            return true;
        }

        public Vector3 GetDashVelocity()
        {
            if (!IsDashing)
            {
                return Vector3.zero;
            }

            return dashDirection * dashSpeed;
        }
    }
}

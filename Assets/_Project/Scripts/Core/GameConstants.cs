using UnityEngine;

namespace GreedIsland.Core
{
    public static class GameConstants
    {
        public const string BootstrapSceneName = "Bootstrap";
        public const string PrototypeSceneName = "PrototypeArena";

        public const float DefaultMoveSpeed = 4.5f;
        public const float DefaultSprintSpeed = 7.5f;
        public const float DefaultAirControlPercent = 0.35f;
        public const float DefaultJumpHeight = 1.3f;
        public const float DefaultGravity = -22f;
        public const float TerminalVelocity = -53f;

        public const float DefaultDashSpeed = 13f;
        public const float DefaultDashDuration = 0.2f;
        public const float DefaultDashCooldown = 0.6f;

        public const float DefaultAuraMax = 100f;
        public const float DefaultAuraRegen = 12f;
        public const float DefaultAuraRegenDelay = 1.1f;

        public const float DefaultHealthMax = 100f;

        public static readonly Vector3 ArenaSize = new(28f, 2f, 28f);
    }
}

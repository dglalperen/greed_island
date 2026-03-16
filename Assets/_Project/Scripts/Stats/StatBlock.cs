using System;
using UnityEngine;

namespace GreedIsland.Stats
{
    [Serializable]
    public class StatBlock
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float moveSpeed = 4.5f;
        [SerializeField] private float sprintSpeed = 7.5f;
        [SerializeField] private float acceleration = 18f;
        [SerializeField] private float deceleration = 22f;
        [SerializeField] private float jumpHeight = 1.3f;
        [SerializeField] private float gravity = -22f;
        [SerializeField] private float airControlPercent = 0.35f;
        [SerializeField] private float dashSpeed = 13f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashCooldown = 0.6f;

        public float MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public float SprintSpeed => sprintSpeed;
        public float Acceleration => acceleration;
        public float Deceleration => deceleration;
        public float JumpHeight => jumpHeight;
        public float Gravity => gravity;
        public float AirControlPercent => airControlPercent;
        public float DashSpeed => dashSpeed;
        public float DashDuration => dashDuration;
        public float DashCooldown => dashCooldown;

        public StatBlock Clone()
        {
            return new StatBlock
            {
                maxHealth = maxHealth,
                moveSpeed = moveSpeed,
                sprintSpeed = sprintSpeed,
                acceleration = acceleration,
                deceleration = deceleration,
                jumpHeight = jumpHeight,
                gravity = gravity,
                airControlPercent = airControlPercent,
                dashSpeed = dashSpeed,
                dashDuration = dashDuration,
                dashCooldown = dashCooldown
            };
        }
    }
}

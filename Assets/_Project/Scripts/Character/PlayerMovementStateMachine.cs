using System;
using UnityEngine;

namespace GreedIsland.Character
{
    [Serializable]
    public sealed class PlayerMovementStateMachine
    {
        private const float LandStateDuration = 0.12f;

        private PlayerMoveState currentState = PlayerMoveState.Idle;
        private float landedUntil;

        public PlayerMoveState CurrentState => currentState;

        public void Tick(bool isGrounded, bool isSprinting, bool isDashing, float horizontalSpeed, float verticalVelocity)
        {
            if (isDashing)
            {
                currentState = PlayerMoveState.Dash;
                return;
            }

            if (!isGrounded)
            {
                currentState = verticalVelocity > 0.05f ? PlayerMoveState.JumpStart : PlayerMoveState.InAir;
                return;
            }

            if (currentState == PlayerMoveState.InAir || currentState == PlayerMoveState.JumpStart)
            {
                landedUntil = Time.time + LandStateDuration;
            }

            if (Time.time < landedUntil)
            {
                currentState = PlayerMoveState.Land;
                return;
            }

            if (horizontalSpeed < 0.05f)
            {
                currentState = PlayerMoveState.Idle;
                return;
            }

            currentState = isSprinting ? PlayerMoveState.Sprint : PlayerMoveState.Move;
        }
    }
}

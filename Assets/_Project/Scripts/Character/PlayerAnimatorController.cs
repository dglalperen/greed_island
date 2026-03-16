using UnityEngine;

namespace GreedIsland.Character
{
    public sealed class PlayerAnimatorController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        private static readonly int VerticalVelocityHash = Animator.StringToHash("VerticalVelocity");
        private static readonly int IsSprintingHash = Animator.StringToHash("IsSprinting");
        private static readonly int DashTriggerHash = Animator.StringToHash("DashTrigger");
        private static readonly int JumpTriggerHash = Animator.StringToHash("JumpTrigger");
        private static readonly int LandTriggerHash = Animator.StringToHash("LandTrigger");
        private static readonly int AbilityTriggerHash = Animator.StringToHash("AbilityTrigger");

        private PlayerMoveState previousState;

        private void Reset()
        {
            animator = GetComponentInChildren<Animator>();
        }

        public void Tick(PlayerMoveState state, float speed, bool isGrounded, float verticalVelocity, bool isSprinting)
        {
            if (animator == null)
            {
                return;
            }

            animator.SetFloat(MoveSpeedHash, speed);
            animator.SetBool(IsGroundedHash, isGrounded);
            animator.SetFloat(VerticalVelocityHash, verticalVelocity);
            animator.SetBool(IsSprintingHash, isSprinting);

            if (state == PlayerMoveState.Dash && previousState != PlayerMoveState.Dash)
            {
                animator.SetTrigger(DashTriggerHash);
            }

            if (state == PlayerMoveState.JumpStart && previousState != PlayerMoveState.JumpStart)
            {
                animator.SetTrigger(JumpTriggerHash);
            }

            if (state == PlayerMoveState.Land && previousState != PlayerMoveState.Land)
            {
                animator.SetTrigger(LandTriggerHash);
            }

            previousState = state;
        }

        public void TriggerAbilityCast()
        {
            if (animator == null)
            {
                return;
            }

            animator.SetTrigger(AbilityTriggerHash);
        }
    }
}

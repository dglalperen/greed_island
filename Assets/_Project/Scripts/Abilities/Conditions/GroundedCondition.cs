using GreedIsland.Character;
using UnityEngine;

namespace GreedIsland.Abilities.Conditions
{
    [CreateAssetMenu(menuName = "GreedIsland/Abilities/Conditions/Grounded")]
    public sealed class GroundedCondition : AbilityCondition
    {
        public override bool CanActivate(in AbilityContext context, out string failReason)
        {
            var playerBrain = context.CasterObject != null ? context.CasterObject.GetComponent<PlayerBrain>() : null;
            if (playerBrain != null && playerBrain.IsGrounded)
            {
                failReason = string.Empty;
                return true;
            }

            failReason = "Must be grounded.";
            return false;
        }
    }
}

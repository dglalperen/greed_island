using GreedIsland.Aura;
using UnityEngine;

namespace GreedIsland.Abilities.Conditions
{
    [CreateAssetMenu(menuName = "GreedIsland/Abilities/Conditions/AuraMode")]
    public sealed class AuraModeCondition : AbilityCondition
    {
        [SerializeField] private AuraMode requiredMode = AuraMode.Neutral;

        public void SetRequiredMode(AuraMode mode)
        {
            requiredMode = mode;
        }

        public override bool CanActivate(in AbilityContext context, out string failReason)
        {
            if (context.AuraController != null && context.AuraController.CurrentMode == requiredMode)
            {
                failReason = string.Empty;
                return true;
            }

            failReason = $"Requires {requiredMode} mode.";
            return false;
        }
    }
}

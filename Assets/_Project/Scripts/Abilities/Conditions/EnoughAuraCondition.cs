using UnityEngine;

namespace GreedIsland.Abilities.Conditions
{
    [CreateAssetMenu(menuName = "GreedIsland/Abilities/Conditions/EnoughAura")]
    public sealed class EnoughAuraCondition : AbilityCondition
    {
        [SerializeField, Min(0f)] private float minimumAura = 1f;

        public void SetMinimumAura(float value)
        {
            minimumAura = Mathf.Max(0f, value);
        }

        public override bool CanActivate(in AbilityContext context, out string failReason)
        {
            if (context.AuraController != null && context.AuraController.HasEnough(minimumAura))
            {
                failReason = string.Empty;
                return true;
            }

            failReason = "Not enough aura.";
            return false;
        }
    }
}

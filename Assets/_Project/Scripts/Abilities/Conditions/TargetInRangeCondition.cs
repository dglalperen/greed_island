using UnityEngine;

namespace GreedIsland.Abilities.Conditions
{
    [CreateAssetMenu(menuName = "GreedIsland/Abilities/Conditions/TargetInRange")]
    public sealed class TargetInRangeCondition : AbilityCondition
    {
        [SerializeField, Min(0f)] private float range = 12f;

        public void SetRange(float value)
        {
            range = Mathf.Max(0f, value);
        }

        public override bool CanActivate(in AbilityContext context, out string failReason)
        {
            if (context.CurrentTarget == null || !context.CurrentTarget.IsTargetable)
            {
                failReason = "No target selected.";
                return false;
            }

            var distance = Vector3.Distance(context.Origin.position, context.CurrentTarget.TargetTransform.position);
            if (distance <= range)
            {
                failReason = string.Empty;
                return true;
            }

            failReason = "Target out of range.";
            return false;
        }
    }
}

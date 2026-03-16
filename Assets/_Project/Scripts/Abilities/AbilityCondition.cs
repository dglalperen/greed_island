using UnityEngine;

namespace GreedIsland.Abilities
{
    public abstract class AbilityCondition : ScriptableObject
    {
        public abstract bool CanActivate(in AbilityContext context, out string failReason);
    }
}

using UnityEngine;

namespace GreedIsland.Abilities
{
    public abstract class AbilityEffect : ScriptableObject
    {
        public abstract void Execute(in AbilityContext context);

        public virtual void Stop(in AbilityContext context)
        {
        }
    }
}

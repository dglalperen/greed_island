using GreedIsland.Aura;
using GreedIsland.Stats;
using UnityEngine;

namespace GreedIsland.Core
{
    public interface IAbilityUser
    {
        Transform Origin { get; }
        AuraController AuraController { get; }
        StatBlock Stats { get; }
    }
}

using System.Collections.Generic;
using GreedIsland.Aura;
using GreedIsland.Core;
using GreedIsland.Stats;
using UnityEngine;

namespace GreedIsland.Abilities
{
    public struct AbilityContext
    {
        public GameObject CasterObject;
        public IAbilityUser Caster;
        public Transform Origin;
        public Vector3 Direction;
        public ITargetable CurrentTarget;
        public List<ITargetable> ResolvedTargets;
        public AuraController AuraController;
        public StatBlock Stats;
        public float TimeStamp;
        public float ChargeAmount;
        public float PowerMultiplier;
    }
}

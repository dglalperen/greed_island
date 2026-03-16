using System;
using UnityEngine;

namespace GreedIsland.Aura
{
    [CreateAssetMenu(menuName = "GreedIsland/Aura/AffinityProfile")]
    public sealed class AffinityProfile : ScriptableObject
    {
        [SerializeField] private AffinityType primaryAffinity = AffinityType.Reinforcement;
        [SerializeField] private AffinityType secondaryAffinity = AffinityType.Projection;
        [SerializeField] private AffinityEfficiency[] efficiencies =
        {
            new() { affinity = AffinityType.Reinforcement, costMultiplier = 1f, powerMultiplier = 1f },
            new() { affinity = AffinityType.Projection, costMultiplier = 1f, powerMultiplier = 1f },
            new() { affinity = AffinityType.Control, costMultiplier = 1f, powerMultiplier = 1f },
            new() { affinity = AffinityType.Manifestation, costMultiplier = 1f, powerMultiplier = 1f },
            new() { affinity = AffinityType.Alteration, costMultiplier = 1f, powerMultiplier = 1f },
            new() { affinity = AffinityType.Unique, costMultiplier = 1f, powerMultiplier = 1f }
        };

        public AffinityType PrimaryAffinity => primaryAffinity;
        public AffinityType SecondaryAffinity => secondaryAffinity;

        public AffinityEfficiency GetEfficiency(AffinityType affinity)
        {
            foreach (var entry in efficiencies)
            {
                if (entry.affinity == affinity)
                {
                    return entry;
                }
            }

            return new AffinityEfficiency { affinity = affinity, costMultiplier = 1f, powerMultiplier = 1f };
        }

        [Serializable]
        public struct AffinityEfficiency
        {
            public AffinityType affinity;
            [Min(0.01f)] public float costMultiplier;
            [Min(0.01f)] public float powerMultiplier;
        }
    }
}

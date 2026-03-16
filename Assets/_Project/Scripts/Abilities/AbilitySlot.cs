using System;
using UnityEngine;

namespace GreedIsland.Abilities
{
    [Serializable]
    public sealed class AbilitySlot
    {
        [SerializeField] private int slotId = 1;
        [SerializeField] private AbilityDefinition definition;

        public int SlotId => slotId;
        public AbilityDefinition Definition => definition;

        public static AbilitySlot Create(int id, AbilityDefinition abilityDefinition)
        {
            return new AbilitySlot
            {
                slotId = id,
                definition = abilityDefinition
            };
        }
    }
}

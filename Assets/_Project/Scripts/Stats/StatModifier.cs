using System;

namespace GreedIsland.Stats
{
    [Serializable]
    public struct StatModifier
    {
        public string id;
        public float additive;
        public float multiplier;
    }
}

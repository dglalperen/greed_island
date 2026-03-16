using GreedIsland.Stats;
using UnityEngine;

namespace GreedIsland.Character
{
    public sealed class PlayerStatsProvider : MonoBehaviour
    {
        [SerializeField] private StatBlock baseStats = new();

        private StatBlock runtimeStats;

        public StatBlock RuntimeStats
        {
            get
            {
                runtimeStats ??= baseStats.Clone();
                return runtimeStats;
            }
        }

        public void ResetRuntimeStats()
        {
            runtimeStats = baseStats.Clone();
        }
    }
}

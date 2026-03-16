using UnityEngine;

namespace GreedIsland.Stats
{
    [CreateAssetMenu(menuName = "GreedIsland/Stats/RegenerationProfile")]
    public sealed class RegenerationProfile : ScriptableObject
    {
        [SerializeField, Min(0f)] private float regenPerSecond = 12f;
        [SerializeField, Min(0f)] private float regenDelaySeconds = 1.1f;

        public float RegenPerSecond => regenPerSecond;
        public float RegenDelaySeconds => regenDelaySeconds;
    }
}

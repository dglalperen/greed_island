using UnityEngine;

namespace GreedIsland.Abilities
{
    public sealed class AbilityCooldown
    {
        private float readyAt;

        public void Start(float durationSeconds)
        {
            readyAt = Time.time + Mathf.Max(0f, durationSeconds);
        }

        public bool IsReady => Time.time >= readyAt;

        public float Remaining => Mathf.Max(0f, readyAt - Time.time);
    }
}

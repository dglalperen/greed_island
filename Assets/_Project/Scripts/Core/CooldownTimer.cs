using UnityEngine;

namespace GreedIsland.Core
{
    [System.Serializable]
    public struct CooldownTimer
    {
        [SerializeField] private float readyAtTime;

        public bool IsCoolingDown => Time.time < readyAtTime;

        public float RemainingSeconds
        {
            get
            {
                var remaining = readyAtTime - Time.time;
                return remaining > 0f ? remaining : 0f;
            }
        }

        public void Start(float durationSeconds)
        {
            var clamped = Mathf.Max(0f, durationSeconds);
            readyAtTime = Time.time + clamped;
        }

        public void Clear()
        {
            readyAtTime = 0f;
        }

        public bool IsReady(float marginSeconds = 0f)
        {
            return Time.time + marginSeconds >= readyAtTime;
        }
    }
}

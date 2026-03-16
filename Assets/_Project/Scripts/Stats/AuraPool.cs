using System;
using UnityEngine;

namespace GreedIsland.Stats
{
    public sealed class AuraPool : ResourcePool
    {
        [SerializeField, Min(0f)] private float baseRegenPerSecond = 12f;
        [SerializeField, Min(0f)] private float regenDelay = 1.1f;
        [SerializeField] private bool canRegenerate = true;

        private float lastSpendAt = -999f;
        private float regenMultiplier = 1f;

        public event Action<bool> RegenStateChanged;

        public float BaseRegenPerSecond => baseRegenPerSecond;
        public float RegenDelay => regenDelay;
        public bool CanRegenerate => canRegenerate;

        public void Configure(float maxAura, float regenPerSecond, float regenDelaySeconds)
        {
            max = Mathf.Max(1f, maxAura);
            baseRegenPerSecond = Mathf.Max(0f, regenPerSecond);
            regenDelay = Mathf.Max(0f, regenDelaySeconds);
            current = Mathf.Clamp(current, 0f, max);
            RaiseValueChanged();
        }

        public override bool Spend(float amount)
        {
            if (!base.Spend(amount))
            {
                return false;
            }

            if (amount > 0f)
            {
                lastSpendAt = Time.time;
            }

            return true;
        }

        public void SetCanRegenerate(bool value)
        {
            if (canRegenerate == value)
            {
                return;
            }

            canRegenerate = value;
            RegenStateChanged?.Invoke(canRegenerate);
        }

        public void SetRegenMultiplier(float value)
        {
            regenMultiplier = Mathf.Max(0f, value);
        }

        private void Update()
        {
            if (!canRegenerate)
            {
                return;
            }

            if (current >= max)
            {
                return;
            }

            if (Time.time < lastSpendAt + regenDelay)
            {
                return;
            }

            var regenAmount = baseRegenPerSecond * regenMultiplier * Time.deltaTime;
            if (regenAmount <= 0f)
            {
                return;
            }

            Restore(regenAmount);
        }
    }
}

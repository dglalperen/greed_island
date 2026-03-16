using System;
using GreedIsland.Core;
using UnityEngine;

namespace GreedIsland.Stats
{
    public abstract class ResourcePool : MonoBehaviour, IResourcePool
    {
        [SerializeField, Min(1f)] protected float max = 100f;
        [SerializeField, Min(0f)] protected float current = 100f;

        public event Action<float, float> ValueChanged;

        public float Current => current;
        public float Max => max;

        protected virtual void Awake()
        {
            current = Mathf.Clamp(current, 0f, max);
        }

        public virtual bool CanAfford(float amount)
        {
            return amount <= 0f || current >= amount;
        }

        public virtual bool Spend(float amount)
        {
            if (amount <= 0f)
            {
                return true;
            }

            if (!CanAfford(amount))
            {
                return false;
            }

            SetCurrent(current - amount);
            return true;
        }

        public virtual void Restore(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            SetCurrent(current + amount);
        }

        protected void SetMax(float value, bool keepRatio = true)
        {
            var oldMax = Mathf.Max(1f, max);
            max = Mathf.Max(1f, value);

            if (keepRatio)
            {
                var ratio = current / oldMax;
                current = Mathf.Clamp(max * ratio, 0f, max);
            }
            else
            {
                current = Mathf.Clamp(current, 0f, max);
            }

            RaiseValueChanged();
        }

        protected void SetCurrent(float value)
        {
            var previous = current;
            current = Mathf.Clamp(value, 0f, max);
            if (!Mathf.Approximately(previous, current))
            {
                RaiseValueChanged();
            }
        }

        protected void RaiseValueChanged()
        {
            ValueChanged?.Invoke(current, max);
        }
    }
}

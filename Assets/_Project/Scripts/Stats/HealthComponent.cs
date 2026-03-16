using System;
using GreedIsland.Combat;
using GreedIsland.Core;
using UnityEngine;

namespace GreedIsland.Stats
{
    public sealed class HealthComponent : ResourcePool, IDamageable
    {
        [SerializeField] private bool destroyOnDeath;

        public event Action<float> Damaged;
        public event Action Died;

        public bool IsAlive => current > 0f;

        private void Start()
        {
            RaiseValueChanged();
        }

        public void ReceiveDamage(in DamagePayload payload)
        {
            if (!IsAlive)
            {
                return;
            }

            var finalDamage = Mathf.Max(0f, payload.Amount);
            if (finalDamage <= 0f)
            {
                return;
            }

            var previous = current;
            SetCurrent(current - finalDamage);

            if (current < previous)
            {
                Damaged?.Invoke(finalDamage);
            }

            if (current <= 0f)
            {
                Died?.Invoke();
                if (destroyOnDeath)
                {
                    Destroy(gameObject);
                }
            }
        }

        public void Heal(float amount)
        {
            Restore(amount);
        }
    }
}

using GreedIsland.Core;
using GreedIsland.Stats;
using UnityEngine;

namespace GreedIsland.Combat
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(DamageableActor))]
    public sealed class TargetDummyController : MonoBehaviour, ITargetable
    {
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private DamageableActor damageableActor;
        [SerializeField] private bool autoResetOnDeath = true;
        [SerializeField, Min(0f)] private float resetDelay = 2.5f;

        private float resetAt = -1f;

        public Transform TargetTransform => transform;
        public bool IsTargetable => healthComponent != null && healthComponent.IsAlive;

        private void Reset()
        {
            healthComponent = GetComponent<HealthComponent>();
            damageableActor = GetComponent<DamageableActor>();
        }

        private void Awake()
        {
            if (healthComponent == null)
            {
                healthComponent = GetComponent<HealthComponent>();
            }

            if (damageableActor == null)
            {
                damageableActor = GetComponent<DamageableActor>();
            }
        }

        private void OnEnable()
        {
            if (healthComponent != null)
            {
                healthComponent.Died += OnDied;
            }
        }

        private void OnDisable()
        {
            if (healthComponent != null)
            {
                healthComponent.Died -= OnDied;
            }
        }

        private void Update()
        {
            if (!autoResetOnDeath || resetAt < 0f || Time.time < resetAt)
            {
                return;
            }

            resetAt = -1f;
            ResetDummy();
        }

        [ContextMenu("Reset Dummy")]
        public void ResetDummy()
        {
            healthComponent?.Restore(float.MaxValue);
            var rigidBody = GetComponent<Rigidbody>();
            if (rigidBody != null)
            {
                rigidBody.linearVelocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero;
            }
        }

        private void OnDied()
        {
            if (!autoResetOnDeath)
            {
                return;
            }

            resetAt = Time.time + resetDelay;
        }
    }
}

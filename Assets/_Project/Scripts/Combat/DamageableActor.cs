using GreedIsland.Core;
using GreedIsland.Stats;
using UnityEngine;

namespace GreedIsland.Combat
{
    [RequireComponent(typeof(HealthComponent))]
    public class DamageableActor : MonoBehaviour, IDamageable, ITargetable
    {
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private ShieldComponent shieldComponent;
        [SerializeField] private ForceReceiver forceReceiver;
        [SerializeField] private Renderer feedbackRenderer;
        [SerializeField, Min(0f)] private float flashDuration = 0.08f;
        [SerializeField] private Color hitFlashColor = new(1f, 0.35f, 0.35f, 1f);

        private MaterialPropertyBlock propertyBlock;
        private Color baseColor;
        private float flashUntil;

        public Transform TargetTransform => transform;
        public bool IsTargetable => healthComponent != null && healthComponent.IsAlive;

        protected virtual void Awake()
        {
            if (healthComponent == null)
            {
                healthComponent = GetComponent<HealthComponent>();
            }

            if (forceReceiver == null)
            {
                forceReceiver = GetComponent<ForceReceiver>();
            }

            if (feedbackRenderer == null)
            {
                feedbackRenderer = GetComponentInChildren<Renderer>();
            }

            if (feedbackRenderer != null)
            {
                propertyBlock = new MaterialPropertyBlock();
                baseColor = feedbackRenderer.sharedMaterial != null && feedbackRenderer.sharedMaterial.HasProperty("_BaseColor")
                    ? feedbackRenderer.sharedMaterial.GetColor("_BaseColor")
                    : Color.white;
            }
        }

        protected virtual void Update()
        {
            if (feedbackRenderer == null || propertyBlock == null)
            {
                return;
            }

            var color = Time.time < flashUntil ? hitFlashColor : baseColor;
            feedbackRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", color);
            feedbackRenderer.SetPropertyBlock(propertyBlock);
        }

        public virtual void ReceiveDamage(in DamagePayload payload)
        {
            if (healthComponent == null || !healthComponent.IsAlive)
            {
                return;
            }

            var finalDamage = payload.Amount;
            if (shieldComponent != null)
            {
                finalDamage = shieldComponent.ApplyMitigation(finalDamage);
            }

            var resolvedPayload = payload;
            resolvedPayload.Amount = finalDamage;
            healthComponent.ReceiveDamage(in resolvedPayload);

            if (payload.Force > 0f && forceReceiver != null)
            {
                forceReceiver.ApplyImpulse(payload.Direction * payload.Force);
            }

            flashUntil = Time.time + flashDuration;
        }
    }
}

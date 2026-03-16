using GreedIsland.Combat;
using GreedIsland.Core;
using UnityEngine;

namespace GreedIsland.Abilities.Effects
{
    [CreateAssetMenu(menuName = "GreedIsland/Abilities/Effects/AuraBurst")]
    public sealed class AuraBurstEffect : AbilityEffect
    {
        [SerializeField, Min(0f)] private float damage = 20f;
        [SerializeField, Min(0f)] private float force = 10f;
        [SerializeField, Min(0f)] private float radius = 4f;
        [SerializeField, Min(0f)] private float forwardOffset = 1.2f;
        [SerializeField] private LayerMask damageMask = ~0;
        [SerializeField] private LayerMask rigidbodyMask = ~0;

        public void Configure(float newDamage, float newForce, float newRadius, float newForwardOffset, LayerMask newDamageMask, LayerMask newRigidbodyMask)
        {
            damage = Mathf.Max(0f, newDamage);
            force = Mathf.Max(0f, newForce);
            radius = Mathf.Max(0f, newRadius);
            forwardOffset = Mathf.Max(0f, newForwardOffset);
            damageMask = newDamageMask;
            rigidbodyMask = newRigidbodyMask;
        }

        public override void Execute(in AbilityContext context)
        {
            if (context.Origin == null)
            {
                return;
            }

            var center = context.Origin.position + context.Direction.normalized * forwardOffset;
            var payload = DamagePayload.Create(
                damage * context.PowerMultiplier,
                context.CasterObject,
                center,
                context.Direction,
                force,
                true,
                "AuraBurst");

            ImpactResolver.ApplyDamageInSphere(center, radius, damageMask, in payload);
            ImpactResolver.ApplyExplosionForce(center, radius, force, rigidbodyMask);

            Debug.DrawRay(center, Vector3.up * 1.5f, Color.red, 0.35f);
        }
    }
}

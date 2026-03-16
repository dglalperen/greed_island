using GreedIsland.Core;
using UnityEngine;

namespace GreedIsland.Combat
{
    public static class ImpactResolver
    {
        public static bool TryApplyDamage(Collider collider, in DamagePayload payload)
        {
            var damageable = collider.GetComponentInParent<IDamageable>();
            if (damageable == null)
            {
                return false;
            }

            damageable.ReceiveDamage(in payload);
            return true;
        }

        public static int ApplyDamageInSphere(Vector3 center, float radius, LayerMask layerMask, in DamagePayload payload)
        {
            var colliders = Physics.OverlapSphere(center, radius, layerMask, QueryTriggerInteraction.Collide);
            var hits = 0;

            for (var i = 0; i < colliders.Length; i++)
            {
                if (!TryApplyDamage(colliders[i], in payload))
                {
                    continue;
                }

                hits++;
            }

            return hits;
        }

        public static void ApplyExplosionForce(Vector3 center, float radius, float force, LayerMask layerMask)
        {
            var colliders = Physics.OverlapSphere(center, radius, layerMask, QueryTriggerInteraction.Collide);
            for (var i = 0; i < colliders.Length; i++)
            {
                var rigidBody = colliders[i].attachedRigidbody;
                if (rigidBody == null)
                {
                    continue;
                }

                rigidBody.AddExplosionForce(force, center, radius, 0.2f, ForceMode.Impulse);
            }
        }
    }
}

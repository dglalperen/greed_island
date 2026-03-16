using GreedIsland.Core;
using UnityEngine;

namespace GreedIsland.Combat
{
    [RequireComponent(typeof(Collider))]
    public sealed class Hitbox : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float baseDamage = 10f;
        [SerializeField, Min(0f)] private float baseForce = 0f;
        [SerializeField] private GameObject owner;

        private void Reset()
        {
            var colliderReference = GetComponent<Collider>();
            colliderReference.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var damageable = other.GetComponentInParent<IDamageable>();
            if (damageable == null)
            {
                return;
            }

            var direction = other.transform.position - transform.position;
            var payload = DamagePayload.Create(baseDamage, owner, transform.position, direction, baseForce, false, "Hitbox");
            damageable.ReceiveDamage(in payload);
        }

        public void Configure(float damage, float force, GameObject ownerObject)
        {
            baseDamage = Mathf.Max(0f, damage);
            baseForce = Mathf.Max(0f, force);
            owner = ownerObject;
        }
    }
}

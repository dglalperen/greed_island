using UnityEngine;

namespace GreedIsland.Combat
{
    public sealed class ShieldComponent : MonoBehaviour
    {
        [SerializeField] private bool active;
        [SerializeField, Range(0f, 1f)] private float damageReduction = 0.45f;

        public bool IsActive => active;
        public float DamageReduction => damageReduction;

        public void SetActive(bool value)
        {
            active = value;
        }

        public void Configure(float reduction)
        {
            damageReduction = Mathf.Clamp01(reduction);
        }

        public float ApplyMitigation(float incomingDamage)
        {
            if (!active)
            {
                return incomingDamage;
            }

            return incomingDamage * (1f - damageReduction);
        }
    }
}

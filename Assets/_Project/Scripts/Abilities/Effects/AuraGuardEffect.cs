using GreedIsland.Combat;
using UnityEngine;

namespace GreedIsland.Abilities.Effects
{
    [CreateAssetMenu(menuName = "GreedIsland/Abilities/Effects/AuraGuard")]
    public sealed class AuraGuardEffect : AbilityEffect
    {
        [SerializeField, Range(0f, 1f)] private float damageReduction = 0.5f;

        public void SetDamageReduction(float value)
        {
            damageReduction = Mathf.Clamp01(value);
        }

        public override void Execute(in AbilityContext context)
        {
            if (context.CasterObject == null)
            {
                return;
            }

            var shield = context.CasterObject.GetComponent<ShieldComponent>();
            if (shield == null)
            {
                shield = context.CasterObject.AddComponent<ShieldComponent>();
            }

            shield.Configure(damageReduction);
            shield.SetActive(true);
        }

        public override void Stop(in AbilityContext context)
        {
            if (context.CasterObject == null)
            {
                return;
            }

            var shield = context.CasterObject.GetComponent<ShieldComponent>();
            if (shield != null)
            {
                shield.SetActive(false);
            }
        }
    }
}

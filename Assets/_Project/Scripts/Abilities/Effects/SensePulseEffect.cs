using GreedIsland.Core;
using UnityEngine;

namespace GreedIsland.Abilities.Effects
{
    [CreateAssetMenu(menuName = "GreedIsland/Abilities/Effects/SensePulse")]
    public sealed class SensePulseEffect : AbilityEffect
    {
        [SerializeField, Min(0f)] private float radius = 14f;
        [SerializeField, Min(0f)] private float revealDuration = 2.2f;
        [SerializeField] private LayerMask targetMask = ~0;

        public void Configure(float newRadius, float newRevealDuration, LayerMask newTargetMask)
        {
            radius = Mathf.Max(0f, newRadius);
            revealDuration = Mathf.Max(0f, newRevealDuration);
            targetMask = newTargetMask;
        }

        public override void Execute(in AbilityContext context)
        {
            if (context.Origin == null)
            {
                return;
            }

            var colliders = Physics.OverlapSphere(context.Origin.position, radius, targetMask, QueryTriggerInteraction.Collide);
            for (var i = 0; i < colliders.Length; i++)
            {
                var targetable = colliders[i].GetComponentInParent<ITargetable>();
                if (targetable == null || !targetable.IsTargetable)
                {
                    continue;
                }

                var revealer = targetable.TargetTransform.GetComponent<SenseRevealTarget>();
                if (revealer == null)
                {
                    revealer = targetable.TargetTransform.gameObject.AddComponent<SenseRevealTarget>();
                }

                revealer.Reveal(revealDuration);
            }

            Debug.DrawRay(context.Origin.position, Vector3.up * 2f, Color.yellow, 0.4f);
        }
    }
}

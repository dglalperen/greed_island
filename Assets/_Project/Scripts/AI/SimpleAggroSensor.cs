using GreedIsland.Core;
using UnityEngine;

namespace GreedIsland.AI
{
    public sealed class SimpleAggroSensor : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float aggroRadius = 12f;
        [SerializeField] private LayerMask targetMask = ~0;

        public ITargetable CurrentTarget { get; private set; }

        private void Update()
        {
            CurrentTarget = ResolveTarget();
        }

        private ITargetable ResolveTarget()
        {
            var colliders = Physics.OverlapSphere(transform.position, aggroRadius, targetMask, QueryTriggerInteraction.Collide);
            for (var i = 0; i < colliders.Length; i++)
            {
                var targetable = colliders[i].GetComponentInParent<ITargetable>();
                if (targetable != null && targetable.IsTargetable)
                {
                    return targetable;
                }
            }

            return null;
        }
    }
}

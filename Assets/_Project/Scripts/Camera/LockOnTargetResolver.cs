using GreedIsland.Core;
using UnityEngine;

namespace GreedIsland.Camera
{
    public sealed class LockOnTargetResolver : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float searchRadius = 20f;
        [SerializeField] private LayerMask targetMask = ~0;

        public ITargetable ResolveBestTarget(Vector3 position, Vector3 forward)
        {
            var colliders = Physics.OverlapSphere(position, searchRadius, targetMask, QueryTriggerInteraction.Collide);
            if (colliders.Length == 0)
            {
                return null;
            }

            ITargetable bestTarget = null;
            var bestScore = float.NegativeInfinity;

            for (var i = 0; i < colliders.Length; i++)
            {
                var targetable = colliders[i].GetComponentInParent<ITargetable>();
                if (targetable == null || !targetable.IsTargetable)
                {
                    continue;
                }

                var targetDirection = targetable.TargetTransform.position - position;
                targetDirection.y = 0f;
                var distance = targetDirection.magnitude;
                if (distance <= 0.001f)
                {
                    continue;
                }

                var alignment = Vector3.Dot(forward.normalized, targetDirection / distance);
                var score = alignment * 2f - distance * 0.06f;
                if (score <= bestScore)
                {
                    continue;
                }

                bestScore = score;
                bestTarget = targetable;
            }

            return bestTarget;
        }
    }
}

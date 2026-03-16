using System.Collections.Generic;
using GreedIsland.Core;
using UnityEngine;

namespace GreedIsland.Abilities
{
    public static class AbilityTargeting
    {
        public static void ResolveTargets(AbilityDefinition definition, ref AbilityContext context)
        {
            context.ResolvedTargets ??= new List<ITargetable>(8);
            context.ResolvedTargets.Clear();

            if (definition == null)
            {
                return;
            }

            switch (definition.TargetingType)
            {
                case AbilityTargetingType.Self:
                    AddSelfTarget(ref context);
                    break;
                case AbilityTargetingType.TargetLock:
                    if (context.CurrentTarget != null && context.CurrentTarget.IsTargetable)
                    {
                        context.ResolvedTargets.Add(context.CurrentTarget);
                    }
                    break;
                case AbilityTargetingType.ForwardRay:
                    ResolveForwardRay(definition, ref context);
                    break;
                case AbilityTargetingType.SphereAroundSelf:
                    ResolveSphere(definition, ref context);
                    break;
                case AbilityTargetingType.OverlapBox:
                    ResolveOverlapBox(definition, ref context);
                    break;
            }
        }

        private static void AddSelfTarget(ref AbilityContext context)
        {
            if (context.CasterObject == null)
            {
                return;
            }

            var targetable = context.CasterObject.GetComponentInParent<ITargetable>();
            if (targetable != null && targetable.IsTargetable)
            {
                context.ResolvedTargets.Add(targetable);
            }
        }

        private static void ResolveForwardRay(AbilityDefinition definition, ref AbilityContext context)
        {
            if (context.Origin == null)
            {
                return;
            }

            var origin = context.Origin.position;
            var direction = context.Direction.sqrMagnitude > 0.0001f ? context.Direction.normalized : context.Origin.forward;
            if (!Physics.Raycast(origin, direction, out var hit, definition.Range, definition.TargetMask, QueryTriggerInteraction.Collide))
            {
                return;
            }

            var targetable = hit.collider.GetComponentInParent<ITargetable>();
            if (targetable != null && targetable.IsTargetable)
            {
                context.ResolvedTargets.Add(targetable);
            }
        }

        private static void ResolveSphere(AbilityDefinition definition, ref AbilityContext context)
        {
            if (context.Origin == null)
            {
                return;
            }

            var colliders = Physics.OverlapSphere(context.Origin.position, definition.Radius, definition.TargetMask, QueryTriggerInteraction.Collide);
            for (var i = 0; i < colliders.Length; i++)
            {
                var targetable = colliders[i].GetComponentInParent<ITargetable>();
                if (targetable == null || !targetable.IsTargetable)
                {
                    continue;
                }

                if (!context.ResolvedTargets.Contains(targetable))
                {
                    context.ResolvedTargets.Add(targetable);
                }
            }
        }

        private static void ResolveOverlapBox(AbilityDefinition definition, ref AbilityContext context)
        {
            if (context.Origin == null)
            {
                return;
            }

            var halfExtents = new Vector3(definition.Radius, definition.Radius, definition.Range * 0.5f);
            var center = context.Origin.position + context.Direction.normalized * definition.Range * 0.5f;
            var colliders = Physics.OverlapBox(center, halfExtents, context.Origin.rotation, definition.TargetMask, QueryTriggerInteraction.Collide);

            for (var i = 0; i < colliders.Length; i++)
            {
                var targetable = colliders[i].GetComponentInParent<ITargetable>();
                if (targetable == null || !targetable.IsTargetable)
                {
                    continue;
                }

                if (!context.ResolvedTargets.Contains(targetable))
                {
                    context.ResolvedTargets.Add(targetable);
                }
            }
        }
    }
}

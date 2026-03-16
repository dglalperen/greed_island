using System;
using System.Collections.Generic;
using GreedIsland.Aura;
using GreedIsland.Camera;
using GreedIsland.Character;
using GreedIsland.Core;
using UnityEngine;

namespace GreedIsland.Abilities
{
    public sealed class AbilityRunner : MonoBehaviour
    {
        [SerializeField] private List<AbilitySlot> equippedSlots = new();
        [SerializeField] private PlayerBrain playerBrain;
        [SerializeField] private PlayerAnimatorController animatorController;
        [SerializeField] private CameraTargetProvider cameraTargetProvider;

        private readonly Dictionary<int, AbilityCooldown> cooldowns = new();
        private readonly Dictionary<int, AbilityExecution> activeExecutions = new();

        private IAbilityUser abilityUser;

        public event Action<int, AbilityDefinition> AbilityExecuted;
        public event Action<int, AbilityDefinition, string> AbilityFailed;
        public event Action<int, AbilityDefinition> AbilityStopped;

        public IReadOnlyList<AbilitySlot> EquippedSlots => equippedSlots;

        private void Awake()
        {
            playerBrain ??= GetComponent<PlayerBrain>();
            animatorController ??= GetComponent<PlayerAnimatorController>();
            cameraTargetProvider ??= GetComponent<CameraTargetProvider>();

            abilityUser = playerBrain as IAbilityUser;
            if (abilityUser == null)
            {
                abilityUser = GetComponent<IAbilityUser>();
            }

            if (abilityUser == null)
            {
                Debug.LogError("AbilityRunner requires a component implementing IAbilityUser.", this);
            }
        }

        private void Update()
        {
            if (activeExecutions.Count == 0)
            {
                return;
            }

            var toStop = ListPool<int>.Get();

            foreach (var pair in activeExecutions)
            {
                var execution = pair.Value;
                var definition = execution.Definition;
                if (definition == null)
                {
                    toStop.Add(pair.Key);
                    continue;
                }

                var upkeep = execution.GetUpkeepThisFrame(Time.deltaTime, abilityUser?.AuraController);
                if (upkeep > 0f)
                {
                    if (abilityUser?.AuraController == null || !abilityUser.AuraController.Spend(upkeep))
                    {
                        toStop.Add(pair.Key);
                        continue;
                    }
                }

                if (execution.ShouldEndByDuration(Time.time))
                {
                    toStop.Add(pair.Key);
                }
            }

            for (var i = 0; i < toStop.Count; i++)
            {
                StopActiveAbility(toStop[i]);
            }

            ListPool<int>.Release(toStop);
        }

        public bool TryActivateSlot(int slotId)
        {
            var slot = FindSlot(slotId);
            if (slot == null || slot.Definition == null)
            {
                AbilityFailed?.Invoke(slotId, null, "No ability equipped.");
                return false;
            }

            var definition = slot.Definition;
            if (!IsCooldownReady(slotId))
            {
                AbilityFailed?.Invoke(slotId, definition, "Ability on cooldown.");
                return false;
            }

            if (definition.ActivationType == AbilityActivationType.Toggle && activeExecutions.ContainsKey(slotId))
            {
                StopActiveAbility(slotId);
                return true;
            }

            var context = BuildContext(definition);
            AbilityTargeting.ResolveTargets(definition, ref context);

            if (definition.RequireTarget && (context.CurrentTarget == null || !context.CurrentTarget.IsTargetable) && context.ResolvedTargets.Count == 0)
            {
                AbilityFailed?.Invoke(slotId, definition, "Target required.");
                return false;
            }

            var conditions = definition.Conditions;
            for (var i = 0; i < conditions.Length; i++)
            {
                var condition = conditions[i];
                if (condition == null)
                {
                    continue;
                }

                if (condition.CanActivate(in context, out var reason))
                {
                    continue;
                }

                AbilityFailed?.Invoke(slotId, definition, string.IsNullOrWhiteSpace(reason) ? "Condition failed." : reason);
                return false;
            }

            var auraCost = ResolveAuraCost(definition, context.AuraController);
            if (auraCost > 0f)
            {
                if (context.AuraController == null || !context.AuraController.Spend(auraCost))
                {
                    AbilityFailed?.Invoke(slotId, definition, "Not enough aura.");
                    return false;
                }
            }

            var effects = definition.Effects;
            for (var i = 0; i < effects.Length; i++)
            {
                effects[i]?.Execute(in context);
            }

            StartCooldown(slotId, definition.CooldownSeconds);
            animatorController?.TriggerAbilityCast();
            AbilityExecuted?.Invoke(slotId, definition);

            if (definition.ActivationType == AbilityActivationType.Toggle || definition.ActivationType == AbilityActivationType.Channeled)
            {
                activeExecutions[slotId] = new AbilityExecution(slotId, definition, Time.time, context);
            }

            return true;
        }

        public float GetCooldownRemaining(int slotId)
        {
            if (!cooldowns.TryGetValue(slotId, out var cooldown))
            {
                return 0f;
            }

            return cooldown.Remaining;
        }

        public void SetEquippedSlots(IEnumerable<AbilitySlot> slots)
        {
            equippedSlots.Clear();
            if (slots == null)
            {
                return;
            }

            foreach (var slot in slots)
            {
                if (slot == null)
                {
                    continue;
                }

                equippedSlots.Add(slot);
            }
        }

        private AbilitySlot FindSlot(int slotId)
        {
            for (var i = 0; i < equippedSlots.Count; i++)
            {
                if (equippedSlots[i].SlotId == slotId)
                {
                    return equippedSlots[i];
                }
            }

            return null;
        }

        private bool IsCooldownReady(int slotId)
        {
            if (!cooldowns.TryGetValue(slotId, out var cooldown))
            {
                return true;
            }

            return cooldown.IsReady;
        }

        private void StartCooldown(int slotId, float duration)
        {
            if (!cooldowns.TryGetValue(slotId, out var cooldown))
            {
                cooldown = new AbilityCooldown();
                cooldowns[slotId] = cooldown;
            }

            cooldown.Start(duration);
        }

        private AbilityContext BuildContext(AbilityDefinition definition)
        {
            var origin = cameraTargetProvider != null && cameraTargetProvider.AbilityAimTarget != null
                ? cameraTargetProvider.AbilityAimTarget
                : transform;

            var direction = origin.forward;
            var lockTarget = playerBrain != null ? playerBrain.CurrentLockTarget : null;
            if (lockTarget != null && lockTarget.IsTargetable)
            {
                var targetDirection = lockTarget.TargetTransform.position - origin.position;
                if (targetDirection.sqrMagnitude > 0.0001f)
                {
                    direction = targetDirection.normalized;
                }
            }

            var context = new AbilityContext
            {
                CasterObject = gameObject,
                Caster = abilityUser,
                Origin = origin,
                Direction = direction,
                CurrentTarget = lockTarget,
                AuraController = abilityUser?.AuraController,
                Stats = abilityUser?.Stats,
                TimeStamp = Time.time,
                ChargeAmount = 0f,
                PowerMultiplier = ResolvePowerMultiplier(definition, abilityUser?.AuraController)
            };

            return context;
        }

        private float ResolveAuraCost(AbilityDefinition definition, AuraController auraController)
        {
            if (definition == null)
            {
                return 0f;
            }

            if (auraController == null)
            {
                return definition.AuraCost;
            }

            return definition.AuraCost * auraController.GetCostMultiplier(definition.AffinityType);
        }

        private float ResolvePowerMultiplier(AbilityDefinition definition, AuraController auraController)
        {
            if (definition == null)
            {
                return 1f;
            }

            if (auraController == null)
            {
                return 1f;
            }

            return auraController.GetPowerMultiplier(definition.AffinityType);
        }

        private void StopActiveAbility(int slotId)
        {
            if (!activeExecutions.TryGetValue(slotId, out var execution))
            {
                return;
            }

            activeExecutions.Remove(slotId);
            var definition = execution.Definition;
            if (definition != null)
            {
                var effects = definition.Effects;
                var context = execution.Context;
                for (var i = 0; i < effects.Length; i++)
                {
                    effects[i]?.Stop(in context);
                }
            }

            AbilityStopped?.Invoke(slotId, definition);
        }

        private static class ListPool<T>
        {
            private static readonly Stack<List<T>> Pool = new();

            public static List<T> Get()
            {
                return Pool.Count > 0 ? Pool.Pop() : new List<T>(8);
            }

            public static void Release(List<T> list)
            {
                list.Clear();
                Pool.Push(list);
            }
        }
    }
}

using System.Collections.Generic;
using GreedIsland.Abilities.Conditions;
using GreedIsland.Abilities.Effects;
using GreedIsland.Aura;
using UnityEngine;

namespace GreedIsland.Abilities
{
    [DisallowMultipleComponent]
    public sealed class PrototypeAbilityLoadout : MonoBehaviour
    {
        [SerializeField] private AbilityRunner abilityRunner;

        private void Awake()
        {
            if (abilityRunner == null)
            {
                abilityRunner = GetComponent<AbilityRunner>();
            }

            if (abilityRunner == null)
            {
                return;
            }

            if (abilityRunner.EquippedSlots != null && abilityRunner.EquippedSlots.Count > 0)
            {
                return;
            }

            abilityRunner.SetEquippedSlots(CreatePrototypeSlots());
        }

        private static IEnumerable<AbilitySlot> CreatePrototypeSlots()
        {
            var slots = new List<AbilitySlot>(3)
            {
                AbilitySlot.Create(1, CreateAuraBurstDefinition()),
                AbilitySlot.Create(2, CreateAuraGuardDefinition()),
                AbilitySlot.Create(3, CreateSensePulseDefinition())
            };

            return slots;
        }

        private static AbilityDefinition CreateAuraBurstDefinition()
        {
            var definition = ScriptableObject.CreateInstance<AbilityDefinition>();
            definition.hideFlags = HideFlags.HideAndDontSave;

            var effect = ScriptableObject.CreateInstance<AuraBurstEffect>();
            effect.hideFlags = HideFlags.HideAndDontSave;
            effect.Configure(
                newDamage: 24f,
                newForce: 9f,
                newRadius: 3.8f,
                newForwardOffset: 1.25f,
                newDamageMask: LayerMask.GetMask("Enemy", "Target", "Default", "Hurtbox"),
                newRigidbodyMask: LayerMask.GetMask("WorldProp", "Default", "Interactable"));

            var auraCondition = ScriptableObject.CreateInstance<EnoughAuraCondition>();
            auraCondition.hideFlags = HideFlags.HideAndDontSave;
            auraCondition.SetMinimumAura(20f);

            definition.ConfigurePrototype(
                newId: "ability.aura_burst",
                newDisplayName: "Aura Burst",
                newDescription: "Short-range offensive burst that damages targets and pushes rigidbodies.",
                newAffinity: AffinityType.Projection,
                newActivation: AbilityActivationType.Instant,
                newTargeting: AbilityTargetingType.SphereAroundSelf,
                newCooldown: 1.4f,
                newAuraCost: 20f,
                newAuraUpkeep: 0f,
                newRange: 0f,
                newRadius: 4f,
                newRequireTarget: false,
                newConditions: new AbilityCondition[] { auraCondition },
                newEffects: new AbilityEffect[] { effect });
            definition.SetDebugColor(new Color(0.95f, 0.35f, 0.25f, 1f));

            return definition;
        }

        private static AbilityDefinition CreateAuraGuardDefinition()
        {
            var definition = ScriptableObject.CreateInstance<AbilityDefinition>();
            definition.hideFlags = HideFlags.HideAndDontSave;

            var effect = ScriptableObject.CreateInstance<AuraGuardEffect>();
            effect.hideFlags = HideFlags.HideAndDontSave;
            effect.SetDamageReduction(0.55f);

            var auraCondition = ScriptableObject.CreateInstance<EnoughAuraCondition>();
            auraCondition.hideFlags = HideFlags.HideAndDontSave;
            auraCondition.SetMinimumAura(10f);

            definition.ConfigurePrototype(
                newId: "ability.aura_guard",
                newDisplayName: "Aura Guard",
                newDescription: "Toggle defensive guard that reduces incoming damage while draining aura.",
                newAffinity: AffinityType.Reinforcement,
                newActivation: AbilityActivationType.Toggle,
                newTargeting: AbilityTargetingType.Self,
                newCooldown: 0.2f,
                newAuraCost: 8f,
                newAuraUpkeep: 9f,
                newRange: 0f,
                newRadius: 0f,
                newRequireTarget: false,
                newConditions: new AbilityCondition[] { auraCondition },
                newEffects: new AbilityEffect[] { effect });
            definition.SetDebugColor(new Color(0.35f, 0.75f, 1f, 1f));

            return definition;
        }

        private static AbilityDefinition CreateSensePulseDefinition()
        {
            var definition = ScriptableObject.CreateInstance<AbilityDefinition>();
            definition.hideFlags = HideFlags.HideAndDontSave;

            var effect = ScriptableObject.CreateInstance<SensePulseEffect>();
            effect.hideFlags = HideFlags.HideAndDontSave;
            effect.Configure(12.5f, 2.5f, LayerMask.GetMask("Enemy", "Target", "Default"));

            var auraCondition = ScriptableObject.CreateInstance<EnoughAuraCondition>();
            auraCondition.hideFlags = HideFlags.HideAndDontSave;
            auraCondition.SetMinimumAura(16f);

            definition.ConfigurePrototype(
                newId: "ability.sense_pulse",
                newDisplayName: "Sense Pulse",
                newDescription: "Pulse that reveals nearby targets for a short duration.",
                newAffinity: AffinityType.Control,
                newActivation: AbilityActivationType.Instant,
                newTargeting: AbilityTargetingType.SphereAroundSelf,
                newCooldown: 4.5f,
                newAuraCost: 16f,
                newAuraUpkeep: 0f,
                newRange: 0f,
                newRadius: 12.5f,
                newRequireTarget: false,
                newConditions: new AbilityCondition[] { auraCondition },
                newEffects: new AbilityEffect[] { effect });
            definition.SetDebugColor(new Color(1f, 0.95f, 0.2f, 1f));

            return definition;
        }
    }
}

using GreedIsland.Aura;
using UnityEngine;

namespace GreedIsland.Abilities
{
    [CreateAssetMenu(menuName = "GreedIsland/Abilities/AbilityDefinition")]
    public sealed class AbilityDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string id = "ability.id";
        [SerializeField] private string displayName = "New Ability";
        [TextArea(2, 4)]
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private AffinityType affinityType = AffinityType.Reinforcement;

        [Header("Activation")]
        [SerializeField] private AbilityActivationType activationType = AbilityActivationType.Instant;
        [SerializeField] private AbilityCastPolicy castPolicy = AbilityCastPolicy.AllowWhileMoving;
        [SerializeField, Min(0f)] private float cooldownSeconds = 1f;
        [SerializeField, Min(0f)] private float castTimeSeconds;
        [SerializeField, Min(0f)] private float channelDuration;

        [Header("Costs")]
        [SerializeField, Min(0f)] private float auraCost = 15f;
        [SerializeField, Min(0f)] private float auraUpkeepPerSecond;

        [Header("Targeting")]
        [SerializeField] private AbilityTargetingType targetingType = AbilityTargetingType.SphereAroundSelf;
        [SerializeField, Min(0f)] private float range = 8f;
        [SerializeField, Min(0f)] private float radius = 4f;
        [SerializeField] private bool requireTarget;
        [SerializeField] private LayerMask targetMask = ~0;

        [Header("Presentation")]
        [SerializeField] private string animationTriggerName = "AbilityTrigger";
        [SerializeField] private GameObject vfxPrefab;
        [SerializeField] private AudioClip sfxClip;
        [SerializeField] private Color debugColor = Color.cyan;

        [Header("Composition")]
        [SerializeField] private AbilityCondition[] conditions = { };
        [SerializeField] private AbilityEffect[] effects = { };

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public AffinityType AffinityType => affinityType;
        public AbilityActivationType ActivationType => activationType;
        public AbilityCastPolicy CastPolicy => castPolicy;
        public float CooldownSeconds => cooldownSeconds;
        public float CastTimeSeconds => castTimeSeconds;
        public float ChannelDuration => channelDuration;
        public float AuraCost => auraCost;
        public float AuraUpkeepPerSecond => auraUpkeepPerSecond;
        public AbilityTargetingType TargetingType => targetingType;
        public float Range => range;
        public float Radius => radius;
        public bool RequireTarget => requireTarget;
        public LayerMask TargetMask => targetMask;
        public string AnimationTriggerName => animationTriggerName;
        public GameObject VfxPrefab => vfxPrefab;
        public AudioClip SfxClip => sfxClip;
        public Color DebugColor => debugColor;
        public AbilityCondition[] Conditions => conditions;
        public AbilityEffect[] Effects => effects;

        public void ConfigurePrototype(
            string newId,
            string newDisplayName,
            string newDescription,
            AffinityType newAffinity,
            AbilityActivationType newActivation,
            AbilityTargetingType newTargeting,
            float newCooldown,
            float newAuraCost,
            float newAuraUpkeep,
            float newRange,
            float newRadius,
            bool newRequireTarget,
            AbilityCondition[] newConditions,
            AbilityEffect[] newEffects)
        {
            id = newId;
            displayName = newDisplayName;
            description = newDescription;
            affinityType = newAffinity;
            activationType = newActivation;
            targetingType = newTargeting;
            cooldownSeconds = Mathf.Max(0f, newCooldown);
            auraCost = Mathf.Max(0f, newAuraCost);
            auraUpkeepPerSecond = Mathf.Max(0f, newAuraUpkeep);
            range = Mathf.Max(0f, newRange);
            radius = Mathf.Max(0f, newRadius);
            requireTarget = newRequireTarget;
            conditions = newConditions ?? System.Array.Empty<AbilityCondition>();
            effects = newEffects ?? System.Array.Empty<AbilityEffect>();
        }

        public void SetDebugColor(Color color)
        {
            debugColor = color;
        }

        public void SetTargetMask(LayerMask mask)
        {
            targetMask = mask;
        }
    }
}

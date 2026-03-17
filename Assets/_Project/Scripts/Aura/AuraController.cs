using System;
using GreedIsland.Abilities;
using GreedIsland.Core;
using GreedIsland.Stats;
using UnityEngine;

namespace GreedIsland.Aura
{
    public enum NenTechnique
    {
        Ten = 0,
        Zetsu = 1,
        Ren = 2,
        Gyo = 3,
        En = 4,
        In = 5,
        Shu = 6,
        Ko = 7,
        Ken = 8,
        Ryu = 9
    }

    public sealed class AuraController : MonoBehaviour
    {
        private static readonly NenTechnique[] TechniqueCycle =
        {
            NenTechnique.Ten,
            NenTechnique.Zetsu,
            NenTechnique.Ren,
            NenTechnique.Gyo,
            NenTechnique.En,
            NenTechnique.In,
            NenTechnique.Shu,
            NenTechnique.Ko,
            NenTechnique.Ken,
            NenTechnique.Ryu
        };

        [SerializeField] private AuraPool auraPool;
        [SerializeField] private AffinityProfile affinityProfile;
        [SerializeField] private AuraSignatureProfile signatureProfile;
        [SerializeField] private NenTechnique currentTechnique = NenTechnique.Ten;
        [SerializeField] private NenTechniqueRuntimeConfig[] techniqueConfigs =
        {
            NenTechniqueRuntimeConfig.Create(
                NenTechnique.Ten, AuraMode.Neutral, 1f, 0f, 1.15f, 1f, 1f, 1f, true, true, 1f, 0f, 0f, 0f,
                "Basic aura shroud. Stable defense and retention."),
            NenTechniqueRuntimeConfig.Create(
                NenTechnique.Zetsu, AuraMode.Concealment, 1.2f, 0f, 0.55f, 0.6f, 1.04f, 0.2f, false, false, 0f, 0f, 0f, 0f,
                "Suppress aura output. Hard to detect, but vulnerable."),
            NenTechniqueRuntimeConfig.Create(
                NenTechnique.Ren, AuraMode.Expansion, 0.35f, 5.5f, 0.95f, 1.4f, 1.03f, 1.3f, true, true, 1.35f, 0f, 0f, 0f,
                "Amplify aura output. Higher power with heavy drain."),
            NenTechniqueRuntimeConfig.Create(
                NenTechnique.Gyo, AuraMode.Perception, 0.7f, 2f, 1f, 1.08f, 0.97f, 1.55f, true, true, 1.1f, 8f, 0.9f, 0.75f,
                "Focus aura for precision sensing and striking."),
            NenTechniqueRuntimeConfig.Create(
                NenTechnique.En, AuraMode.Perception, 0.5f, 4.8f, 0.95f, 1f, 0.9f, 1.9f, true, true, 1.2f, 14f, 1.2f, 1.1f,
                "Expand aura field to detect nearby targets."),
            NenTechniqueRuntimeConfig.Create(
                NenTechnique.In, AuraMode.Concealment, 0.8f, 2.2f, 0.9f, 1f, 1f, 0.45f, true, true, 0.72f, 0f, 0f, 0f,
                "Conceal aura presence while keeping aura active."),
            NenTechniqueRuntimeConfig.Create(
                NenTechnique.Shu, AuraMode.Reinforcement, 0.75f, 2.8f, 1.12f, 1.15f, 1f, 1f, true, true, 1.05f, 0f, 0f, 0f,
                "Extend aura into objects to reinforce and empower them."),
            NenTechniqueRuntimeConfig.Create(
                NenTechnique.Ko, AuraMode.Expansion, 0.1f, 7.2f, 0.45f, 1.85f, 0.9f, 1.1f, true, true, 1.5f, 0f, 0f, 0f,
                "Concentrate aura into one point for massive offense."),
            NenTechniqueRuntimeConfig.Create(
                NenTechnique.Ken, AuraMode.Reinforcement, 0.55f, 4f, 1.4f, 1f, 0.92f, 1f, true, true, 1.15f, 0f, 0f, 0f,
                "Sustained defensive aura state for long engagements."),
            NenTechniqueRuntimeConfig.Create(
                NenTechnique.Ryu, AuraMode.Reinforcement, 0.65f, 3.2f, 1.18f, 1.18f, 1.02f, 1.2f, true, true, 1.2f, 0f, 0f, 0f,
                "Dynamic aura distribution for adaptive combat.")
        };

        private NenTechniqueRuntimeConfig activeConfig;
        private float nextDetectionPulseAt;

        public event Action<AuraMode> ModeChanged;
        public event Action<NenTechnique> TechniqueChanged;

        public AuraMode CurrentMode => activeConfig.Mode;
        public NenTechnique CurrentTechnique => currentTechnique;
        public AuraPool AuraPool => auraPool;
        public AffinityProfile AffinityProfile => affinityProfile;
        public AuraSignatureProfile SignatureProfile => signatureProfile;

        public float DefenseMultiplier => activeConfig.DefenseMultiplier;
        public float OutgoingPowerMultiplier => activeConfig.OutgoingPowerMultiplier;
        public float MovementMultiplier => activeConfig.MovementMultiplier;
        public float DetectionMultiplier => activeConfig.DetectionMultiplier;
        public float RegenMultiplier => activeConfig.RegenMultiplier;
        public float UpkeepPerSecond => activeConfig.UpkeepPerSecond;
        public bool CanUseAbilities => activeConfig.CanUseAbilities;
        public bool IsAuraVisible => activeConfig.AuraVisible;
        public float AuraVisualIntensity => activeConfig.VisualIntensity;
        public string TechniqueDescription => activeConfig.Description;

        private void Awake()
        {
            if (auraPool == null)
            {
                auraPool = GetComponent<AuraPool>();
            }

            activeConfig = ResolveTechniqueConfig(currentTechnique);
            ApplyConfig();
        }

        private void Update()
        {
            if (auraPool == null)
            {
                return;
            }

            var upkeep = activeConfig.UpkeepPerSecond * Time.deltaTime;
            if (upkeep > 0f)
            {
                if (!auraPool.Spend(upkeep))
                {
                    SetTechnique(NenTechnique.Ten);
                    return;
                }
            }

            TickDetectionPulse();
        }

        public bool SetMode(AuraMode mode)
        {
            return SetTechnique(MapModeToTechnique(mode));
        }

        public bool SetTechnique(NenTechnique technique)
        {
            if (currentTechnique == technique)
            {
                return false;
            }

            currentTechnique = technique;
            activeConfig = ResolveTechniqueConfig(currentTechnique);
            ApplyConfig();
            TechniqueChanged?.Invoke(currentTechnique);
            ModeChanged?.Invoke(CurrentMode);
            return true;
        }

        public NenTechnique CycleTechnique()
        {
            var currentIndex = 0;
            for (var i = 0; i < TechniqueCycle.Length; i++)
            {
                if (TechniqueCycle[i] != currentTechnique)
                {
                    continue;
                }

                currentIndex = i;
                break;
            }

            var nextIndex = (currentIndex + 1) % TechniqueCycle.Length;
            SetTechnique(TechniqueCycle[nextIndex]);
            return currentTechnique;
        }

        public bool HasEnough(float amount)
        {
            return auraPool != null && auraPool.CanAfford(amount);
        }

        public bool Spend(float amount)
        {
            if (auraPool == null)
            {
                return false;
            }

            return auraPool.Spend(amount);
        }

        public void Restore(float amount)
        {
            auraPool?.Restore(amount);
        }

        public float GetCostMultiplier(AffinityType affinity)
        {
            if (affinityProfile == null)
            {
                return 1f;
            }

            return affinityProfile.GetEfficiency(affinity).costMultiplier;
        }

        public float GetPowerMultiplier(AffinityType affinity)
        {
            if (affinityProfile == null)
            {
                return 1f;
            }

            return affinityProfile.GetEfficiency(affinity).powerMultiplier * activeConfig.OutgoingPowerMultiplier;
        }

        private NenTechniqueRuntimeConfig ResolveTechniqueConfig(NenTechnique technique)
        {
            for (var i = 0; i < techniqueConfigs.Length; i++)
            {
                if (techniqueConfigs[i].Technique == technique)
                {
                    return techniqueConfigs[i];
                }
            }

            return NenTechniqueRuntimeConfig.Create(
                NenTechnique.Ten, AuraMode.Neutral, 1f, 0f, 1.15f, 1f, 1f, 1f, true, true, 1f, 0f, 0f, 0f,
                "Basic aura shroud. Stable defense and retention.");
        }

        private void ApplyConfig()
        {
            if (auraPool == null)
            {
                return;
            }

            auraPool.SetRegenMultiplier(activeConfig.RegenMultiplier);
        }

        private void TickDetectionPulse()
        {
            if (activeConfig.PulseRadius <= 0f || activeConfig.PulseInterval <= 0f)
            {
                return;
            }

            if (Time.time < nextDetectionPulseAt)
            {
                return;
            }

            nextDetectionPulseAt = Time.time + activeConfig.PulseInterval;

            var mask = LayerMask.GetMask("Enemy", "Target", "Default");
            var colliders = Physics.OverlapSphere(transform.position, activeConfig.PulseRadius, mask, QueryTriggerInteraction.Collide);
            for (var i = 0; i < colliders.Length; i++)
            {
                var targetable = colliders[i].GetComponentInParent<ITargetable>();
                if (targetable == null || !targetable.IsTargetable)
                {
                    continue;
                }

                var reveal = targetable.TargetTransform.GetComponent<SenseRevealTarget>();
                if (reveal == null)
                {
                    reveal = targetable.TargetTransform.gameObject.AddComponent<SenseRevealTarget>();
                }

                reveal.Reveal(activeConfig.PulseRevealDuration);
            }

            Debug.DrawRay(transform.position, Vector3.up * 2f, Color.white, 0.2f);
        }

        private static NenTechnique MapModeToTechnique(AuraMode mode)
        {
            return mode switch
            {
                AuraMode.Concealment => NenTechnique.In,
                AuraMode.Reinforcement => NenTechnique.Ken,
                AuraMode.Expansion => NenTechnique.Ren,
                AuraMode.Perception => NenTechnique.En,
                _ => NenTechnique.Ten
            };
        }

        [Serializable]
        public struct NenTechniqueRuntimeConfig
        {
            [SerializeField] private NenTechnique technique;
            [SerializeField] private AuraMode mode;
            [SerializeField, Min(0f)] private float regenMultiplier;
            [SerializeField, Min(0f)] private float upkeepPerSecond;
            [SerializeField, Min(0.01f)] private float defenseMultiplier;
            [SerializeField, Min(0.01f)] private float outgoingPowerMultiplier;
            [SerializeField, Min(0.01f)] private float movementMultiplier;
            [SerializeField, Min(0.01f)] private float detectionMultiplier;
            [SerializeField] private bool canUseAbilities;
            [SerializeField] private bool auraVisible;
            [SerializeField, Min(0f)] private float visualIntensity;
            [SerializeField, Min(0f)] private float pulseRadius;
            [SerializeField, Min(0f)] private float pulseInterval;
            [SerializeField, Min(0f)] private float pulseRevealDuration;
            [SerializeField] private string description;

            public NenTechnique Technique => technique;
            public AuraMode Mode => mode;
            public float RegenMultiplier => regenMultiplier;
            public float UpkeepPerSecond => upkeepPerSecond;
            public float DefenseMultiplier => defenseMultiplier;
            public float OutgoingPowerMultiplier => outgoingPowerMultiplier;
            public float MovementMultiplier => movementMultiplier;
            public float DetectionMultiplier => detectionMultiplier;
            public bool CanUseAbilities => canUseAbilities;
            public bool AuraVisible => auraVisible;
            public float VisualIntensity => visualIntensity;
            public float PulseRadius => pulseRadius;
            public float PulseInterval => pulseInterval;
            public float PulseRevealDuration => pulseRevealDuration;
            public string Description => description;

            public static NenTechniqueRuntimeConfig Create(
                NenTechnique techniqueValue,
                AuraMode modeValue,
                float regen,
                float upkeep,
                float defense,
                float power,
                float movement,
                float detection,
                bool allowAbilities,
                bool showAura,
                float visualStrength,
                float detectRadius,
                float detectInterval,
                float detectRevealDuration,
                string text)
            {
                return new NenTechniqueRuntimeConfig
                {
                    technique = techniqueValue,
                    mode = modeValue,
                    regenMultiplier = regen,
                    upkeepPerSecond = upkeep,
                    defenseMultiplier = defense,
                    outgoingPowerMultiplier = power,
                    movementMultiplier = movement,
                    detectionMultiplier = detection,
                    canUseAbilities = allowAbilities,
                    auraVisible = showAura,
                    visualIntensity = visualStrength,
                    pulseRadius = detectRadius,
                    pulseInterval = detectInterval,
                    pulseRevealDuration = detectRevealDuration,
                    description = text
                };
            }
        }
    }
}

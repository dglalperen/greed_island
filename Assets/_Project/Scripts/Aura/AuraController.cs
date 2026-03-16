using System;
using GreedIsland.Stats;
using UnityEngine;

namespace GreedIsland.Aura
{
    public sealed class AuraController : MonoBehaviour
    {
        [SerializeField] private AuraPool auraPool;
        [SerializeField] private AffinityProfile affinityProfile;
        [SerializeField] private AuraSignatureProfile signatureProfile;
        [SerializeField] private AuraModeRuntimeConfig[] modeConfigs =
        {
            AuraModeRuntimeConfig.Create(AuraMode.Neutral, 1f, 0f, 1f, 1f, 1f),
            AuraModeRuntimeConfig.Create(AuraMode.Concealment, 0.9f, 1.5f, 1f, 0.95f, 1.25f),
            AuraModeRuntimeConfig.Create(AuraMode.Reinforcement, 0.6f, 4f, 0.75f, 0.95f, 1f),
            AuraModeRuntimeConfig.Create(AuraMode.Expansion, 0.5f, 5f, 1f, 1.2f, 0.95f),
            AuraModeRuntimeConfig.Create(AuraMode.Perception, 0.7f, 3f, 1f, 0.9f, 1.4f)
        };

        private readonly AuraStateMachine stateMachine = new();
        private AuraModeRuntimeConfig activeConfig;

        public event Action<AuraMode> ModeChanged;

        public AuraMode CurrentMode => stateMachine.CurrentMode;
        public AuraPool AuraPool => auraPool;
        public AffinityProfile AffinityProfile => affinityProfile;
        public AuraSignatureProfile SignatureProfile => signatureProfile;

        public float DefenseMultiplier => activeConfig.DefenseMultiplier;
        public float OutgoingPowerMultiplier => activeConfig.OutgoingPowerMultiplier;
        public float MovementMultiplier => activeConfig.MovementMultiplier;
        public float DetectionMultiplier => activeConfig.DetectionMultiplier;

        private void Awake()
        {
            if (auraPool == null)
            {
                auraPool = GetComponent<AuraPool>();
            }

            activeConfig = ResolveConfig(stateMachine.CurrentMode);
            ApplyConfigToPool(activeConfig);
        }

        private void Update()
        {
            if (auraPool == null)
            {
                return;
            }

            var upkeep = activeConfig.UpkeepPerSecond * Time.deltaTime;
            if (upkeep <= 0f)
            {
                return;
            }

            if (!auraPool.Spend(upkeep))
            {
                SetMode(AuraMode.Neutral);
            }
        }

        public bool SetMode(AuraMode mode)
        {
            if (!stateMachine.TrySetMode(mode))
            {
                return false;
            }

            activeConfig = ResolveConfig(mode);
            ApplyConfigToPool(activeConfig);
            ModeChanged?.Invoke(mode);
            return true;
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

        private AuraModeRuntimeConfig ResolveConfig(AuraMode mode)
        {
            foreach (var modeConfig in modeConfigs)
            {
                if (modeConfig.Mode == mode)
                {
                    return modeConfig;
                }
            }

            return AuraModeRuntimeConfig.Create(AuraMode.Neutral, 1f, 0f, 1f, 1f, 1f);
        }

        private void ApplyConfigToPool(AuraModeRuntimeConfig modeConfig)
        {
            if (auraPool == null)
            {
                return;
            }

            auraPool.SetRegenMultiplier(modeConfig.RegenMultiplier);
        }

        [Serializable]
        public struct AuraModeRuntimeConfig
        {
            [SerializeField] private AuraMode mode;
            [SerializeField, Min(0f)] private float regenMultiplier;
            [SerializeField, Min(0f)] private float upkeepPerSecond;
            [SerializeField, Min(0.01f)] private float defenseMultiplier;
            [SerializeField, Min(0.01f)] private float outgoingPowerMultiplier;
            [SerializeField, Min(0.01f)] private float movementMultiplier;
            [SerializeField, Min(0.01f)] private float detectionMultiplier;

            public AuraMode Mode => mode;
            public float RegenMultiplier => regenMultiplier;
            public float UpkeepPerSecond => upkeepPerSecond;
            public float DefenseMultiplier => defenseMultiplier;
            public float OutgoingPowerMultiplier => outgoingPowerMultiplier;
            public float MovementMultiplier => movementMultiplier;
            public float DetectionMultiplier => detectionMultiplier;

            public static AuraModeRuntimeConfig Create(
                AuraMode modeValue,
                float regen,
                float upkeep,
                float defense,
                float power,
                float movement)
            {
                return new AuraModeRuntimeConfig
                {
                    mode = modeValue,
                    regenMultiplier = regen,
                    upkeepPerSecond = upkeep,
                    defenseMultiplier = defense,
                    outgoingPowerMultiplier = power,
                    movementMultiplier = movement,
                    detectionMultiplier = 1f
                };
            }
        }
    }
}

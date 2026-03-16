using GreedIsland.Abilities;
using GreedIsland.Aura;
using GreedIsland.Character;
using GreedIsland.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace GreedIsland.UI
{
    public sealed class PlayerHudPresenter : MonoBehaviour
    {
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private AuraPool auraPool;
        [SerializeField] private AuraController auraController;
        [SerializeField] private AbilityRunner abilityRunner;

        [SerializeField] private HealthBarView healthBarView;
        [SerializeField] private AuraBarView auraBarView;
        [SerializeField] private CooldownBarView[] cooldownViews;
        [SerializeField] private Text auraModeText;
        [SerializeField] private Text abilityStatusText;
        [SerializeField] private DebugStatePanel debugPanel;

        private void Awake()
        {
            if (healthComponent == null)
            {
                healthComponent = FindAnyObjectByType<PlayerBrain>()?.GetComponent<HealthComponent>();
            }

            if (auraPool == null)
            {
                auraPool = FindAnyObjectByType<AuraPool>();
            }

            if (auraController == null)
            {
                auraController = FindAnyObjectByType<AuraController>();
            }

            if (abilityRunner == null)
            {
                abilityRunner = FindAnyObjectByType<AbilityRunner>();
            }
        }

        private void OnEnable()
        {
            if (healthComponent != null)
            {
                healthComponent.ValueChanged += OnHealthChanged;
                OnHealthChanged(healthComponent.Current, healthComponent.Max);
            }

            if (auraPool != null)
            {
                auraPool.ValueChanged += OnAuraChanged;
                OnAuraChanged(auraPool.Current, auraPool.Max);
            }

            if (auraController != null)
            {
                auraController.ModeChanged += OnAuraModeChanged;
                OnAuraModeChanged(auraController.CurrentMode);
            }

            if (abilityRunner != null)
            {
                abilityRunner.AbilityExecuted += OnAbilityExecuted;
                abilityRunner.AbilityFailed += OnAbilityFailed;
                abilityRunner.AbilityStopped += OnAbilityStopped;
            }

            if (debugPanel != null)
            {
                var playerBrain = FindAnyObjectByType<PlayerBrain>();
                debugPanel.Bind(playerBrain, auraController, abilityRunner);
            }
        }

        private void OnDisable()
        {
            if (healthComponent != null)
            {
                healthComponent.ValueChanged -= OnHealthChanged;
            }

            if (auraPool != null)
            {
                auraPool.ValueChanged -= OnAuraChanged;
            }

            if (auraController != null)
            {
                auraController.ModeChanged -= OnAuraModeChanged;
            }

            if (abilityRunner != null)
            {
                abilityRunner.AbilityExecuted -= OnAbilityExecuted;
                abilityRunner.AbilityFailed -= OnAbilityFailed;
                abilityRunner.AbilityStopped -= OnAbilityStopped;
            }
        }

        private void Update()
        {
            if (abilityRunner == null || cooldownViews == null)
            {
                return;
            }

            for (var i = 0; i < cooldownViews.Length; i++)
            {
                var view = cooldownViews[i];
                if (view == null)
                {
                    continue;
                }

                view.SetCooldown(abilityRunner.GetCooldownRemaining(view.SlotId));
            }
        }

        private void OnHealthChanged(float current, float max)
        {
            healthBarView?.SetValue(current, max);
        }

        private void OnAuraChanged(float current, float max)
        {
            auraBarView?.SetValue(current, max);
        }

        private void OnAuraModeChanged(AuraMode mode)
        {
            if (auraModeText != null)
            {
                auraModeText.text = $"Mode: {mode} ({DescribeMode(mode)})";
            }

            if (abilityStatusText != null && auraController != null)
            {
                abilityStatusText.text =
                    $"Aura {mode}: regen x{auraController.RegenMultiplier:0.00}, upkeep {auraController.UpkeepPerSecond:0.0}/s";
            }
        }

        private void OnAbilityExecuted(int slotId, AbilityDefinition definition)
        {
            if (abilityStatusText != null)
            {
                abilityStatusText.text = $"Slot {slotId}: {definition.DisplayName}";
            }
        }

        private void OnAbilityFailed(int slotId, AbilityDefinition definition, string reason)
        {
            if (abilityStatusText != null)
            {
                var abilityName = definition != null ? definition.DisplayName : "None";
                abilityStatusText.text = $"Slot {slotId} ({abilityName}) failed: {reason}";
            }
        }

        private void OnAbilityStopped(int slotId, AbilityDefinition definition)
        {
            if (abilityStatusText == null || definition == null)
            {
                return;
            }

            abilityStatusText.text = $"Slot {slotId}: {definition.DisplayName} ended";
        }

        private static string DescribeMode(AuraMode mode)
        {
            return mode switch
            {
                AuraMode.Concealment => "quiet flow",
                AuraMode.Reinforcement => "guard focus",
                AuraMode.Expansion => "power output",
                AuraMode.Perception => "sense focus",
                _ => "balanced"
            };
        }

        public void ConfigureViews(
            HealthBarView healthView,
            AuraBarView auraView,
            CooldownBarView[] cooldownBarViews,
            Text modeText,
            Text statusText,
            DebugStatePanel panel)
        {
            healthBarView = healthView;
            auraBarView = auraView;
            cooldownViews = cooldownBarViews;
            auraModeText = modeText;
            abilityStatusText = statusText;
            debugPanel = panel;
        }

        public void ConfigureSources(HealthComponent health, AuraPool aura, AuraController controller, AbilityRunner runner)
        {
            healthComponent = health;
            auraPool = aura;
            auraController = controller;
            abilityRunner = runner;
        }
    }
}

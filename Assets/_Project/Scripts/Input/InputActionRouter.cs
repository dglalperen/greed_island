using GreedIsland.Abilities;
using GreedIsland.Aura;
using GreedIsland.Character;
using GreedIsland.UI;
using UnityEngine;

namespace GreedIsland.Input
{
    public sealed class InputActionRouter : MonoBehaviour
    {
        private static readonly AuraMode[] AuraModeCycle =
        {
            AuraMode.Neutral,
            AuraMode.Concealment,
            AuraMode.Reinforcement,
            AuraMode.Expansion,
            AuraMode.Perception
        };

        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private PlayerBrain playerBrain;
        [SerializeField] private AbilityRunner abilityRunner;
        [SerializeField] private AuraController auraController;
        [SerializeField] private DebugStatePanel debugPanel;

        private void Awake()
        {
            if (inputReader == null)
            {
                inputReader = GetComponent<PlayerInputReader>();
            }

            if (playerBrain == null)
            {
                playerBrain = GetComponent<PlayerBrain>();
            }

            if (abilityRunner == null)
            {
                abilityRunner = GetComponent<AbilityRunner>();
            }

            if (auraController == null)
            {
                auraController = GetComponent<AuraController>();
            }

            if (debugPanel == null)
            {
                debugPanel = FindAnyObjectByType<DebugStatePanel>();
            }
        }

        private void Reset()
        {
            inputReader = GetComponent<PlayerInputReader>();
            playerBrain = GetComponent<PlayerBrain>();
            abilityRunner = GetComponent<AbilityRunner>();
            auraController = GetComponent<AuraController>();
        }

        private void OnEnable()
        {
            if (inputReader == null)
            {
                return;
            }

            inputReader.JumpPressed += OnJumpPressed;
            inputReader.DashPressed += OnDashPressed;
            inputReader.AbilityPressed += OnAbilityPressed;
            inputReader.ToggleFocusPressed += OnToggleFocusPressed;
            inputReader.LockOnPressed += OnLockOnPressed;
            inputReader.ToggleDebugPressed += OnToggleDebugPressed;
        }

        private void OnDisable()
        {
            if (inputReader == null)
            {
                return;
            }

            inputReader.JumpPressed -= OnJumpPressed;
            inputReader.DashPressed -= OnDashPressed;
            inputReader.AbilityPressed -= OnAbilityPressed;
            inputReader.ToggleFocusPressed -= OnToggleFocusPressed;
            inputReader.LockOnPressed -= OnLockOnPressed;
            inputReader.ToggleDebugPressed -= OnToggleDebugPressed;
        }

        private void Update()
        {
            if (playerBrain == null || inputReader == null)
            {
                return;
            }

            playerBrain.SetMoveIntent(inputReader.Move, inputReader.Look, inputReader.SprintHeld);
        }

        private void OnJumpPressed()
        {
            playerBrain?.RequestJump();
        }

        private void OnDashPressed()
        {
            playerBrain?.RequestDash();
        }

        private void OnAbilityPressed(int slot)
        {
            abilityRunner?.TryActivateSlot(slot);
        }

        private void OnToggleFocusPressed()
        {
            if (auraController == null)
            {
                return;
            }

            var current = auraController.CurrentMode;
            var index = 0;
            for (var i = 0; i < AuraModeCycle.Length; i++)
            {
                if (AuraModeCycle[i] != current)
                {
                    continue;
                }

                index = i;
                break;
            }

            var nextIndex = (index + 1) % AuraModeCycle.Length;
            auraController.SetMode(AuraModeCycle[nextIndex]);
        }

        private void OnLockOnPressed()
        {
            playerBrain?.ToggleLockOn();
        }

        private void OnToggleDebugPressed()
        {
            if (debugPanel == null)
            {
                debugPanel = FindAnyObjectByType<DebugStatePanel>();
            }

            debugPanel?.ToggleVisible();
        }
    }
}

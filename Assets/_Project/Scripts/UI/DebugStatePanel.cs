using GreedIsland.Abilities;
using GreedIsland.Aura;
using GreedIsland.Character;
using UnityEngine;
using UnityEngine.UI;

namespace GreedIsland.UI
{
    public sealed class DebugStatePanel : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Text contentText;
        [SerializeField] private bool visible = true;
        [SerializeField] private int[] trackedSlots = { 1, 2, 3 };

        private PlayerBrain playerBrain;
        private AuraController auraController;
        private AbilityRunner abilityRunner;

        private void Start()
        {
            ApplyVisibility();
        }

        private void Update()
        {
            if (!visible || contentText == null)
            {
                return;
            }

            if (playerBrain == null)
            {
                playerBrain = FindAnyObjectByType<PlayerBrain>();
            }

            if (auraController == null)
            {
                auraController = FindAnyObjectByType<AuraController>();
            }

            if (abilityRunner == null)
            {
                abilityRunner = FindAnyObjectByType<AbilityRunner>();
            }

            if (playerBrain == null || auraController == null)
            {
                contentText.text = "Debug systems not bound.";
                return;
            }

            var targetText = playerBrain.CurrentLockTarget != null ? playerBrain.CurrentLockTarget.TargetTransform.name : "None";

            var cooldownLines = string.Empty;
            if (abilityRunner != null)
            {
                for (var i = 0; i < trackedSlots.Length; i++)
                {
                    var slot = trackedSlots[i];
                    var remaining = abilityRunner.GetCooldownRemaining(slot);
                    cooldownLines += $"\nCD{slot}: {remaining:0.00}s";
                }
            }

            contentText.text =
                $"MoveState: {playerBrain.CurrentMoveState}" +
                $"\nGrounded: {playerBrain.IsGrounded}" +
                $"\nSpeed: {playerBrain.CurrentSpeed:0.00}" +
                $"\nVerticalVel: {playerBrain.VerticalVelocity:0.00}" +
                $"\nAura: {auraController.AuraPool.Current:0.0}/{auraController.AuraPool.Max:0.0}" +
                $"\nAuraMode: {auraController.CurrentMode}" +
                $"\nTarget: {targetText}" +
                cooldownLines;
        }

        public void Bind(PlayerBrain brain, AuraController aura, AbilityRunner runner)
        {
            playerBrain = brain;
            auraController = aura;
            abilityRunner = runner;
        }

        public void ToggleVisible()
        {
            visible = !visible;
            ApplyVisibility();
        }

        public void Configure(GameObject root, Text text, bool startVisible = true)
        {
            panelRoot = root;
            contentText = text;
            visible = startVisible;
            ApplyVisibility();
        }

        private void ApplyVisibility()
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(visible);
            }
        }
    }
}

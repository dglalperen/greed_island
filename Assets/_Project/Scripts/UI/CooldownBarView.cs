using UnityEngine;
using UnityEngine.UI;

namespace GreedIsland.UI
{
    public sealed class CooldownBarView : MonoBehaviour
    {
        [SerializeField] private int slotId = 1;
        [SerializeField] private Slider slider;
        [SerializeField] private Text timerText;
        [SerializeField, Min(0.01f)] private float maxDisplayCooldown = 10f;

        public int SlotId => slotId;

        public void SetCooldown(float remainingSeconds)
        {
            if (slider != null)
            {
                slider.value = 1f - Mathf.Clamp01(remainingSeconds / maxDisplayCooldown);
            }

            if (timerText != null)
            {
                timerText.text = remainingSeconds <= 0.01f ? "Ready" : remainingSeconds.ToString("0.0");
            }
        }

        public void SetMaxDisplayCooldown(float value)
        {
            maxDisplayCooldown = Mathf.Max(0.01f, value);
        }

        public void Configure(int slot, Slider cooldownSlider, Text cooldownText, float maxCooldown)
        {
            slotId = Mathf.Max(1, slot);
            slider = cooldownSlider;
            timerText = cooldownText;
            maxDisplayCooldown = Mathf.Max(0.01f, maxCooldown);
        }
    }
}

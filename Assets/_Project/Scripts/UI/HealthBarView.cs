using UnityEngine;
using UnityEngine.UI;

namespace GreedIsland.UI
{
    public sealed class HealthBarView : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Text valueText;

        public void Configure(Slider newSlider, Text newValueText)
        {
            slider = newSlider;
            valueText = newValueText;
        }

        public void SetValue(float current, float max)
        {
            if (slider != null)
            {
                slider.value = max <= 0f ? 0f : current / max;
            }

            if (valueText != null)
            {
                valueText.text = $"Health {current:0}/{max:0}";
            }
        }
    }
}

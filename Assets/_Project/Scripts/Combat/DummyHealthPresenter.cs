using GreedIsland.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace GreedIsland.Combat
{
    public sealed class DummyHealthPresenter : MonoBehaviour
    {
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private Slider healthSlider;

        public void Configure(HealthComponent health, Slider slider)
        {
            healthComponent = health;
            healthSlider = slider;
        }

        private void OnEnable()
        {
            if (healthComponent != null)
            {
                healthComponent.ValueChanged += OnHealthChanged;
                OnHealthChanged(healthComponent.Current, healthComponent.Max);
            }
        }

        private void OnDisable()
        {
            if (healthComponent != null)
            {
                healthComponent.ValueChanged -= OnHealthChanged;
            }
        }

        private void OnHealthChanged(float current, float max)
        {
            if (healthSlider == null)
            {
                return;
            }

            healthSlider.value = max <= 0f ? 0f : current / max;
        }
    }
}

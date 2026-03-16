using UnityEngine;

namespace GreedIsland.Abilities
{
    public sealed class SenseRevealTarget : MonoBehaviour
    {
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private Color revealColor = new(1f, 0.95f, 0.35f, 1f);

        private MaterialPropertyBlock propertyBlock;
        private Color originalColor = Color.white;
        private float revealedUntil = -1f;

        private void Awake()
        {
            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<Renderer>();
            }

            if (targetRenderer != null)
            {
                propertyBlock = new MaterialPropertyBlock();
                if (targetRenderer.sharedMaterial != null && targetRenderer.sharedMaterial.HasProperty("_BaseColor"))
                {
                    originalColor = targetRenderer.sharedMaterial.GetColor("_BaseColor");
                }
            }
        }

        private void Update()
        {
            if (targetRenderer == null || propertyBlock == null)
            {
                return;
            }

            var isRevealed = Time.time < revealedUntil;
            var color = isRevealed ? revealColor : originalColor;
            targetRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", color);
            targetRenderer.SetPropertyBlock(propertyBlock);
        }

        public void Reveal(float duration)
        {
            revealedUntil = Mathf.Max(revealedUntil, Time.time + Mathf.Max(0f, duration));
        }
    }
}

using UnityEngine;

namespace GreedIsland.Aura
{
    [CreateAssetMenu(menuName = "GreedIsland/Aura/AuraSignatureProfile")]
    public sealed class AuraSignatureProfile : ScriptableObject
    {
        [SerializeField] private string signatureName = "Default Signature";
        [SerializeField, Range(0f, 1f)] private float concealmentFactor = 1f;
        [SerializeField, Range(0f, 2f)] private float projectionFactor = 1f;
        [SerializeField, Range(0f, 2f)] private float stabilityFactor = 1f;

        public string SignatureName => signatureName;
        public float ConcealmentFactor => concealmentFactor;
        public float ProjectionFactor => projectionFactor;
        public float StabilityFactor => stabilityFactor;
    }
}

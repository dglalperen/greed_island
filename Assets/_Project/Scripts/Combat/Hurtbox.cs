using UnityEngine;

namespace GreedIsland.Combat
{
    [RequireComponent(typeof(Collider))]
    public sealed class Hurtbox : MonoBehaviour
    {
        [SerializeField] private DamageableActor owner;

        public DamageableActor Owner => owner;

        private void Reset()
        {
            owner = GetComponentInParent<DamageableActor>();
        }
    }
}

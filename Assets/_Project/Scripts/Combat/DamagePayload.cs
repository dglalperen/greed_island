using UnityEngine;

namespace GreedIsland.Combat
{
    public struct DamagePayload
    {
        public float Amount;
        public GameObject Source;
        public Vector3 HitPoint;
        public Vector3 Direction;
        public float Force;
        public bool IsAbility;
        public string DebugTag;

        public static DamagePayload Create(float amount, GameObject source, Vector3 origin, Vector3 direction, float force, bool isAbility, string debugTag)
        {
            return new DamagePayload
            {
                Amount = amount,
                Source = source,
                HitPoint = origin,
                Direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector3.forward,
                Force = force,
                IsAbility = isAbility,
                DebugTag = debugTag
            };
        }
    }
}

using UnityEngine;

namespace GreedIsland.Core
{
    public interface ITargetable
    {
        Transform TargetTransform { get; }
        bool IsTargetable { get; }
    }
}

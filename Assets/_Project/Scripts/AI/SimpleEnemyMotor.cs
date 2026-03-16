using UnityEngine;

namespace GreedIsland.AI
{
    public sealed class SimpleEnemyMotor : MonoBehaviour
    {
        [SerializeField] private bool enabledForPrototype;

        private void Update()
        {
            if (!enabledForPrototype)
            {
                return;
            }

            // Reserved for post-milestone enemy behavior.
        }
    }
}

using UnityEngine;

namespace GreedIsland.Core
{
    public sealed class LayerCollisionConfigurator : MonoBehaviour
    {
        private static bool configured;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ConfigureAtStartup()
        {
            if (configured)
            {
                return;
            }

            configured = true;
            ConfigureCollisionMatrix();
        }

        private static void ConfigureCollisionMatrix()
        {
            var playerLayer = LayerMaskConfig.Player;
            var enemyLayer = LayerMaskConfig.Enemy;
            var hitboxLayer = LayerMaskConfig.Hitbox;
            var hurtboxLayer = LayerMaskConfig.Hurtbox;
            var abilityLayer = LayerMaskConfig.Ability;
            var worldPropLayer = LayerMaskConfig.WorldProp;

            IgnoreSelfCollision(hitboxLayer);
            IgnoreSelfCollision(abilityLayer);

            IgnoreCollision(playerLayer, hitboxLayer, true);
            IgnoreCollision(playerLayer, abilityLayer, true);
            IgnoreCollision(enemyLayer, hitboxLayer, true);
            IgnoreCollision(hitboxLayer, worldPropLayer, true);

            IgnoreCollision(hitboxLayer, hurtboxLayer, false);
            IgnoreCollision(abilityLayer, hurtboxLayer, false);
            IgnoreCollision(abilityLayer, worldPropLayer, false);
        }

        private static void IgnoreSelfCollision(int layer)
        {
            if (layer < 0)
            {
                return;
            }

            Physics.IgnoreLayerCollision(layer, layer, true);
        }

        private static void IgnoreCollision(int layerA, int layerB, bool ignore)
        {
            if (layerA < 0 || layerB < 0)
            {
                return;
            }

            Physics.IgnoreLayerCollision(layerA, layerB, ignore);
        }
    }
}

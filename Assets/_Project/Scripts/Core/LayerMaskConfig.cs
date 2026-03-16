using UnityEngine;

namespace GreedIsland.Core
{
    public static class LayerMaskConfig
    {
        public static int Player => LayerMask.NameToLayer("Player");
        public static int Enemy => LayerMask.NameToLayer("Enemy");
        public static int Hitbox => LayerMask.NameToLayer("Hitbox");
        public static int Hurtbox => LayerMask.NameToLayer("Hurtbox");
        public static int Ability => LayerMask.NameToLayer("Ability");
        public static int Interactable => LayerMask.NameToLayer("Interactable");
        public static int WorldProp => LayerMask.NameToLayer("WorldProp");
        public static int Ground => LayerMask.NameToLayer("Ground");
        public static int Target => LayerMask.NameToLayer("Target");

        public static LayerMask GroundMask
        {
            get
            {
                var mask = BuildMask("Ground", "WorldProp", "Interactable", "Default");
                if (mask == 0)
                {
                    return LayerMask.GetMask("Default");
                }

                return mask;
            }
        }

        private static int BuildMask(params string[] layerNames)
        {
            var mask = 0;
            for (var i = 0; i < layerNames.Length; i++)
            {
                var layer = LayerMask.NameToLayer(layerNames[i]);
                if (layer < 0)
                {
                    continue;
                }

                mask |= 1 << layer;
            }

            return mask;
        }
    }
}

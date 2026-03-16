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
                var ground = LayerMask.NameToLayer("Ground");
                if (ground < 0)
                {
                    return LayerMask.GetMask("Default");
                }

                return 1 << ground;
            }
        }
    }
}

using System;

namespace GreedIsland.Aura
{
    [Serializable]
    public sealed class AuraStateMachine
    {
        private AuraMode currentMode = AuraMode.Neutral;

        public AuraMode CurrentMode => currentMode;

        public bool TrySetMode(AuraMode mode)
        {
            if (currentMode == mode)
            {
                return false;
            }

            currentMode = mode;
            return true;
        }
    }
}

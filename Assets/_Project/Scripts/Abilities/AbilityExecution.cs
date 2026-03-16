using GreedIsland.Aura;

namespace GreedIsland.Abilities
{
    public sealed class AbilityExecution
    {
        public AbilityExecution(int slotId, AbilityDefinition definition, float startedAt, AbilityContext context)
        {
            SlotId = slotId;
            Definition = definition;
            StartedAt = startedAt;
            Context = context;
        }

        public int SlotId { get; }
        public AbilityDefinition Definition { get; }
        public float StartedAt { get; }
        public AbilityContext Context { get; }

        public bool ShouldEndByDuration(float now)
        {
            if (Definition == null || Definition.ChannelDuration <= 0f)
            {
                return false;
            }

            return now >= StartedAt + Definition.ChannelDuration;
        }

        public float GetUpkeepThisFrame(float deltaTime, AuraController auraController)
        {
            if (Definition == null || auraController == null)
            {
                return 0f;
            }

            if (Definition.AuraUpkeepPerSecond <= 0f)
            {
                return 0f;
            }

            return Definition.AuraUpkeepPerSecond * auraController.GetCostMultiplier(Definition.AffinityType) * deltaTime;
        }
    }
}

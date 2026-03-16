namespace GreedIsland.Core
{
    public interface IResourcePool
    {
        float Current { get; }
        float Max { get; }

        bool CanAfford(float amount);
        bool Spend(float amount);
        void Restore(float amount);
    }
}

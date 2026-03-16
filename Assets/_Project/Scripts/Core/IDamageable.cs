using GreedIsland.Combat;

namespace GreedIsland.Core
{
    public interface IDamageable
    {
        void ReceiveDamage(in DamagePayload payload);
    }
}

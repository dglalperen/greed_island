using GreedIsland.Abilities;
using GreedIsland.Aura;
using GreedIsland.Core;
using GreedIsland.Stats;
using NUnit.Framework;
using UnityEngine;

namespace GreedIsland.Tests.PlayMode
{
    public sealed class FoundationPlayModeTests
    {
        [Test]
        public void AuraPool_SpendAndRestoreFlow_IsConsistent()
        {
            var go = new GameObject("AuraPoolTest");
            var pool = go.AddComponent<AuraPool>();
            pool.Configure(100f, 20f, 0.05f);

            Assert.IsTrue(pool.Spend(30f));
            Assert.AreEqual(70f, pool.Current, 0.01f);
            Assert.IsFalse(pool.CanAfford(80f));
            Assert.IsTrue(pool.CanAfford(50f));
            pool.Restore(15f);
            Assert.AreEqual(85f, pool.Current, 0.01f);

            Object.Destroy(go);
        }

        [Test]
        public void AbilityRunner_StartsCooldownAfterActivation()
        {
            var go = new GameObject("AbilityRunnerTest");

            var pool = go.AddComponent<AuraPool>();
            pool.Configure(100f, 0f, 0f);
            var auraController = go.AddComponent<AuraController>();

            var user = go.AddComponent<AbilityUserStub>();
            user.Initialize(auraController, new StatBlock());

            var runner = go.AddComponent<AbilityRunner>();

            var definition = ScriptableObject.CreateInstance<AbilityDefinition>();
            definition.ConfigurePrototype(
                newId: "test.cooldown",
                newDisplayName: "Cooldown Test",
                newDescription: "",
                newAffinity: AffinityType.Reinforcement,
                newActivation: AbilityActivationType.Instant,
                newTargeting: AbilityTargetingType.Self,
                newCooldown: 0.4f,
                newAuraCost: 0f,
                newAuraUpkeep: 0f,
                newRange: 0f,
                newRadius: 0f,
                newRequireTarget: false,
                newConditions: System.Array.Empty<AbilityCondition>(),
                newEffects: System.Array.Empty<AbilityEffect>());

            runner.SetEquippedSlots(new[] { AbilitySlot.Create(1, definition) });

            Assert.IsTrue(runner.TryActivateSlot(1));
            Assert.Greater(runner.GetCooldownRemaining(1), 0f);
            Assert.IsFalse(runner.TryActivateSlot(1));

            Object.Destroy(go);
        }

        private sealed class AbilityUserStub : MonoBehaviour, IAbilityUser
        {
            private AuraController auraController;
            private StatBlock stats;

            public Transform Origin => transform;
            public AuraController AuraController => auraController;
            public StatBlock Stats => stats;

            public void Initialize(AuraController controller, StatBlock statBlock)
            {
                auraController = controller;
                stats = statBlock;
            }
        }
    }
}

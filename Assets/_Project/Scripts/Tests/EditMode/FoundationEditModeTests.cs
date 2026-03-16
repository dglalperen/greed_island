#if UNITY_INCLUDE_TESTS
using GreedIsland.Abilities;
using GreedIsland.Aura;
using GreedIsland.Stats;
using NUnit.Framework;

namespace GreedIsland.Tests.EditMode
{
    public sealed class FoundationEditModeTests
    {
        [Test]
        public void StatBlockClone_CopiesCoreValues()
        {
            var original = new StatBlock();
            var clone = original.Clone();

            Assert.AreNotSame(original, clone);
            Assert.AreEqual(original.MoveSpeed, clone.MoveSpeed);
            Assert.AreEqual(original.SprintSpeed, clone.SprintSpeed);
            Assert.AreEqual(original.JumpHeight, clone.JumpHeight);
        }

        [Test]
        public void AuraStateMachine_ChangesModeWhenDifferent()
        {
            var stateMachine = new AuraStateMachine();

            var changed = stateMachine.TrySetMode(AuraMode.Perception);
            var changedAgain = stateMachine.TrySetMode(AuraMode.Perception);

            Assert.IsTrue(changed);
            Assert.IsFalse(changedAgain);
            Assert.AreEqual(AuraMode.Perception, stateMachine.CurrentMode);
        }

        [Test]
        public void AbilitySlotFactory_AssignsSlotAndDefinition()
        {
            var definition = UnityEngine.ScriptableObject.CreateInstance<AbilityDefinition>();
            var slot = AbilitySlot.Create(3, definition);

            Assert.AreEqual(3, slot.SlotId);
            Assert.AreSame(definition, slot.Definition);
        }
    }
}
#endif

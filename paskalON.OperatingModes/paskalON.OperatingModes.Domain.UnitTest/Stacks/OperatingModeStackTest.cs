// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Stacks;
using paskalON.OperatingModes.Domain.Transitions;

namespace paskalON.OperatingModes.Domain.UnitTest.Stacks
{
    /// <summary>
    /// Tests selection and priority behavior of <see cref="OperatingModeStack"/>.
    /// </summary>
    [TestClass]
    public class OperatingModeStackTest
    {
        /// <summary>
        /// Confirms a new stack contains no selected modes.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackInitiallyEmptyTest()
        {
            OperatingModeStack stack = new OperatingModeStack();

            Assert.IsEmpty(stack.OperatingModes);
        }


        /// <summary>
        /// Adds the first mode to an empty stack.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackAddToEmptyStackTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode mode = CreateOperatingMode("ModeA");

            stack.Add(mode, 1);

            AssertModes(stack, mode);
        }


        /// <summary>
        /// Adds a mode at an unused priority without changing the order of existing modes.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackAddAtUnusedPriorityTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode firstMode = CreateOperatingMode("ModeA");
            IStackableMode secondMode = CreateOperatingMode("ModeB");
            IStackableMode thirdMode = CreateOperatingMode("ModeC");
            stack.Add(firstMode, 1);
            stack.Add(secondMode, 2);
            stack.Add(thirdMode, 3);

            AssertModes(stack, firstMode, secondMode, thirdMode);
        }


        /// <summary>
        /// Supports the minimum integer priority and shifts it safely when occupied.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackAddAtMinimumPriorityTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode existingMode = CreateOperatingMode("ModeA");
            IStackableMode insertedMode = CreateOperatingMode("ModeB");
            stack.Add(existingMode, int.MinValue);
            stack.Add(insertedMode, int.MinValue);

            AssertModes(stack, insertedMode, existingMode);
        }


        /// <summary>
        /// Inserts at an occupied priority and shifts that mode and all later modes.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackAddAtOccupiedPriorityShiftsModesTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode firstMode = CreateOperatingMode("ModeA");
            IStackableMode secondMode = CreateOperatingMode("ModeB");
            IStackableMode thirdMode = CreateOperatingMode("ModeC");
            IStackableMode insertedMode = CreateOperatingMode("ModeD");
            IStackableMode followUpMode = CreateOperatingMode("ModeE");
            stack.Add(firstMode, 1);
            stack.Add(secondMode, 2);
            stack.Add(thirdMode, 3);
            stack.Add(insertedMode, 2);
            stack.Add(followUpMode, 3);

            AssertModes(stack, firstMode, insertedMode, followUpMode, secondMode, thirdMode);
        }


        /// <summary>
        /// Rejects a second mode with the same name without changing the stack.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackAddDuplicateNameTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode existingMode = CreateOperatingMode("ModeA");
            IStackableMode duplicateMode = CreateOperatingMode("ModeA");
            stack.Add(existingMode, 1);

            Assert.ThrowsExactly<InvalidOperationException>(() => stack.Add(duplicateMode, 2));
        }


        /// <summary>
        /// Rejects a null mode.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackAddNullModeTest()
        {
            OperatingModeStack stack = new OperatingModeStack();

            Assert.ThrowsExactly<ArgumentNullException>(() => stack.Add(null!, 1));
        }


        /// <summary>
        /// Leaves the stack unchanged if insertion would overflow an existing priority.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackAddPriorityOverflowPreservesStackTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode existingMode = CreateOperatingMode("ModeA");
            IStackableMode insertedMode = CreateOperatingMode("ModeB");
            stack.Add(existingMode, int.MaxValue);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => stack.Add(insertedMode, int.MaxValue));
        }


        /// <summary>
        /// Moves a mode into an occupied priority and shifts the other affected modes.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackMoveAtOccupiedPriorityShiftsModesTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode firstMode = CreateOperatingMode("ModeA");
            IStackableMode secondMode = CreateOperatingMode("ModeB");
            IStackableMode thirdMode = CreateOperatingMode("ModeC");
            IStackableMode movedMode = CreateOperatingMode("ModeD");
            IStackableMode lastMode = CreateOperatingMode("ModeE");
            IStackableMode insertedMode = CreateOperatingMode("ModeF");
            stack.Add(firstMode, 1);
            stack.Add(secondMode, 2);
            stack.Add(thirdMode, 3);
            stack.Add(movedMode, 4);
            stack.Add(lastMode, 5);
            stack.Move("ModeD", 2);
            stack.Add(insertedMode, 5);

            AssertModes(stack, firstMode, movedMode, secondMode, thirdMode, insertedMode, lastMode);
        }


        /// <summary>
        /// Moves a mode later into an occupied priority and shifts affected modes.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackMoveToOccupiedLaterPriorityTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode movedMode = CreateOperatingMode("ModeA");
            IStackableMode secondMode = CreateOperatingMode("ModeB");
            IStackableMode thirdMode = CreateOperatingMode("ModeC");
            IStackableMode insertedMode = CreateOperatingMode("ModeD");
            stack.Add(movedMode, 1);
            stack.Add(secondMode, 2);
            stack.Add(thirdMode, 3);
            stack.Move("ModeA", 2);
            stack.Add(insertedMode, 2);

            AssertModes(stack, insertedMode, movedMode, secondMode, thirdMode);
        }


        /// <summary>
        /// Moves a mode to an unused later priority without changing the other modes' order.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackMoveToUnusedLaterPriorityTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode firstMode = CreateOperatingMode("ModeA");
            IStackableMode secondMode = CreateOperatingMode("ModeB");
            IStackableMode thirdMode = CreateOperatingMode("ModeC");
            stack.Add(firstMode, 1);
            stack.Add(secondMode, 2);
            stack.Add(thirdMode, 3);
            stack.Move("ModeA", 5);

            AssertModes(stack, secondMode, thirdMode, firstMode);
        }


        /// <summary>
        /// Moving a mode to its current priority leaves the stack unchanged.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackMoveToCurrentPriorityTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode firstMode = CreateOperatingMode("ModeA");
            IStackableMode secondMode = CreateOperatingMode("ModeB");
            stack.Add(firstMode, 1);
            stack.Add(secondMode, 2);
            stack.Move("ModeA", 1);

            AssertModes(stack, firstMode, secondMode);
        }


        /// <summary>
        /// Keeps name lookup correct after a mode is moved more than once.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackMoveUpdatesNamePriorityTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode firstMode = CreateOperatingMode("ModeA");
            IStackableMode movedMode = CreateOperatingMode("ModeB");
            IStackableMode thirdMode = CreateOperatingMode("ModeC");
            IStackableMode insertedMode = CreateOperatingMode("ModeD");
            stack.Add(firstMode, 1);
            stack.Add(movedMode, 2);
            stack.Add(thirdMode, 3);
            stack.Move("ModeB", 1);
            stack.Move("ModeB", 3);
            stack.Add(insertedMode, 3);

            AssertModes(stack, firstMode, insertedMode, movedMode, thirdMode);
        }


        /// <summary>
        /// Rejects a move for a name that is not in the stack.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackMoveUnknownNameTest()
        {
            OperatingModeStack stack = new OperatingModeStack();

            Assert.ThrowsExactly<KeyNotFoundException>(() => stack.Move("Missing", 1));
        }


        /// <summary>
        /// Leaves the stack unchanged if moving would overflow an existing priority.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackMovePriorityOverflowPreservesStackTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode movedMode = CreateOperatingMode("ModeA");
            IStackableMode lastMode = CreateOperatingMode("ModeB");
            stack.Add(movedMode, 1);
            stack.Add(lastMode, int.MaxValue);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => stack.Move("ModeA", int.MaxValue));
            AssertModes(stack, movedMode, lastMode);
        }


        /// <summary>
        /// Rejects a null name for move operations.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackMoveNullNameTest()
        {
            OperatingModeStack stack = new OperatingModeStack();

            Assert.ThrowsExactly<ArgumentNullException>(() => stack.Move(null!, 1));
            Assert.IsEmpty(stack.OperatingModes);
        }


        /// <summary>
        /// Rejects empty and whitespace-only names for move operations.
        /// </summary>
        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        public void OperatingModeStackMoveEmptyNameTest(string operatingModeName)
        {
            OperatingModeStack stack = new OperatingModeStack();

            Assert.ThrowsExactly<ArgumentException>(() => stack.Move(operatingModeName, 1));
            Assert.IsEmpty(stack.OperatingModes);
        }


        /// <summary>
        /// Removes the requested mode while preserving the order of the remaining modes.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackRemoveExistingModeTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode firstMode = CreateOperatingMode("ModeA");
            IStackableMode removedMode = CreateOperatingMode("ModeB");
            IStackableMode lastMode = CreateOperatingMode("ModeC");
            IStackableMode insertedMode = CreateOperatingMode("ModeD");
            stack.Add(firstMode, 1);
            stack.Add(removedMode, 2);
            stack.Add(lastMode, 3);
            stack.Remove(removedMode);
            stack.Add(insertedMode, 2);

            AssertModes(stack, firstMode, insertedMode, lastMode);
        }


        /// <summary>
        /// Allows a removed operating mode name to be added again.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackRemoveAllowsReaddingNameTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode mode = CreateOperatingMode("ModeA");
            stack.Add(mode, 1);
            stack.Remove(mode);
            stack.Add(mode, 2);

            AssertModes(stack, mode);
        }


        /// <summary>
        /// Removes the last mode and leaves the stack empty.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackRemoveLastModeTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode mode = CreateOperatingMode("ModeA");
            stack.Add(mode, 1);
            stack.Remove(mode);

            Assert.IsEmpty(stack.OperatingModes);
        }


        /// <summary>
        /// Rejects removal of a name that is not in the stack.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackRemoveUnknownNameTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode mode = CreateOperatingMode("ModeA");
            stack.Add(mode, 1);
            IStackableMode missing = CreateOperatingMode("Missing");

            Assert.ThrowsExactly<KeyNotFoundException>(() => stack.Remove(missing));
            AssertModes(stack, mode);
        }


        /// <summary>
        /// Rejects a null name for removal operations.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackRemoveNullNameTest()
        {
            OperatingModeStack stack = new OperatingModeStack();

            Assert.ThrowsExactly<ArgumentNullException>(() => stack.Remove(null!));
            Assert.IsEmpty(stack.OperatingModes);
        }


        /// <summary>
        /// Rejects a change that violates a classification rule without changing the stack.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackAddInvalidStackThrowsAndPreservesStackTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode baseMode = new TestBaseActiveMode("ActivePowerMode");
            IStackableMode secondBaseMode = new TestBaseActiveMode("MaximumPowerPointTrackingMode");
            stack.Add(baseMode, 1);

            Assert.ThrowsExactly<InvalidOperationException>(() => stack.Add(secondBaseMode, 2));
            AssertModes(stack, baseMode);
        }


        /// <summary>
        /// Rejects a move that puts an additive mode before its base mode.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackMoveInvalidOrderThrowsAndPreservesStackTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode baseMode = new TestBaseActiveMode("ActivePowerMode");
            IStackableMode additiveMode = new TestAdditiveActiveMode("FrequencyWattMode");
            stack.Add(baseMode, 1);
            stack.Add(additiveMode, 2);

            Assert.ThrowsExactly<InvalidOperationException>(() => stack.Move("FrequencyWattMode", 1));
            AssertModes(stack, baseMode, additiveMode);
        }


        /// <summary>
        /// Rejects removing a base mode that is required by an additive mode.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackRemoveRequiredBaseThrowsAndPreservesStackTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode baseMode = new TestBaseActiveMode("ActivePowerMode");
            IStackableMode additiveMode = new TestAdditiveActiveMode("FrequencyWattMode");
            stack.Add(baseMode, 1);
            stack.Add(additiveMode, 2);

            Assert.ThrowsExactly<InvalidOperationException>(() => stack.Remove(baseMode));
            AssertModes(stack, baseMode, additiveMode);
            Assert.HasCount(2, stack.PendingChanges);
        }


        /// <summary>
        /// Confirms adding and removing records the changes for the next calculation cycle without activating the modes.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackAddRemoveRecordsChangesTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            TestBaseActiveMode baseMode = new TestBaseActiveMode("ActivePowerMode");

            stack.Add(baseMode, 1);
            stack.Remove(baseMode);

            IReadOnlyList<OperatingModeChange<IStackableMode>> changes = stack.PendingChanges;
            Assert.HasCount(2, changes);
            Assert.AreEqual(OperatingModeChangeKind.Added, changes[0].Kind);
            Assert.AreSame(baseMode, changes[0].OperatingMode);
            Assert.AreEqual(OperatingModeChangeKind.Removed, changes[1].Kind);
            Assert.IsFalse(baseMode.IsActivated);
        }


        /// <summary>
        /// Confirms a replacement takes the priority of its predecessor and records one atomic change.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackReplaceKeepsPriorityAndRecordsChangeTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode predecessor = new TestBaseActiveMode("ActivePowerMode");
            IStackableMode reactiveMode = new TestBaseReactiveMode("ReactivePowerMode");
            IStackableMode replacement = new TestBaseActiveMode("MaximumPowerPointTrackingMode");
            stack.Add(predecessor, 1);
            stack.Add(reactiveMode, 2);
            stack.TakeCycle();

            stack.Replace("ActivePowerMode", replacement);

            AssertModes(stack, replacement, reactiveMode);
            IReadOnlyList<OperatingModeChange<IStackableMode>> changes = stack.PendingChanges;
            Assert.HasCount(1, changes);
            Assert.AreEqual(OperatingModeChangeKind.Replaced, changes[0].Kind);
            Assert.AreSame(replacement, changes[0].OperatingMode);
            Assert.AreSame(predecessor, changes[0].Predecessor);
        }


        /// <summary>
        /// Confirms invalid replacements are rejected without changing the stack.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackReplaceInvalidThrowsAndPreservesStackTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode baseMode = new TestBaseActiveMode("ActivePowerMode");
            IStackableMode additiveMode = new TestAdditiveActiveMode("FrequencyWattMode");
            stack.Add(baseMode, 1);
            stack.TakeCycle();

            Assert.ThrowsExactly<KeyNotFoundException>(() => stack.Replace("Missing", new TestBaseActiveMode("MaximumPowerPointTrackingMode")));
            Assert.ThrowsExactly<InvalidOperationException>(() => stack.Replace("ActivePowerMode", baseMode));
            Assert.ThrowsExactly<InvalidOperationException>(() => stack.Replace("ActivePowerMode", additiveMode));
            AssertModes(stack, baseMode);
            Assert.IsEmpty(stack.PendingChanges);
        }


        /// <summary>
        /// Confirms taking a cycle returns the modes with the changes once.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackTakeCycleReturnsChangesOnceTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            IStackableMode baseMode = new TestBaseActiveMode("ActivePowerMode");
            stack.Add(baseMode, 1);

            OperatingModeCycle<IStackableMode> firstCycle = stack.TakeCycle();
            OperatingModeCycle<IStackableMode> secondCycle = stack.TakeCycle();

            CollectionAssert.AreEqual(new[] { baseMode }, firstCycle.OperatingModes.ToArray());
            Assert.HasCount(1, firstCycle.Changes);
            CollectionAssert.AreEqual(new[] { baseMode }, secondCycle.OperatingModes.ToArray());
            Assert.IsEmpty(secondCycle.Changes);
        }


        /// <summary>
        /// Confirms moving does not record a change because the operating mode stays active.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackMoveDoesNotRecordChangeTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            stack.Add(new TestBaseActiveMode("ActivePowerMode"), 1);
            stack.Add(new TestBaseReactiveMode("ReactivePowerMode"), 2);
            stack.TakeCycle();

            stack.Move("ReactivePowerMode", 1);

            Assert.IsEmpty(stack.PendingChanges);
        }


        /// <summary>
        /// Clear removes the selected modes and the pending changes.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackClearRemovesModesAndChangesTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            stack.Add(new TestBaseActiveMode("ActivePowerMode"), 1);

            stack.Clear();

            Assert.IsEmpty(stack.OperatingModes);
            Assert.IsEmpty(stack.PendingChanges);
        }


        /// <summary>
        /// Asserts the selected operating modes and their order.
        /// </summary>
        /// <param name="stack">The operating mode stack.</param>
        /// <param name="expectedModes">The expected operating modes in ascending priority.</param>
        private void AssertModes(OperatingModeStack stack, params IStackableMode[] expectedModes)
        {
            CollectionAssert.AreEqual(expectedModes, stack.OperatingModes.ToArray());
        }


        /// <summary>
        /// Creates a stackable operating mode without classification.
        /// </summary>
        /// <param name="name">The name of the operating mode.</param>
        /// <returns>The stackable operating mode.</returns>
        private IStackableMode CreateOperatingMode(string name)
        {
            return new TestStackableMode(name);
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Application.Stacks;
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Application.UnitTest.Stacks
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
            OperatingModeBase mode = CreateOperatingMode("ModeA");

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
            OperatingModeBase firstMode = CreateOperatingMode("ModeA");
            OperatingModeBase secondMode = CreateOperatingMode("ModeB");
            OperatingModeBase thirdMode = CreateOperatingMode("ModeC");
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
            OperatingModeBase existingMode = CreateOperatingMode("ModeA");
            OperatingModeBase insertedMode = CreateOperatingMode("ModeB");
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
            OperatingModeBase firstMode = CreateOperatingMode("ModeA");
            OperatingModeBase secondMode = CreateOperatingMode("ModeB");
            OperatingModeBase thirdMode = CreateOperatingMode("ModeC");
            OperatingModeBase insertedMode = CreateOperatingMode("ModeD");
            OperatingModeBase followUpMode = CreateOperatingMode("ModeE");
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
        public void OperatingModeStackAddDuplicateNameThrowsTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            OperatingModeBase existingMode = CreateOperatingMode("ModeA");
            OperatingModeBase duplicateMode = CreateOperatingMode("ModeA");
            stack.Add(existingMode, 1);

            Assert.ThrowsExactly<InvalidOperationException>(() => stack.Add(duplicateMode, 2));
        }


        /// <summary>
        /// Rejects a null mode.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackAddNullModeThrowsTest()
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
            OperatingModeBase existingMode = CreateOperatingMode("ModeA");
            OperatingModeBase insertedMode = CreateOperatingMode("ModeB");
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
            OperatingModeBase firstMode = CreateOperatingMode("ModeA");
            OperatingModeBase secondMode = CreateOperatingMode("ModeB");
            OperatingModeBase thirdMode = CreateOperatingMode("ModeC");
            OperatingModeBase movedMode = CreateOperatingMode("ModeD");
            OperatingModeBase lastMode = CreateOperatingMode("ModeE");
            OperatingModeBase insertedMode = CreateOperatingMode("ModeF");
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
            OperatingModeBase movedMode = CreateOperatingMode("ModeA");
            OperatingModeBase secondMode = CreateOperatingMode("ModeB");
            OperatingModeBase thirdMode = CreateOperatingMode("ModeC");
            OperatingModeBase insertedMode = CreateOperatingMode("ModeD");
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
            OperatingModeBase firstMode = CreateOperatingMode("ModeA");
            OperatingModeBase secondMode = CreateOperatingMode("ModeB");
            OperatingModeBase thirdMode = CreateOperatingMode("ModeC");
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
            OperatingModeBase firstMode = CreateOperatingMode("ModeA");
            OperatingModeBase secondMode = CreateOperatingMode("ModeB");
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
            OperatingModeBase firstMode = CreateOperatingMode("ModeA");
            OperatingModeBase movedMode = CreateOperatingMode("ModeB");
            OperatingModeBase thirdMode = CreateOperatingMode("ModeC");
            OperatingModeBase insertedMode = CreateOperatingMode("ModeD");
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
        public void OperatingModeStackMoveUnknownNameThrowsTest()
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
            OperatingModeBase movedMode = CreateOperatingMode("ModeA");
            OperatingModeBase lastMode = CreateOperatingMode("ModeB");
            stack.Add(movedMode, 1);
            stack.Add(lastMode, int.MaxValue);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => stack.Move("ModeA", int.MaxValue));
            AssertModes(stack, movedMode, lastMode);
        }


        /// <summary>
        /// Rejects a null name for move operations.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackMoveNullNameThrowsTest()
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
        public void OperatingModeStackMoveEmptyNameThrowsTest(string operatingModeName)
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
            OperatingModeBase firstMode = CreateOperatingMode("ModeA");
            OperatingModeBase removedMode = CreateOperatingMode("ModeB");
            OperatingModeBase lastMode = CreateOperatingMode("ModeC");
            OperatingModeBase insertedMode = CreateOperatingMode("ModeD");
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
            OperatingModeBase mode = CreateOperatingMode("ModeA");
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
            OperatingModeBase mode = CreateOperatingMode("ModeA");
            stack.Add(mode, 1);
            stack.Remove(mode);

            Assert.IsEmpty(stack.OperatingModes);
        }


        /// <summary>
        /// Rejects removal of a name that is not in the stack.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackRemoveUnknownNameThrowsTest()
        {
            OperatingModeStack stack = new OperatingModeStack();
            OperatingModeBase mode = CreateOperatingMode("ModeA");
            stack.Add(mode, 1);
            OperatingModeBase missing = CreateOperatingMode("Missing");

            Assert.ThrowsExactly<KeyNotFoundException>(() => stack.Remove(missing));
            AssertModes(stack, mode);
        }


        /// <summary>
        /// Rejects a null name for removal operations.
        /// </summary>
        [TestMethod]
        public void OperatingModeStackRemoveNullNameThrowsTest()
        {
            OperatingModeStack stack = new OperatingModeStack();

            Assert.ThrowsExactly<ArgumentNullException>(() => stack.Remove(null!));
            Assert.IsEmpty(stack.OperatingModes);
        }


        private void AssertModes(OperatingModeStack stack, params OperatingModeBase[] expectedModes)
        {
            CollectionAssert.AreEqual(expectedModes, stack.OperatingModes.ToArray());
        }


        private OperatingModeBase CreateOperatingMode(string name)
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            rampController.Setup(controller => controller.ShallowCopy()).Returns(rampController.Object);

            OperatingModeStackTestConfig config = new OperatingModeStackTestConfig
            {
                ChangedBy = "Test",
                Name = name,
                IsActive = true,
                Type = PowerControlType.Bess,
                RampConfig = new RampTimeConfig
                {
                    ChangedBy = "Test",
                    RampUpTimeSeconds = 0,
                    RampDownTimeSeconds = 0,
                },
            };

            SystemConfig systemConfig = new SystemConfig
            {
                ChangedBy = "Test",
                Type = PowerControlType.Bess,
                ReferenceFrequency = 50,
                NameplateMinimumActivePowerWatt = double.MinValue,
                NameplateMaximumActivePowerWatt = double.MaxValue,
                NameplateMinimumReactivePowerVars = double.MinValue,
                NameplateMaximumReactivePowerVars = double.MaxValue,
            };

            OperatingModeBaseMap map = new OperatingModeBaseMap
            {
                AvailableActivePower = () => null,
                AvailableReactivePower = () => null,
            };

            return new OperatingModeStackTestMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object,
                systemConfig, config, map, rampController.Object);
        }


        private sealed class OperatingModeStackTestMode : OperatingModeBase
        {
            public OperatingModeStackTestMode(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig,
                OperatingModeBaseConfig config, OperatingModeBaseMap map, IRampController rampController)
                : base(logger, timeProvider, publisher, systemConfig, config, map, rampController)
            {
            }

            public override Task CalculateAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

        }


        private sealed class OperatingModeStackTestConfig : OperatingModeBaseConfig
        {
        }
    }
}
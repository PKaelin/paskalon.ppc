// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Application.Stacks;
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.OpenModes;
using paskalON.OperatingModes.Domain.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Transitions;

namespace paskalON.OperatingModes.Application.UnitTest.Stacks
{
    /// <summary>
    /// Tests the selection and lifecycle behavior of <see cref="StackManager"/>.
    /// </summary>
    [TestClass]
    public class StackManagerTest
    {
        /// <summary>
        /// The stack manager under test.
        /// </summary>
        private StackManager? _stackManager;


        /// <summary>
        /// Exclusive active mode.
        /// </summary>
        private ActivePowerFixedMode? _activePowerFixedMode;


        /// <summary>
        /// Exclusive reactive mode.
        /// </summary>
        private ReactivePowerFixedMode? _reactivePowerFixedMode;


        /// <summary>
        /// Base active mode.
        /// </summary>
        private MaximumPowerPointTrackingMode? _maximumPowerPointTrackingMode;


        /// <summary>
        /// Override mode that is not stackable.
        /// </summary>
        private MaintenanceMode? _maintenanceMode;


        /// <summary>
        /// Creates the stack manager with the configured operating modes.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _activePowerFixedMode = ApplicationTestData.CreateActivePowerFixedMode();
            _reactivePowerFixedMode = ApplicationTestData.CreateReactivePowerFixedMode();
            _maximumPowerPointTrackingMode = ApplicationTestData.CreateMaximumPowerPointTrackingMode();
            _maintenanceMode = ApplicationTestData.CreateMaintenanceMode("MaintenanceBess1", "Bess1");
            _stackManager = new StackManager();
            _stackManager.InitializeStack(ApplicationTestData.ToDictionary(_activePowerFixedMode, _reactivePowerFixedMode, _maximumPowerPointTrackingMode, _maintenanceMode));
        }


        /// <summary>
        /// Confirms only stackable operating modes are available for selection.
        /// </summary>
        [TestMethod]
        public void StackManagerInitializeStackOnlyStackableAvailableTest()
        {
            CollectionAssert.AreEquivalent(new IStackableMode[] { _activePowerFixedMode!, _reactivePowerFixedMode!, _maximumPowerPointTrackingMode! },
                _stackManager!.AvailableOperatingModes.ToArray());
            Assert.IsEmpty(_stackManager!.ActiveOperatingModes);
        }


        /// <summary>
        /// Confirms adding an operating mode selects it and records the change for the next calculation cycle.
        /// </summary>
        [TestMethod]
        public void StackManagerAddOperatingModeRecordsChangeTest()
        {
            _stackManager!.AddOperatingMode("ActivePowerFixedMode", 1);

            CollectionAssert.AreEqual(new IStackableMode[] { _activePowerFixedMode! }, _stackManager!.ActiveOperatingModes.ToArray());
            Assert.IsFalse(_activePowerFixedMode!.IsActivated);
            Assert.HasCount(1, _stackManager!.Stack.PendingChanges);
            Assert.AreEqual(OperatingModeChangeKind.Added, _stackManager!.Stack.PendingChanges[0].Kind);
        }


        /// <summary>
        /// Confirms override and unknown operating modes cannot be added.
        /// </summary>
        /// <param name="operatingModeName">The name of the operating mode.</param>
        [TestMethod]
        [DataRow("MaintenanceBess1")]
        [DataRow("Missing")]
        public void StackManagerAddNotSelectableThrowsTest(string operatingModeName)
        {
            Assert.ThrowsExactly<KeyNotFoundException>(() => _stackManager!.AddOperatingMode(operatingModeName, 1));
            Assert.IsFalse(_maintenanceMode!.IsActivated);
        }


        /// <summary>
        /// Confirms an invalid stack is rejected and the operating mode is not enabled.
        /// </summary>
        [TestMethod]
        public void StackManagerAddInvalidStackDoesNotEnableTest()
        {
            _stackManager!.AddOperatingMode("ActivePowerFixedMode", 1);

            Assert.ThrowsExactly<InvalidOperationException>(() => _stackManager!.AddOperatingMode("MaximumPowerPointTrackingMode", 2));
            Assert.IsFalse(_maximumPowerPointTrackingMode!.IsActivated);
            Assert.HasCount(1, _stackManager!.ActiveOperatingModes);
        }


        /// <summary>
        /// Confirms moving an operating mode changes its priority.
        /// </summary>
        [TestMethod]
        public void StackManagerMoveOperatingModeTest()
        {
            _stackManager!.AddOperatingMode("ActivePowerFixedMode", 1);
            _stackManager!.AddOperatingMode("ReactivePowerFixedMode", 2);

            _stackManager!.MoveOperatingMode("ReactivePowerFixedMode", 1);

            CollectionAssert.AreEqual(new IStackableMode[] { _reactivePowerFixedMode!, _activePowerFixedMode! }, _stackManager!.ActiveOperatingModes.ToArray());
        }


        /// <summary>
        /// Confirms removing an operating mode records the change for the next calculation cycle.
        /// </summary>
        [TestMethod]
        public void StackManagerRemoveOperatingModeRecordsChangeTest()
        {
            _stackManager!.AddOperatingMode("ActivePowerFixedMode", 1);

            _stackManager!.RemoveOperatingMode("ActivePowerFixedMode");

            Assert.IsEmpty(_stackManager!.ActiveOperatingModes);
            Assert.HasCount(2, _stackManager!.Stack.PendingChanges);
            Assert.AreEqual(OperatingModeChangeKind.Removed, _stackManager!.Stack.PendingChanges[1].Kind);
            Assert.AreSame(_activePowerFixedMode, _stackManager!.Stack.PendingChanges[1].OperatingMode);
        }


        /// <summary>
        /// Confirms removing an operating mode that is not selected throws.
        /// </summary>
        [TestMethod]
        public void StackManagerRemoveNotSelectedThrowsTest()
        {
            Assert.ThrowsExactly<KeyNotFoundException>(() => _stackManager!.RemoveOperatingMode("ActivePowerFixedMode"));
            Assert.AreEqual(OperatingModeState.Disabled, _activePowerFixedMode!.State);
        }


        /// <summary>
        /// Confirms a replacement takes the priority of its predecessor in one atomic change.
        /// </summary>
        [TestMethod]
        public void StackManagerReplaceOperatingModeTest()
        {
            _stackManager!.AddOperatingMode("ActivePowerFixedMode", 1);
            _stackManager!.AddOperatingMode("ReactivePowerFixedMode", 2);

            _stackManager!.ReplaceOperatingMode("ActivePowerFixedMode", "MaximumPowerPointTrackingMode");

            CollectionAssert.AreEqual(new IStackableMode[] { _maximumPowerPointTrackingMode!, _reactivePowerFixedMode! }, _stackManager!.ActiveOperatingModes.ToArray());
            Assert.AreEqual(OperatingModeChangeKind.Replaced, _stackManager!.Stack.PendingChanges[2].Kind);
            Assert.AreSame(_activePowerFixedMode, _stackManager!.Stack.PendingChanges[2].Predecessor);
        }


        /// <summary>
        /// Confirms invalid names are rejected.
        /// </summary>
        [TestMethod]
        public void StackManagerInvalidNamesThrowTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _stackManager!.AddOperatingMode(null!, 1));
            Assert.ThrowsExactly<ArgumentException>(() => _stackManager!.AddOperatingMode(" ", 1));
            Assert.ThrowsExactly<ArgumentNullException>(() => _stackManager!.RemoveOperatingMode(null!));
            Assert.ThrowsExactly<ArgumentNullException>(() => _stackManager!.ReplaceOperatingMode(null!, "ActivePowerFixedMode"));
            Assert.ThrowsExactly<ArgumentNullException>(() => _stackManager!.InitializeStack(null!));
        }
    }
}

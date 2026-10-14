// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Application.Overrides;
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.OpenModes;
using paskalON.OperatingModes.Domain.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.OperatingModes.Domain.SafetyFunctions;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Application.UnitTest.Overrides
{
    /// <summary>
    /// Tests the activation and lifecycle behavior of <see cref="OverrideManager"/>.
    /// </summary>
    [TestClass]
    public class OverrideManagerTest
    {
        /// <summary>
        /// The override manager under test.
        /// </summary>
        private OverrideManager? _overrideManager;


        /// <summary>
        /// Maintenance mode of DER unit Bess1.
        /// </summary>
        private MaintenanceMode? _maintenanceBess1;


        /// <summary>
        /// Second maintenance mode of DER unit Bess1.
        /// </summary>
        private MaintenanceMode? _maintenanceBess1Second;


        /// <summary>
        /// Plant-wide shutdown mode.
        /// </summary>
        private ShutdownMode? _shutdownMode;


        /// <summary>
        /// Stackable operating mode that is not an override.
        /// </summary>
        private ActivePowerFixedMode? _activePowerFixedMode;


        /// <summary>
        /// Creates the override manager with the configured operating modes.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _maintenanceBess1 = ApplicationTestData.CreateMaintenanceMode("MaintenanceBess1", "Bess1");
            _maintenanceBess1Second = ApplicationTestData.CreateMaintenanceMode("MaintenanceBess1Second", "Bess1");
            _shutdownMode = ApplicationTestData.CreateShutdownMode();
            _activePowerFixedMode = ApplicationTestData.CreateActivePowerFixedMode();
            _overrideManager = new OverrideManager(new OperatingModeOverrides());
            _overrideManager.InitializeOverrides(ApplicationTestData.ToDictionary(_maintenanceBess1, _maintenanceBess1Second, _shutdownMode, _activePowerFixedMode));
        }


        /// <summary>
        /// Confirms only override modes are available for activation.
        /// </summary>
        [TestMethod]
        public void OverrideManagerInitializeOnlyOverridesAvailableTest()
        {
            CollectionAssert.AreEquivalent(new IOverrideMode[] { _maintenanceBess1!, _maintenanceBess1Second!, _shutdownMode! }, _overrideManager!.AvailableOperatingModes.ToArray());
            Assert.IsEmpty(_overrideManager!.ActiveOperatingModes);
        }


        /// <summary>
        /// Confirms activating an override mode overrides its DER unit. The mode is activated by the next calculation cycle.
        /// </summary>
        [TestMethod]
        public void OverrideManagerActivateOperatingModeTest()
        {
            _overrideManager!.ActivateOperatingMode("MaintenanceBess1");

            Assert.IsFalse(_maintenanceBess1!.IsActivated);
            Assert.IsTrue(_overrideManager!.Overrides.IsOverridden("Bess1"));
            CollectionAssert.AreEqual(new IOverrideMode[] { _maintenanceBess1! }, _overrideManager!.ActiveOperatingModes.ToArray());
        }


        /// <summary>
        /// Confirms a second override of the same DER unit is rejected.
        /// </summary>
        [TestMethod]
        public void OverrideManagerActivateSameUnitThrowsTest()
        {
            _overrideManager!.ActivateOperatingMode("MaintenanceBess1");

            Assert.ThrowsExactly<InvalidOperationException>(() => _overrideManager!.ActivateOperatingMode("MaintenanceBess1Second"));
            Assert.IsFalse(_maintenanceBess1Second!.IsActivated);
        }


        /// <summary>
        /// Confirms deactivating an override mode releases its DER unit.
        /// </summary>
        [TestMethod]
        public void OverrideManagerDeactivateOperatingModeTest()
        {
            _overrideManager!.ActivateOperatingMode("MaintenanceBess1");

            _overrideManager!.DeactivateOperatingMode("MaintenanceBess1");

            Assert.IsEmpty(_overrideManager!.ActiveOperatingModes);
            Assert.IsFalse(_overrideManager!.Overrides.IsOverridden("Bess1"));
        }


        /// <summary>
        /// Confirms stackable and unknown operating modes cannot be activated.
        /// </summary>
        /// <param name="operatingModeName">The name of the operating mode.</param>
        [TestMethod]
        [DataRow("ActivePowerFixedMode")]
        [DataRow("Missing")]
        public void OverrideManagerActivateNotOverrideThrowsTest(string operatingModeName)
        {
            Assert.ThrowsExactly<KeyNotFoundException>(() => _overrideManager!.ActivateOperatingMode(operatingModeName));
            Assert.IsFalse(_activePowerFixedMode!.IsActivated);
        }


        /// <summary>
        /// Confirms the operator setpoints of a maintenance mode are set.
        /// </summary>
        [TestMethod]
        public void OverrideManagerSetSetpointMaintenanceTest()
        {
            _overrideManager!.SetSetpoint("MaintenanceBess1", new ActivePower(1500000), new ReactivePower(-500000));

            Assert.AreEqual(1500000, _maintenanceBess1!.SetpointActivePower.Watts);
            Assert.AreEqual(-500000, _maintenanceBess1!.SetpointReactivePower.VoltAmperesReactive);
        }


        /// <summary>
        /// Confirms override modes without operator setpoints reject setpoints.
        /// </summary>
        [TestMethod]
        public void OverrideManagerSetSetpointNotMaintenanceThrowsTest()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => _overrideManager!.SetSetpoint("ShutdownMode", new ActivePower(0), new ReactivePower(0)));
        }
    }
}

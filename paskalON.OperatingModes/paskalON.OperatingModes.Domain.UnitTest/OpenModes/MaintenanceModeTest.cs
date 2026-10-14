// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes;
using paskalON.OperatingModes.Domain.OpenModes;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.UnitTest.OpenModes
{
    /// <summary>
    /// Tests <see cref="MaintenanceMode"/>. The ramp behavior is tested in the power control tests.
    /// </summary>
    [TestClass]
    public class MaintenanceModeTest
    {
        /// <summary>
        /// The operating mode under test.
        /// </summary>
        private MaintenanceMode? _mode;


        /// <summary>
        /// The mocked active power ramp controller.
        /// </summary>
        private Mock<IRampController>? _rampActive;


        /// <summary>
        /// The mocked reactive power ramp controller.
        /// </summary>
        private Mock<IRampController>? _rampReactive;


        /// <summary>
        /// Creates the maintenance mode of DER unit Bess1 with 2 MW / 1 MVar available power.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            MaintenanceModeMap map = new MaintenanceModeMap
            {
                AvailableActivePower = () => new ActivePower(2000000),
                AvailableReactivePower = () => new ReactivePower(1000000),
                DerUnitName = "Bess1"
            };
            MaintenanceModeConfig config = new MaintenanceModeConfig
            {
                ChangedBy = "Test",
                Name = "MaintenanceBess1",
                IsActive = true,
                Type = PowerControlType.Bess,
                DerUnitName = "Bess1",
                DeadbandSetpoint = OperatingModeTestData.Deadband,
                DeadbandAvailable = OperatingModeTestData.Deadband,
                RampConfig = OperatingModeTestData.CreateRampConfig()
            };
            _rampActive = new Mock<IRampController>();
            _rampReactive = new Mock<IRampController>();
            _rampActive.Setup(controller => controller.ShallowCopy()).Returns(_rampReactive.Object);
            _mode = new MaintenanceMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object, OperatingModeTestData.CreateSystemConfig(),
                config, map, _rampActive.Object);
        }


        /// <summary>
        /// Confirms the mode is an override mode for its DER unit with both power axes.
        /// </summary>
        [TestMethod]
        public void MaintenanceModeConstructorTest()
        {
            Assert.AreEqual(OperatingModeClassification.Override, _mode!.Classification);
            Assert.AreEqual("Bess1", _mode!.TargetDerUnit);
            Assert.IsNotNull(_mode!.ActiveControl);
            Assert.IsNotNull(_mode!.ReactiveControl);
            Assert.AreEqual(0, _mode!.SetpointActivePower.Watts);
            Assert.AreEqual(0, _mode!.SetpointReactivePower.VoltAmperesReactive);
        }


        /// <summary>
        /// Confirms the operator setpoints are stored.
        /// </summary>
        [TestMethod]
        public void MaintenanceModeSetSetpointTest()
        {
            _mode!.SetSetpoint(new ActivePower(1500000), new ReactivePower(-500000));

            Assert.AreEqual(1500000, _mode!.SetpointActivePower.Watts);
            Assert.AreEqual(-500000, _mode!.SetpointReactivePower.VoltAmperesReactive);
        }


        /// <summary>
        /// Confirms operator setpoints outside the nameplate are rejected.
        /// </summary>
        /// <param name="activePower">The active power setpoint in Watts.</param>
        /// <param name="reactivePower">The reactive power setpoint in Vars.</param>
        [TestMethod]
        [DataRow(10000001, 0)]
        [DataRow(-10000001, 0)]
        [DataRow(0, 5000001)]
        [DataRow(0, -5000001)]
        public void MaintenanceModeSetSetpointOutsideNameplateTest(double activePower, double reactivePower)
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _mode!.SetSetpoint(new ActivePower(activePower), new ReactivePower(reactivePower)));
        }


        /// <summary>
        /// Confirms the targets follow the operator setpoints limited to the unit's available power and the signals stay unchanged.
        /// </summary>
        [TestMethod]
        public void MaintenanceModeCalculateUsesOperatorSetpointsTest()
        {
            _mode!.SetSetpoint(new ActivePower(3000000), new ReactivePower(-500000));
            _mode!.Activate();
            OperatingModeSignals signals = new OperatingModeSignals { SetpointActivePower = new ActivePower(8000000), SetpointReactivePower = new ReactivePower(4000000) };

            OperatingModeSignals result = _mode!.Calculate(signals);

            Assert.AreEqual(0, result.FinalActivePower.Watts);
            Assert.AreEqual(0, result.FinalReactivePower.VoltAmperesReactive);
            _rampActive!.Verify(controller => controller.Start(0, 2000000), Times.Once);
            _rampReactive!.Verify(controller => controller.Start(0, -500000), Times.Once);
        }
    }
}

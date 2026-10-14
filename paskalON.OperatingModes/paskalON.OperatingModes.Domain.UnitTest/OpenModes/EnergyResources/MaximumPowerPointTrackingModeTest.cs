// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.UnitTest.OpenModes.EnergyResources
{
    /// <summary>
    /// Tests <see cref="MaximumPowerPointTrackingMode"/>. The ramp behavior is tested in the power control tests.
    /// </summary>
    [TestClass]
    public class MaximumPowerPointTrackingModeTest
    {
        /// <summary>
        /// The operating mode under test.
        /// </summary>
        private MaximumPowerPointTrackingMode? _mode;


        /// <summary>
        /// The operating mode map.
        /// </summary>
        private MaximumPowerPointTrackingModeMap? _map;


        /// <summary>
        /// The mocked ramp controller.
        /// </summary>
        private Mock<IRampController>? _rampController;


        /// <summary>
        /// Creates the operating mode with 6 MW available power.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _map = new MaximumPowerPointTrackingModeMap { AvailableActivePower = () => new ActivePower(6000000), AvailableReactivePower = () => null };
            _rampController = OperatingModeTestData.CreateRampController();
            MaximumPowerPointTrackingModeConfig config = new MaximumPowerPointTrackingModeConfig
            {
                ChangedBy = "Test",
                Name = "MaximumPowerPointTrackingMode",
                IsActive = true,
                Type = PowerControlType.Solar,
                DeadbandSetpoint = OperatingModeTestData.Deadband,
                DeadbandAvailable = OperatingModeTestData.Deadband,
                RampConfig = OperatingModeTestData.CreateRampConfig()
            };

            _mode = new MaximumPowerPointTrackingMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object, OperatingModeTestData.CreateSystemConfig(),
                config, _map, _rampController.Object);
        }


        /// <summary>
        /// Confirms the mode is a base active mode.
        /// </summary>
        [TestMethod]
        public void MaximumPowerPointTrackingModeConstructorTest()
        {
            Assert.AreEqual(OperatingModeClassification.BaseActive, _mode!.Classification);
            Assert.IsNotNull(_mode!.ActiveControl);
            Assert.IsNull(_mode!.ReactiveControl);
        }


        /// <summary>
        /// Confirms the available power is used as setpoint and the supervisory setpoint is ignored.
        /// </summary>
        [TestMethod]
        public void MaximumPowerPointTrackingModeCalculateUsesAvailableAsSetpointTest()
        {
            _rampController!.Setup(controller => controller.Calculate()).Returns(6000000);
            _mode!.Activate();

            OperatingModeSignals signals = _mode!.Calculate(new OperatingModeSignals { SetpointActivePower = new ActivePower(1000000), SetpointReactivePower = new ReactivePower(0) });

            Assert.AreEqual(6000000, signals.BaseActivePower.Watts);
            Assert.AreEqual(6000000, _mode!.ActiveControl!.Setpoint.Watts);
            _rampController!.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Confirms the mode waits without available power.
        /// </summary>
        [TestMethod]
        public void MaximumPowerPointTrackingModeCalculateWithoutAvailableWaitsTest()
        {
            _map!.AvailableActivePower = () => null;
            _mode!.Activate();

            OperatingModeSignals signals = _mode!.Calculate(new OperatingModeSignals { SetpointActivePower = new ActivePower(1000000), SetpointReactivePower = new ReactivePower(0) });

            Assert.AreEqual(0, signals.BaseActivePower.Watts);
            Assert.AreEqual(OperatingModeState.Enabling, _mode!.State);
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.UnitTest.OpenModes.VoltageReactives
{
    /// <summary>
    /// Tests <see cref="ReactivePowerFixedMode"/>. The ramp behavior is tested in the power control tests.
    /// </summary>
    [TestClass]
    public class ReactivePowerFixedModeTest
    {
        /// <summary>
        /// The operating mode under test.
        /// </summary>
        private ReactivePowerFixedMode? _mode;


        /// <summary>
        /// The mocked ramp controller.
        /// </summary>
        private Mock<IRampController>? _rampController;


        /// <summary>
        /// Creates the operating mode with 4 MVar available power.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            ReactivePowerFixedModeMap map = new ReactivePowerFixedModeMap { AvailableActivePower = () => new ActivePower(8000000), AvailableReactivePower = () => new ReactivePower(4000000) };
            _rampController = OperatingModeTestData.CreateRampController();
            _mode = new ReactivePowerFixedMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object, OperatingModeTestData.CreateSystemConfig(),
                CreateConfig(), map, _rampController.Object);
        }


        /// <summary>
        /// Confirms the mode is an exclusive reactive mode with a reactive power control only that uses the ramp controller directly.
        /// </summary>
        [TestMethod]
        public void ReactivePowerFixedModeConstructorTest()
        {
            Assert.AreEqual(OperatingModeClassification.ExclusiveReactive, _mode!.Classification);
            Assert.IsNull(_mode!.ActiveControl);
            Assert.AreSame(_rampController!.Object, _mode!.ReactiveControl!.RampController);
            _rampController!.Verify(controller => controller.ShallowCopy(), Times.Never);
        }


        /// <summary>
        /// Confirms the ramped supervisory setpoint is written into the base reactive power.
        /// </summary>
        /// <param name="setpoint">The supervisory setpoint in Vars.</param>
        /// <param name="expectedTargetSetpoint">The expected target setpoint in Vars.</param>
        [TestMethod]
        [DataRow(1000000, 1000000)]
        [DataRow(-6000000, -4000000)]
        public void ReactivePowerFixedModeCalculateWritesBaseReactivePowerTest(double setpoint, double expectedTargetSetpoint)
        {
            _rampController!.Setup(controller => controller.Calculate()).Returns(expectedTargetSetpoint);
            _mode!.Activate();

            OperatingModeSignals signals = _mode!.Calculate(new OperatingModeSignals { SetpointActivePower = new ActivePower(5000000), SetpointReactivePower = new ReactivePower(setpoint) });

            Assert.AreEqual(expectedTargetSetpoint, signals.BaseReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, signals.BaseActivePower.Watts);
            Assert.AreEqual(OperatingModeState.Enabled, _mode!.State);
            _rampController!.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Creates the operating mode configuration.
        /// </summary>
        /// <returns>The operating mode configuration.</returns>
        private ReactivePowerFixedModeConfig CreateConfig()
        {
            return new ReactivePowerFixedModeConfig
            {
                ChangedBy = "Test",
                Name = "ReactivePowerFixedMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = OperatingModeTestData.Deadband,
                DeadbandAvailable = OperatingModeTestData.Deadband,
                RampConfig = OperatingModeTestData.CreateRampConfig()
            };
        }
    }
}

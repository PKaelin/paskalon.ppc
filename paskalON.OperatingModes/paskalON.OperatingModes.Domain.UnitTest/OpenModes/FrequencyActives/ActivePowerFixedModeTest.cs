// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.UnitTest.OpenModes.FrequencyActives
{
    /// <summary>
    /// Tests <see cref="ActivePowerFixedMode"/>. The ramp behavior is tested in the power control tests.
    /// </summary>
    [TestClass]
    public class ActivePowerFixedModeTest
    {
        /// <summary>
        /// The operating mode under test.
        /// </summary>
        private ActivePowerFixedMode? _mode;


        /// <summary>
        /// The operating mode map.
        /// </summary>
        private ActivePowerFixedModeMap? _map;


        /// <summary>
        /// The mocked ramp controller.
        /// </summary>
        private Mock<IRampController>? _rampController;


        /// <summary>
        /// Creates the operating mode with 8 MW available power.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _map = new ActivePowerFixedModeMap { AvailableActivePower = () => new ActivePower(8000000), AvailableReactivePower = () => new ReactivePower(4000000) };
            _rampController = OperatingModeTestData.CreateRampController();
            _mode = new ActivePowerFixedMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object, OperatingModeTestData.CreateSystemConfig(),
                CreateConfig(), _map, _rampController.Object);
        }


        /// <summary>
        /// Confirms the mode is an exclusive active mode with an active power control only.
        /// </summary>
        [TestMethod]
        public void ActivePowerFixedModeConstructorTest()
        {
            Assert.AreEqual(OperatingModeClassification.ExclusiveActive, _mode!.Classification);
            Assert.IsNotNull(_mode!.ActiveControl);
            Assert.IsNull(_mode!.ReactiveControl);
            Assert.AreEqual(OperatingModeState.Disabled, _mode!.State);
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void ActivePowerFixedModeConstructorNullArgumentsThrowTest()
        {
            IMetricsPublisher publisher = new Mock<IMetricsPublisher>().Object;
            SystemConfig systemConfig = OperatingModeTestData.CreateSystemConfig();

            Assert.ThrowsExactly<ArgumentNullException>(() => new ActivePowerFixedMode(NullLogger.Instance, TimeProvider.System, publisher, systemConfig, null!, _map!, _rampController!.Object));
            Assert.ThrowsExactly<ArgumentNullException>(() => new ActivePowerFixedMode(NullLogger.Instance, TimeProvider.System, publisher, systemConfig, CreateConfig(), null!, _rampController!.Object));
        }


        /// <summary>
        /// Confirms the supervisory setpoint reduced to the available power is written into the base active power immediately.
        /// </summary>
        /// <remarks>
        /// The transition from the current applied target is done by the system operating mode control.
        /// </remarks>
        /// <param name="setpoint">The supervisory setpoint in Watts.</param>
        /// <param name="expectedBase">The expected base active power in Watts.</param>
        [TestMethod]
        [DataRow(1000000, 1000000)]
        [DataRow(9000000, 8000000)]
        [DataRow(-3000000, -3000000)]
        public void ActivePowerFixedModeCalculateWritesBaseActivePowerTest(double setpoint, double expectedBase)
        {
            _map!.AvailableActivePower = () => new ActivePower(8000000 * Math.Sign(setpoint));
            _mode!.Activate();

            OperatingModeSignals signals = _mode!.Calculate(CreateSignals(setpoint));

            Assert.AreEqual(expectedBase, signals.BaseActivePower.Watts);
            Assert.AreEqual(OperatingModeState.Enabled, _mode!.State);
            Assert.AreEqual(0, signals.BaseReactivePower.VoltAmperesReactive);
            _rampController!.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Confirms a supervisory setpoint change after the target is established ramps the base active power.
        /// </summary>
        [TestMethod]
        public void ActivePowerFixedModeCalculateSetpointChangeRampsTest()
        {
            _mode!.Activate();
            _mode!.Calculate(CreateSignals(1000000));
            _rampController!.Setup(controller => controller.Calculate()).Returns(1500000);

            OperatingModeSignals signals = _mode!.Calculate(CreateSignals(3000000));

            Assert.AreEqual(1500000, signals.BaseActivePower.Watts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _mode!.State);
            _rampController!.Verify(controller => controller.Start(1000000, 3000000), Times.Once);
        }



        /// <summary>
        /// Confirms a disabled mode writes zero.
        /// </summary>
        [TestMethod]
        public void ActivePowerFixedModeCalculateDisabledWritesZeroTest()
        {
            OperatingModeSignals signals = _mode!.Calculate(CreateSignals(1000000));

            Assert.AreEqual(0, signals.BaseActivePower.Watts);
            _rampController!.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Confirms null signals are rejected.
        /// </summary>
        [TestMethod]
        public void ActivePowerFixedModeCalculateNullSignalsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _mode!.Calculate(null!));
        }


        /// <summary>
        /// Creates the operating mode configuration.
        /// </summary>
        /// <returns>The operating mode configuration.</returns>
        private ActivePowerFixedModeConfig CreateConfig()
        {
            return new ActivePowerFixedModeConfig
            {
                ChangedBy = "Test",
                Name = "ActivePowerFixedMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = OperatingModeTestData.Deadband,
                DeadbandAvailable = OperatingModeTestData.Deadband,
                RampConfig = OperatingModeTestData.CreateRampConfig()
            };
        }


        /// <summary>
        /// Creates the signals with the supervisory active power setpoint.
        /// </summary>
        /// <param name="setpoint">The supervisory active power setpoint in Watts.</param>
        /// <returns>The operating mode signals.</returns>
        private OperatingModeSignals CreateSignals(double setpoint)
        {
            return new OperatingModeSignals { SetpointActivePower = new ActivePower(setpoint), SetpointReactivePower = new ReactivePower(1000000) };
        }
    }
}

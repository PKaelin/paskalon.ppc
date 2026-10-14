// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Domain.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.UnitTest.ClosedModes.VoltageReactives
{
    /// <summary>
    /// Tests <see cref="ReactivePowerMode"/>. The ramp and feedback behavior is tested in the control tests.
    /// </summary>
    [TestClass]
    public class ReactivePowerModeTest
    {
        /// <summary>
        /// The operating mode under test.
        /// </summary>
        private ReactivePowerMode? _mode;


        /// <summary>
        /// The operating mode map.
        /// </summary>
        private ReactivePowerModeMap? _map;


        /// <summary>
        /// Creates the operating mode with 4 MVar available power and a ramp that reaches -1 MVar.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _map = new ReactivePowerModeMap
            {
                AvailableActivePower = () => new ActivePower(8000000),
                AvailableReactivePower = () => new ReactivePower(-4000000),
                ReactivePowerAtPoi = () => null
            };
            Mock<IRampController> rampController = OperatingModeTestData.CreateRampController();
            rampController.Setup(controller => controller.Calculate()).Returns(-1000000);
            ReactivePowerModeConfig config = new ReactivePowerModeConfig
            {
                ChangedBy = "Test",
                Name = "ReactivePowerMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = OperatingModeTestData.Deadband,
                DeadbandAvailable = OperatingModeTestData.Deadband,
                DeadbandError = 5000,
                ProportionalGain = 0.5,
                RampConfig = OperatingModeTestData.CreateRampConfig()
            };
            _mode = new ReactivePowerMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object, OperatingModeTestData.CreateSystemConfig(),
                config, _map, rampController.Object);
        }


        /// <summary>
        /// Confirms the mode is a base reactive mode with a reactive power feedback.
        /// </summary>
        [TestMethod]
        public void ReactivePowerModeConstructorTest()
        {
            Assert.AreEqual(OperatingModeClassification.BaseReactive, _mode!.Classification);
            Assert.IsNull(_mode!.ActiveControl);
            Assert.IsNotNull(_mode!.ReactiveControl);
            Assert.IsNull(_mode!.ActiveFeedback);
            Assert.AreEqual("ReactivePower", _mode!.ReactiveFeedback!.Quantity);
        }


        /// <summary>
        /// Confirms the corrected target is written into the base reactive power using the measurement at the POI from the map.
        /// </summary>
        /// <param name="measured">The measured reactive power at the POI in Vars.</param>
        /// <param name="expectedBase">The expected base reactive power in Vars.</param>
        [TestMethod]
        [DataRow(null, -1000000)]
        [DataRow(-900000.0, -1050000)]
        public void ReactivePowerModeCalculateWritesCorrectedBaseReactivePowerTest(double? measured, double expectedBase)
        {
            _map!.ReactivePowerAtPoi = () => measured.HasValue ? new ReactivePower(measured.Value) : null;
            _mode!.Activate();

            OperatingModeSignals signals = _mode!.Calculate(new OperatingModeSignals { SetpointActivePower = new ActivePower(0), SetpointReactivePower = new ReactivePower(-1000000) });

            Assert.AreEqual(expectedBase, signals.BaseReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, signals.BaseActivePower.Watts);
        }
    }
}

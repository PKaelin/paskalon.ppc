// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Domain.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.UnitTest.ClosedModes.FrequencyActives
{
    /// <summary>
    /// Tests <see cref="ActivePowerMode"/>. The ramp and feedback behavior is tested in the control tests.
    /// </summary>
    [TestClass]
    public class ActivePowerModeTest
    {
        /// <summary>
        /// The operating mode under test.
        /// </summary>
        private ActivePowerMode? _mode;


        /// <summary>
        /// The operating mode map.
        /// </summary>
        private ActivePowerModeMap? _map;


        /// <summary>
        /// The mocked ramp controller.
        /// </summary>
        private Mock<IRampController>? _rampController;


        /// <summary>
        /// Creates the operating mode with 8 MW available power and a ramp that reaches 1 MW.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _map = new ActivePowerModeMap
            {
                AvailableActivePower = () => new ActivePower(8000000),
                AvailableReactivePower = () => new ReactivePower(4000000),
                ActivePowerAtPoi = () => null
            };

            _rampController = OperatingModeTestData.CreateRampController();
            _rampController.Setup(controller => controller.Calculate()).Returns(1000000);

            ActivePowerModeConfig config = new ActivePowerModeConfig
            {
                ChangedBy = "Test",
                Name = "ActivePowerMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = OperatingModeTestData.Deadband,
                DeadbandAvailable = OperatingModeTestData.Deadband,
                DeadbandError = 5000,
                ProportionalGain = 0.25,
                RampConfig = OperatingModeTestData.CreateRampConfig()
            };

            _mode = new ActivePowerMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object, OperatingModeTestData.CreateSystemConfig(),
                config, _map, _rampController.Object);
        }


        /// <summary>
        /// Confirms the mode is a base active mode with an active power feedback.
        /// </summary>
        [TestMethod]
        public void ActivePowerModeConstructorTest()
        {
            Assert.AreEqual(OperatingModeClassification.BaseActive, _mode!.Classification);
            Assert.IsNotNull(_mode!.ActiveControl);
            Assert.IsNull(_mode!.ReactiveControl);
            Assert.AreEqual("ActivePower", _mode!.ActiveFeedback!.Quantity);
            Assert.AreEqual(0.25, _mode!.ActiveFeedback!.Settings.ProportionalGain);
            Assert.AreEqual(5000, _mode!.ActiveFeedback!.Settings.DeadbandError);
            Assert.IsNull(_mode!.ReactiveFeedback);
        }


        /// <summary>
        /// Confirms the corrected target is written into the base active power using the measurement at the POI from the map.
        /// </summary>
        /// <param name="measured">The measured active power at the POI in Watts.</param>
        /// <param name="expectedBase">The expected base active power in Watts.</param>
        [TestMethod]
        [DataRow(null, 1000000)]
        [DataRow(900000.0, 1025000)]
        [DataRow(1100000.0, 975000)]
        [DataRow(998000.0, 1000000)]
        public void ActivePowerModeCalculateWritesCorrectedBaseActivePowerTest(double? measured, double expectedBase)
        {
            _map!.ActivePowerAtPoi = () => measured.HasValue ? new ActivePower(measured.Value) : null;
            _mode!.Activate();

            OperatingModeSignals signals = _mode!.Calculate(new OperatingModeSignals { SetpointActivePower = new ActivePower(1000000), SetpointReactivePower = new ReactivePower(0) });

            Assert.AreEqual(expectedBase, signals.BaseActivePower.Watts);
            Assert.AreEqual(1000000, _mode!.ActiveControl!.Target.Watts);
            Assert.AreEqual(expectedBase, _mode!.ActiveFeedback!.Corrected.Watts);
        }


        /// <summary>
        /// Confirms activating resets the feedback correction.
        /// </summary>
        [TestMethod]
        public void ActivePowerModeActivateResetsFeedbackTest()
        {
            _map!.ActivePowerAtPoi = () => new ActivePower(900000);
            _mode!.Activate();
            _mode!.Calculate(new OperatingModeSignals { SetpointActivePower = new ActivePower(1000000), SetpointReactivePower = new ReactivePower(0) });
            _mode!.Deactivate();

            _mode!.Activate();

            Assert.AreEqual(0, _mode!.ActiveFeedback!.ErrorAdjustment.Watts);
        }


        /// <summary>
        /// Confirms resuming after holding discards the feedback correction built up from the measurements before holding.
        /// </summary>
        [TestMethod]
        public void ActivePowerModeResumeResetsFeedbackTest()
        {
            _map!.ActivePowerAtPoi = () => new ActivePower(900000);
            _mode!.Activate();
            _mode!.Calculate(new OperatingModeSignals { SetpointActivePower = new ActivePower(1000000), SetpointReactivePower = new ReactivePower(0) });
            double adjustmentBeforeResume = _mode!.ActiveFeedback!.ErrorAdjustment.Watts;

            _mode!.Resume();

            Assert.AreEqual(25000, adjustmentBeforeResume);
            Assert.AreEqual(0, _mode!.ActiveFeedback!.ErrorAdjustment.Watts);
            Assert.IsTrue(_mode!.IsActivated);
            Assert.AreEqual(1000000, _mode!.ActiveControl!.Target.Watts);
        }
    }
}

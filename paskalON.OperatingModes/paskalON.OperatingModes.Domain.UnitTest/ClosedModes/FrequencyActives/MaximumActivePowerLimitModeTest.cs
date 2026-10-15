// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Domain.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Controls;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.UnitTest.ClosedModes.FrequencyActives
{
    /// <summary>
    /// Tests <see cref="MaximumActivePowerLimitMode"/>. The ramp-limited integral correction is tested in the feedback control tests.
    /// </summary>
    [TestClass]
    public class MaximumActivePowerLimitModeTest
    {
        /// <summary>
        /// The operating mode map.
        /// </summary>
        private MaximumActivePowerLimitModeMap? _map;


        /// <summary>
        /// The mocked ramp controller of the power control.
        /// </summary>
        private Mock<IRampController>? _rampController;


        /// <summary>
        /// The mocked ramp controller of the feedback correction (shallow copy of the ramp controller).
        /// </summary>
        private Mock<IRampController>? _feedbackRamp;


        /// <summary>
        /// Creates the map without a measurement at the POI and the mocked ramp controllers.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _map = new MaximumActivePowerLimitModeMap
            {
                AvailableActivePower = () => new ActivePower(8000000),
                AvailableReactivePower = () => new ReactivePower(4000000),
                ActivePowerAtPoi = () => null
            };
            _feedbackRamp = new Mock<IRampController>();
            _rampController = new Mock<IRampController>();
            _rampController.Setup(controller => controller.ShallowCopy()).Returns(_feedbackRamp.Object);
        }


        /// <summary>
        /// Confirms the mode is a limiting active mode with a ramp-limited integral feedback on the active power at the POI.
        /// </summary>
        [TestMethod]
        public void MaximumActivePowerLimitModeConstructorTest()
        {
            MaximumActivePowerLimitMode mode = CreateMode(5000000);

            Assert.AreEqual(OperatingModeClassification.LimitingActive, mode.Classification);
            Assert.IsNotNull(mode.ActiveControl);
            Assert.IsNull(mode.ReactiveControl);
            Assert.AreEqual("ActivePower", mode.ActiveFeedback!.Quantity);
            Assert.AreEqual(FeedbackCorrection.RampLimitedIntegral, mode.ActiveFeedback!.Settings.Correction);
            Assert.AreEqual(5000000, mode.MaximumActivePower.Watts);
        }


        /// <summary>
        /// Confirms the nameplate is the cap if no maximum active power limit is configured.
        /// </summary>
        [TestMethod]
        public void MaximumActivePowerLimitModeWithoutConfiguredCapUsesNameplateTest()
        {
            MaximumActivePowerLimitMode mode = CreateMode(null);

            ActivePower maximum = mode.MaximumActivePower;

            Assert.AreEqual(OperatingModeTestData.NameplateActivePowerWatt, maximum.Watts);
        }


        /// <summary>
        /// Confirms the final active power is limited to the cap and values below the cap are not changed.
        /// </summary>
        /// <param name="baseActivePower">The base active power in Watts.</param>
        /// <param name="additiveDelta">The additive delta in Watts.</param>
        /// <param name="expectedLimited">The expected limited active power in Watts.</param>
        /// <param name="expectedIsLimiting">Whether the limit is expected to be binding.</param>
        [TestMethod]
        [DataRow(4000000, 0, 4000000, false)]
        [DataRow(6000000, 0, 5000000, true)]
        [DataRow(4000000, 2000000, 5000000, true)]
        [DataRow(-3000000, 0, -3000000, false)]
        public void MaximumActivePowerLimitModeCalculateLimitsFinalActivePowerTest(double baseActivePower, double additiveDelta, double expectedLimited, bool expectedIsLimiting)
        {
            MaximumActivePowerLimitMode mode = CreateActivatedMode(5000000);
            OperatingModeSignals signals = CreateSignals(baseActivePower, additiveDelta);

            mode.Calculate(signals);

            Assert.AreEqual(expectedLimited, signals.FinalActivePower.Watts);
            Assert.AreEqual(expectedIsLimiting, mode.IsLimiting);
            _feedbackRamp!.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Confirms a binding limit is corrected with the measured active power at the POI.
        /// </summary>
        [TestMethod]
        public void MaximumActivePowerLimitModeCalculateCorrectsBindingLimitTest()
        {
            _map!.ActivePowerAtPoi = () => new ActivePower(4800000);
            _feedbackRamp!.Setup(controller => controller.Calculate()).Returns(100000);
            MaximumActivePowerLimitMode mode = CreateActivatedMode(5000000);
            OperatingModeSignals signals = CreateSignals(6000000, 0);

            mode.Calculate(signals);

            Assert.IsTrue(mode.IsLimiting);
            Assert.AreEqual(5100000, signals.FinalActivePower.Watts);
            _feedbackRamp!.Verify(controller => controller.Start(0, 200000), Times.Once);
        }


        /// <summary>
        /// Confirms the correction is reset once the limit is no longer binding.
        /// </summary>
        [TestMethod]
        public void MaximumActivePowerLimitModeCalculateResetsCorrectionWhenNotBindingTest()
        {
            _map!.ActivePowerAtPoi = () => new ActivePower(4800000);
            _feedbackRamp!.Setup(controller => controller.Calculate()).Returns(100000);
            MaximumActivePowerLimitMode mode = CreateActivatedMode(5000000);
            mode.Calculate(CreateSignals(6000000, 0));
            OperatingModeSignals signals = CreateSignals(4000000, 0);

            mode.Calculate(signals);

            Assert.IsFalse(mode.IsLimiting);
            Assert.AreEqual(4000000, signals.FinalActivePower.Watts);
            Assert.AreEqual(0, mode.ActiveFeedback!.ErrorAdjustment.Watts);
            // Once by the reset at the activation and once when the limit is no longer binding
            _feedbackRamp!.Verify(controller => controller.Stop(), Times.Exactly(2));
        }


        /// <summary>
        /// Confirms null signals are rejected.
        /// </summary>
        [TestMethod]
        public void MaximumActivePowerLimitModeCalculateNullSignalsThrowsTest()
        {
            MaximumActivePowerLimitMode mode = CreateActivatedMode(5000000);

            Assert.ThrowsExactly<ArgumentNullException>(() => mode.Calculate(null!));
        }


        /// <summary>
        /// Creates the mode.
        /// </summary>
        /// <param name="maximumActivePowerWatts">The configured cap in Watts or null for the nameplate.</param>
        /// <returns>The maximum active power limit mode.</returns>
        private MaximumActivePowerLimitMode CreateMode(double? maximumActivePowerWatts)
        {
            MaximumActivePowerLimitModeConfig config = new MaximumActivePowerLimitModeConfig
            {
                ChangedBy = "Test",
                Name = "MaximumActivePowerLimitMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = OperatingModeTestData.Deadband,
                DeadbandAvailable = OperatingModeTestData.Deadband,
                DeadbandError = 5000,
                MaximumActivePowerLimitWatts = maximumActivePowerWatts,
                RampConfig = OperatingModeTestData.CreateRampConfig()
            };

            return new MaximumActivePowerLimitMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object, OperatingModeTestData.CreateSystemConfig(),
                config, _map!, _rampController!.Object);
        }


        /// <summary>
        /// Creates the mode activated as by the system operating mode control.
        /// </summary>
        /// <param name="maximumActivePowerWatts">The configured cap in Watts or null for the nameplate.</param>
        /// <returns>The activated maximum active power limit mode.</returns>
        private MaximumActivePowerLimitMode CreateActivatedMode(double? maximumActivePowerWatts)
        {
            MaximumActivePowerLimitMode mode = CreateMode(maximumActivePowerWatts);
            mode.Activate();

            return mode;
        }


        /// <summary>
        /// Creates the signals with the base active power and the additive delta of the stack before the limiting mode.
        /// </summary>
        /// <param name="baseActivePower">The base active power in Watts.</param>
        /// <param name="additiveDelta">The additive delta in Watts.</param>
        /// <returns>The operating mode signals.</returns>
        private OperatingModeSignals CreateSignals(double baseActivePower, double additiveDelta)
        {
            return new OperatingModeSignals
            {
                SetpointActivePower = new ActivePower(baseActivePower),
                SetpointReactivePower = new ReactivePower(0),
                BaseActivePower = new ActivePower(baseActivePower),
                AdditiveActivePowerDelta = new ActivePower(additiveDelta)
            };
        }
    }
}

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
    /// Tests <see cref="ActivePowerLimitMode"/>. The ramp-limited integral correction is tested in the feedback control tests.
    /// </summary>
    [TestClass]
    public class ActivePowerLimitModeTest
    {
        /// <summary>
        /// The operating mode map.
        /// </summary>
        private ActivePowerLimitModeMap? _map;


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
            _map = new ActivePowerLimitModeMap
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
        public void ActivePowerLimitModeConstructorTest()
        {
            ActivePowerLimitMode mode = CreateMode(5000000, -2000000);

            Assert.AreEqual(OperatingModeClassification.LimitingActive, mode.Classification);
            Assert.IsNotNull(mode.ActiveControl);
            Assert.IsNull(mode.ReactiveControl);
            Assert.AreEqual("ActivePower", mode.ActiveFeedback!.Quantity);
            Assert.AreEqual(FeedbackCorrection.RampLimitedIntegral, mode.ActiveFeedback!.Settings.Correction);
            Assert.AreEqual(5000000, mode.MaximumActivePower.Watts);
            Assert.AreEqual(-2000000, mode.MinimumActivePower.Watts);
            Assert.IsFalse(mode.IsLimiting);
        }


        /// <summary>
        /// Confirms the nameplate is used for the limits that are not configured.
        /// </summary>
        /// <param name="maximumActivePowerWatts">The configured maximum limit in Watts or null.</param>
        /// <param name="minimumActivePowerWatts">The configured minimum limit in Watts or null.</param>
        /// <param name="expectedMaximum">The expected maximum limit in Watts.</param>
        /// <param name="expectedMinimum">The expected minimum limit in Watts.</param>
        [TestMethod]
        [DataRow(null, null, OperatingModeTestData.NameplateActivePowerWatt, -OperatingModeTestData.NameplateActivePowerWatt)]
        [DataRow(5000000d, null, 5000000d, -OperatingModeTestData.NameplateActivePowerWatt)]
        [DataRow(null, -2000000d, OperatingModeTestData.NameplateActivePowerWatt, -2000000d)]
        public void ActivePowerLimitModeWithoutConfiguredLimitUsesNameplateTest(double? maximumActivePowerWatts, double? minimumActivePowerWatts, double expectedMaximum,
            double expectedMinimum)
        {
            ActivePowerLimitMode mode = CreateMode(maximumActivePowerWatts, minimumActivePowerWatts);

            ActivePower maximum = mode.MaximumActivePower;
            ActivePower minimum = mode.MinimumActivePower;

            Assert.AreEqual(expectedMaximum, maximum.Watts);
            Assert.AreEqual(expectedMinimum, minimum.Watts);
        }


        /// <summary>
        /// Confirms the final active power is limited to the band between the minimum and maximum limit and values inside the band are not changed.
        /// </summary>
        /// <param name="baseActivePower">The base active power in Watts.</param>
        /// <param name="additiveDelta">The additive delta in Watts.</param>
        /// <param name="expectedLimited">The expected limited active power in Watts.</param>
        /// <param name="expectedIsLimitingMaximum">Whether the maximum limit is expected to be binding.</param>
        /// <param name="expectedIsLimitingMinimum">Whether the minimum limit is expected to be binding.</param>
        [TestMethod]
        [DataRow(4000000, 0, 4000000, false, false)]
        [DataRow(5000000, 0, 5000000, false, false)]
        [DataRow(6000000, 0, 5000000, true, false)]
        [DataRow(4000000, 2000000, 5000000, true, false)]
        [DataRow(-1000000, 0, -1000000, false, false)]
        [DataRow(-2000000, 0, -2000000, false, false)]
        [DataRow(-3000000, 0, -2000000, false, true)]
        [DataRow(-1000000, -2000000, -2000000, false, true)]
        [DataRow(-8000000, 0, -2000000, false, true)]
        public void ActivePowerLimitModeCalculateLimitsFinalActivePowerTest(double baseActivePower, double additiveDelta, double expectedLimited, bool expectedIsLimitingMaximum,
            bool expectedIsLimitingMinimum)
        {
            ActivePowerLimitMode mode = CreateActivatedMode(5000000, -2000000);
            OperatingModeSignals signals = CreateSignals(baseActivePower, additiveDelta);

            mode.Calculate(signals);

            Assert.AreEqual(expectedLimited, signals.FinalActivePower.Watts);
            Assert.AreEqual(expectedIsLimitingMaximum, mode.IsLimitingMaximum);
            Assert.AreEqual(expectedIsLimitingMinimum, mode.IsLimitingMinimum);
            Assert.AreEqual(expectedIsLimitingMaximum || expectedIsLimitingMinimum, mode.IsLimiting);
            _feedbackRamp!.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Confirms the final active power is the limit if the minimum and maximum limit are equal.
        /// </summary>
        /// <param name="baseActivePower">The base active power in Watts.</param>
        [TestMethod]
        [DataRow(-6000000)]
        [DataRow(3000000)]
        [DataRow(9000000)]
        public void ActivePowerLimitModeCalculateEqualLimitsTest(double baseActivePower)
        {
            ActivePowerLimitMode mode = CreateActivatedMode(3000000, 3000000);
            OperatingModeSignals signals = CreateSignals(baseActivePower, 0);

            mode.Calculate(signals);

            Assert.AreEqual(3000000, signals.FinalActivePower.Watts);
        }


        /// <summary>
        /// Confirms a binding maximum limit is corrected with the measured active power at the POI.
        /// </summary>
        [TestMethod]
        public void ActivePowerLimitModeCalculateCorrectsBindingMaximumTest()
        {
            _map!.ActivePowerAtPoi = () => new ActivePower(4800000);
            _feedbackRamp!.Setup(controller => controller.Calculate()).Returns(100000);
            ActivePowerLimitMode mode = CreateActivatedMode(5000000, -2000000);
            OperatingModeSignals signals = CreateSignals(6000000, 0);

            mode.Calculate(signals);

            Assert.IsTrue(mode.IsLimitingMaximum);
            Assert.AreEqual(5100000, signals.FinalActivePower.Watts);
            _feedbackRamp!.Verify(controller => controller.Start(0, 200000), Times.Once);
        }


        /// <summary>
        /// Confirms a binding minimum limit is corrected with the measured active power at the POI.
        /// </summary>
        [TestMethod]
        public void ActivePowerLimitModeCalculateCorrectsBindingMinimumTest()
        {
            _map!.ActivePowerAtPoi = () => new ActivePower(-2200000);
            _feedbackRamp!.Setup(controller => controller.Calculate()).Returns(100000);
            ActivePowerLimitMode mode = CreateActivatedMode(5000000, -2000000);
            OperatingModeSignals signals = CreateSignals(-3000000, 0);

            mode.Calculate(signals);

            Assert.IsTrue(mode.IsLimitingMinimum);
            Assert.AreEqual(-1900000, signals.FinalActivePower.Watts);
            _feedbackRamp!.Verify(controller => controller.Start(0, 200000), Times.Once);
        }


        /// <summary>
        /// Confirms the correction of a configured maximum limit may compensate losses beyond the configured limit up to the nameplate.
        /// </summary>
        [TestMethod]
        public void ActivePowerLimitModeCalculateCorrectionLimitedByNameplateTest()
        {
            _map!.ActivePowerAtPoi = () => new ActivePower(8500000);
            ActivePowerLimitMode mode = CreateActivatedMode(9500000, null);
            OperatingModeSignals signals = CreateSignals(OperatingModeTestData.NameplateActivePowerWatt, 0);

            mode.Calculate(signals);

            Assert.IsTrue(mode.IsLimitingMaximum);
            _feedbackRamp!.Verify(controller => controller.Start(0, OperatingModeTestData.NameplateActivePowerWatt - 9500000), Times.Once);
        }


        /// <summary>
        /// Confirms the correction is reset once the limit is no longer binding.
        /// </summary>
        [TestMethod]
        public void ActivePowerLimitModeCalculateResetsCorrectionWhenNotBindingTest()
        {
            _map!.ActivePowerAtPoi = () => new ActivePower(4800000);
            _feedbackRamp!.Setup(controller => controller.Calculate()).Returns(100000);
            ActivePowerLimitMode mode = CreateActivatedMode(5000000, -2000000);
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
        /// Confirms the correction of the maximum limit is reset and not applied to the minimum limit when the binding limit switches.
        /// </summary>
        [TestMethod]
        public void ActivePowerLimitModeCalculateResetsCorrectionWhenBindingLimitSwitchesTest()
        {
            ActivePower activePowerAtPoi = new ActivePower(4800000);
            _map!.ActivePowerAtPoi = () => activePowerAtPoi;
            _feedbackRamp!.Setup(controller => controller.Calculate()).Returns(100000);
            ActivePowerLimitMode mode = CreateActivatedMode(5000000, -2000000);
            mode.Calculate(CreateSignals(6000000, 0));
            activePowerAtPoi = new ActivePower(-2200000);
            OperatingModeSignals signals = CreateSignals(-3000000, 0);

            mode.Calculate(signals);

            Assert.IsFalse(mode.IsLimitingMaximum);
            Assert.IsTrue(mode.IsLimitingMinimum);
            Assert.AreEqual(-1900000, signals.FinalActivePower.Watts);
            // Once by the reset at the activation and once when the binding limit switched
            _feedbackRamp!.Verify(controller => controller.Stop(), Times.Exactly(2));
            _feedbackRamp!.Verify(controller => controller.Start(0, 200000), Times.Exactly(2));
        }


        /// <summary>
        /// Confirms null signals are rejected.
        /// </summary>
        [TestMethod]
        public void ActivePowerLimitModeCalculateNullSignalsThrowsTest()
        {
            ActivePowerLimitMode mode = CreateActivatedMode(5000000, -2000000);

            Assert.ThrowsExactly<ArgumentNullException>(() => mode.Calculate(null!));
        }


        /// <summary>
        /// Creates the mode.
        /// </summary>
        /// <param name="maximumActivePowerWatts">The configured maximum limit in Watts or null for the nameplate.</param>
        /// <param name="minimumActivePowerWatts">The configured minimum limit in Watts or null for the nameplate.</param>
        /// <returns>The active power limit mode.</returns>
        private ActivePowerLimitMode CreateMode(double? maximumActivePowerWatts, double? minimumActivePowerWatts)
        {
            ActivePowerLimitModeConfig config = new ActivePowerLimitModeConfig
            {
                ChangedBy = "Test",
                Name = "ActivePowerLimitMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = OperatingModeTestData.Deadband,
                DeadbandAvailable = OperatingModeTestData.Deadband,
                DeadbandError = 5000,
                MaximumActivePowerLimitWatt = maximumActivePowerWatts,
                MinimumActivePowerLimitWatt = minimumActivePowerWatts,
                RampConfig = OperatingModeTestData.CreateRampConfig()
            };

            return new ActivePowerLimitMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object, OperatingModeTestData.CreateSystemConfig(),
                config, _map!, _rampController!.Object);
        }


        /// <summary>
        /// Creates the mode activated as by the system operating mode control.
        /// </summary>
        /// <param name="maximumActivePowerWatts">The configured maximum limit in Watts or null for the nameplate.</param>
        /// <param name="minimumActivePowerWatts">The configured minimum limit in Watts or null for the nameplate.</param>
        /// <returns>The activated active power limit mode.</returns>
        private ActivePowerLimitMode CreateActivatedMode(double? maximumActivePowerWatts, double? minimumActivePowerWatts)
        {
            ActivePowerLimitMode mode = CreateMode(maximumActivePowerWatts, minimumActivePowerWatts);
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

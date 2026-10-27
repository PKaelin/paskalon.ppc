// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Domain.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Controls;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.UnitTest.ClosedModes.VoltageReactives
{
    /// <summary>
    /// Tests <see cref="ReactivePowerLimitMode"/>. The ramp-limited integral correction is tested in the feedback control tests.
    /// </summary>
    [TestClass]
    public class ReactivePowerLimitModeTest
    {
        /// <summary>
        /// The operating mode map.
        /// </summary>
        private ReactivePowerLimitModeMap? _map;


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
            _map = new ReactivePowerLimitModeMap
            {
                AvailableActivePower = () => new ActivePower(8000000),
                AvailableReactivePower = () => new ReactivePower(4000000),
                ReactivePowerAtPoi = () => null
            };
            _feedbackRamp = new Mock<IRampController>();
            _rampController = new Mock<IRampController>();
            _rampController.Setup(controller => controller.ShallowCopy()).Returns(_feedbackRamp.Object);
        }


        /// <summary>
        /// Confirms the mode is a limiting reactive mode with a ramp-limited integral feedback on the reactive power at the POI.
        /// </summary>
        [TestMethod]
        public void ReactivePowerLimitModeConstructorTest()
        {
            ReactivePowerLimitMode mode = CreateMode(2000000, -1500000);

            Assert.AreEqual(OperatingModeClassification.LimitingReactive, mode.Classification);
            Assert.IsNull(mode.ActiveControl);
            Assert.IsNotNull(mode.ReactiveControl);
            Assert.AreEqual("ReactivePower", mode.ReactiveFeedback!.Quantity);
            Assert.AreEqual(FeedbackCorrection.RampLimitedIntegral, mode.ReactiveFeedback!.Settings.Correction);
            Assert.AreEqual(2000000, mode.MaximumReactivePower.VoltAmperesReactive);
            Assert.AreEqual(-1500000, mode.MinimumReactivePower.VoltAmperesReactive);
            Assert.IsFalse(mode.IsLimiting);
        }


        /// <summary>
        /// Confirms the nameplate is used for the limits that are not configured.
        /// </summary>
        /// <param name="maximumReactivePowerVars">The configured maximum limit in Vars or null.</param>
        /// <param name="minimumReactivePowerVars">The configured minimum limit in Vars or null.</param>
        /// <param name="expectedMaximum">The expected maximum limit in Vars.</param>
        /// <param name="expectedMinimum">The expected minimum limit in Vars.</param>
        [TestMethod]
        [DataRow(null, null, OperatingModeTestData.NameplateReactivePowerVars, -OperatingModeTestData.NameplateReactivePowerVars)]
        [DataRow(2000000d, null, 2000000d, -OperatingModeTestData.NameplateReactivePowerVars)]
        [DataRow(null, -1500000d, OperatingModeTestData.NameplateReactivePowerVars, -1500000d)]
        public void ReactivePowerLimitModeWithoutConfiguredLimitUsesNameplateTest(double? maximumReactivePowerVars, double? minimumReactivePowerVars, double expectedMaximum,
            double expectedMinimum)
        {
            ReactivePowerLimitMode mode = CreateMode(maximumReactivePowerVars, minimumReactivePowerVars);

            ReactivePower maximum = mode.MaximumReactivePower;
            ReactivePower minimum = mode.MinimumReactivePower;

            Assert.AreEqual(expectedMaximum, maximum.VoltAmperesReactive);
            Assert.AreEqual(expectedMinimum, minimum.VoltAmperesReactive);
        }


        /// <summary>
        /// Confirms the final reactive power is limited to the band between the minimum and maximum limit and values inside the band are not changed.
        /// </summary>
        /// <param name="baseReactivePower">The base reactive power in Vars.</param>
        /// <param name="additiveDelta">The additive delta in Vars.</param>
        /// <param name="expectedLimited">The expected limited reactive power in Vars.</param>
        /// <param name="expectedIsLimitingMaximum">Whether the maximum limit is expected to be binding.</param>
        /// <param name="expectedIsLimitingMinimum">Whether the minimum limit is expected to be binding.</param>
        [TestMethod]
        [DataRow(1000000, 0, 1000000, false, false)]
        [DataRow(2000000, 0, 2000000, false, false)]
        [DataRow(3000000, 0, 2000000, true, false)]
        [DataRow(1000000, 1500000, 2000000, true, false)]
        [DataRow(-1000000, 0, -1000000, false, false)]
        [DataRow(-1500000, 0, -1500000, false, false)]
        [DataRow(-2500000, 0, -1500000, false, true)]
        [DataRow(-1000000, -1000000, -1500000, false, true)]
        public void ReactivePowerLimitModeCalculateLimitsFinalReactivePowerTest(double baseReactivePower, double additiveDelta, double expectedLimited,
            bool expectedIsLimitingMaximum, bool expectedIsLimitingMinimum)
        {
            ReactivePowerLimitMode mode = CreateActivatedMode(2000000, -1500000);
            OperatingModeSignals signals = CreateSignals(baseReactivePower, additiveDelta);

            mode.Calculate(signals);

            Assert.AreEqual(expectedLimited, signals.FinalReactivePower.VoltAmperesReactive);
            Assert.AreEqual(expectedIsLimitingMaximum, mode.IsLimitingMaximum);
            Assert.AreEqual(expectedIsLimitingMinimum, mode.IsLimitingMinimum);
            Assert.AreEqual(expectedIsLimitingMaximum || expectedIsLimitingMinimum, mode.IsLimiting);
            _feedbackRamp!.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Confirms the final reactive power is the limit if the minimum and maximum limit are equal.
        /// </summary>
        /// <param name="baseReactivePower">The base reactive power in Vars.</param>
        [TestMethod]
        [DataRow(-4000000)]
        [DataRow(0)]
        [DataRow(4000000)]
        public void ReactivePowerLimitModeCalculateEqualLimitsTest(double baseReactivePower)
        {
            ReactivePowerLimitMode mode = CreateActivatedMode(0, 0);
            OperatingModeSignals signals = CreateSignals(baseReactivePower, 0);

            mode.Calculate(signals);

            Assert.AreEqual(0, signals.FinalReactivePower.VoltAmperesReactive);
        }


        /// <summary>
        /// Confirms a binding maximum limit is corrected with the measured reactive power at the POI.
        /// </summary>
        [TestMethod]
        public void ReactivePowerLimitModeCalculateCorrectsBindingMaximumTest()
        {
            _map!.ReactivePowerAtPoi = () => new ReactivePower(1800000);
            _feedbackRamp!.Setup(controller => controller.Calculate()).Returns(100000);
            ReactivePowerLimitMode mode = CreateActivatedMode(2000000, -1500000);
            OperatingModeSignals signals = CreateSignals(3000000, 0);

            mode.Calculate(signals);

            Assert.IsTrue(mode.IsLimitingMaximum);
            Assert.AreEqual(2100000, signals.FinalReactivePower.VoltAmperesReactive);
            _feedbackRamp!.Verify(controller => controller.Start(0, 200000), Times.Once);
        }


        /// <summary>
        /// Confirms a binding minimum limit is corrected with the measured reactive power at the POI.
        /// </summary>
        [TestMethod]
        public void ReactivePowerLimitModeCalculateCorrectsBindingMinimumTest()
        {
            _map!.ReactivePowerAtPoi = () => new ReactivePower(-1700000);
            _feedbackRamp!.Setup(controller => controller.Calculate()).Returns(100000);
            ReactivePowerLimitMode mode = CreateActivatedMode(2000000, -1500000);
            OperatingModeSignals signals = CreateSignals(-2500000, 0);

            mode.Calculate(signals);

            Assert.IsTrue(mode.IsLimitingMinimum);
            Assert.AreEqual(-1400000, signals.FinalReactivePower.VoltAmperesReactive);
            _feedbackRamp!.Verify(controller => controller.Start(0, 200000), Times.Once);
        }


        /// <summary>
        /// Confirms the correction of a configured maximum limit may compensate losses beyond the configured limit up to the nameplate.
        /// </summary>
        [TestMethod]
        public void ReactivePowerLimitModeCalculateCorrectionLimitedByNameplateTest()
        {
            _map!.ReactivePowerAtPoi = () => new ReactivePower(3500000);
            ReactivePowerLimitMode mode = CreateActivatedMode(4500000, null);
            OperatingModeSignals signals = CreateSignals(OperatingModeTestData.NameplateReactivePowerVars, 0);

            mode.Calculate(signals);

            Assert.IsTrue(mode.IsLimitingMaximum);
            _feedbackRamp!.Verify(controller => controller.Start(0, OperatingModeTestData.NameplateReactivePowerVars - 4500000), Times.Once);
        }


        /// <summary>
        /// Confirms the correction is reset once the limit is no longer binding.
        /// </summary>
        [TestMethod]
        public void ReactivePowerLimitModeCalculateResetsCorrectionWhenNotBindingTest()
        {
            _map!.ReactivePowerAtPoi = () => new ReactivePower(1800000);
            _feedbackRamp!.Setup(controller => controller.Calculate()).Returns(100000);
            ReactivePowerLimitMode mode = CreateActivatedMode(2000000, -1500000);
            mode.Calculate(CreateSignals(3000000, 0));
            OperatingModeSignals signals = CreateSignals(1000000, 0);

            mode.Calculate(signals);

            Assert.IsFalse(mode.IsLimiting);
            Assert.AreEqual(1000000, signals.FinalReactivePower.VoltAmperesReactive);
            Assert.AreEqual(0, mode.ReactiveFeedback!.ErrorAdjustment.VoltAmperesReactive);
            // Once by the reset at the activation and once when the limit is no longer binding
            _feedbackRamp!.Verify(controller => controller.Stop(), Times.Exactly(2));
        }


        /// <summary>
        /// Confirms the correction of the maximum limit is reset and not applied to the minimum limit when the binding limit switches.
        /// </summary>
        [TestMethod]
        public void ReactivePowerLimitModeCalculateResetsCorrectionWhenBindingLimitSwitchesTest()
        {
            ReactivePower reactivePowerAtPoi = new ReactivePower(1800000);
            _map!.ReactivePowerAtPoi = () => reactivePowerAtPoi;
            _feedbackRamp!.Setup(controller => controller.Calculate()).Returns(100000);
            ReactivePowerLimitMode mode = CreateActivatedMode(2000000, -1500000);
            mode.Calculate(CreateSignals(3000000, 0));
            reactivePowerAtPoi = new ReactivePower(-1700000);
            OperatingModeSignals signals = CreateSignals(-2500000, 0);

            mode.Calculate(signals);

            Assert.IsFalse(mode.IsLimitingMaximum);
            Assert.IsTrue(mode.IsLimitingMinimum);
            Assert.AreEqual(-1400000, signals.FinalReactivePower.VoltAmperesReactive);
            // Once by the reset at the activation and once when the binding limit switched
            _feedbackRamp!.Verify(controller => controller.Stop(), Times.Exactly(2));
            _feedbackRamp!.Verify(controller => controller.Start(0, 200000), Times.Exactly(2));
        }


        /// <summary>
        /// Confirms null signals are rejected.
        /// </summary>
        [TestMethod]
        public void ReactivePowerLimitModeCalculateNullSignalsThrowsTest()
        {
            ReactivePowerLimitMode mode = CreateActivatedMode(2000000, -1500000);

            Assert.ThrowsExactly<ArgumentNullException>(() => mode.Calculate(null!));
        }


        /// <summary>
        /// Creates the mode.
        /// </summary>
        /// <param name="maximumReactivePowerVars">The configured maximum limit in Vars or null for the nameplate.</param>
        /// <param name="minimumReactivePowerVars">The configured minimum limit in Vars or null for the nameplate.</param>
        /// <returns>The reactive power limit mode.</returns>
        private ReactivePowerLimitMode CreateMode(double? maximumReactivePowerVars, double? minimumReactivePowerVars)
        {
            ReactivePowerLimitModeConfig config = new ReactivePowerLimitModeConfig
            {
                ChangedBy = "Test",
                Name = "ReactivePowerLimitMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = OperatingModeTestData.Deadband,
                DeadbandAvailable = OperatingModeTestData.Deadband,
                DeadbandError = 5000,
                MaximumReactivePowerLimitVars = maximumReactivePowerVars,
                MinimumReactivePowerLimitVars = minimumReactivePowerVars,
                RampConfig = OperatingModeTestData.CreateRampConfig()
            };

            return new ReactivePowerLimitMode(NullLogger.Instance, TimeProvider.System, new Mock<IMetricsPublisher>().Object, OperatingModeTestData.CreateSystemConfig(),
                config, _map!, _rampController!.Object);
        }


        /// <summary>
        /// Creates the mode activated as by the system operating mode control.
        /// </summary>
        /// <param name="maximumReactivePowerVars">The configured maximum limit in Vars or null for the nameplate.</param>
        /// <param name="minimumReactivePowerVars">The configured minimum limit in Vars or null for the nameplate.</param>
        /// <returns>The activated reactive power limit mode.</returns>
        private ReactivePowerLimitMode CreateActivatedMode(double? maximumReactivePowerVars, double? minimumReactivePowerVars)
        {
            ReactivePowerLimitMode mode = CreateMode(maximumReactivePowerVars, minimumReactivePowerVars);
            mode.Activate();

            return mode;
        }


        /// <summary>
        /// Creates the signals with the base reactive power and the additive delta of the stack before the limiting mode.
        /// </summary>
        /// <param name="baseReactivePower">The base reactive power in Vars.</param>
        /// <param name="additiveDelta">The additive delta in Vars.</param>
        /// <returns>The operating mode signals.</returns>
        private OperatingModeSignals CreateSignals(double baseReactivePower, double additiveDelta)
        {
            return new OperatingModeSignals
            {
                SetpointActivePower = new ActivePower(0),
                SetpointReactivePower = new ReactivePower(baseReactivePower),
                BaseReactivePower = new ReactivePower(baseReactivePower),
                AdditiveReactivePowerDelta = new ReactivePower(additiveDelta)
            };
        }
    }
}

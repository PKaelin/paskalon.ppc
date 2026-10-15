// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.OperatingModes.Domain.Controls;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.UnitTest.Controls
{
    /// <summary>
    /// Tests the closed loop correction of <see cref="FeedbackControl{TPower}"/>.
    /// </summary>
    [TestClass]
    public class FeedbackControlTest
    {
        /// <summary>
        /// Proportional gain of the test feedback control.
        /// </summary>
        private const double ProportionalGain = 0.25;


        /// <summary>
        /// Error deadband of the test feedback control in Watts (5 kW).
        /// </summary>
        private const double DeadbandError = 5000;


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void FeedbackControlConstructorNullLoggerTest()
        {
            FeedbackSettings settings = new FeedbackSettings(ProportionalGain, DeadbandError);
            PowerControlLimits limits = OperatingModeTestData.CreateActiveLimits();
            Func<double?> measuredSource = () => null;

            Assert.ThrowsExactly<ArgumentNullException>(() => new FeedbackControl<ActivePower>(null!, "Mode", "ActivePower", measuredSource, settings, limits, PowerConverters.Active));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void FeedbackControlConstructorNullModeTest()
        {
            FeedbackSettings settings = new FeedbackSettings(ProportionalGain, DeadbandError);
            PowerControlLimits limits = OperatingModeTestData.CreateActiveLimits();
            Func<double?> measuredSource = () => null;

            Assert.ThrowsExactly<ArgumentNullException>(() => new FeedbackControl<ActivePower>(NullLogger.Instance, null!, "ActivePower", measuredSource, settings, limits, PowerConverters.Active));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void FeedbackControlConstructorNullQuantityTest()
        {
            FeedbackSettings settings = new FeedbackSettings(ProportionalGain, DeadbandError);
            PowerControlLimits limits = OperatingModeTestData.CreateActiveLimits();
            Func<double?> measuredSource = () => null;

            Assert.ThrowsExactly<ArgumentNullException>(() => new FeedbackControl<ActivePower>(NullLogger.Instance, "Mode", null!, measuredSource, settings, limits, PowerConverters.Active));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void FeedbackControlConstructorNullMeasuredSourceTest()
        {
            FeedbackSettings settings = new FeedbackSettings(ProportionalGain, DeadbandError);
            PowerControlLimits limits = OperatingModeTestData.CreateActiveLimits();

            Assert.ThrowsExactly<ArgumentNullException>(() => new FeedbackControl<ActivePower>(NullLogger.Instance, "Mode", "ActivePower", null!, settings, limits, PowerConverters.Active));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void FeedbackControlConstructorNullSettingsTest()
        {
            PowerControlLimits limits = OperatingModeTestData.CreateActiveLimits();
            Func<double?> measuredSource = () => null;

            Assert.ThrowsExactly<ArgumentNullException>(() => new FeedbackControl<ActivePower>(NullLogger.Instance, "Mode", "ActivePower", measuredSource, null!, limits, PowerConverters.Active));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void FeedbackControlConstructorNullLimitsTest()
        {
            FeedbackSettings settings = new FeedbackSettings(ProportionalGain, DeadbandError);
            Func<double?> measuredSource = () => null;

            Assert.ThrowsExactly<ArgumentNullException>(() => new FeedbackControl<ActivePower>(NullLogger.Instance, "Mode", "ActivePower", measuredSource, settings, null!, PowerConverters.Active));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void FeedbackControlConstructorNullPowerConvertersTest()
        {
            FeedbackSettings settings = new FeedbackSettings(ProportionalGain, DeadbandError);
            PowerControlLimits limits = OperatingModeTestData.CreateActiveLimits();
            Func<double?> measuredSource = () => null;

            Assert.ThrowsExactly<ArgumentNullException>(() => new FeedbackControl<ActivePower>(NullLogger.Instance, "Mode", "ActivePower", measuredSource, settings, limits, null!));
        }


        /// <summary>
        /// Confirms the target is not corrected without a measurement.
        /// </summary>
        [TestMethod]
        public void FeedbackControlCorrectWithoutMeasurementReturnsTargetTest()
        {
            FeedbackControl<ActivePower> feedback = CreateFeedback(() => null);

            ActivePower corrected = feedback.Correct(new ActivePower(1000000), 1000000);

            Assert.AreEqual(1000000, corrected.Watts);
            Assert.AreEqual(0, feedback.ErrorAdjustment.Watts);
            Assert.AreEqual(0, feedback.Error);
            Assert.IsNull(feedback.Measured);
        }


        /// <summary>
        /// Confirms the proportional correction for errors outside the deadband and no correction inside the deadband.
        /// </summary>
        /// <param name="measured">The measured active power in Watts.</param>
        /// <param name="expectedError">The expected error in Watts.</param>
        /// <param name="expectedAdjustment">The expected error adjustment in Watts.</param>
        [TestMethod]
        [DataRow(900000, 100000, 25000)]
        [DataRow(1100000, -100000, -25000)]
        [DataRow(997000, 3000, 0)]
        public void FeedbackControlCorrectAppliesProportionalGainTest(double measured, double expectedError, double expectedAdjustment)
        {
            FeedbackControl<ActivePower> feedback = CreateFeedback(() => measured);
            // Considers settings ProportionalGain
            ActivePower corrected = feedback.Correct(new ActivePower(1000000), 1000000);

            Assert.AreEqual(1000000, feedback.Reference);
            Assert.AreEqual(measured, feedback.Measured);
            Assert.AreEqual(expectedError, feedback.Error);
            Assert.AreEqual(expectedAdjustment, feedback.ErrorAdjustment.Watts);
            Assert.AreEqual(1000000 + expectedAdjustment, corrected.Watts);
            Assert.AreEqual(corrected.Watts, feedback.Corrected.Watts);
        }


        /// <summary>
        /// Confirms the adjustment is kept when the measurement is stuck.
        /// </summary>
        [TestMethod]
        public void FeedbackControlCorrectStuckMeasurementKeepsAdjustmentTest()
        {
            FeedbackControl<ActivePower> feedback = CreateFeedback(() => 900000);
            feedback.Correct(new ActivePower(1000000), 1000000);

            ActivePower corrected = feedback.Correct(new ActivePower(1200000), 1200000);

            Assert.AreEqual(300000, feedback.Error);
            Assert.AreEqual(25000, feedback.ErrorAdjustment.Watts);
            Assert.AreEqual(1225000, corrected.Watts);
        }


        /// <summary>
        /// Confirms the corrected value is limited by the limits.
        /// </summary>
        [TestMethod]
        public void FeedbackControlCorrectLimitsCorrectedValueTest()
        {
            PowerControlLimits limits = new PowerControlLimits(-1000000, 1000000, OperatingModeTestData.Deadband, OperatingModeTestData.Deadband);
            FeedbackControl<ActivePower> feedback = new FeedbackControl<ActivePower>(NullLogger.Instance, "Mode", "ActivePower", () => 600000,
                new FeedbackSettings(ProportionalGain, DeadbandError), limits, PowerConverters.Active);

            ActivePower corrected = feedback.Correct(new ActivePower(1000000), 1000000);

            Assert.AreEqual(100000, feedback.ErrorAdjustment.Watts);
            Assert.AreEqual(1000000, corrected.Watts);
        }


        /// <summary>
        /// Confirms the feedback works with a controlled quantity other than power.
        /// </summary>
        [TestMethod]
        public void FeedbackControlCorrectOtherQuantityTest()
        {
            FeedbackControl<ReactivePower> feedback = new FeedbackControl<ReactivePower>(NullLogger.Instance, "Mode", "Voltage", () => 19800,
                new FeedbackSettings(500, 50), new PowerControlLimits(-5000000, 5000000, OperatingModeTestData.Deadband, OperatingModeTestData.Deadband), PowerConverters.Reactive);

            ReactivePower corrected = feedback.Correct(new ReactivePower(0), 20000);

            Assert.AreEqual("Voltage", feedback.Quantity);
            Assert.AreEqual(PowerAxis.Reactive, feedback.Axis);
            Assert.AreEqual(200, feedback.Error);
            Assert.AreEqual(100000, corrected.VoltAmperesReactive);
        }


        /// <summary>
        /// Confirms reset clears the correction.
        /// </summary>
        [TestMethod]
        public void FeedbackControlResetClearsAdjustmentTest()
        {
            FeedbackControl<ActivePower> feedback = CreateFeedback(() => 900000);
            feedback.Correct(new ActivePower(1000000), 1000000);

            feedback.Reset();

            Assert.AreEqual(0, feedback.ErrorAdjustment.Watts);
            Assert.AreEqual(0, feedback.Error);
        }


        /// <summary>
        /// Confirms the ramp-limited integral correction requires a ramp controller.
        /// </summary>
        [TestMethod]
        public void FeedbackControlConstructorIntegralWithoutRampThrowsTest()
        {
            FeedbackSettings settings = new FeedbackSettings(0, DeadbandError) { Correction = FeedbackCorrection.RampLimitedIntegral };

            Assert.ThrowsExactly<ArgumentNullException>(() => new FeedbackControl<ActivePower>(NullLogger.Instance, "Mode", "ActivePower", () => null, settings,
                OperatingModeTestData.CreateActiveLimits(), PowerConverters.Active));
        }


        /// <summary>
        /// Confirms the integral correction ramps to the full error without a gain.
        /// </summary>
        [TestMethod]
        public void FeedbackControlCorrectIntegralRampsToFullErrorTest()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            rampController.Setup(controller => controller.Calculate()).Returns(100000);
            FeedbackControl<ActivePower> feedback = CreateIntegralFeedback(() => 4800000, rampController);

            ActivePower corrected = feedback.Correct(new ActivePower(5000000), 5000000);

            Assert.AreEqual(200000, feedback.Error);
            Assert.AreEqual(100000, feedback.ErrorAdjustment.Watts);
            Assert.AreEqual(5100000, corrected.Watts);
            rampController.Verify(controller => controller.Start(0, 200000), Times.Once);
        }


        /// <summary>
        /// Confirms a new error is not accumulated while the previous correction is not reached.
        /// </summary>
        [TestMethod]
        public void FeedbackControlCorrectIntegralWaitsForCorrectionTest()
        {
            double measured = 4800000;
            Mock<IRampController> rampController = new Mock<IRampController>();
            rampController.Setup(controller => controller.Calculate()).Returns(100000);
            FeedbackControl<ActivePower> feedback = CreateIntegralFeedback(() => measured, rampController);
            feedback.Correct(new ActivePower(5000000), 5000000);
            measured = 4900000;
            rampController.Setup(controller => controller.Calculate()).Returns(150000);

            ActivePower corrected = feedback.Correct(new ActivePower(5000000), 5000000);

            Assert.AreEqual(5150000, corrected.Watts);
            rampController.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
        }


        /// <summary>
        /// Confirms a new error is accumulated onto the reached correction once the measurement changed.
        /// </summary>
        [TestMethod]
        public void FeedbackControlCorrectIntegralAccumulatesAfterCorrectionReachedTest()
        {
            double measured = 4800000;
            Mock<IRampController> rampController = new Mock<IRampController>();
            rampController.Setup(controller => controller.Calculate()).Returns(200000);
            FeedbackControl<ActivePower> feedback = CreateIntegralFeedback(() => measured, rampController);
            feedback.Correct(new ActivePower(5000000), 5000000);
            measured = 4950000;

            feedback.Correct(new ActivePower(5000000), 5000000);

            rampController.Verify(controller => controller.Start(0, 200000), Times.Once);
            rampController.Verify(controller => controller.Start(200000, 250000), Times.Once);
            rampController.Verify(controller => controller.Stop(), Times.Once);
        }


        /// <summary>
        /// Confirms the reached correction is held while the measurement is stuck or the error is inside the deadband.
        /// </summary>
        /// <param name="secondMeasured">The measured active power of the second calculation in Watts.</param>
        [TestMethod]
        [DataRow(4800000.0)]
        [DataRow(4997000.0)]
        public void FeedbackControlCorrectIntegralHoldsCorrectionTest(double secondMeasured)
        {
            double measured = 4800000;
            Mock<IRampController> rampController = new Mock<IRampController>();
            rampController.Setup(controller => controller.Calculate()).Returns(200000);
            FeedbackControl<ActivePower> feedback = CreateIntegralFeedback(() => measured, rampController);
            feedback.Correct(new ActivePower(5000000), 5000000);
            measured = secondMeasured;

            ActivePower corrected = feedback.Correct(new ActivePower(5000000), 5000000);

            Assert.AreEqual(200000, feedback.ErrorAdjustment.Watts);
            Assert.AreEqual(5200000, corrected.Watts);
            rampController.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
        }


        /// <summary>
        /// Confirms the integral correction target is limited so that the corrected value stays within the limits.
        /// </summary>
        [TestMethod]
        public void FeedbackControlCorrectIntegralLimitsCorrectionTargetTest()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            PowerControlLimits limits = new PowerControlLimits(-10000000, 5100000, OperatingModeTestData.Deadband, OperatingModeTestData.Deadband);
            FeedbackSettings settings = new FeedbackSettings(0, DeadbandError) { Correction = FeedbackCorrection.RampLimitedIntegral };
            FeedbackControl<ActivePower> feedback = new FeedbackControl<ActivePower>(NullLogger.Instance, "Mode", "ActivePower", () => 4500000, settings, limits,
                PowerConverters.Active, rampController.Object);

            feedback.Correct(new ActivePower(5000000), 5000000);

            rampController.Verify(controller => controller.Start(0, 100000), Times.Once);
        }


        /// <summary>
        /// Confirms reset stops the integral correction.
        /// </summary>
        [TestMethod]
        public void FeedbackControlResetStopsIntegralCorrectionTest()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            rampController.Setup(controller => controller.Calculate()).Returns(100000);
            FeedbackControl<ActivePower> feedback = CreateIntegralFeedback(() => 4800000, rampController);
            feedback.Correct(new ActivePower(5000000), 5000000);

            feedback.Reset();

            Assert.AreEqual(0, feedback.ErrorAdjustment.Watts);
            rampController.Verify(controller => controller.Stop(), Times.Once);
        }


        /// <summary>
        /// Creates an active power feedback control with ramp-limited integral correction.
        /// </summary>
        /// <param name="measuredSource">The measured source.</param>
        /// <param name="rampController">The mocked ramp controller.</param>
        /// <returns>The feedback control.</returns>
        private FeedbackControl<ActivePower> CreateIntegralFeedback(Func<double?> measuredSource, Mock<IRampController> rampController)
        {
            FeedbackSettings settings = new FeedbackSettings(0, DeadbandError) { Correction = FeedbackCorrection.RampLimitedIntegral };

            return new FeedbackControl<ActivePower>(NullLogger.Instance, "Mode", "ActivePower", measuredSource, settings, OperatingModeTestData.CreateActiveLimits(),
                PowerConverters.Active, rampController.Object);
        }


        /// <summary>
        /// Creates an active power feedback control.
        /// </summary>
        /// <param name="measuredSource">The measured source.</param>
        /// <returns>The feedback control.</returns>
        private FeedbackControl<ActivePower> CreateFeedback(Func<double?> measuredSource)
        {
            return new FeedbackControl<ActivePower>(NullLogger.Instance, "Mode", "ActivePower", measuredSource,
                new FeedbackSettings(ProportionalGain, DeadbandError), OperatingModeTestData.CreateActiveLimits(), PowerConverters.Active);
        }
    }
}

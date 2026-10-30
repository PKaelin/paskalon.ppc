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
    /// Tests the activation, ramp and state behavior of <see cref="PowerControl{TPower}"/>.
    /// </summary>
    [TestClass]
    public class PowerControlTest
    {
        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void PowerControlConstructorNullLoggerTest()
        {
            IRampController rampController = new Mock<IRampController>().Object;
            PowerControlLimits limits = OperatingModeTestData.CreateActiveLimits();

            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerControl<ActivePower>(null!, "Mode", rampController, limits, PowerConverters.Active));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void PowerControlConstructorNullModeTest()
        {
            IRampController rampController = new Mock<IRampController>().Object;
            PowerControlLimits limits = OperatingModeTestData.CreateActiveLimits();

            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerControl<ActivePower>(NullLogger.Instance, null!, rampController, limits, PowerConverters.Active));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void PowerControlConstructorNullRampControllerTest()
        {
            PowerControlLimits limits = OperatingModeTestData.CreateActiveLimits();

            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerControl<ActivePower>(NullLogger.Instance, "Mode", null!, limits, PowerConverters.Active));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void PowerControlConstructorNullLimitsTest()
        {
            IRampController rampController = new Mock<IRampController>().Object;

            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerControl<ActivePower>(NullLogger.Instance, "Mode", rampController, null!, PowerConverters.Active));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void PowerControlConstructorNullPowerConvertersTest()
        {
            IRampController rampController = new Mock<IRampController>().Object;
            PowerControlLimits limits = OperatingModeTestData.CreateActiveLimits();

            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerControl<ActivePower>(NullLogger.Instance, "Mode", rampController, limits, null!));
        }


        /// <summary>
        /// Confirms a deactivated power control keeps a zero target and does not start the ramp.
        /// </summary>
        [TestMethod]
        public void PowerControlCalculateDeactivatedReturnsZeroTest()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            PowerControl<ActivePower> control = CreateControl(rampController);

            // OperatingModeState = Disabled
            ActivePower target = control.Calculate(new ActivePower(1000000), new ActivePower(8000000));

            Assert.AreEqual(OperatingModeState.Disabled, control.State);
            Assert.AreEqual(0, target.Watts);
            rampController.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Confirms the power control waits in enabling until the available power is known.
        /// </summary>
        /// <param name="start">How the target is established.</param>
        [TestMethod]
        [DataRow(PowerControlStart.AtTargetSetpoint)]
        [DataRow(PowerControlStart.RampFromCurrentTarget)]
        public void PowerControlCalculateEnablingWithoutAvailableWaitsTest(PowerControlStart start)
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            PowerControl<ActivePower> control = CreateControl(rampController);
            control.Activate(start);

            ActivePower target = control.Calculate(new ActivePower(1000000), null);

            Assert.AreEqual(OperatingModeState.Enabling, control.State);
            Assert.AreEqual(0, target.Watts);
            rampController.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Confirms a stack mode establishes the target setpoint immediately without a ramp.
        /// </summary>
        /// <param name="setpoint">The setpoint in Watts.</param>
        /// <param name="available">The available power in Watts.</param>
        /// <param name="expectedTargetSetpoint">The expected target setpoint in Watts.</param>
        [TestMethod]
        [DataRow(1000000, 8000000, 1000000)]
        [DataRow(9000000, 8000000, 8000000)]
        [DataRow(-9000000, -8000000, -8000000)]
        [DataRow(-9000000, 8000000, -8000000)]
        public void PowerControlCalculateAtTargetSetpointEstablishesTargetTest(double setpoint, double available, double expectedTargetSetpoint)
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            PowerControl<ActivePower> control = CreateControl(rampController);
            // The target is set to the target setpoint immediately
            control.Activate(PowerControlStart.AtTargetSetpoint);

            ActivePower target = control.Calculate(new ActivePower(setpoint), new ActivePower(available));

            Assert.AreEqual(OperatingModeState.Enabled, control.State);
            Assert.AreEqual(expectedTargetSetpoint, target.Watts);
            Assert.AreEqual(expectedTargetSetpoint, control.TargetSetpoint.Watts);
            Assert.AreEqual(setpoint, control.Setpoint.Watts);
            Assert.AreEqual(available, control.Available!.Value.Watts);
            rampController.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Confirms an override mode ramps from the current target to the target setpoint.
        /// </summary>
        [TestMethod]
        public void PowerControlCalculateRampFromCurrentTargetStartsRampTest()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            rampController.Setup(controller => controller.Calculate()).Returns(400000);
            PowerControl<ActivePower> control = CreateControl(rampController);
            // Ramp from the current target to the target setpoint first ramp calculation returns 400000
            control.Activate(PowerControlStart.RampFromCurrentTarget);

            ActivePower target = control.Calculate(new ActivePower(1000000), new ActivePower(8000000));

            Assert.AreEqual(OperatingModeState.RampingToEnabled, control.State);
            Assert.AreEqual(400000, target.Watts);
            rampController.Verify(controller => controller.Start(0, 1000000), Times.Once);
        }


        /// <summary>
        /// Confirms the target setpoint is limited by the configured limits.
        /// </summary>
        /// <param name="setpoint">The setpoint in Watts.</param>
        /// <param name="expectedTargetSetpoint">The expected target setpoint in Watts.</param>
        [TestMethod]
        [DataRow(3000000, 2000000)]
        [DataRow(-3000000, -1000000)]
        public void PowerControlCalculateTargetSetpointLimitedTest(double setpoint, double expectedTargetSetpoint)
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            PowerControlLimits limits = new PowerControlLimits(-1000000, 2000000, OperatingModeTestData.Deadband, OperatingModeTestData.Deadband);
            PowerControl<ActivePower> control = new PowerControl<ActivePower>(NullLogger.Instance, "Mode", rampController.Object, limits, PowerConverters.Active);
            control.Activate(PowerControlStart.AtTargetSetpoint);

            ActivePower target = control.Calculate(new ActivePower(setpoint), new ActivePower(8000000 * Math.Sign(setpoint)));

            Assert.AreEqual(expectedTargetSetpoint, target.Watts);
        }


        /// <summary>
        /// Confirms reaching the target setpoint within the deadband sets the exact target and the enabled state.
        /// </summary>
        [TestMethod]
        public void PowerControlCalculateRampReachedSetsEnabledTest()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            rampController.Setup(controller => controller.Calculate()).Returns(995000);
            PowerControl<ActivePower> control = CreateControl(rampController);
            control.Activate(PowerControlStart.RampFromCurrentTarget);

            ActivePower target = control.Calculate(new ActivePower(1000000), new ActivePower(8000000));

            Assert.AreEqual(OperatingModeState.Enabled, control.State);
            Assert.AreEqual(1000000, target.Watts);
            rampController.Verify(controller => controller.Stop(), Times.Once);
        }


        /// <summary>
        /// Confirms a stack mode follows a setpoint change outside the deadband immediately in both directions and reports the change.
        /// </summary>
        /// <remarks>
        /// The step is transitioned by the system operating mode control with the ramp of the operating mode (one ramp layer).
        /// </remarks>
        /// <param name="newSetpoint">The new setpoint in Watts.</param>
        [TestMethod]
        [DataRow(1500000)]
        [DataRow(500000)]
        [DataRow(-1000000)]
        public void PowerControlCalculateSetpointChangeAtTargetSetpointFollowsImmediatelyTest(double newSetpoint)
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            PowerControl<ActivePower> control = CreateControl(rampController);
            control.Activate(PowerControlStart.AtTargetSetpoint);
            control.Calculate(new ActivePower(1000000), new ActivePower(8000000));

            ActivePower target = control.Calculate(new ActivePower(newSetpoint), new ActivePower(8000000 * Math.Sign(newSetpoint)));

            Assert.AreEqual(OperatingModeState.Enabled, control.State);
            Assert.AreEqual(newSetpoint, target.Watts);
            Assert.AreEqual(newSetpoint, control.TargetSetpoint.Watts);
            Assert.IsTrue(control.HasTargetSetpointChanged);
            rampController.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Confirms an override mode ramps a setpoint change outside the deadband from the current target in both directions.
        /// </summary>
        /// <param name="newSetpoint">The new setpoint in Watts.</param>
        [TestMethod]
        [DataRow(1500000)]
        [DataRow(500000)]
        [DataRow(-1000000)]
        public void PowerControlCalculateSetpointChangeRampFromCurrentTargetRampsTest(double newSetpoint)
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            rampController.Setup(controller => controller.Calculate()).Returns(1000000);
            PowerControl<ActivePower> control = CreateControl(rampController);
            control.Activate(PowerControlStart.RampFromCurrentTarget);
            control.Calculate(new ActivePower(1000000), new ActivePower(8000000));

            control.Calculate(new ActivePower(newSetpoint), new ActivePower(8000000 * Math.Sign(newSetpoint)));

            Assert.AreEqual(OperatingModeState.RampingToEnabled, control.State);
            Assert.AreEqual(newSetpoint, control.TargetSetpoint.Watts);
            Assert.IsFalse(control.HasTargetSetpointChanged);
            rampController.Verify(controller => controller.Start(1000000, newSetpoint), Times.Once);
        }


        /// <summary>
        /// Confirms the target setpoint change is only reported in the calculation it happened.
        /// </summary>
        [TestMethod]
        public void PowerControlCalculateTargetSetpointChangeReportedOnceTest()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            PowerControl<ActivePower> control = CreateControl(rampController);
            control.Activate(PowerControlStart.AtTargetSetpoint);
            control.Calculate(new ActivePower(1000000), new ActivePower(8000000));
            control.Calculate(new ActivePower(2000000), new ActivePower(8000000));

            control.Calculate(new ActivePower(2000000), new ActivePower(8000000));

            Assert.IsFalse(control.HasTargetSetpointChanged);
            Assert.AreEqual(2000000, control.Target.Watts);
        }


        /// <summary>
        /// Confirms a setpoint change inside the deadband keeps the current target.
        /// </summary>
        [TestMethod]
        public void PowerControlCalculateSetpointChangeInsideDeadbandKeepsTargetTest()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            PowerControl<ActivePower> control = CreateControl(rampController);
            control.Activate(PowerControlStart.AtTargetSetpoint);
            control.Calculate(new ActivePower(1000000), new ActivePower(8000000));

            control.Calculate(new ActivePower(1005000), new ActivePower(8000000));

            Assert.AreEqual(OperatingModeState.Enabled, control.State);
            Assert.AreEqual(1000000, control.Target.Watts);
            rampController.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Confirms growing available power above the setpoint does not restart the ramp.
        /// </summary>
        [TestMethod]
        public void PowerControlCalculateAvailableGrowsAboveSetpointKeepsTargetTest()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            PowerControl<ActivePower> control = CreateControl(rampController);
            control.Activate(PowerControlStart.AtTargetSetpoint);
            control.Calculate(new ActivePower(1000000), new ActivePower(8000000));

            control.Calculate(new ActivePower(1000000), new ActivePower(9000000));

            Assert.AreEqual(OperatingModeState.Enabled, control.State);
            Assert.AreEqual(1000000, control.Target.Watts);
            rampController.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Confirms a stack mode follows available power dropping below the setpoint immediately and reports the change.
        /// </summary>
        [TestMethod]
        public void PowerControlCalculateAvailableDropsBelowSetpointFollowsImmediatelyTest()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            PowerControl<ActivePower> control = CreateControl(rampController);
            control.Activate(PowerControlStart.AtTargetSetpoint);
            control.Calculate(new ActivePower(1000000), new ActivePower(8000000));

            ActivePower target = control.Calculate(new ActivePower(1000000), new ActivePower(600000));

            Assert.AreEqual(OperatingModeState.Enabled, control.State);
            Assert.AreEqual(600000, target.Watts);
            Assert.IsTrue(control.HasTargetSetpointChanged);
            rampController.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Confirms the current target is kept when the available power is temporarily unknown.
        /// </summary>
        [TestMethod]
        public void PowerControlCalculateAvailableUnknownKeepsTargetTest()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            PowerControl<ActivePower> control = CreateControl(rampController);
            control.Activate(PowerControlStart.AtTargetSetpoint);
            control.Calculate(new ActivePower(1000000), new ActivePower(8000000));

            ActivePower target = control.Calculate(new ActivePower(2000000), null);

            Assert.AreEqual(OperatingModeState.Enabled, control.State);
            Assert.AreEqual(1000000, target.Watts);
            rampController.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Confirms deactivating resets the target to zero immediately and stops the ramp.
        /// </summary>
        [TestMethod]
        public void PowerControlDeactivateResetsTargetImmediatelyTest()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            PowerControl<ActivePower> control = CreateControl(rampController);
            control.Activate(PowerControlStart.AtTargetSetpoint);
            control.Calculate(new ActivePower(1000000), new ActivePower(8000000));
            // The system operating mode control transitions the applied target
            control.Deactivate();
            ActivePower target = control.Calculate(new ActivePower(2000000), new ActivePower(8000000));

            Assert.AreEqual(OperatingModeState.Disabled, control.State);
            Assert.AreEqual(0, target.Watts);
            Assert.AreEqual(0, control.Setpoint.Watts);
            Assert.AreEqual(0, control.TargetSetpoint.Watts);
            rampController.Verify(controller => controller.Stop(), Times.Once);
            rampController.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Confirms activating an already enabled power control does not restart it.
        /// </summary>
        [TestMethod]
        public void PowerControlActivateWhileEnabledIsIgnoredTest()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            PowerControl<ActivePower> control = CreateControl(rampController);
            control.Activate(PowerControlStart.AtTargetSetpoint);
            control.Calculate(new ActivePower(1000000), new ActivePower(8000000));

            control.Activate(PowerControlStart.AtTargetSetpoint);

            Assert.AreEqual(OperatingModeState.Enabled, control.State);
            Assert.AreEqual(1000000, control.Target.Watts);
        }


        /// <summary>
        /// Confirms a deactivated power control can be activated again.
        /// </summary>
        [TestMethod]
        public void PowerControlActivateAfterDeactivateEstablishesTargetTest()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            PowerControl<ActivePower> control = CreateControl(rampController);
            control.Activate(PowerControlStart.AtTargetSetpoint);
            control.Calculate(new ActivePower(1000000), new ActivePower(8000000));
            control.Deactivate();

            control.Activate(PowerControlStart.AtTargetSetpoint);
            ActivePower target = control.Calculate(new ActivePower(2000000), new ActivePower(8000000));

            Assert.AreEqual(OperatingModeState.Enabled, control.State);
            Assert.AreEqual(2000000, target.Watts);
        }


        /// <summary>
        /// Confirms the ramp starts from the start target set after the activation, e.g. the applied target for a shutdown.
        /// </summary>
        [TestMethod]
        public void PowerControlSetStartTargetRampsFromStartTargetTest()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            rampController.Setup(controller => controller.Calculate()).Returns(2500000);
            PowerControl<ActivePower> control = CreateControl(rampController);
            control.Activate(PowerControlStart.RampFromCurrentTarget);
            control.SetStartTarget(new ActivePower(5000000));

            ActivePower target = control.Calculate(new ActivePower(0), new ActivePower(0));

            Assert.AreEqual(OperatingModeState.RampingToEnabled, control.State);
            Assert.AreEqual(2500000, target.Watts);
            rampController.Verify(controller => controller.Start(5000000, 0), Times.Once);
        }


        /// <summary>
        /// Confirms the start target is ignored once the target is established.
        /// </summary>
        [TestMethod]
        public void PowerControlSetStartTargetIgnoredWhenEstablishedTest()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            PowerControl<ActivePower> control = CreateControl(rampController);
            control.Activate(PowerControlStart.AtTargetSetpoint);
            control.Calculate(new ActivePower(1000000), new ActivePower(8000000));

            control.SetStartTarget(new ActivePower(5000000));

            Assert.AreEqual(1000000, control.Target.Watts);
        }


        /// <summary>
        /// Confirms a running ramp restarts from the current target after holding, so that the held time does not cause a jump.
        /// </summary>
        [TestMethod]
        public void PowerControlRestartRampRampingRestartsFromCurrentTargetTest()
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            rampController.Setup(controller => controller.Calculate()).Returns(400000);
            PowerControl<ActivePower> control = CreateControl(rampController);
            control.Activate(PowerControlStart.RampFromCurrentTarget);
            control.Calculate(new ActivePower(1000000), new ActivePower(8000000));

            control.RestartRamp();

            Assert.AreEqual(OperatingModeState.RampingToEnabled, control.State);
            rampController.Verify(controller => controller.Start(0, 1000000), Times.Once);
            rampController.Verify(controller => controller.Start(400000, 1000000), Times.Once);
        }


        /// <summary>
        /// Confirms the restart of the ramp does nothing if the power control is not ramping.
        /// </summary>
        /// <param name="start">How the target is established.</param>
        [TestMethod]
        [DataRow(PowerControlStart.AtTargetSetpoint)]
        [DataRow(PowerControlStart.RampFromCurrentTarget)]
        public void PowerControlRestartRampNotRampingDoesNotStartRampTest(PowerControlStart start)
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            PowerControl<ActivePower> control = CreateControl(rampController);
            // AtTargetSetpoint is enabled after the calculation, RampFromCurrentTarget is enabling without available power
            control.Activate(start);
            control.Calculate(new ActivePower(1000000), start == PowerControlStart.AtTargetSetpoint ? new ActivePower(8000000) : null);

            control.RestartRamp();

            Assert.AreNotEqual(OperatingModeState.RampingToEnabled, control.State);
            rampController.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Creates a deactivated active power control.
        /// </summary>
        /// <param name="rampController">The mocked ramp controller.</param>
        /// <returns>The active power control.</returns>
        private PowerControl<ActivePower> CreateControl(Mock<IRampController> rampController)
        {
            return new PowerControl<ActivePower>(NullLogger.Instance, "Mode", rampController.Object, OperatingModeTestData.CreateActiveLimits(), PowerConverters.Active);
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.OperatingModes.Domain.Stacks;
using paskalON.OperatingModes.Domain.Systems;
using paskalON.OperatingModes.Domain.Transitions;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.UnitTest.Systems
{
    /// <summary>
    /// Tests the calculation cycle and the transitions of <see cref="SystemOperatingModeControl"/>.
    /// </summary>
    /// <remarks>
    /// A transition segment uses the shallow copy of the ramp controller of the operating mode causing the change, so the tests
    /// control the segments by the mocked shallow copies. A mocked ramp returning zero finishes its segment in the next cycle.
    /// A segment is not advanced in the cycle it starts, so the applied target equals the origin at the change.
    /// </remarks>
    [TestClass]
    public class SystemOperatingModeControlTest
    {
        /// <summary>
        /// The operating mode stack.
        /// </summary>
        private OperatingModeStack? _stack;


        /// <summary>
        /// The override slot.
        /// </summary>
        private OperatingModeOverrides? _overrides;


        /// <summary>
        /// The measured active power at the POI.
        /// </summary>
        private ActivePower? _activePowerAtPoi;


        /// <summary>
        /// The system operating mode control under test.
        /// </summary>
        private SystemOperatingModeControl? _control;


        /// <summary>
        /// Creates the system operating mode control with an empty stack and override slot.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _stack = new OperatingModeStack();
            _overrides = new OperatingModeOverrides();
            _activePowerAtPoi = null;
            SystemOperatingModeControlMap map = new SystemOperatingModeControlMap { ActivePowerAtPoi = () => _activePowerAtPoi, ReactivePowerAtPoi = () => null };
            _control = new SystemOperatingModeControl(NullLogger.Instance, OperatingModeTestData.CreateSystemConfig(), _stack, _overrides, map, new Mock<IMetricsPublisher>().Object);
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlConstructorNullLoggerTest()
        {
            SystemConfig systemConfig = OperatingModeTestData.CreateSystemConfig();
            SystemOperatingModeControlMap map = new SystemOperatingModeControlMap { ActivePowerAtPoi = () => null, ReactivePowerAtPoi = () => null };
            IMetricsPublisher publisher = new Mock<IMetricsPublisher>().Object;

            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemOperatingModeControl(null!, systemConfig, _stack!, _overrides!, map, publisher));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlConstructorNullSystemConfigTest()
        {
            SystemOperatingModeControlMap map = new SystemOperatingModeControlMap { ActivePowerAtPoi = () => null, ReactivePowerAtPoi = () => null };
            IMetricsPublisher publisher = new Mock<IMetricsPublisher>().Object;

            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemOperatingModeControl(NullLogger.Instance, null!, _stack!, _overrides!, map, publisher));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlConstructorNullStackTest()
        {
            SystemConfig systemConfig = OperatingModeTestData.CreateSystemConfig();
            SystemOperatingModeControlMap map = new SystemOperatingModeControlMap { ActivePowerAtPoi = () => null, ReactivePowerAtPoi = () => null };
            IMetricsPublisher publisher = new Mock<IMetricsPublisher>().Object;

            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemOperatingModeControl(NullLogger.Instance, systemConfig, null!, _overrides!, map, publisher));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlConstructorNullOverridesTest()
        {
            SystemConfig systemConfig = OperatingModeTestData.CreateSystemConfig();
            SystemOperatingModeControlMap map = new SystemOperatingModeControlMap { ActivePowerAtPoi = () => null, ReactivePowerAtPoi = () => null };
            IMetricsPublisher publisher = new Mock<IMetricsPublisher>().Object;

            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemOperatingModeControl(NullLogger.Instance, systemConfig, _stack!, null!, map, publisher));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlConstructorNullMapTest()
        {
            SystemConfig systemConfig = OperatingModeTestData.CreateSystemConfig();
            IMetricsPublisher publisher = new Mock<IMetricsPublisher>().Object;

            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemOperatingModeControl(NullLogger.Instance, systemConfig, _stack!, _overrides!, null!, publisher));
        }


        /// <summary>
        /// Confirms the constructor validates its arguments.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlConstructorNullPublisherTest()
        {
            SystemConfig systemConfig = OperatingModeTestData.CreateSystemConfig();
            SystemOperatingModeControlMap map = new SystemOperatingModeControlMap { ActivePowerAtPoi = () => null, ReactivePowerAtPoi = () => null };
            IMetricsPublisher publisher = new Mock<IMetricsPublisher>().Object;

            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemOperatingModeControl(NullLogger.Instance, systemConfig, _stack!, _overrides!, map, null!));
        }


        /// <summary>
        /// Confirms the supervisory setpoints are stored.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlUpdateSetpointsTest()
        {
            _control!.UpdateSetpoints(new ActivePower(5000000), new ReactivePower(-1000000));

            Assert.AreEqual(5000000, _control!.SetpointActivePower.Watts);
            Assert.AreEqual(-1000000, _control!.SetpointReactivePower.VoltAmperesReactive);
            Assert.AreEqual(SystemState.Unknown, _control!.State);
        }


        /// <summary>
        /// Confirms supervisory setpoints outside the nameplate are rejected.
        /// </summary>
        /// <param name="activePower">The active power setpoint in Watts.</param>
        /// <param name="reactivePower">The reactive power setpoint in Vars.</param>
        [TestMethod]
        [DataRow(10000001, 0)]
        [DataRow(-10000001, 0)]
        [DataRow(0, 5000001)]
        [DataRow(0, -5000001)]
        public void SystemOperatingModeControlUpdateSetpointsOutsideNameplateTest(double activePower, double reactivePower)
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _control!.UpdateSetpoints(new ActivePower(activePower), new ReactivePower(reactivePower)));
            Assert.AreEqual(0, _control!.SetpointActivePower.Watts);
            Assert.AreEqual(0, _control!.SetpointReactivePower.VoltAmperesReactive);
        }


        /// <summary>
        /// Confirms an empty stack results in zero applied targets.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlCalculateEmptyStackTest()
        {
            _control!.UpdateSetpoints(new ActivePower(5000000), new ReactivePower(1000000));

            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(SystemState.Running, _control!.State);
            Assert.AreEqual(0, _control!.SetpointActivePowerActual.Watts);
            Assert.AreEqual(0, _control!.SetpointReactivePowerActual.VoltAmperesReactive);
            Assert.AreEqual(TransitionKind.None, _control!.ActivePowerTransition.Kind);
            Assert.IsEmpty(_control!.OverrideSetpoints);
        }


        /// <summary>
        /// Confirms committed modes are activated by the next calculation cycle and not before.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlCalculateActivatesCommittedModesTest()
        {
            TestBaseActiveMode baseMode = CreateBaseActiveMode("ActivePowerMode", 5000000);
            _stack!.Add(baseMode, 1);
            bool isActivatedBeforeCycle = baseMode.IsActivated;

            _control!.Calculate(CancellationToken.None);
            OperatingModeState stateWhileTransitioning = baseMode.State;
            _control!.Calculate(CancellationToken.None);

            Assert.IsFalse(isActivatedBeforeCycle);
            Assert.IsTrue(baseMode.IsActivated);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, stateWhileTransitioning);
            Assert.AreEqual(OperatingModeState.Enabled, baseMode.State);
        }


        /// <summary>
        /// Confirms the state of a mode is ramping to enabled while its transition segment runs and enabled once it finished.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlStateFollowsTransitionSegmentTest()
        {
            Mock<IRampController> transitionRamp = CreateTransitionRamp(-2000000);
            TestBaseActiveMode baseMode = CreateBaseActiveMode("ActivePowerMode", 5000000, transitionRamp);
            _stack!.Add(baseMode, 1);
            _control!.Calculate(CancellationToken.None);
            _control!.Calculate(CancellationToken.None);
            OperatingModeState stateWhileRamping = baseMode.State;
            transitionRamp.Setup(controller => controller.Calculate()).Returns(0);

            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(OperatingModeState.RampingToEnabled, stateWhileRamping);
            Assert.AreEqual(OperatingModeState.Enabled, baseMode.State);
            Assert.IsFalse(baseMode.ActiveControl!.IsTransitioning);
        }


        /// <summary>
        /// Confirms base, additive and limiting modes are applied and their contributions add up to the objective.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlCalculateAppliesStackTest()
        {
            TestBaseActiveMode baseActive = CreateBaseActiveMode("ActivePowerMode", 5000000);
            TestBaseReactiveMode baseReactive = new TestBaseReactiveMode("ReactivePowerMode")
            {
                CalculateAction = signals => { signals.BaseReactivePower = signals.SetpointReactivePower; return signals; }
            };
            TestAdditiveActiveMode additiveActive = CreateAdditiveActiveMode("FrequencyWattMode", 1000000);
            TestLimitingActiveMode limitingActive = CreateLimitingActiveMode("MaximumActivePowerLimitMode", 5500000);
            _stack!.Add(baseActive, 1);
            _stack!.Add(baseReactive, 2);
            _stack!.Add(additiveActive, 3);
            _stack!.Add(limitingActive, 4);
            _control!.UpdateSetpoints(new ActivePower(5000000), new ReactivePower(1000000));

            CalculateSettled();

            Assert.AreEqual(SystemState.Running, _control!.State);
            // Limited by the limiting mode to 5.5 MW, so the objective is 5.5 MW and the applied target is 5.5 MW.
            Assert.AreEqual(5500000, _control!.ActivePowerObjective.Watts);
            Assert.AreEqual(5500000, _control!.SetpointActivePowerActual.Watts);
            Assert.AreEqual(1000000, _control!.SetpointReactivePowerActual.VoltAmperesReactive);
            Assert.AreEqual(5000000, _control!.BaseActivePower.Watts);
            Assert.AreEqual(1000000, _control!.AdditiveActivePowerDelta.Watts);
            Assert.IsTrue(_control!.IsActivePowerLimited);
            Assert.IsFalse(_control!.IsReactivePowerLimited);
            Assert.AreEqual(5000000, baseActive.ActivePowerContribution.Watts);
            Assert.AreEqual(1000000, additiveActive.ActivePowerContribution.Watts);
            Assert.AreEqual(-500000, limitingActive.ActivePowerContribution.Watts);
            Assert.AreEqual(1000000, baseReactive.ReactivePowerContribution.VoltAmperesReactive);
        }


        /// <summary>
        /// Confirms the initial activation starts from the measured power at the POI if it is valid otherwise from zero.
        /// </summary>
        /// <param name="activePowerAtPoi">The measured active power at the POI in Watts or NaN if not available.</param>
        /// <param name="expectedOrigin">The expected origin in Watts.</param>
        [TestMethod]
        [DataRow(3000000, 3000000)]
        [DataRow(double.NaN, 0)]
        [DataRow(20000000, 0)]
        public void SystemOperatingModeControlInitialActivationStartsFromPoiTest(double activePowerAtPoi, double expectedOrigin)
        {
            _activePowerAtPoi = double.IsNaN(activePowerAtPoi) ? null : new ActivePower(activePowerAtPoi);
            Mock<IRampController> transitionRamp = CreateTransitionRamp(-1000000);
            _stack!.Add(CreateBaseActiveMode("ActivePowerMode", 5000000, transitionRamp), 1);

            _control!.Calculate(CancellationToken.None);
            double appliedAtChange = _control!.SetpointActivePowerActual.Watts;
            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(expectedOrigin, appliedAtChange);
            Assert.AreEqual(TransitionKind.InitialActivation, _control!.ActivePowerTransition.Kind);
            Assert.AreEqual(4000000, _control!.SetpointActivePowerActual.Watts);
            transitionRamp.Verify(controller => controller.Start(expectedOrigin - 5000000, 0), Times.Once);
        }


        /// <summary>
        /// Confirms present additive and limiting modes start with their own segment and ramp at the initial activation.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlInitialActivationSegmentPerModeTest()
        {
            _activePowerAtPoi = new ActivePower(3000000);
            Mock<IRampController> baseRamp = new Mock<IRampController>();
            Mock<IRampController> additiveRamp = new Mock<IRampController>();
            _stack!.Add(CreateBaseActiveMode("ActivePowerMode", 5000000, baseRamp), 1);
            _stack!.Add(CreateAdditiveActiveMode("FrequencyWattMode", 1000000, additiveRamp), 2);

            _control!.Calculate(CancellationToken.None);

            Assert.HasCount(2, _control!.ActivePowerTransition.Segments);
            Assert.AreEqual(3000000, _control!.SetpointActivePowerActual.Watts);
            additiveRamp.Verify(controller => controller.Start(-1000000, 0), Times.Once);
            baseRamp.Verify(controller => controller.Start(-2000000, 0), Times.Once);
        }


        /// <summary>
        /// Confirms a replacement transfers control from the current applied target to the new objective with the ramp of the new mode.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlReplacementTransitionsFromAppliedTargetTest()
        {
            TestBaseActiveMode predecessor = CreateBaseActiveMode("ActivePowerMode", 5000000);
            Mock<IRampController> transitionRamp = CreateTransitionRamp(-1000000);
            TestBaseActiveMode replacement = CreateBaseActiveMode("MaximumPowerPointTrackingMode", 7000000, transitionRamp);
            _stack!.Add(predecessor, 1);
            CalculateSettled();
            _stack!.Replace("ActivePowerMode", replacement);

            _control!.Calculate(CancellationToken.None);
            double appliedAtChange = _control!.SetpointActivePowerActual.Watts;
            _control!.Calculate(CancellationToken.None);

            Assert.IsFalse(predecessor.IsActivated);
            Assert.IsTrue(replacement.IsActivated);
            Assert.AreEqual(5000000, appliedAtChange);
            Assert.HasCount(1, _control!.ActivePowerTransition.Segments);
            Assert.AreEqual(TransitionKind.ModeReplacement, _control!.ActivePowerTransition.Kind);
            Assert.AreEqual("MaximumPowerPointTrackingMode", _control!.ActivePowerTransition.OperatingModeName);
            Assert.AreEqual(7000000, _control!.ActivePowerObjective.Watts);
            Assert.AreEqual(6000000, _control!.SetpointActivePowerActual.Watts);
            transitionRamp.Verify(controller => controller.Start(-2000000, 0), Times.Once);
        }


        /// <summary>
        /// Confirms a base replacement under a binding limit does not start a transition because the applied target does not change.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlReplacementUnderBindingLimitTest()
        {
            Mock<IRampController> transitionRamp = new Mock<IRampController>();
            _stack!.Add(CreateBaseActiveMode("ActivePowerMode", 5000000), 1);
            _stack!.Add(CreateLimitingActiveMode("MaximumActivePowerLimitMode", 4000000), 2);
            CalculateSettled();
            _stack!.Replace("ActivePowerMode", CreateBaseActiveMode("MaximumPowerPointTrackingMode", 7000000, transitionRamp));

            _control!.Calculate(CancellationToken.None);

            Assert.IsEmpty(_control!.ActivePowerTransition.Segments);
            Assert.AreEqual(4000000, _control!.SetpointActivePowerActual.Watts);
            transitionRamp.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Never);
        }


        /// <summary>
        /// Confirms removing the last base mode transitions to zero with the ramp of the removed mode.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlRemovalWithoutReplacementTransitionsToZeroTest()
        {
            Mock<IRampController> transitionRamp = new Mock<IRampController>();
            TestBaseActiveMode baseMode = CreateBaseActiveMode("ActivePowerMode", 5000000, transitionRamp);
            _stack!.Add(baseMode, 1);
            CalculateSettled();
            transitionRamp.Setup(controller => controller.Calculate()).Returns(3000000);
            _stack!.Remove(baseMode);

            CalculateSettled();

            Assert.IsFalse(baseMode.IsActivated);
            Assert.AreEqual(OperatingModeState.RampingToDisabled, baseMode.State);
            Assert.AreEqual(TransitionKind.ModeRemoved, _control!.ActivePowerTransition.Kind);
            Assert.AreEqual(0, _control!.ActivePowerObjective.Watts);
            Assert.AreEqual(3000000, _control!.SetpointActivePowerActual.Watts);
            transitionRamp.Verify(controller => controller.Start(5000000, 0), Times.Once);
        }


        /// <summary>
        /// Confirms a removed mode is ramping to disabled while its contribution ramps to zero and disabled once its segment finished.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlRemovedModeStateFollowsTransitionSegmentTest()
        {
            Mock<IRampController> transitionRamp = new Mock<IRampController>();
            TestBaseActiveMode baseMode = CreateBaseActiveMode("ActivePowerMode", 5000000, transitionRamp);
            _stack!.Add(baseMode, 1);
            CalculateSettled();
            _stack!.Remove(baseMode);
            _control!.Calculate(CancellationToken.None);
            OperatingModeState stateAtRemoval = baseMode.State;

            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(OperatingModeState.RampingToDisabled, stateAtRemoval);
            Assert.AreEqual(OperatingModeState.Disabled, baseMode.State);
            Assert.IsFalse(baseMode.ActiveControl!.IsTransitioning);
            Assert.IsEmpty(_control!.ActivePowerTransition.Segments);
        }


        /// <summary>
        /// Confirms a setpoint change while the segment of the mode is running is merged into that segment from the current applied target.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlSetpointChangeWhileRampingMergesSegmentTest()
        {
            Mock<IRampController> transitionRamp = CreateTransitionRamp(-5000000);
            TestBaseActiveMode baseMode = CreateBaseActiveMode("ActivePowerFixedMode", null, transitionRamp);
            _stack!.Add(baseMode, 1);
            _control!.UpdateSetpoints(new ActivePower(8000000), new ReactivePower(0));
            _control!.Calculate(CancellationToken.None);
            _control!.Calculate(CancellationToken.None);
            double appliedBeforeChange = _control!.SetpointActivePowerActual.Watts;
            _control!.UpdateSetpoints(new ActivePower(6000000), new ReactivePower(0));

            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(3000000, appliedBeforeChange);
            Assert.AreEqual(6000000, _control!.ActivePowerObjective.Watts);
            Assert.AreEqual(3000000, _control!.SetpointActivePowerActual.Watts);
            Assert.HasCount(1, _control!.ActivePowerTransition.Segments);
            Assert.AreEqual(TransitionKind.TargetChanged, _control!.ActivePowerTransition.Kind);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, baseMode.State);
            transitionRamp.Verify(controller => controller.Start(-8000000, 0), Times.Once);
            transitionRamp.Verify(controller => controller.Start(-3000000, 0), Times.Once);
        }


        /// <summary>
        /// Confirms a setpoint change in steady state transitions with the ramp of the mode while the objective follows immediately.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlSetpointChangeSteadyStateTransitionsTest()
        {
            Mock<IRampController> transitionRamp = new Mock<IRampController>();
            _stack!.Add(CreateBaseActiveMode("ActivePowerFixedMode", null, transitionRamp), 1);
            _control!.UpdateSetpoints(new ActivePower(5000000), new ReactivePower(0));
            CalculateSettled();
            _control!.UpdateSetpoints(new ActivePower(2000000), new ReactivePower(0));

            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(2000000, _control!.ActivePowerObjective.Watts);
            Assert.AreEqual(5000000, _control!.SetpointActivePowerActual.Watts);
            Assert.AreEqual(TransitionKind.TargetChanged, _control!.ActivePowerTransition.Kind);
            transitionRamp.Verify(controller => controller.Start(3000000, 0), Times.Once);
        }


        /// <summary>
        /// Confirms two operating modes removed in the same cycle transition with one segment and their own ramp each.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlTwoModesRemovedSameCycleTest()
        {
            Mock<IRampController> firstRamp = new Mock<IRampController>();
            Mock<IRampController> secondRamp = new Mock<IRampController>();
            TestAdditiveActiveMode firstAdditive = CreateAdditiveActiveMode("FrequencyWattMode", 1000000, firstRamp);
            TestAdditiveActiveMode secondAdditive = CreateAdditiveActiveMode("FrequencyDroopMode", 500000, secondRamp);
            _stack!.Add(CreateBaseActiveMode("ActivePowerMode", 5000000), 1);
            _stack!.Add(firstAdditive, 2);
            _stack!.Add(secondAdditive, 3);
            CalculateSettled();
            _stack!.Remove(firstAdditive);
            _stack!.Remove(secondAdditive);

            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(5000000, _control!.ActivePowerObjective.Watts);
            Assert.AreEqual(6500000, _control!.SetpointActivePowerActual.Watts);
            Assert.HasCount(2, _control!.ActivePowerTransition.Segments);
            firstRamp.Verify(controller => controller.Start(1000000, 0), Times.Once);
            secondRamp.Verify(controller => controller.Start(500000, 0), Times.Once);
        }


        /// <summary>
        /// Confirms a change while another segment is running keeps the running segment and its ramp.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlChangeWhileTransitionRunningTest()
        {
            Mock<IRampController> firstRamp = new Mock<IRampController>();
            Mock<IRampController> secondRamp = new Mock<IRampController>();
            _stack!.Add(CreateBaseActiveMode("ActivePowerMode", 5000000), 1);
            CalculateSettled();
            firstRamp.Setup(controller => controller.Calculate()).Returns(-500000);
            _stack!.Add(CreateAdditiveActiveMode("FrequencyWattMode", 1000000, firstRamp), 2);
            _control!.Calculate(CancellationToken.None);
            _stack!.Add(CreateAdditiveActiveMode("FrequencyDroopMode", 2000000, secondRamp), 3);

            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(8000000, _control!.ActivePowerObjective.Watts);
            Assert.AreEqual(5500000, _control!.SetpointActivePowerActual.Watts);
            Assert.HasCount(2, _control!.ActivePowerTransition.Segments);
            firstRamp.Verify(controller => controller.Start(-1000000, 0), Times.Once);
            firstRamp.Verify(controller => controller.Start(It.IsAny<double>(), It.IsAny<double>()), Times.Once);
            secondRamp.Verify(controller => controller.Start(-2000000, 0), Times.Once);
        }


        /// <summary>
        /// Confirms adding and removing an additive mode transitions with the ramp of the additive mode.
        /// </summary>
        /// <param name="isAdded">Whether the additive mode is added or removed.</param>
        /// <param name="expectedOffset">The expected transition offset in Watts.</param>
        /// <param name="expectedKind">The expected transition kind.</param>
        [TestMethod]
        [DataRow(true, -1000000, TransitionKind.ModeAdded)]
        [DataRow(false, 1000000, TransitionKind.ModeRemoved)]
        public void SystemOperatingModeControlAdditiveTransitionTest(bool isAdded, double expectedOffset, TransitionKind expectedKind)
        {
            Mock<IRampController> transitionRamp = new Mock<IRampController>();
            TestAdditiveActiveMode additiveMode = CreateAdditiveActiveMode("FrequencyWattMode", 1000000, transitionRamp);
            _stack!.Add(CreateBaseActiveMode("ActivePowerMode", 5000000), 1);

            if (isAdded is false)
            {
                _stack!.Add(additiveMode, 2);
            }

            CalculateSettled();
            ChangeMode(additiveMode, isAdded, 2);

            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(expectedKind, _control!.ActivePowerTransition.Kind);
            Assert.AreEqual("FrequencyWattMode", _control!.ActivePowerTransition.OperatingModeName);
            transitionRamp.Verify(controller => controller.Start(expectedOffset, 0), Times.Once);
        }


        /// <summary>
        /// Confirms adding and removing a limiting mode transitions with the ramp of the limiting mode.
        /// </summary>
        /// <param name="isAdded">Whether the limiting mode is added or removed.</param>
        /// <param name="expectedOffset">The expected transition offset in Watts.</param>
        /// <param name="expectedObjective">The expected objective in Watts.</param>
        [TestMethod]
        [DataRow(true, 1000000, 4000000)]
        [DataRow(false, -1000000, 5000000)]
        public void SystemOperatingModeControlLimitingTransitionTest(bool isAdded, double expectedOffset, double expectedObjective)
        {
            Mock<IRampController> transitionRamp = new Mock<IRampController>();
            TestLimitingActiveMode limitingMode = CreateLimitingActiveMode("MaximumActivePowerLimitMode", 4000000, transitionRamp);
            _stack!.Add(CreateBaseActiveMode("ActivePowerMode", 5000000), 1);

            if (isAdded is false)
            {
                _stack!.Add(limitingMode, 2);
            }

            CalculateSettled();
            ChangeMode(limitingMode, isAdded, 2);

            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(expectedObjective, _control!.ActivePowerObjective.Watts);
            transitionRamp.Verify(controller => controller.Start(expectedOffset, 0), Times.Once);
        }


        /// <summary>
        /// Confirms a mode waiting for available power starts its transition once it established its target.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlModeEstablishingTargetLaterTransitionsTest()
        {
            ActivePower? available = null;
            Mock<IRampController> transitionRamp = new Mock<IRampController>();
            TestBaseActiveMode baseMode = CreateBaseActiveMode("ActivePowerMode", 5000000, transitionRamp, () => available);
            _stack!.Add(baseMode, 1);
            _control!.Calculate(CancellationToken.None);
            available = new ActivePower(8000000);

            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(OperatingModeState.RampingToEnabled, baseMode.State);
            Assert.AreEqual(TransitionKind.InitialActivation, _control!.ActivePowerTransition.Kind);
            transitionRamp.Verify(controller => controller.Start(-5000000, 0), Times.Once);
        }


        /// <summary>
        /// Confirms the nameplate is enforced immediately.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlNameplateEnforcedImmediatelyTest()
        {
            _stack!.Add(CreateBaseActiveMode("ActivePowerMode", 8000000), 1);
            _stack!.Add(CreateAdditiveActiveMode("FrequencyWattMode", 3000000), 2);

            CalculateSettled();

            // Objective is not constraint by the nameplate
            Assert.AreEqual(11000000, _control!.ActivePowerObjective.Watts);
            // Applied target is constrained by the nameplate
            Assert.AreEqual(OperatingModeTestData.NameplateActivePowerWatt, _control!.SetpointActivePowerActual.Watts);
        }


        /// <summary>
        /// Confirms a plant-wide override forces the applied target immediately and its removal transitions back to the objective.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlPlantWideOverrideTest()
        {
            _stack!.Add(CreateBaseActiveMode("ActivePowerMode", 5000000), 1);
            Mock<IRampController> transitionRamp = new Mock<IRampController>();
            Mock<IRampController> reactiveRamp = new Mock<IRampController>();
            reactiveRamp.Setup(controller => controller.ShallowCopy()).Returns(new Mock<IRampController>().Object);
            Mock<IRampController> overrideRamp = new Mock<IRampController>();
            overrideRamp.Setup(controller => controller.Calculate()).Returns(2000000);
            overrideRamp.SetupSequence(controller => controller.ShallowCopy()).Returns(reactiveRamp.Object).Returns(transitionRamp.Object);
            TestOverrideMode shutdown = new TestOverrideMode("ShutdownMode", null, rampController: overrideRamp.Object);
            shutdown.CalculateAction = signals => { shutdown.ActiveControl!.Calculate(new ActivePower(2000000), new ActivePower(8000000)); return signals; };
            CalculateSettled();
            _overrides!.Add(shutdown);

            _control!.Calculate(CancellationToken.None);
            double forcedApplied = _control!.SetpointActivePowerActual.Watts;
            _overrides!.Remove(shutdown);
            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(2000000, forcedApplied);
            Assert.AreEqual(2000000, _control!.SetpointActivePowerActual.Watts);
            Assert.AreEqual(TransitionKind.ModeRemoved, _control!.ActivePowerTransition.Kind);
            transitionRamp.Verify(controller => controller.Start(-3000000, 0), Times.Once);
        }


        /// <summary>
        /// Confirms override modes are calculated and published as override setpoints without changing the plant signals.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlCalculateOverrideSetpointsTest()
        {
            _stack!.Add(CreateBaseActiveMode("ActivePowerMode", 5000000), 1);
            int overrideCalculations = 0;
            TestOverrideMode maintenance = new TestOverrideMode("MaintenanceUnit1", "Bess1")
            {
                CalculateAction = signals => { overrideCalculations++; return signals; }
            };
            _overrides!.Add(maintenance);

            CalculateSettled();

            Assert.AreEqual(2, overrideCalculations);
            Assert.IsTrue(maintenance.IsActivated);
            Assert.AreEqual(5000000, _control!.SetpointActivePowerActual.Watts);
            Assert.HasCount(1, _control!.OverrideSetpoints);
            Assert.AreEqual("MaintenanceUnit1", _control!.OverrideSetpoints[0].OperatingModeName);
            Assert.AreEqual("Bess1", _control!.OverrideSetpoints[0].DerUnitName);
            Assert.AreEqual(maintenance.ActiveControl!.Target, _control!.OverrideSetpoints[0].ActivePower);
            Assert.AreEqual(maintenance.ReactiveControl!.Target, _control!.OverrideSetpoints[0].ReactivePower);
        }


        /// <summary>
        /// Confirms a failing operating mode holds the last applied targets and faults the control.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlCalculateModeThrowsHoldsActualTest()
        {
            TestBaseActiveMode baseActive = CreateBaseActiveMode("ActivePowerMode", 5000000);
            _stack!.Add(baseActive, 1);
            CalculateSettled();
            baseActive.CalculateAction = signals => throw new InvalidOperationException("Measurement failed.");

            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(SystemState.Faulted, _control!.State);
            Assert.AreEqual(5000000, _control!.SetpointActivePowerActual.Watts);
        }


        /// <summary>
        /// Confirms a cancelled calculation throws and does not fault the control.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlCalculateCancelledTest()
        {
            _stack!.Add(CreateBaseActiveMode("ActivePowerMode", 5000000), 1);
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            Assert.ThrowsExactly<OperationCanceledException>(() => _control!.Calculate(cancellation.Token));
            Assert.AreEqual(SystemState.Unknown, _control!.State);
        }


        /// <summary>
        /// Calculates two cycles so that segments with a ramp returning zero are finished.
        /// </summary>
        private void CalculateSettled()
        {
            _control!.Calculate(CancellationToken.None);
            _control!.Calculate(CancellationToken.None);
        }


        /// <summary>
        /// Adds or removes an operating mode.
        /// </summary>
        /// <param name="operatingMode">The operating mode.</param>
        /// <param name="isAdded">Whether the operating mode is added or removed.</param>
        /// <param name="priority">The priority for adding.</param>
        private void ChangeMode(TestOperatingModeBase operatingMode, bool isAdded, int priority)
        {
            IStackableMode stackableMode = (IStackableMode)operatingMode;

            if (isAdded)
            {
                _stack!.Add(stackableMode, priority);

                return;
            }

            _stack!.Remove(stackableMode);
        }


        /// <summary>
        /// Creates a transition ramp returning a constant offset.
        /// </summary>
        /// <param name="offset">The offset returned by the ramp in Watts.</param>
        /// <returns>The mocked transition ramp.</returns>
        private Mock<IRampController> CreateTransitionRamp(double offset)
        {
            Mock<IRampController> transitionRamp = new Mock<IRampController>();
            transitionRamp.Setup(controller => controller.Calculate()).Returns(offset);

            return transitionRamp;
        }


        /// <summary>
        /// Creates a ramp controller whose shallow copy is the transition ramp.
        /// </summary>
        /// <param name="transitionRamp">The transition ramp or null for a ramp returning zero.</param>
        /// <returns>The ramp controller of the operating mode.</returns>
        private IRampController CreateModeRamp(Mock<IRampController>? transitionRamp)
        {
            Mock<IRampController> rampController = new Mock<IRampController>();
            rampController.Setup(controller => controller.ShallowCopy()).Returns((transitionRamp ?? new Mock<IRampController>()).Object);

            return rampController.Object;
        }


        /// <summary>
        /// Creates a base active mode that writes the target of its power control into the base active power.
        /// </summary>
        /// <param name="name">The name of the operating mode.</param>
        /// <param name="setpoint">The setpoint of the operating mode in Watts or null for the supervisory setpoint.</param>
        /// <param name="transitionRamp">The transition ramp or null for a ramp returning zero.</param>
        /// <param name="available">The available power source or null for 8 MW.</param>
        /// <returns>The base active mode.</returns>
        private TestBaseActiveMode CreateBaseActiveMode(string name, double? setpoint, Mock<IRampController>? transitionRamp = null, Func<ActivePower?>? available = null)
        {
            TestBaseActiveMode operatingMode = new TestBaseActiveMode(name, rampController: CreateModeRamp(transitionRamp));
            operatingMode.CalculateAction = signals =>
            {
                ActivePower modeSetpoint = setpoint.HasValue ? new ActivePower(setpoint.Value) : signals.SetpointActivePower;
                signals.BaseActivePower = operatingMode.ActiveControl!.Calculate(modeSetpoint, available is null ? new ActivePower(8000000) : available.Invoke());

                return signals;
            };

            return operatingMode;
        }


        /// <summary>
        /// Creates an additive active mode adding a constant delta.
        /// </summary>
        /// <param name="name">The name of the operating mode.</param>
        /// <param name="delta">The delta in Watts.</param>
        /// <param name="transitionRamp">The transition ramp or null for a ramp returning zero.</param>
        /// <returns>The additive active mode.</returns>
        private TestAdditiveActiveMode CreateAdditiveActiveMode(string name, double delta, Mock<IRampController>? transitionRamp = null)
        {
            return new TestAdditiveActiveMode(name, CreateModeRamp(transitionRamp))
            {
                CalculateAction = signals => { signals.AdditiveActivePowerDelta += new ActivePower(delta); return signals; }
            };
        }


        /// <summary>
        /// Creates a limiting active mode limiting the final active power to a maximum.
        /// </summary>
        /// <param name="name">The name of the operating mode.</param>
        /// <param name="maximum">The maximum active power in Watts.</param>
        /// <param name="transitionRamp">The transition ramp or null for a ramp returning zero.</param>
        /// <returns>The limiting active mode.</returns>
        private TestLimitingActiveMode CreateLimitingActiveMode(string name, double maximum, Mock<IRampController>? transitionRamp = null)
        {
            return new TestLimitingActiveMode(name, rampController: CreateModeRamp(transitionRamp))
            {
                CalculateAction = signals => { signals.LimitedActivePower = new ActivePower(Math.Min(signals.FinalActivePower.Watts, maximum)); return signals; }
            };
        }
    }
}

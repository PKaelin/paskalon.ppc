// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Application.Ramps;
using paskalON.OperatingModes.Domain.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.IntegrationTest.DummyModes;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.OperatingModes.Domain.SafetyFunctions;
using paskalON.OperatingModes.Domain.Stacks;
using paskalON.OperatingModes.Domain.Systems;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.IntegrationTest.Systems
{
    /// <summary>
    /// This test class implements basic integration tests for the ActivePowerMode.
    /// </summary>
    /// <remarks>
    /// ActivePowerMode is a basic mode and can be combined with additive and/or limiting modes.
    /// ActivePowerMode is a closed feedback loop. In these tests the feedback (measured active power at the POI) always matches the
    /// target of the mode, so the feedback never corrects the target.
    /// </remarks>
    [TestClass]
    public sealed class SystemControlActivePowerModeBasicTest
    {
        /// <summary>
        /// Fake time provider to advance the ramps.
        /// </summary>
        private FakeTimeProvider _timeProvider = null!;


        /// <summary>
        /// Holds the system configuration instance for internal use.
        /// </summary>
        private SystemConfig _systemConfig = null!;


        /// <summary>
        /// Base active mode with a 1MW ramp per second whose feedback matches its target.
        /// </summary>
        private ActivePowerMode _activePowerMode = null!;


        /// <summary>
        /// Additive active mode adding 2MW with a 500kW ramp per second.
        /// </summary>
        private DummyAdditiveActiveMode _additiveMode = null!;


        /// <summary>
        /// The operating mode stack.
        /// </summary>
        private OperatingModeStack _stack = null!;


        /// <summary>
        /// The operating mode overrides.
        /// </summary>
        private OperatingModeOverrides _overrides = null!;


        /// <summary>
        /// The system operating mode control.
        /// </summary>
        private SystemOperatingModeControl _control = null!;


        /// <summary>
        /// Creates a 10 MW plant with 10 MW available power.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 10, 15, 8, 0, 0, TimeSpan.Zero));
            _systemConfig = DomainTestData.CreateSystemConfig();

            RampRateConfig activePowerRamp = new RampRateConfig
            {
                ChangedBy = "Test",
                RampUpRatePerSecond = ActivePower.FromMega(1).Watts,
                RampDownRatePerSecond = ActivePower.FromMega(1).Watts
            };

            ActivePowerModeConfig activePowerConfig = new ActivePowerModeConfig
            {
                ChangedBy = "Test",
                Name = "ActivePowerMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = 1000,
                DeadbandAvailable = 1000,
                RampConfig = activePowerRamp
            };

            ActivePowerModeMap activePowerMap = new ActivePowerModeMap
            {
                AvailableActivePower = () => ActivePower.FromMega(10),
                AvailableReactivePower = () => null,
                ActivePowerAtPoi = () => _activePowerMode?.ActiveControl?.Target
            };

            _activePowerMode = new ActivePowerMode(NullLogger.Instance, _timeProvider, new Mock<IMetricsPublisher>().Object, _systemConfig, activePowerConfig,
                activePowerMap, new RampController(NullLogger.Instance, _timeProvider, activePowerRamp));

            _additiveMode = DomainTestData.CreateDummyAdditiveActiveMode(_timeProvider, 2);
            _stack = new OperatingModeStack();
            _overrides = new OperatingModeOverrides();

            SystemOperatingModeControlMap systemMap = new SystemOperatingModeControlMap { ActivePowerAtPoi = () => null, ReactivePowerAtPoi = () => null };
            _control = new SystemOperatingModeControl(NullLogger.Instance, _systemConfig, _stack, _overrides, systemMap, new Mock<IMetricsPublisher>().Object, new Mock<ISetpointsPublisher>().Object);
        }


        /// <summary>
        /// Modes used: ActivePowerMode
        /// Feedback: Always matches the target of the ActivePowerMode.
        /// Scenario Step1: Activate mode, set SetPoint to maximum nameplate, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// Scenario Step2: Set SetPoint to 0, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerModeFeedbackAtTargetNewSetpointToZeroMegaWattWhenRampedTest()
        {
            // Step 1
            _stack.Add(_activePowerMode, 1);            // Stack only commits and records. The control doesnt know about it yet
            _control.UpdateSetpoints(ActivePower.FromMega(10), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(10, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);
            AssertFeedbackAtTarget();

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(1, _control.SetpointActivePowerActual.MegaWatts);

            _timeProvider.Advance(TimeSpan.FromSeconds(4));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);

            _timeProvider.Advance(TimeSpan.FromSeconds(4));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(9, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(10, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            AssertFeedbackAtTarget();

            // Step 2
            _control.UpdateSetpoints(ActivePower.FromMega(0), new ReactivePower(0));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(10, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);
            AssertFeedbackAtTarget();

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            AssertFeedbackAtTarget();
        }


        /// <summary>
        /// Modes used: ActivePowerMode
        /// Feedback: Always matches the target of the ActivePowerMode.
        /// Scenario Step1: Activate mode, set SetPoint to maximum nameplate, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// Scenario Step2: Set SetPoint to 3, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerModeFeedbackAtTargetNewSetpointTo3MegaWattWhenRampedTest()
        {
            // Step 1
            _stack.Add(_activePowerMode, 1);            // Stack only commits and records. The control doesnt know about it yet
            _control.UpdateSetpoints(ActivePower.FromMega(10), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(10, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(10, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            AssertFeedbackAtTarget();

            // Step 2
            _control.UpdateSetpoints(ActivePower.FromMega(3), new ReactivePower(0));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(3, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(3, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(10, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(3, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(2));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(3, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(3, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            AssertFeedbackAtTarget();
        }


        /// <summary>
        /// Modes used: ActivePowerMode
        /// Feedback: Always matches the target of the ActivePowerMode.
        /// Scenario Step1: Activate mode, set SetPoint to maximum nameplate, wait for the ramp to be in the middle.
        /// Scenario Step2: Set SetPoint to less than old setpoint, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerModeFeedbackAtTargetNewSetpointWhenRampingTest()
        {
            // Step 1
            _stack.Add(_activePowerMode, 1);            // Stack only commits and records. The control doesnt know about it yet
            _control.UpdateSetpoints(ActivePower.FromMega(10), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(10, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);

            // Step 2
            _control.UpdateSetpoints(ActivePower.FromMega(7), new ReactivePower(0));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(7, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(7, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);
            AssertFeedbackAtTarget();

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(7, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(6, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(7, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(7, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            AssertFeedbackAtTarget();
        }


        /// <summary>
        /// Modes used: ActivePowerMode
        /// Feedback: Always matches the target of the ActivePowerMode.
        /// Scenario Step1: Activate mode, set SetPoint to maximum nameplate, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// Scenario Step2: Deactivate mode, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerModeFeedbackAtTargetRemoveModeWhenRampedTest()
        {
            // Step 1
            _stack.Add(_activePowerMode, 1);            // Stack only commits and records. The control doesnt know about it yet
            _control.UpdateSetpoints(ActivePower.FromMega(10), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(10));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(10, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            AssertFeedbackAtTarget();

            // Step 2
            _stack.Remove(_activePowerMode);            // Stack only commits and records. The control doesnt know about it yet
            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and deactivates the mode
            Assert.AreEqual(10, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(10, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToDisabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToDisabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Disabled, _activePowerMode.State);
        }


        /// <summary>
        /// Modes used: ActivePowerMode
        /// Feedback: Always matches the target of the ActivePowerMode.
        /// Scenario Step1: Activate mode, set SetPoint to maximum nameplate, wait for the ramp to be in the middle.
        /// Scenario Step2: Deactivate mode, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerModeFeedbackAtTargetRemoveModeWhenRampingTest()
        {
            // Step 1
            _stack.Add(_activePowerMode, 1);            // Stack only commits and records. The control doesnt know about it yet
            _control.UpdateSetpoints(ActivePower.FromMega(10), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);
            AssertFeedbackAtTarget();

            // Step 2
            _stack.Remove(_activePowerMode);            // Stack only commits and records. The control doesnt know about it yet
            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and deactivates the mode
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToDisabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(2));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(3, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToDisabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(3));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Disabled, _activePowerMode.State);
        }


        /// <summary>
        /// Modes used: ActivePowerMode & ShutdownMode
        /// Feedback: Always matches the target of the ActivePowerMode.
        /// Scenario Step1: Activate mode, set SetPoint to maximum nameplate, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// Scenario Step2: Add override mode, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerModeFeedbackAtTargetOverrideModeAddedWhenRampedTest()
        {
            // Step 1
            _stack.Add(_activePowerMode, 1);            // Stack only commits and records. The control doesnt know about it yet
            _control.UpdateSetpoints(ActivePower.FromMega(10), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(10));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(10, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);

            // Step 2
            ShutdownMode shutdownMode = DomainTestData.CreateShutdownMode(_timeProvider);
            _overrides.Add(shutdownMode);               // Overrides only commit and record. The control doesnt know about it yet
            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(10, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, shutdownMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(6, _control.SetpointActivePowerActual.MegaWatts);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(2, _control.SetpointActivePowerActual.MegaWatts);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, shutdownMode.State);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            AssertFeedbackAtTarget();
        }


        /// <summary>
        /// Modes used: ActivePowerMode & ShutdownMode
        /// Feedback: Always matches the target of the ActivePowerMode.
        /// Scenario Step1: Activate mode, set SetPoint to maximum nameplate, wait for the ramp to be in the middle.
        /// Scenario Step2: Add override mode, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerModeFeedbackAtTargetOverrideModeAddedWhenRampingTest()
        {
            // Step 1
            _stack.Add(_activePowerMode, 1);            // Stack only commits and records. The control doesnt know about it yet
            _control.UpdateSetpoints(ActivePower.FromMega(10), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);

            // Step 2
            ShutdownMode shutdownMode = DomainTestData.CreateShutdownMode(_timeProvider);
            _overrides.Add(shutdownMode);               // Overrides only commit and record. The control doesnt know about it yet
            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, shutdownMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(1, _control.SetpointActivePowerActual.MegaWatts);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, shutdownMode.State);
            AssertFeedbackAtTarget();
        }


        /// <summary>
        /// Modes used: ActivePowerMode & DummyAdditiveActiveMode
        /// Feedback: Always matches the target of the ActivePowerMode.
        /// Scenario Step1: Activate ActivePowerMode, set SetPoint to 6, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// Scenario Step2: Add additive mode with 2, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerModeFeedbackAtTargetAdditiveModeAddedWhenRampedTest()
        {
            // Step 1
            _stack.Add(_activePowerMode, 1);            // Stack only commits and records. The control doesnt know about it yet
            _control.UpdateSetpoints(ActivePower.FromMega(6), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(6, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(6));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(6, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(6, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);

            // Step 2
            _stack.Add(_additiveMode, 2);               // Stack only commits and records. The control doesnt know about it yet
            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(6, _control.BaseActivePower.MegaWatts);
            Assert.AreEqual(2, _control.AdditiveActivePowerDelta.MegaWatts);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(6, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _additiveMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(6.5, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _additiveMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(3));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(8, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            Assert.AreEqual(OperatingModeState.Enabled, _additiveMode.State);
            AssertFeedbackAtTarget();
        }


        /// <summary>
        /// Modes used: ActivePowerMode & DummyAdditiveActiveMode
        /// Feedback: Always matches the target of the ActivePowerMode.
        /// Scenario Step1: Activate ActivePowerMode, set SetPoint to 6, wait for the ramp to be in the middle.
        /// Scenario Step2: Add additive mode with 2, wait for both ramps to complete, check that the SetPoint and States are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerModeFeedbackAtTargetAdditiveModeAddedWhenRampingTest()
        {
            // Step 1
            _stack.Add(_activePowerMode, 1);            // Stack only commits and records. The control doesnt know about it yet
            _control.UpdateSetpoints(ActivePower.FromMega(6), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(6, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(3));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(6, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(3, _control.SetpointActivePowerActual.MegaWatts);

            // Step 2
            _stack.Add(_additiveMode, 2);               // Stack only commits and records. The control doesnt know about it yet
            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(3, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _additiveMode.State);

            // Both modes ramp their own part, the base mode with 1 MW and the additive mode with 0.5 MW per second
            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(4.5, _control.SetpointActivePowerActual.MegaWatts);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(6, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _additiveMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(7.5, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _additiveMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(8, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            Assert.AreEqual(OperatingModeState.Enabled, _additiveMode.State);
            AssertFeedbackAtTarget();
        }


        /// <summary>
        /// Modes used: ActivePowerMode & DummyAdditiveActiveMode
        /// Feedback: Always matches the target of the ActivePowerMode.
        /// Scenario Step1: Activate both modes at once, set SetPoint to 6, wait for both ramps to complete, check that the SetPoint and States are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerModeFeedbackAtTargetAdditiveModeAddedWithBaseModeTest()
        {
            // Step 1
            _stack.Add(_activePowerMode, 1);            // Stack only commits and records. The control doesnt know about it yet
            _stack.Add(_additiveMode, 2);
            _control.UpdateSetpoints(ActivePower.FromMega(6), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the changes (TakeCycle) and activates the modes
            Assert.AreEqual(6, _control.BaseActivePower.MegaWatts);
            Assert.AreEqual(2, _control.AdditiveActivePowerDelta.MegaWatts);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _additiveMode.State);

            // Both modes ramp their own part, the base mode with 1 MW and the additive mode with 0.5 MW per second
            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(1.5, _control.SetpointActivePowerActual.MegaWatts);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(3, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _additiveMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(2));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(6, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);
            Assert.AreEqual(OperatingModeState.Enabled, _additiveMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(2));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(8, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            Assert.AreEqual(OperatingModeState.Enabled, _additiveMode.State);
            AssertFeedbackAtTarget();
        }


        /// <summary>
        /// Modes used: ActivePowerMode & DummyAdditiveActiveMode
        /// Feedback: Always matches the target of the ActivePowerMode.
        /// Scenario Step1: Activate both modes, set SetPoint to 6, wait for the ramps to complete, check that the SetPoint and States are reached.
        /// Scenario Step2: Remove additive mode, wait for the ramp to complete, check that the SetPoint and States are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerModeFeedbackAtTargetAdditiveModeRemovedWhenRampedTest()
        {
            // Step 1
            _stack.Add(_activePowerMode, 1);            // Stack only commits and records. The control doesnt know about it yet
            _stack.Add(_additiveMode, 2);
            _control.UpdateSetpoints(ActivePower.FromMega(6), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the changes (TakeCycle) and activates the modes
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);

            _timeProvider.Advance(TimeSpan.FromSeconds(6));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(8, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            Assert.AreEqual(OperatingModeState.Enabled, _additiveMode.State);

            // Step 2
            _stack.Remove(_additiveMode);               // Stack only commits and records. The control doesnt know about it yet
            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and deactivates the mode
            Assert.AreEqual(0, _control.AdditiveActivePowerDelta.MegaWatts);
            Assert.AreEqual(6, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(8, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            Assert.AreEqual(OperatingModeState.RampingToDisabled, _additiveMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(6, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(7.5, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToDisabled, _additiveMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(3));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(6, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(6, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            Assert.AreEqual(OperatingModeState.Disabled, _additiveMode.State);
            AssertFeedbackAtTarget();
        }


        /// <summary>
        /// Modes used: ActivePowerMode & DummyAdditiveActiveMode
        /// Feedback: Always matches the target of the ActivePowerMode.
        /// Scenario Step1: Activate both modes, set SetPoint to 6, wait for the ramps to complete, check that the SetPoint and States are reached.
        /// Scenario Step2: Change the additive delta from 2 to 4, wait for the ramp to complete, check that the SetPoint and States are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerModeFeedbackAtTargetAdditiveDeltaChangedWhenRampedTest()
        {
            // Step 1
            _stack.Add(_activePowerMode, 1);            // Stack only commits and records. The control doesnt know about it yet
            _stack.Add(_additiveMode, 2);
            _control.UpdateSetpoints(ActivePower.FromMega(6), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the changes (TakeCycle) and activates the modes
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);

            _timeProvider.Advance(TimeSpan.FromSeconds(6));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(8, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _additiveMode.State);

            // Step 2
            _additiveMode.DeltaActivePower = ActivePower.FromMega(4);
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(4, _control.AdditiveActivePowerDelta.MegaWatts);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(8, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _additiveMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(8.5, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _additiveMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(3));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(10, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _additiveMode.State);
            AssertFeedbackAtTarget();
        }


        /// <summary>
        /// Modes used: ActivePowerMode & DummyAdditiveActiveMode
        /// Feedback: Always matches the target of the ActivePowerMode.
        /// Scenario Step1: Activate both modes, set SetPoint to 6, wait for the ramps to complete, check that the SetPoint and States are reached.
        /// Scenario Step2: Set SetPoint to 3, wait for the ramp to complete, check that the additive delta is kept and the SetPoint and States are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerModeFeedbackAtTargetNewSetpointWithAdditiveModeWhenRampedTest()
        {
            // Step 1
            _stack.Add(_activePowerMode, 1);            // Stack only commits and records. The control doesnt know about it yet
            _stack.Add(_additiveMode, 2);
            _control.UpdateSetpoints(ActivePower.FromMega(6), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the changes (TakeCycle) and activates the modes
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);

            _timeProvider.Advance(TimeSpan.FromSeconds(6));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(8, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);

            // Step 2
            _control.UpdateSetpoints(ActivePower.FromMega(3), new ReactivePower(0));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(3, _control.BaseActivePower.MegaWatts);
            Assert.AreEqual(2, _control.AdditiveActivePowerDelta.MegaWatts);
            Assert.AreEqual(5, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(8, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);
            Assert.AreEqual(OperatingModeState.Enabled, _additiveMode.State);
            AssertFeedbackAtTarget();

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(5, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(7, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(2));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(5, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            Assert.AreEqual(OperatingModeState.Enabled, _additiveMode.State);
            AssertFeedbackAtTarget();
        }


        /// <summary>
        /// Modes used: ActivePowerMode & DummyAdditiveActiveMode & ShutdownMode
        /// Feedback: Always matches the target of the ActivePowerMode.
        /// Scenario Step1: Activate both modes, set SetPoint to 6, wait for the ramps to complete, check that the SetPoint and States are reached.
        /// Scenario Step2: Add override mode, wait for the ramp to complete, check that the SetPoint and States are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerModeFeedbackAtTargetOverrideModeAddedWithAdditiveModeWhenRampedTest()
        {
            // Step 1
            _stack.Add(_activePowerMode, 1);            // Stack only commits and records. The control doesnt know about it yet
            _stack.Add(_additiveMode, 2);
            _control.UpdateSetpoints(ActivePower.FromMega(6), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the changes (TakeCycle) and activates the modes
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);

            _timeProvider.Advance(TimeSpan.FromSeconds(6));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(8, _control.SetpointActivePowerActual.MegaWatts);

            // Step 2
            ShutdownMode shutdownMode = DomainTestData.CreateShutdownMode(_timeProvider);
            _overrides.Add(shutdownMode);               // Overrides only commit and record. The control doesnt know about it yet
            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(8, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, shutdownMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(4, _control.SetpointActivePowerActual.MegaWatts);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(8, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, shutdownMode.State);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerMode.State);
            Assert.AreEqual(OperatingModeState.Enabled, _additiveMode.State);
            AssertFeedbackAtTarget();
        }


        /// <summary>
        /// Asserts that the feedback of the ActivePowerMode matches its target, so the closed loop did not correct the target.
        /// </summary>
        private void AssertFeedbackAtTarget()
        {
            Assert.AreEqual(_activePowerMode.ActiveControl!.Target.Watts, _activePowerMode.ActiveFeedback!.Measured);
            Assert.AreEqual(0, _activePowerMode.ActiveFeedback!.ErrorAdjustment.Watts);
        }
    }
}

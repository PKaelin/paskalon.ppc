// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Application.Ramps;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.OperatingModes.Domain.SafetyFunctions;
using paskalON.OperatingModes.Domain.Stacks;
using paskalON.OperatingModes.Domain.Systems;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.IntegrationTest.Systems
{
    /// <summary>
    /// This test class implements basic integration tests for the ActivePowerFixedMode.
    /// </summary>
    /// <remarks>
    /// ActivePowerFixedMode is an exclusive mode and cannot be combined with additive or limiting modes.
    /// </remarks>
    [TestClass]
    public sealed class SystemControlActivePowerFixedModeBasicTest
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
        /// Exclusive active mode with a 1MW ramp per second.
        /// </summary>
        private ActivePowerFixedMode _activePowerFixedMode = null!;


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
        /// Creates a 10 MW plant with 8 MW available power.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 10, 15, 8, 0, 0, TimeSpan.Zero));
            _systemConfig = DomainTestData.CreateSystemConfig();

            ActivePowerFixedModeConfig fixedConfig = new ActivePowerFixedModeConfig
            {
                ChangedBy = "Test",
                Name = "ActivePowerFixedMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = 1000,
                DeadbandAvailable = 1000,
                RampConfig = new RampRateConfig
                {
                    ChangedBy = "Test",
                    RampUpRatePerSecond = ActivePower.FromMega(1).Watts,
                    RampDownRatePerSecond = ActivePower.FromMega(1).Watts
                },
            };

            ActivePowerFixedModeMap fixedMap = new ActivePowerFixedModeMap { AvailableActivePower = () => ActivePower.FromMega(10), AvailableReactivePower = () => null };

            _activePowerFixedMode = new ActivePowerFixedMode(NullLogger.Instance, _timeProvider, new Mock<IMetricsPublisher>().Object, _systemConfig, fixedConfig, fixedMap,
                new RampController(NullLogger.Instance, _timeProvider, fixedConfig.RampConfig));

            _stack = new OperatingModeStack();
            _overrides = new OperatingModeOverrides();

            SystemOperatingModeControlMap systemMap = new SystemOperatingModeControlMap { ActivePowerAtPoi = () => null, ReactivePowerAtPoi = () => null };
            _control = new SystemOperatingModeControl(NullLogger.Instance, _systemConfig, _stack, _overrides, systemMap, new Mock<IMetricsPublisher>().Object, new Mock<ISetpointsPublisher>().Object);
        }


        /// <summary>
        /// Modes used: ActivePowerFixedMode
        /// Scenario Step1: Activate mode, set SetPoint to maximum nameplate, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// Scenario Step2: Set SetPoint to 0, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerFixedModeNewSetpointToZeroMegaWattWhenRampedTest()
        {
            // Step 1
            _stack.Add(_activePowerFixedMode, 1);       // Stack only commits and records. The control doesnt know about it yet
            _control.UpdateSetpoints(ActivePower.FromMega(10), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(10, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerFixedMode.State);

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
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(10, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerFixedMode.State);

            // Step 2
            _control.UpdateSetpoints(ActivePower.FromMega(0), new ReactivePower(0));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(10, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerFixedMode.State);
        }


        /// <summary>
        /// Modes used: ActivePowerFixedMode
        /// Scenario Step1: Activate mode, set SetPoint to maximum nameplate, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// Scenario Step2: Set SetPoint to 3, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerFixedModeNewSetpointTo3MegaWattWhenRampedTest()
        {
            // Step 1
            _stack.Add(_activePowerFixedMode, 1);       // Stack only commits and records. The control doesnt know about it yet
            _control.UpdateSetpoints(ActivePower.FromMega(10), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(10, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerFixedMode.State);

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
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(10, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerFixedMode.State);

            // Step 2
            _control.UpdateSetpoints(ActivePower.FromMega(3), new ReactivePower(0));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(3, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(3, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(10, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(3, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(3, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(3, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerFixedMode.State);
        }


        /// <summary>
        /// Modes used: ActivePowerFixedMode
        /// Scenario Step1: Activate mode, set SetPoint to maximum nameplate, wait for the ramp to be in the middle
        /// Scenario Step2: Set SetPoint to less than old setpoint, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerFixedModeNewSetpointWhenRampingTest()
        {
            // Step 1
            _stack.Add(_activePowerFixedMode, 1);       // Stack only commits and records. The control doesnt know about it yet
            _control.UpdateSetpoints(ActivePower.FromMega(10), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(10, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);

            // Step 2
            _control.UpdateSetpoints(ActivePower.FromMega(7), new ReactivePower(0));
            _control.Calculate(CancellationToken.None);
            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(7, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(7, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(6, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(7, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(7, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerFixedMode.State);
        }


        /// <summary>
        /// Modes used: ActivePowerFixedMode
        /// Scenario Step1: Activate mode, set SetPoint to maximum nameplate, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// Scenario Step2: Deactivate mode, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerFixedRemoveModeWhenRampedTest()
        {
            // Step 1
            _stack.Add(_activePowerFixedMode, 1);       // Stack only commits and records. The control doesnt know about it yet
            _control.UpdateSetpoints(ActivePower.FromMega(10), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(10, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(1, _control.SetpointActivePowerActual.MegaWatts);

            _timeProvider.Advance(TimeSpan.FromSeconds(8));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(9, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(10, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerFixedMode.State);

            // Step 2
            _stack.Remove(_activePowerFixedMode);       // Stack only commits and records. The control doesnt know about it yet
            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and deactivates the mode
            Assert.AreEqual(10, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(10, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToDisabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToDisabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Disabled, _activePowerFixedMode.State);
        }


        /// <summary>
        /// Modes used: ActivePowerFixedMode
        /// Scenario Step1: Activate mode, set SetPoint to maximum nameplate, wait for the ramp to be in the middle
        /// Scenario Step2: Deactivate mode, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerFixedRemoveModeWhenRampingTest()
        {
            // Step 1 
            _stack.Add(_activePowerFixedMode, 1);       // Stack only commits and records. The control doesnt know about it yet
            _control.UpdateSetpoints(ActivePower.FromMega(10), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(10, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(1, _control.SetpointActivePowerActual.MegaWatts);

            _timeProvider.Advance(TimeSpan.FromSeconds(4));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);

            // Step 2
            _stack.Remove(_activePowerFixedMode);       // Stack only commits and records. The control doesnt know about it yet
            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and deactivates the mode
            Assert.AreEqual(10, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToDisabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(2));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(3, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToDisabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(3));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Disabled, _activePowerFixedMode.State);
        }


        /// <summary>
        /// Modes used: ActivePowerFixedMode & ShutdownMode
        /// Scenario Step1: Activate mode, set SetPoint to maximum nameplate, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// Scenario Step2: Add override mode, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerFixedOverrideModeAddedWhenRampedTest()
        {
            // Step 1
            _stack.Add(_activePowerFixedMode, 1);       // Stack only commits and records. The control doesnt know about it yet
            _control.UpdateSetpoints(ActivePower.FromMega(10), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(10, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(1, _control.SetpointActivePowerActual.MegaWatts);

            _timeProvider.Advance(TimeSpan.FromSeconds(8));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(9, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(10, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerFixedMode.State);

            // Step 2
            ShutdownMode shutdownMode = DomainTestData.CreateShutdownMode(_timeProvider);
            _overrides.Add(shutdownMode);               // Stack only commits and records. The control doesnt know about it yet
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
        }


        /// <summary>
        /// Modes used: ActivePowerFixedMode & ShutdownMode
        /// Scenario Step1: Activate mode, set SetPoint to maximum nameplate, wait for the ramp to be in the middle.
        /// Scenario Step2: Add override mode, wait for the ramp to complete, check that the SetPoint and State are reached.
        /// </summary>
        [TestMethod]
        public void ActivePowerFixedOverrideModeAddedWhenRampingTest()
        {
            // Step 1
            _stack.Add(_activePowerFixedMode, 1);       // Stack only commits and records. The control doesnt know about it yet
            _control.UpdateSetpoints(ActivePower.FromMega(10), new ReactivePower(0));

            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(10, _control.SetpointActivePower.MegaWatts);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(0, _control.SetpointActivePowerActual.MegaWatts);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _activePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ActivePowerObjective.MegaWatts);
            Assert.AreEqual(5, _control.SetpointActivePowerActual.MegaWatts);

            // Step 2
            ShutdownMode shutdownMode = DomainTestData.CreateShutdownMode(_timeProvider);
            _overrides.Add(shutdownMode);               // Stack only commits and records. The control doesnt know about it yet
            _control.Calculate(CancellationToken.None); // Control takes the change (TakeCycle) and activates the mode
            Assert.AreEqual(OperatingModeState.Enabled, _activePowerFixedMode.State);
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
        }

    }
}

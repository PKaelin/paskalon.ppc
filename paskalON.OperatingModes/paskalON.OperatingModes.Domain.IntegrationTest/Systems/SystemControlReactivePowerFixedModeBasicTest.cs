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
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.OperatingModes.Domain.SafetyFunctions;
using paskalON.OperatingModes.Domain.Stacks;
using paskalON.OperatingModes.Domain.Systems;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.IntegrationTest.Systems
{
    /// <summary>
    /// This test class implements basic integration tests for the ReactivePowerFixedMode.
    /// </summary>
    [TestClass]
    public sealed class SystemControlReactivePowerFixedModeBasicTest
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
        /// Exclusive reactive mode with a 1 MVar ramp per second.
        /// </summary>
        private ReactivePowerFixedMode _reactivePowerFixedMode = null!;


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
        /// Creates a 10 MVar plant with 10 MVar available reactive power.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 10, 15, 8, 0, 0, TimeSpan.Zero));
            _systemConfig = DomainTestData.CreateSystemConfig();

            ReactivePowerFixedModeConfig fixedConfig = new ReactivePowerFixedModeConfig
            {
                ChangedBy = "Test",
                Name = "ReactivePowerFixedMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = 1000,
                DeadbandAvailable = 1000,
                RampConfig = new RampRateConfig
                {
                    ChangedBy = "Test",
                    RampUpRatePerSecond = ReactivePower.FromMega(1).VoltAmperesReactive,
                    RampDownRatePerSecond = ReactivePower.FromMega(1).VoltAmperesReactive
                },
            };

            ReactivePowerFixedModeMap fixedMap = new ReactivePowerFixedModeMap { AvailableActivePower = () => ActivePower.FromMega(10), AvailableReactivePower = () => ReactivePower.FromMega(10) };

            _reactivePowerFixedMode = new ReactivePowerFixedMode(NullLogger.Instance, _timeProvider, new Mock<IMetricsPublisher>().Object, _systemConfig, fixedConfig, fixedMap,
                new RampController(NullLogger.Instance, _timeProvider, fixedConfig.RampConfig));

            _stack = new OperatingModeStack();
            _overrides = new OperatingModeOverrides();

            SystemOperatingModeControlMap systemMap = new SystemOperatingModeControlMap { ActivePowerAtPoi = () => null, ReactivePowerAtPoi = () => null };
            _control = new SystemOperatingModeControl(NullLogger.Instance, _systemConfig, _stack, _overrides, systemMap, new Mock<IMetricsPublisher>().Object,
                new Mock<ISetpointsPublisher>().Object);
        }


        /// <summary>
        /// Modes used: ReactivePowerFixedMode.
        /// Scenario Step 1: Activate mode, set setpoint to maximum nameplate, wait for the ramp to complete, and check that the setpoint and state are reached.
        /// Scenario Step 2: Set setpoint to 0, wait for the ramp to complete, and check that the setpoint and state are reached.
        /// </summary>
        [TestMethod]
        public void ReactivePowerFixedModeNewSetpointToZeroMegaVarWhenRampedTest()
        {
            // Step 1
            _stack.Add(_reactivePowerFixedMode, 1);
            _control.UpdateSetpoints(new ActivePower(0), ReactivePower.FromMega(10));

            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.SetpointReactivePower.MegaVoltAmperesReactive);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(0, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(1, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);

            _timeProvider.Advance(TimeSpan.FromSeconds(4));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(5, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);

            _timeProvider.Advance(TimeSpan.FromSeconds(4));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(9, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(10, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.Enabled, _reactivePowerFixedMode.State);

            _control.UpdateSetpoints(new ActivePower(0), ReactivePower.FromMega(0));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.SetpointReactivePower.MegaVoltAmperesReactive);
            Assert.AreEqual(0, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(10, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(5, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(0, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.Enabled, _reactivePowerFixedMode.State);
        }


        /// <summary>
        /// Modes used: ReactivePowerFixedMode.
        /// Scenario Step 1: Activate mode, set setpoint to maximum nameplate, wait for the ramp to complete, and check that the setpoint and state are reached.
        /// Scenario Step 2: Set setpoint to 3, wait for the ramp to complete, and check that the setpoint and state are reached.
        /// </summary>
        [TestMethod]
        public void ReactivePowerFixedModeNewSetpointTo3MegaVarWhenRampedTest()
        {
            // Step 1
            _stack.Add(_reactivePowerFixedMode, 1);
            _control.UpdateSetpoints(new ActivePower(0), ReactivePower.FromMega(10));

            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.SetpointReactivePower.MegaVoltAmperesReactive);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(0, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(1, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);

            _timeProvider.Advance(TimeSpan.FromSeconds(4));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(5, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);

            _timeProvider.Advance(TimeSpan.FromSeconds(4));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(9, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(10, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.Enabled, _reactivePowerFixedMode.State);

            // Step 2
            _control.UpdateSetpoints(new ActivePower(0), ReactivePower.FromMega(3));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(3, _control.SetpointReactivePower.MegaVoltAmperesReactive);
            Assert.AreEqual(3, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(10, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(3, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(5, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(3, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(3, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.Enabled, _reactivePowerFixedMode.State);
        }


        /// <summary>
        /// Modes used: ReactivePowerFixedMode.
        /// Scenario Step 1: Activate mode, set setpoint to maximum nameplate, and wait for the ramp to be in the middle.
        /// Scenario Step 2: Set setpoint to less than the old setpoint, wait for the ramp to complete, and check that the setpoint and state are reached.
        /// </summary>
        [TestMethod]
        public void ReactivePowerFixedModeNewSetpointWhenRampingTest()
        {
            // Step 1
            _stack.Add(_reactivePowerFixedMode, 1);
            _control.UpdateSetpoints(new ActivePower(0), ReactivePower.FromMega(10));

            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.SetpointReactivePower.MegaVoltAmperesReactive);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(0, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(5, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);

            // Step 2
            _control.UpdateSetpoints(new ActivePower(0), ReactivePower.FromMega(7));
            _control.Calculate(CancellationToken.None);
            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(7, _control.SetpointReactivePower.MegaVoltAmperesReactive);
            Assert.AreEqual(7, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(6, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(7, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(7, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.Enabled, _reactivePowerFixedMode.State);
        }


        /// <summary>
        /// Modes used: ReactivePowerFixedMode.
        /// Scenario Step 1: Activate mode and wait for the ramp to complete.
        /// Scenario Step 2: Deactivate mode, wait for the ramp to complete, and check that the setpoint and state are reached.
        /// </summary>
        [TestMethod]
        public void ReactivePowerFixedRemoveModeWhenRampedTest()
        {
            _stack.Add(_reactivePowerFixedMode, 1);
            _control.UpdateSetpoints(new ActivePower(0), ReactivePower.FromMega(10));

            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.SetpointReactivePower.MegaVoltAmperesReactive);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(0, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(1, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);

            _timeProvider.Advance(TimeSpan.FromSeconds(8));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(9, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(10, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.Enabled, _reactivePowerFixedMode.State);

            _stack.Remove(_reactivePowerFixedMode);
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.SetpointReactivePower.MegaVoltAmperesReactive);
            Assert.AreEqual(0, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(10, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToDisabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(5, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToDisabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(0, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.Disabled, _reactivePowerFixedMode.State);
        }


        /// <summary>
        /// Modes used: ReactivePowerFixedMode.
        /// Scenario Step 1: Activate mode and wait for the ramp to be in the middle.
        /// Scenario Step 2: Deactivate mode, wait for the ramp to complete, and check that the setpoint and state are reached.
        /// </summary>
        [TestMethod]
        public void ReactivePowerFixedRemoveModeWhenRampingTest()
        {
            _stack.Add(_reactivePowerFixedMode, 1);
            _control.UpdateSetpoints(new ActivePower(0), ReactivePower.FromMega(10));

            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.SetpointReactivePower.MegaVoltAmperesReactive);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(0, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(1, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);

            _timeProvider.Advance(TimeSpan.FromSeconds(4));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(5, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _reactivePowerFixedMode.State);

            _stack.Remove(_reactivePowerFixedMode);
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.SetpointReactivePower.MegaVoltAmperesReactive);
            Assert.AreEqual(0, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(5, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToDisabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(2));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(3, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToDisabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(3));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(0, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(0, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.Disabled, _reactivePowerFixedMode.State);
        }


        /// <summary>
        /// Modes used: ReactivePowerFixedMode and ShutdownMode.
        /// Scenario Step 1: Activate mode, set setpoint to maximum nameplate, wait for the ramp to complete, and check that the setpoint and state are reached.
        /// Scenario Step 2: Add override mode, wait for the ramp to complete, and check that the setpoint and state are reached.
        /// </summary>
        [TestMethod]
        public void ReactivePowerFixedOverrideModeAddedWhenRampedTest()
        {
            // Step 1
            _stack.Add(_reactivePowerFixedMode, 1);
            _control.UpdateSetpoints(new ActivePower(0), ReactivePower.FromMega(10));

            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.SetpointReactivePower.MegaVoltAmperesReactive);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(0, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(1, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);

            _timeProvider.Advance(TimeSpan.FromSeconds(8));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(9, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(10, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.Enabled, _reactivePowerFixedMode.State);

            // Step 2
            ShutdownMode shutdownMode = DomainTestData.CreateShutdownMode(_timeProvider);
            _overrides.Add(shutdownMode);
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(10, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, shutdownMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(6, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(2, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(0, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.Enabled, shutdownMode.State);
        }


        /// <summary>
        /// Modes used: ReactivePowerFixedMode and ShutdownMode.
        /// Scenario Step 1: Activate mode, set setpoint to maximum nameplate, and wait for the ramp to be in the middle.
        /// Scenario Step 2: Add override mode, wait for the ramp to complete, and check that the setpoint and state are reached.
        /// </summary>
        [TestMethod]
        public void ReactivePowerFixedOverrideModeAddedWhenRampingTest()
        {
            // Step 1
            _stack.Add(_reactivePowerFixedMode, 1);
            _control.UpdateSetpoints(new ActivePower(0), ReactivePower.FromMega(10));

            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.SetpointReactivePower.MegaVoltAmperesReactive);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(0, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, _reactivePowerFixedMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(5));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(5, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);

            // Step 2
            ShutdownMode shutdownMode = DomainTestData.CreateShutdownMode(_timeProvider);
            _overrides.Add(shutdownMode);
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(OperatingModeState.Enabled, _reactivePowerFixedMode.State);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(5, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.RampingToEnabled, shutdownMode.State);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(1, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);

            _timeProvider.Advance(TimeSpan.FromSeconds(1));
            _control.Calculate(CancellationToken.None);
            Assert.AreEqual(10, _control.ReactivePowerObjective.MegaVoltAmperesReactive);
            Assert.AreEqual(0, _control.SetpointReactivePowerActual.MegaVoltAmperesReactive);
            Assert.AreEqual(OperatingModeState.Enabled, shutdownMode.State);
        }
    }
}

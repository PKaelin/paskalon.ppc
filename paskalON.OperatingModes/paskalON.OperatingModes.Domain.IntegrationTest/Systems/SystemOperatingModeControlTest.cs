// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Application.Ramps;
using paskalON.OperatingModes.Application.Stacks;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.OperatingModes.Domain.Systems;
using paskalON.OperatingModes.Domain.Transitions;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.IntegrationTest.Systems
{
    /// <summary>
    /// Integration tests of the transitions of <see cref="SystemOperatingModeControl"/> with real operating modes, ramp controllers and the stack manager.
    /// </summary>
    [TestClass]
    public class SystemOperatingModeControlTest
    {
        /// <summary>
        /// Tolerance for ramp values in Watts.
        /// </summary>
        private const double Tolerance = 1;


        /// <summary>
        /// Fake time provider to advance the ramps.
        /// </summary>
        private FakeTimeProvider? _timeProvider;


        /// <summary>
        /// Measured active power at the POI.
        /// </summary>
        private ActivePower? _activePowerAtPoi;


        /// <summary>
        /// Exclusive active mode with a 60 second ramp.
        /// </summary>
        private ActivePowerFixedMode? _activePowerFixedMode;


        /// <summary>
        /// Base active mode with a 30 second ramp.
        /// </summary>
        private MaximumPowerPointTrackingMode? _maximumPowerPointTrackingMode;


        /// <summary>
        /// The stack manager.
        /// </summary>
        private StackManager? _stackManager;


        /// <summary>
        /// The system operating mode control.
        /// </summary>
        private SystemOperatingModeControl? _control;


        /// <summary>
        /// Creates a 10 MW plant with 8 MW available power.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 10, 13, 8, 0, 0, TimeSpan.Zero));
            _activePowerAtPoi = null;
            SystemConfig systemConfig = new SystemConfig
            {
                ChangedBy = "Test",
                Type = PowerControlType.Bess,
                ReferenceFrequency = 50,
                NameplateMinimumActivePowerWatt = -10000000,
                NameplateMaximumActivePowerWatt = 10000000,
                NameplateMinimumReactivePowerVars = -5000000,
                NameplateMaximumReactivePowerVars = 5000000,
            };
            ActivePowerFixedModeConfig fixedConfig = new ActivePowerFixedModeConfig
            {
                ChangedBy = "Test",
                Name = "ActivePowerFixedMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = 10000,
                DeadbandAvailable = 10000,
                RampConfig = new RampTimeConfig { ChangedBy = "Test", RampUpTimeSeconds = 60, RampDownTimeSeconds = 60 },
            };
            MaximumPowerPointTrackingModeConfig trackingConfig = new MaximumPowerPointTrackingModeConfig
            {
                ChangedBy = "Test",
                Name = "MaximumPowerPointTrackingMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = 10000,
                DeadbandAvailable = 10000,
                RampConfig = new RampTimeConfig { ChangedBy = "Test", RampUpTimeSeconds = 30, RampDownTimeSeconds = 30 },
            };
            ActivePowerFixedModeMap fixedMap = new ActivePowerFixedModeMap { AvailableActivePower = () => new ActivePower(8000000), AvailableReactivePower = () => null };
            MaximumPowerPointTrackingModeMap trackingMap = new MaximumPowerPointTrackingModeMap { AvailableActivePower = () => new ActivePower(8000000), AvailableReactivePower = () => null };
            _activePowerFixedMode = new ActivePowerFixedMode(NullLogger.Instance, _timeProvider, new Mock<IMetricsPublisher>().Object, systemConfig, fixedConfig, fixedMap,
                new RampController(NullLogger.Instance, _timeProvider, fixedConfig.RampConfig));
            _maximumPowerPointTrackingMode = new MaximumPowerPointTrackingMode(NullLogger.Instance, _timeProvider, new Mock<IMetricsPublisher>().Object, systemConfig, trackingConfig,
                trackingMap, new RampController(NullLogger.Instance, _timeProvider, trackingConfig.RampConfig));
            _stackManager = new StackManager();
            _stackManager.InitializeStack(new Dictionary<string, OperatingModeBase>
            {
                { _activePowerFixedMode.Name, _activePowerFixedMode },
                { _maximumPowerPointTrackingMode.Name, _maximumPowerPointTrackingMode }
            });
            SystemOperatingModeControlMap systemMap = new SystemOperatingModeControlMap { ActivePowerAtPoi = () => _activePowerAtPoi, ReactivePowerAtPoi = () => null };
            _control = new SystemOperatingModeControl(NullLogger.Instance, systemConfig, _stackManager.Stack, new OperatingModeOverrides(), systemMap, new Mock<IMetricsPublisher>().Object, new Mock<ISetpointsPublisher>().Object);
            _control.UpdateSetpoints(new ActivePower(6000000), new ReactivePower(0));
        }


        /// <summary>
        /// Confirms the initial activation starts from the measured power at the POI and transitions with the ramp of the mode.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlInitialActivationFromPoiTest()
        {
            _activePowerAtPoi = new ActivePower(2000000);
            _stackManager!.AddOperatingMode("ActivePowerFixedMode", 1);

            _control!.Calculate(CancellationToken.None);
            double startApplied = _control!.SetpointActivePowerActual.Watts;
            TransitionKind startKind = _control!.ActivePowerTransition.Kind;
            _timeProvider!.Advance(TimeSpan.FromSeconds(30));
            _control!.Calculate(CancellationToken.None);
            double halfApplied = _control!.SetpointActivePowerActual.Watts;
            _timeProvider!.Advance(TimeSpan.FromSeconds(30));
            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(2000000, startApplied, Tolerance);
            Assert.AreEqual(TransitionKind.InitialActivation, startKind);
            Assert.AreEqual(4000000, halfApplied, Tolerance);
            Assert.AreEqual(6000000, _control!.SetpointActivePowerActual.Watts, Tolerance);
            Assert.AreEqual(TransitionKind.None, _control!.ActivePowerTransition.Kind);
        }


        /// <summary>
        /// Confirms a replacement hands over from the current applied target and transitions with the ramp of the new mode.
        /// </summary>
        [TestMethod]
        public void SystemOperatingModeControlReplacementHandoverTest()
        {
            _stackManager!.AddOperatingMode("ActivePowerFixedMode", 1);
            _control!.Calculate(CancellationToken.None);
            _timeProvider!.Advance(TimeSpan.FromSeconds(60));
            _control!.Calculate(CancellationToken.None);
            double appliedBeforeReplacement = _control!.SetpointActivePowerActual.Watts;
            _stackManager!.ReplaceOperatingMode("ActivePowerFixedMode", "MaximumPowerPointTrackingMode");

            _control!.Calculate(CancellationToken.None);
            double handoverApplied = _control!.SetpointActivePowerActual.Watts;
            TransitionKind handoverKind = _control!.ActivePowerTransition.Kind;
            _timeProvider!.Advance(TimeSpan.FromSeconds(15));
            _control!.Calculate(CancellationToken.None);
            double halfApplied = _control!.SetpointActivePowerActual.Watts;
            _timeProvider!.Advance(TimeSpan.FromSeconds(15));
            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(6000000, appliedBeforeReplacement, Tolerance);
            Assert.AreEqual(6000000, handoverApplied, Tolerance);
            Assert.AreEqual(TransitionKind.ModeReplacement, handoverKind);
            Assert.AreEqual(7000000, halfApplied, Tolerance);
            Assert.AreEqual(8000000, _control!.SetpointActivePowerActual.Watts, Tolerance);
            Assert.IsFalse(_activePowerFixedMode!.IsActivated);
            Assert.AreEqual(OperatingModeState.Disabled, _activePowerFixedMode!.State);
            Assert.IsTrue(_maximumPowerPointTrackingMode!.IsActivated);
        }
    }
}

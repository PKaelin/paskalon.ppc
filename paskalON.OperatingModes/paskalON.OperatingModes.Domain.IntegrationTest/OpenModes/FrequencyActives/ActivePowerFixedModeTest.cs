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
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.OperatingModes.Domain.Systems;
using paskalON.OperatingModes.Domain.Transitions;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.IntegrationTest.OpenModes.FrequencyActives
{
    /// <summary>
    /// Integration tests of <see cref="ActivePowerFixedMode"/> with the real ramp controller, the stack manager and the system operating mode control.
    /// </summary>
    [TestClass]
    public class ActivePowerFixedModeTest
    {
        /// <summary>
        /// Tolerance for ramp values in Watts.
        /// </summary>
        private const double Tolerance = 1;


        /// <summary>
        /// Fake time provider to advance the ramp.
        /// </summary>
        private FakeTimeProvider? _timeProvider;


        /// <summary>
        /// The operating mode under test.
        /// </summary>
        private ActivePowerFixedMode? _mode;


        /// <summary>
        /// The stack manager.
        /// </summary>
        private StackManager? _stackManager;


        /// <summary>
        /// The system operating mode control.
        /// </summary>
        private SystemOperatingModeControl? _control;


        /// <summary>
        /// Creates a 10 MW plant with 8 MW available power and a 60 second ramp.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 10, 12, 8, 0, 0, TimeSpan.Zero));
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
            ActivePowerFixedModeConfig config = new ActivePowerFixedModeConfig
            {
                ChangedBy = "Test",
                Name = "ActivePowerFixedMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = 10000,
                DeadbandAvailable = 10000,
                RampConfig = new RampTimeConfig { ChangedBy = "Test", RampUpTimeSeconds = 60, RampDownTimeSeconds = 60 },
            };
            ActivePowerFixedModeMap map = new ActivePowerFixedModeMap { AvailableActivePower = () => new ActivePower(8000000), AvailableReactivePower = () => new ReactivePower(4000000) };
            RampController rampController = new RampController(NullLogger.Instance, _timeProvider, config.RampConfig);
            _mode = new ActivePowerFixedMode(NullLogger.Instance, _timeProvider, new Mock<IMetricsPublisher>().Object, systemConfig, config, map, rampController);
            _stackManager = new StackManager();
            _stackManager.InitializeStack(new Dictionary<string, OperatingModeBase> { { _mode.Name, _mode } });
            SystemOperatingModeControlMap systemMap = new SystemOperatingModeControlMap { ActivePowerAtPoi = () => null, ReactivePowerAtPoi = () => null };
            _control = new SystemOperatingModeControl(NullLogger.Instance, systemConfig, _stackManager.Stack, new OperatingModeOverrides(), systemMap, new Mock<IMetricsPublisher>().Object, new Mock<ISetpointsPublisher>().Object);
        }


        /// <summary>
        /// Confirms the actual setpoint ramps to the supervisory setpoint.
        /// </summary>
        [TestMethod]
        public void ActivePowerFixedModeRampsToSupervisorySetpointTest()
        {
            _stackManager!.AddOperatingMode("ActivePowerFixedMode", 1);
            _control!.UpdateSetpoints(new ActivePower(6000000), new ReactivePower(0));

            _control!.Calculate(CancellationToken.None);
            double startActual = _control!.SetpointActivePowerActual.Watts;
            _timeProvider!.Advance(TimeSpan.FromSeconds(30));
            _control!.Calculate(CancellationToken.None);
            double halfActual = _control!.SetpointActivePowerActual.Watts;
            _timeProvider!.Advance(TimeSpan.FromSeconds(30));
            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(0, startActual, Tolerance);
            Assert.AreEqual(3000000, halfActual, Tolerance);
            Assert.AreEqual(6000000, _control!.SetpointActivePowerActual.Watts, Tolerance);
            Assert.AreEqual(OperatingModeState.Enabled, _mode!.State);
            Assert.AreEqual(6000000, _mode!.ActivePowerContribution.Watts, Tolerance);
        }


        /// <summary>
        /// Confirms a supervisory setpoint change while enabled ramps to the new setpoint.
        /// </summary>
        [TestMethod]
        public void ActivePowerFixedModeFollowsSetpointChangeTest()
        {
            RampToEnabled(6000000);
            _control!.UpdateSetpoints(new ActivePower(2000000), new ReactivePower(0));

            _control!.Calculate(CancellationToken.None);
            _timeProvider!.Advance(TimeSpan.FromSeconds(30));
            _control!.Calculate(CancellationToken.None);
            double halfActual = _control!.SetpointActivePowerActual.Watts;
            _timeProvider!.Advance(TimeSpan.FromSeconds(30));
            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(4000000, halfActual, Tolerance);
            Assert.AreEqual(2000000, _control!.SetpointActivePowerActual.Watts, Tolerance);
            Assert.AreEqual(OperatingModeState.Enabled, _mode!.State);
        }


        /// <summary>
        /// Confirms the supervisory setpoint is reduced to the available power.
        /// </summary>
        [TestMethod]
        public void ActivePowerFixedModeLimitedToAvailableTest()
        {
            RampToEnabled(9000000);

            Assert.AreEqual(8000000, _control!.SetpointActivePowerActual.Watts, Tolerance);
            Assert.AreEqual(9000000, _mode!.ActiveControl!.Setpoint.Watts);
            Assert.AreEqual(8000000, _mode!.ActiveControl!.TargetSetpoint.Watts);
        }


        /// <summary>
        /// Confirms removing the only mode deactivates it immediately and the applied target transitions to zero with its ramp.
        /// </summary>
        [TestMethod]
        public void ActivePowerFixedModeRemovedRampsToZeroTest()
        {
            RampToEnabled(6000000);
            _stackManager!.RemoveOperatingMode("ActivePowerFixedMode");

            _control!.Calculate(CancellationToken.None);
            _timeProvider!.Advance(TimeSpan.FromSeconds(30));
            _control!.Calculate(CancellationToken.None);
            double halfActual = _control!.SetpointActivePowerActual.Watts;
            TransitionKind halfKind = _control!.ActivePowerTransition.Kind;
            OperatingModeState halfState = _mode!.State;
            _timeProvider!.Advance(TimeSpan.FromSeconds(30));
            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(3000000, halfActual, Tolerance);
            Assert.AreEqual(TransitionKind.ModeRemoved, halfKind);
            Assert.AreEqual(OperatingModeState.RampingToDisabled, halfState);
            Assert.AreEqual(0, _control!.SetpointActivePowerActual.Watts, Tolerance);
            Assert.AreEqual(OperatingModeState.Disabled, _mode!.State);
            Assert.AreEqual(TransitionKind.None, _control!.ActivePowerTransition.Kind);
        }


        /// <summary>
        /// Adds the mode to the stack and ramps it to the setpoint.
        /// </summary>
        /// <param name="setpoint">The supervisory active power setpoint in Watts.</param>
        private void RampToEnabled(double setpoint)
        {
            _stackManager!.AddOperatingMode("ActivePowerFixedMode", 1);
            _control!.UpdateSetpoints(new ActivePower(setpoint), new ReactivePower(0));
            _control!.Calculate(CancellationToken.None);
            _timeProvider!.Advance(TimeSpan.FromSeconds(60));
            _control!.Calculate(CancellationToken.None);
        }
    }
}

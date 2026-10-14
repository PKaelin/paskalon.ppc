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
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.Ramps;
using paskalON.OperatingModes.Domain.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.OperatingModes.Domain.Systems;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.IntegrationTest.OpenModes.VoltageReactives
{
    /// <summary>
    /// Integration tests of <see cref="ReactivePowerFixedMode"/> with the real ramp controller, the stack manager and the system operating mode control.
    /// </summary>
    [TestClass]
    public class ReactivePowerFixedModeTest
    {
        /// <summary>
        /// Tolerance for ramp values in Vars.
        /// </summary>
        private const double Tolerance = 1;


        /// <summary>
        /// Fake time provider to advance the ramp.
        /// </summary>
        private FakeTimeProvider? _timeProvider;


        /// <summary>
        /// The operating mode under test.
        /// </summary>
        private ReactivePowerFixedMode? _mode;


        /// <summary>
        /// The stack manager.
        /// </summary>
        private StackManager? _stackManager;


        /// <summary>
        /// The system operating mode control.
        /// </summary>
        private SystemOperatingModeControl? _control;


        /// <summary>
        /// Creates a 5 MVar plant with 4 MVar available power and a 20 second ramp.
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
            ReactivePowerFixedModeConfig config = new ReactivePowerFixedModeConfig
            {
                ChangedBy = "Test",
                Name = "ReactivePowerFixedMode",
                IsActive = true,
                Type = PowerControlType.Bess,
                DeadbandSetpoint = 10000,
                DeadbandAvailable = 10000,
                RampConfig = new RampTimeConfig { ChangedBy = "Test", RampUpTimeSeconds = 20, RampDownTimeSeconds = 20 },
            };
            ReactivePowerFixedModeMap map = new ReactivePowerFixedModeMap { AvailableActivePower = () => new ActivePower(8000000), AvailableReactivePower = () => new ReactivePower(-4000000) };
            RampController rampController = new RampController(NullLogger.Instance, _timeProvider, config.RampConfig);
            _mode = new ReactivePowerFixedMode(NullLogger.Instance, _timeProvider, new Mock<IMetricsPublisher>().Object, systemConfig, config, map, rampController);
            _stackManager = new StackManager();
            _stackManager.InitializeStack(new Dictionary<string, OperatingModeBase> { { _mode.Name, _mode } });
            SystemOperatingModeControlMap systemMap = new SystemOperatingModeControlMap { ActivePowerAtPoi = () => null, ReactivePowerAtPoi = () => null };
            _control = new SystemOperatingModeControl(NullLogger.Instance, systemConfig, _stackManager.Stack, new OperatingModeOverrides(), systemMap, new Mock<IMetricsPublisher>().Object);
        }


        /// <summary>
        /// Confirms the actual reactive setpoint ramps to the supervisory setpoint reduced to the available reactive power.
        /// </summary>
        /// <param name="setpoint">The supervisory reactive power setpoint in Vars.</param>
        /// <param name="expectedFinal">The expected final reactive power in Vars.</param>
        [TestMethod]
        [DataRow(-2000000, -2000000)]
        [DataRow(-5000000, -4000000)]
        public void ReactivePowerFixedModeRampsToSupervisorySetpointTest(double setpoint, double expectedFinal)
        {
            _stackManager!.AddOperatingMode("ReactivePowerFixedMode", 1);
            _control!.UpdateSetpoints(new ActivePower(0), new ReactivePower(setpoint));

            _control!.Calculate(CancellationToken.None);
            _timeProvider!.Advance(TimeSpan.FromSeconds(10));
            _control!.Calculate(CancellationToken.None);
            double halfActual = _control!.SetpointReactivePowerActual.VoltAmperesReactive;
            _timeProvider!.Advance(TimeSpan.FromSeconds(10));
            _control!.Calculate(CancellationToken.None);

            Assert.AreEqual(expectedFinal / 2, halfActual, Tolerance);
            Assert.AreEqual(expectedFinal, _control!.SetpointReactivePowerActual.VoltAmperesReactive, Tolerance);
            Assert.AreEqual(0, _control!.SetpointActivePowerActual.Watts);
            Assert.AreEqual(OperatingModeState.Enabled, _mode!.State);
        }
    }
}

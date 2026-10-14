// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.OpenModes
{
    /// <summary>
    /// Mode: Maintenance Mode
    /// Purpose: Takes a unit out of available units and commands P and or Q setpoints
    /// Inputs: Active Power setpoint, Reactive Power setpoint, Available Reactive and Active Power
    /// Output Controlled: Active Power (P) and or Reactive Power (Q)
    /// What Output Influences: Active Power and or Reactive Power
    /// </summary>
    /// <remarks>
    /// The setpoints are set by an operator with <see cref="SetSetpoint"/> and not by the supervisory control.
    /// The calculated targets are read by the system operating mode control from the power controls.
    /// </remarks>
    public class MaintenanceMode : OperatingOpenModeBase, IOverrideMode
    {
        /// <summary>
        /// Lock object for thread safe access to the operator setpoints.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Maintenance mode configuration.
        /// </summary>
        protected readonly MaintenanceModeConfig _config;


        /// <summary>
        /// Maintenance mode map.
        /// </summary>
        protected readonly MaintenanceModeMap _map;


        /// <summary>
        /// DER unit to put in maintenance mode.
        /// </summary>
        /// <remarks>
        /// A DER unit in maintenance mode is no longer included under control of the power control.
        /// Nonetheless it's influence in the input/output of the plant has to be considered.
        /// </remarks>
        public string? TargetDerUnit { get => _config.DerUnitName; }


        /// <summary>
        /// Gets the active power setpoint set by the operator.
        /// </summary>
        public ActivePower SetpointActivePower
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = new ActivePower(0);


        /// <summary>
        /// Gets the reactive power setpoint set by the operator.
        /// </summary>
        public ReactivePower SetpointReactivePower
        {
            get { lock (_dataLock) { return field; } }
            private set { lock (_dataLock) { field = value; } }
        } = new ReactivePower(0);


        /// <summary>
        /// Constructor of <see cref="MaintenanceMode"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public MaintenanceMode(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig, MaintenanceModeConfig config,
            MaintenanceModeMap map, IRampController rampController, ICurveController? curveController = null)
            : base(logger, timeProvider, publisher, systemConfig, config, map, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

            _config = config;
            _map = map;
            RegisterMetrics();
        }


        /// <summary>
        /// Sets the operator setpoints of the overridden unit.
        /// </summary>
        /// <param name="activePower">The active power setpoint.</param>
        /// <param name="reactivePower">The reactive power setpoint.</param>
        /// <exception cref="ArgumentOutOfRangeException">A setpoint is outside the nameplate.</exception>
        public void SetSetpoint(ActivePower activePower, ReactivePower reactivePower)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(activePower.Watts, _systemConfig.NameplateMinimumActivePowerWatt);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(activePower.Watts, _systemConfig.NameplateMaximumActivePowerWatt);
            ArgumentOutOfRangeException.ThrowIfLessThan(reactivePower.VoltAmperesReactive, _systemConfig.NameplateMinimumReactivePowerVars);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(reactivePower.VoltAmperesReactive, _systemConfig.NameplateMaximumReactivePowerVars);

            SetpointActivePower = activePower;
            SetpointReactivePower = reactivePower;
            _logger.LogInformation("{Name} setpoints changed to {SetpointActivePower} and {SetpointReactivePower}.", Name, activePower, reactivePower);
        }


        /// <inheritdoc/>
        public override OperatingModeSignals Calculate(OperatingModeSignals signals)
        {
            ArgumentNullException.ThrowIfNull(signals);

            ActiveControl!.Calculate(SetpointActivePower, _map.AvailableActivePower.Invoke());
            ReactiveControl!.Calculate(SetpointReactivePower, _map.AvailableReactivePower.Invoke());

            return signals;
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Ders;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.Modes.ComplexPower;
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
    public class MaintenanceMode : OperatingOpenModeBase, IOperatingOpenMode, IExclusiveMode
    {
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
        public DerUnit TargetDerUnit { get; init; }


        /// <summary>
        /// Constructor of <see cref="MaintenanceMode"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="targetDerUnit"></param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public MaintenanceMode(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig, MaintenanceModeConfig config, DerUnit targetDerUnit,
            MaintenanceModeMap map, IRampController rampController, ICurveController? curveController = null)
            : base(logger, timeProvider, publisher, systemConfig, config, map, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);
            ArgumentNullException.ThrowIfNull(targetDerUnit);

            TargetDerUnit = targetDerUnit;
            _config = config;
            _map = map;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override Task CalculateAsync(CancellationToken cancellationToken = default)
        {
            if (State != OperatingModeState.Disabled)
            {
                // Calculate active power
                ActivePower? availableActive = _map.AvailableActivePower?.Invoke();
                double? targetActive = CheckNewActiveTargetSetpoint(availableActive);

                if (targetActive != null)
                {
                    // Apply configured limits if configured
                    targetActive = ApplyActiveLimits(targetActive.Value);
                    _logger.LogInformation("Operating mode changed due setpoint or available change: {Name}. Active Target-Setpoint set to {ActiveTargetSetpoint}", Name, targetActive.Value);
                    RampControllerActive.Start(TargetActivePower.KiloWatts, targetActive.Value);
                }

                _targetActivePower.KiloWatts = RampControllerActive.Calculate();
                CheckFinalActiveTarget();

                // Calculate reactive power
                ReactivePower? availableReactive = _map.AvailableReactivePower?.Invoke();
                double? targetReactive = CheckNewReactiveTargetSetpoint(availableReactive);

                if (targetReactive != null)
                {
                    // Apply configured limits if configured
                    targetReactive = ApplyReactiveLimits(targetReactive.Value);
                    _logger.LogInformation("Operating mode changed due setpoint or available change: {Name}. Reactive Target-Setpoint set to {ReactiveTargetSetpoint}", Name, targetReactive.Value);
                    RampControllerReactive.Start(TargetReactivePower.KiloVoltAmperesReactive, targetReactive.Value);
                }

                _targetReactivePower.KiloVoltAmperesReactive = RampControllerReactive.Calculate();
                CheckFinalReactiveTarget();
            }

            return Task.CompletedTask;
        }



    }
}

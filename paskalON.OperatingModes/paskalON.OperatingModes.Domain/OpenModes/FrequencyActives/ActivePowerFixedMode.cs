// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.OpenModes.FrequencyActives
{
    /// <summary>
    /// Mode: Active Power Fixed Mode
    /// Purpose: Set a fixed setpoint without feedback signal
    /// Inputs: Active Power setpoint, Available active power
    /// Output Controlled: Active Power (P)
    /// What Output Influences: Active Power
    /// </summary>
    public class ActivePowerFixedMode : OperatingOpenModeBase
    {
        /// <summary>
        /// Active power fixed mode configuration.
        /// </summary>
        protected readonly ActivePowerFixedModeConfig _config;


        /// <summary>
        /// Active power fixed mode map.
        /// </summary>
        protected readonly ActivePowerFixedModeMap _map;


        /// <summary>
        /// Constructor of <see cref="ActivePowerFixedMode"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public ActivePowerFixedMode(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, ActivePowerFixedModeConfig config,
            ActivePowerFixedModeMap map, IRampController rampController, ICurveController? curveController = null)
            : base(logger, timeProvider, systemConfig, config, map, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

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
                ActivePower? available = _map.AvailableActivePower?.Invoke();
                double? target = CheckNewActiveTarget(available);

                if (target != null)
                {
                    // Restart/Start when setpoint wasn't set before or setpoint was 0 before and now it is not 0 anymore
                    // Don't restart when if available gets bigger but still bigger than setpoint and setpoint hasn't changed
                    if (target.Value != 0 && (_lastSetpointActive.HasValue == false || _lastSetpointActive.Value.Watts == 0) &&
                       (target != _lastSetpointActive?.KiloWatts))
                    {
                        // In case things have changed after the Enable() command
                        if (State == OperatingModeState.Enabling)
                        {
                            State = OperatingModeState.RampingToEnabled;
                        }

                        // Apply configured limits if configured
                        target = ApplyLimits(target.Value);

                        _logger.LogInformation("Operating mode changed due setpoint or available change: {Name}. Target set to {Setpoint}", Name, target.Value);
                        RampControllerActive.Start(TargetActivePower.KiloWatts, target.Value);
                    }

                    // Only now update the last available and the last setpoint
                    _lastAvailableActive = available;
                    _lastSetpointActive = SetpointActivePower;
                }

                _targetActivePower.KiloWatts = RampControllerActive.Calculate();
                CheckFinalActiveTarget();
            }

            return Task.CompletedTask;
        }


        /// <summary>
        /// Apply configured limits to targets if configured.
        /// </summary>
        /// <param name="target">The intended target.</param>
        /// <returns>Target or limited target.</returns>
        private double ApplyLimits(double target)
        {
            if ((_config.MaximumActivePowerLimitKiloWatt.HasValue == true) && (target > _config.MaximumActivePowerLimitKiloWatt.Value))
            {
                _logger.LogInformation("{Name} operating mode limited due MaximumActivePowerLimitKiloWatt configuration. Setpoint set to {Setpoint}", Name, target);
                target = _config.MaximumActivePowerLimitKiloWatt.Value;
            }
            else if ((_config.MinimumActivePowerLimitKiloWatt.HasValue == true) && (target < _config.MinimumActivePowerLimitKiloWatt.Value))
            {
                _logger.LogInformation("{Name} operating mode limited due MinimumActivePowerLimitKiloWatt configuration. Setpoint set to {Setpoint}", Name, target);
                target = _config.MinimumActivePowerLimitKiloWatt.Value;
            }

            return target;
        }
    }
}
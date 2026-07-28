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
        /// <param name="config">The operating mode base configuration.</param>
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
                // Restart ramp controller if available is outside deadband of lastAvailable or if setpoint is outside deadband of lastSetpoint
                if (State != OperatingModeState.RampingToDisabled && (available != null && _lastAvailableActive != null && _lastSetpointActive != null) &&
                   ((Math.Abs(available.Value.KiloWatts) > (Math.Abs(_lastAvailableActive.Value.KiloWatts) + _config.DeadbandAvailableKilo)) ||
                   (Math.Abs(SetpointActivePower.KiloWatts) > (Math.Abs(_lastSetpointActive.Value.KiloWatts) + _config.DeadbandSetpointKilo))))
                {
                    double target = 0;
                    // Available is less then setpoint use available so that we dont set an unachievable setpoint.
                    if (Math.Abs(available.Value.KiloWatts) <= Math.Abs(SetpointActivePower.KiloWatts))
                    {
                        target = available.Value.KiloWatts;
                    }
                    // Available is more then setpoint use setpoint
                    else
                    {
                        target = SetpointActivePower.KiloWatts;
                    }

                    // Apply configured limits if configured
                    target = ApplyLimits(target);

                    if (target != 0 && (_lastSetpointActive.HasValue == false || _lastSetpointActive.Value.Watts == 0))
                    {
                        // In case things have changed after the Enable() command
                        if (State == OperatingModeState.Enabling)
                        {
                            State = OperatingModeState.RampingToEnabled;
                        }

                        _logger.LogInformation("Operating mode changed due setpoint or available change: {Name}. Target set to {Setpoint}", Name, target);
                        RampControllerActive.Start(TargetActivePower.KiloWatts, target);
                    }

                    _lastAvailableActive = available;
                    _lastSetpointActive = SetpointActivePower;
                }

                _targetActivePower.KiloWatts = RampControllerActive.Calculate();

                // Set final target and change state to enabled if we are within a deadband
                if (Math.Abs(TargetActivePower.KiloWatts) > (Math.Abs(SetpointActivePower.KiloWatts) - _config.DeadbandSetpointKilo))
                {
                    if (State == OperatingModeState.RampingToEnabled)
                    {
                        // Once within deadband we set the actual the precise target regardless available and set state to enabled 
                        _targetActivePower.KiloWatts = SetpointActivePower.KiloWatts;
                        State = OperatingModeState.Enabled;
                    }
                    else if (State == OperatingModeState.RampingToDisabled)
                    {
                        // Once within deadband we set the actual the precise target regardless available and set state to disabled
                        _targetActivePower.KiloWatts = SetpointActivePower.KiloWatts;
                        State = OperatingModeState.Disabled;
                        RampControllerActive.Stop();
                    }
                }
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
            if (_config.MaximumActivePowerLimitKiloWatt.HasValue == true)
            {
                if (target > _config.MaximumActivePowerLimitKiloWatt.Value)
                {
                    _logger.LogInformation("{Name} operating mode limited due MaximumActivePowerLimitKiloWatt configuration. Setpoint set to {Setpoint}", Name, target);
                    target = _config.MaximumActivePowerLimitKiloWatt.Value;
                }
            }
            else if (_config.MinimumActivePowerLimitKiloWatt.HasValue == true)
            {
                if (target < _config.MinimumActivePowerLimitKiloWatt.Value)
                {
                    _logger.LogInformation("{Name} operating mode limited due MinimumActivePowerLimitKiloWatt configuration. Setpoint set to {Setpoint}", Name, target);
                    target = _config.MinimumActivePowerLimitKiloWatt.Value;
                }
            }

            return target;
        }
    }
}

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
    public class ActivePowerFixedMode : OperatingOpenModeBase
    {
        protected readonly ActivePowerFixedModeConfig _config;
        protected readonly ActivePowerFixedModeMap _map;


        public ActivePowerFixedMode(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, ActivePowerFixedModeConfig config,
            ActivePowerFixedModeMap map, IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, systemConfig, config, map, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

            _config = config;
            _map = map;
        }




        public override Task CalculateAsync(CancellationToken cancellationToken = default)
        {
            ActivePower? available = _map.AvailableActivePower?.Invoke();

            // Restart ramp controller if available is outside deadband of lastAvailable or if setpoint is outside deadband of lastSetpoint
            if ((available != null && _lastAvailableActive != null && _lastSetpointActive != null) &&
                 ((_lastAvailableActive.Value.KiloWatts - _config.DeadbandAvailableKilo <= available.Value.KiloWatts &&
                 _lastAvailableActive.Value.KiloWatts + _config.DeadbandAvailableKilo >= available.Value.KiloWatts) ||
                 (_lastSetpointActive.Value.KiloWatts - _config.DeadbandSetpointKilo <= SetpointActivePower.KiloWatts &&
                 _lastSetpointActive.Value.KiloWatts + _config.DeadbandSetpointKilo >= SetpointActivePower.KiloWatts)))
            {
                double setpoint = 0;
                _lastAvailableActive = available;
                _lastSetpointActive = SetpointActivePower;
                // Available is less then setpoint use available so that we dont set an unachievable setpoint.
                if (_lastAvailableActive.Value.KiloWatts <= SetpointActivePower.KiloWatts)
                {
                    setpoint = _lastAvailableActive.Value.KiloWatts;
                }
                // Available is more then setpoint use setpoint
                else
                {
                    setpoint = SetpointActivePower.KiloWatts;
                }

                _logger.LogInformation("Operating mode changed due setpoint or available change: {Name}. Target set to {Target}", Name, setpoint);
                RampController.Start(TargetActivePower.KiloWatts, setpoint);
            }

            _targetActivePower.KiloWatts = RampController.Calculate();

            if (SetpointActivePower.KiloWatts - _config.DeadbandSetpointKilo >= TargetActivePower.KiloWatts &&
                SetpointActivePower.KiloWatts + _config.DeadbandSetpointKilo <= TargetActivePower.KiloWatts)
            {
                if (State == OperatingModeState.RampingToEnabled)
                {
                    // Once within deadband we set the actual the precise target
                    _targetActivePower.KiloWatts = SetpointActivePower.KiloWatts;
                    State = OperatingModeState.Enabled;
                }
                else if (State == OperatingModeState.RampingToDisabled)
                {
                    // Once within deadband we set the actual the precise target
                    _targetActivePower.KiloWatts = SetpointActivePower.KiloWatts;
                    State = OperatingModeState.Disabled;
                    RampController.Stop();
                }
            }

            return Task.CompletedTask;
        }

    }
}

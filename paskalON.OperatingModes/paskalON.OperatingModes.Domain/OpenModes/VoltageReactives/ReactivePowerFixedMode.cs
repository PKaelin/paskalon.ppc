// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.OpenModes.VoltageReactives
{
    /// <summary>
    /// Mode: Reactive Power Fixed Mode
    /// Purpose: Set a fixed setpoint without feedback signal
    /// Inputs: Reactive Power setpoint, Available reactive power
    /// Output Controlled: Reactive Power (Q)
    /// What Output Influences: Reactive Power
    /// </summary>
    public class ReactivePowerFixedMode : OperatingOpenModeBase
    {
        /// <summary>
        /// Reactive power fixed mode configuration.
        /// </summary>
        protected readonly ReactivePowerFixedModeConfig _config;


        /// <summary>
        /// Active power fixed mode map.
        /// </summary>
        protected readonly ReactivePowerFixedModeMap _map;


        /// <summary>
        /// Constructor of <see cref="ReactivePowerFixedMode"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public ReactivePowerFixedMode(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, ReactivePowerFixedModeConfig config,
            ReactivePowerFixedModeMap map, IRampController rampController, ICurveController? curveController = null)
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
                ReactivePower? available = _map.AvailableReactivePower?.Invoke();
                double? target = CheckNewReactiveTarget(available);

                if (target != null)
                {
                    // Restart/Start when setpoint wasn't set before or setpoint was 0 before and now it is not 0 anymore
                    // Don't restart when if available gets bigger but still bigger than setpoint and setpoint hasn't changed
                    if (target.Value != 0 && (_lastSetpointReactive.HasValue == false || _lastSetpointReactive.Value.VoltAmperesReactive == 0) &&
                       (target != _lastSetpointReactive?.KiloVoltAmperesReactive))
                    {
                        // In case things have changed after the Enable() command
                        if (State == OperatingModeState.Enabling)
                        {
                            State = OperatingModeState.RampingToEnabled;
                        }

                        // Apply configured limits if configured
                        target = ApplyLimits(target.Value);

                        _logger.LogInformation("Operating mode changed due setpoint or available change: {Name}. Target set to {Setpoint}", Name, target.Value);
                        RampControllerReactive.Start(TargetReactivePower.KiloVoltAmperesReactive, target.Value);
                    }

                    // Only now update the last available and the last setpoint
                    _lastAvailableReactive = available;
                    _lastSetpointReactive = SetpointReactivePower;
                }

                _targetReactivePower.KiloVoltAmperesReactive = RampControllerReactive.Calculate();
                CheckFinalReactiveTarget();
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
            if ((_config.MaximumReactivePowerLimitKiloVars.HasValue == true) && (target > _config.MaximumReactivePowerLimitKiloVars.Value))
            {
                _logger.LogInformation("{Name} operating mode limited due MaximumReactivePowerLimitKiloVars configuration. Setpoint set to {Setpoint}", Name, target);
                target = _config.MaximumReactivePowerLimitKiloVars.Value;
            }
            else if ((_config.MinimumReactivePowerLimitKiloVars.HasValue == true) && (target < _config.MinimumReactivePowerLimitKiloVars.Value))
            {
                _logger.LogInformation("{Name} operating mode limited due MinimumReactivePowerLimitKiloVars configuration. Setpoint set to {Setpoint}", Name, target);
                target = _config.MinimumReactivePowerLimitKiloVars.Value;

            }

            return target;
        }
    }
}
// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.ClosedModes.VoltageReactives
{
    /// <summary>
    /// Mode: Reactive Power Mode
    /// Purpose: Fixed Volt-Ampere Reactive setpoint
    /// Inputs: Reactive Power setpoint, Available Reactive Power, Measured Reactive Power at POI
    /// Output Controlled: Reactive Power (Q)
    /// What Output Influences: Reactive Power, Grid voltage
    /// </summary>
    public class ReactivePowerMode : OperatingClosedModeBase
    {
        /// <summary>
        /// Last reactive power at point of interconnection (POI).
        /// </summary>
        private double? _lastReactivePowerAtPoi;


        /// <summary>
        /// Reactive power mode configuration.
        /// </summary>
        protected readonly ReactivePowerModeConfig _config;


        /// <summary>
        /// Reactive power mode map.
        /// </summary>
        protected readonly ReactivePowerModeMap _map;


        /// <summary>
        /// Constructor of <see cref="ReactivePowerMode"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>

        public ReactivePowerMode(ILogger logger, TimeProvider timeProvider, SystemConfig systemConfig, ReactivePowerModeConfig config,
            ReactivePowerModeMap map, IRampController rampController, ICurveController? curveController = null)
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
                double? target = CheckNewReactiveTargetSetpoint(available);

                if (target != null)
                {
                    // Apply configured limits if configured
                    target = ApplyReactiveLimits(target.Value);
                    _logger.LogInformation("Operating mode changed due setpoint or available change: {Name}. Reactive Target-Setpoint set to {ReactiveTargetSetpoint}", Name, target.Value);
                    RampControllerReactive.Start(TargetReactivePower.KiloVoltAmperesReactive, target.Value);
                }

                // Calculate the error between the current target and the measured feedback
                double powerAtPoi = _map.ReactivePowerAtPoi?.Invoke()?.VoltAmperesReactive ?? 0;
                // Don't try to fix minor noise. Set the error adjustment within this statement
                if (Math.Abs(TargetReactivePower.VoltAmperesReactive - powerAtPoi) < _config.DeadbandErrorKilo * 1000)
                {
                    _errorAdjustmentReactive.VoltAmperesReactive = 0;
                }
                else
                {
                    // Do not increase if last reactive power at POI is stuck somehow
                    if (_lastReactivePowerAtPoi.HasValue == false || _lastReactivePowerAtPoi.Value != powerAtPoi)
                    {
                        // Apply proportional gain to the error to calculate the adjustment for the next iteration
                        _errorAdjustmentReactive.KiloVoltAmperesReactive = (TargetReactivePower.VoltAmperesReactive - powerAtPoi) * _config.ProportionalGain / 1000;
                    }
                }

                // Set last reactive power
                _lastReactivePowerAtPoi = powerAtPoi;
                // Include the error into the next iteration but don't exceed the configured limits
                _targetReactivePower.KiloVoltAmperesReactive = ApplyReactiveLimits(RampControllerReactive.Calculate() + _errorAdjustmentReactive.KiloVoltAmperesReactive);
                CheckFinalReactiveTarget();
            }

            return Task.CompletedTask;
        }
    }
}

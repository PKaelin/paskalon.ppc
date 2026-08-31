// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.ClosedModes.FrequencyActives
{
    /// <summary>
    /// Mode: Active Power Mode
    /// Purpose: Fixed Watt setpoint
    /// Inputs: Active Power setpoint, Available Active Power, Measured Active Power at POI
    /// Output Controlled: Active Power (P)
    /// What Output Influences: Active Power, Grid frequency, Power Balance
    /// </summary>
    public class ActivePowerMode : OperatingClosedModeBase
    {
        /// <summary>
        /// Last active power at point of interconnection (POI).
        /// </summary>
        private double? _lastActivePowerAtPoi;


        /// <summary>
        /// Active power mode configuration.
        /// </summary>
        protected readonly ActivePowerModeConfig _config;


        /// <summary>
        /// Active power mode map.
        /// </summary>
        protected readonly ActivePowerModeMap _map;


        /// <summary>
        /// Constructor of <see cref="ActivePowerMode"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public ActivePowerMode(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig, ActivePowerModeConfig config,
            ActivePowerModeMap map, IRampController rampController, ICurveController? curveController = null)
            : base(logger, timeProvider, publisher, systemConfig, config, map, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

            _config = config;
            _map = map;
            StateActive = OperatingModeState.Disabled;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override Task CalculateAsync(CancellationToken cancellationToken = default)
        {
            if (StateActive != OperatingModeState.Disabled)
            {
                ActivePower? available = _map.AvailableActivePower?.Invoke();
                double? target = CheckNewActiveTargetSetpoint(available);

                if (target != null)
                {
                    // Apply configured limits if configured
                    target = ApplyActiveLimits(target.Value);
                    _logger.LogInformation("Operating mode changed due setpoint or available change: {Name}. Active Target-Setpoint set to {ActiveTargetSetpoint}", Name, target.Value);
                    RampControllerActive.Start(TargetActivePower.KiloWatts, target.Value);
                }

                // Calculate the error between the current target and the measured feedback
                double powerAtPoi = _map.ActivePowerAtPoi?.Invoke()?.Watts ?? 0;

                // Don't try to fix minor noise. Set the error adjustment within this statement
                if (Math.Abs(TargetActivePower.Watts - powerAtPoi) < _config.DeadbandErrorKilo * 1000)
                {
                    _errorAdjustmentActive.Watts = 0;
                }
                else
                {
                    // Do not increase if last active power at POI is stuck somehow
                    if (_lastActivePowerAtPoi.HasValue == false || _lastActivePowerAtPoi.Value != powerAtPoi)
                    {
                        // Apply proportional gain to the error to calculate the adjustment for the next iteration
                        _errorAdjustmentActive.KiloWatts = (TargetActivePower.Watts - powerAtPoi) * _config.ProportionalGain / 1000;
                    }
                }

                // Set last active power
                _lastActivePowerAtPoi = powerAtPoi;
                // Include the error into the next iteration but don't exceed the configured limits
                _targetActivePower.KiloWatts = ApplyActiveLimits(RampControllerActive.Calculate() + _errorAdjustmentActive.KiloWatts);
                CheckFinalActiveTarget();
            }

            return Task.CompletedTask;
        }
    }
}

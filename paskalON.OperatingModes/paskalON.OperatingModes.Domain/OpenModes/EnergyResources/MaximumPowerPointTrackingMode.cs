// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.OpenModes.EnergyResources
{
    /// <summary>
    /// Mode: Maximum Power Point Tracking Mode (MPPT) 
    /// Purpose: Maximize energy yield by continuously adjusting the inverter's input electrical characteristics
    /// Inputs: Active Power setpoint, Available Active Power
    /// Output Controlled: Active Power (P)
    /// What Output Influences: Active Power
    /// </summary>
    public class MaximumPowerPointTrackingMode : OperatingOpenModeBase
    {
        /// <summary>
        /// Maximum power point tracking mode configuration.
        /// </summary>
        protected readonly MaximumPowerPointTrackingModeConfig _config;


        /// <summary>
        /// Maximum power point tracking mode map.
        /// </summary>
        protected readonly MaximumPowerPointTrackingModeMap _map;


        /// <summary>
        /// Constructor of <see cref="MaximumPowerPointTrackingMode"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public MaximumPowerPointTrackingMode(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig, MaximumPowerPointTrackingModeConfig config,
            MaximumPowerPointTrackingModeMap map, IRampController rampController, ICurveController? curveController = null)
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

                _targetActivePower.KiloWatts = RampControllerActive.Calculate();
                CheckFinalActiveTarget();
            }

            return Task.CompletedTask;
        }
    }
}

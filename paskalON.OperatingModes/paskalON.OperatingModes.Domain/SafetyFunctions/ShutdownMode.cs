// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.SafetyFunctions;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Domain.SafetyFunctions
{
    /// <summary>
    /// Mode: (Graceful) Shutdown Mode
    /// Purpose: Gracefully shutdown the plant in specific situation.
    /// Inputs: Reactive Power setpoint = 0, Available Reactive Power = 0
    /// Output Controlled: Active Power (P) and Reactive Power (Q)
    /// What Output Influences: Active Power and Reactive Power
    /// </summary>
    public class ShutdownMode : OperatingOpenModeBase
    {
        /// <summary>
        /// Shutdown mode configuration.
        /// </summary>
        private readonly ShutdownModeConfig _config;


        /// <summary>
        /// Constructor of <see cref="ShutdownMode"/>.
        /// </summary>
        /// <param name="logger">Logger for handling logging and diagnostics.</param>
        /// <param name="timeProvider">The time provider (TimeProvider.System for prod, FakeTimeProvider for tests.</param>
        /// <param name="publisher">The metrics publisher interface.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="map">Input mapping class for signals.</param>
        /// <param name="rampController">The ramp controller interface.</param>
        /// <param name="curveController">The curve controller interface.</param>
        public ShutdownMode(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig, ShutdownModeConfig config,
            OperatingModeBaseMap map, IRampController rampController, ICurveController? curveController)
            : base(logger, timeProvider, publisher, systemConfig, config, map, rampController, curveController)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override Task CalculateAsync(CancellationToken cancellationToken = default)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }
    }
}

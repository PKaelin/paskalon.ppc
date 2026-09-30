// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Telemetry;
using paskalON.Telemetry.Factories;

namespace paskalON.OperatingModes.Application
{
    /// <summary>
    /// Operating mode manager responsible for managing systems operating modes and configurations.
    /// </summary>
    public class OperatingModeManager : IOperatingModeManager
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<OperatingModeManager> _logger;


        /// <summary>
        /// Metrics publisher factory to create metrics publishers for telemetry.
        /// </summary>
        private readonly IMetricsPublisherFactory _metricsPublisherFactory;


        /// <inheritdoc/>
        public ICollection<IMetricsPublisher> MetricsPublishers { get; } = new List<IMetricsPublisher>();


        public OperatingModeManager(ILogger<OperatingModeManager> logger, IMetricsPublisherFactory metricsPublisherFactory)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(metricsPublisherFactory);

            _logger = logger;
            _metricsPublisherFactory = metricsPublisherFactory;
        }
    }
}

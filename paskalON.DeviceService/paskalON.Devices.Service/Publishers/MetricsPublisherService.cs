// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Telemetry;

namespace paskalON.Devices.Service.Publishers
{
    /// <summary>
    /// Metrics publisher service that publishes metrics registered via metrics publisher.
    /// </summary>
    public class MetricsPublisherService : BackgroundService
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// List of metric publishers.
        /// </summary>
        private readonly IEnumerable<IMetricsPublisher> _metricsPublishers;


        /// <summary>
        /// Time based interval for metrics publishers.
        /// </summary>
        /// <remarks>
        /// Time based interval means that the publisher is called periodically with this time interval.
        /// </remarks>
        private readonly int _intervalMilliseconds;


        /// <summary>
        /// Constructor of <see cref="MetricsPublisherService"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="metricsPublishers">List of metric publishers.</param>
        /// <param name="intervalMilliseconds">Time based interval for metrics publishers.</param>
        public MetricsPublisherService(ILogger<MetricsPublisherService> logger, IEnumerable<IMetricsPublisher> metricsPublishers, int intervalMilliseconds)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(metricsPublishers);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(intervalMilliseconds);

            _logger = logger;
            _metricsPublishers = metricsPublishers;
            _intervalMilliseconds = intervalMilliseconds;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int interval = 0;
            using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(_intervalMilliseconds));

            ParallelOptions options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 10,
                CancellationToken = stoppingToken
            };

            // awaits make sure that no overload should occur
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                if (++interval == int.MaxValue)
                {
                    interval = 1;
                }

                try
                {
                    await Parallel.ForEachAsync(_metricsPublishers,
                        options, (pub, token) => { pub.Publish(interval); return ValueTask.CompletedTask; });
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unexpected error while publishing metrics data. {Error}", ex);
                }
            }
        }
    }
}

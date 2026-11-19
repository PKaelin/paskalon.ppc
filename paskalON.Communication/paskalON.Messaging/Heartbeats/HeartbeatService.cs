// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace paskalON.Messaging.Heartbeats
{
    /// <summary>
    /// Hosted service that runs the <see cref="HeartbeatPublisher"/> of the service.
    /// </summary>
    /// <remarks>
    /// Without <see cref="Initialize(HeartbeatPublisher)"/>, e.g. because no heartbeat topic is configured, no heartbeat is published.
    /// </remarks>
    public sealed class HeartbeatService : BackgroundService
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<HeartbeatService> _logger;


        /// <summary>
        /// The heartbeat publisher or null if not initialized.
        /// </summary>
        private HeartbeatPublisher? _publisher;


        /// <summary>
        /// Constructor of <see cref="HeartbeatService"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        public HeartbeatService(ILogger<HeartbeatService> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }


        /// <summary>
        /// Initializes the service with the heartbeat publisher to run.
        /// </summary>
        /// <param name="publisher">The heartbeat publisher.</param>
        public void Initialize(HeartbeatPublisher publisher)
        {
            ArgumentNullException.ThrowIfNull(publisher);

            _publisher = publisher;
        }


        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_publisher is null)
            {
                _logger.LogWarning("Heartbeat service is not initialized, no heartbeat is published.");

                return;
            }

            await _publisher.RunAsync(stoppingToken);
        }
    }
}

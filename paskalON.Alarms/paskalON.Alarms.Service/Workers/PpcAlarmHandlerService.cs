// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Application.Handlers;
using paskalON.Alarms.Application.Subscribers;

namespace paskalON.Alarms.Service.Workers
{
    /// <summary>
    /// Background service that evaluates the received published data with the alarm handler.
    /// </summary>
    /// <remarks>
    /// - The data is evaluated one after another in receive order, so that the data of the same publisher is never
    ///   evaluated concurrently.
    /// - Each evaluation runs in its own scope, so that every evaluation uses a new database context.
    /// - A failed evaluation (e.g. database not available) is logged and the next data is evaluated. The handler removed
    ///   the values of the publisher from the stateless store, so the next data of the publisher is evaluated completely again.
    /// </remarks>
    public class PpcAlarmHandlerService : BackgroundService
    {
        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILogger<PpcAlarmHandlerService> _logger;


        /// <summary>
        /// Queue of the received data.
        /// </summary>
        private readonly IPpcAlarmMessageQueue _queue;


        /// <summary>
        /// Factory of the scope of each evaluation.
        /// </summary>
        private readonly IServiceScopeFactory _scopeFactory;


        /// <summary>
        /// Constructor of <see cref="PpcAlarmHandlerService"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="queue">Queue of the received data.</param>
        /// <param name="scopeFactory">Factory of the scope of each evaluation.</param>
        public PpcAlarmHandlerService(ILogger<PpcAlarmHandlerService> logger, IPpcAlarmMessageQueue queue, IServiceScopeFactory scopeFactory)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(queue);
            ArgumentNullException.ThrowIfNull(scopeFactory);

            _logger = logger;
            _queue = queue;
            _scopeFactory = scopeFactory;
        }


        /// <summary>
        /// Evaluates one received data with the alarm handler.
        /// </summary>
        /// <param name="message">Received data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the data was evaluated, false if the evaluation failed.</returns>
        public async Task<bool> HandleAsync(PpcAlarmMessage message, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(message);

            try
            {
                await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
                IPpcAlarmHandler handler = scope.ServiceProvider.GetRequiredService<IPpcAlarmHandler>();
                await handler.HandleAsync(message.Key, message.Data, message.ReceivedOn, cancellationToken);

                return true;
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogError(exception, "Evaluating the alarms of {PpcAlarmType} device {DeviceId} failed.",
                    message.Key.PpcAlarmType, message.Key.DeviceId);

                return false;
            }
        }


        /// <inheritdoc/>
        /// <remarks>
        /// ExecuteAsync is called only after the application starts running (app.Run).
        /// Data received before is queued and evaluated once the service runs.
        /// </remarks>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await foreach (PpcAlarmMessage message in _queue.ReadAllAsync(stoppingToken))
                {
                    await HandleAsync(message, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Expected when normal shutdown.
            }
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Application.Outboxes;

namespace paskalON.Alarms.Service.Publishers
{
    /// <summary>
    /// Background service that sends the pending alarm actions of the outbox to the message broker.
    /// </summary>
    /// <remarks>
    /// - New actions are sent immediately: the handler signals the saved actions and the service sends them right away.
    ///   Some actions are time critical (e.g. ReducePower) and must not wait for an interval.
    /// - The send interval is the safety net: without a signal the pending actions are sent at the latest after the interval.
    /// - A failed send (e.g. broker or database not available) is retried after the retry interval, signals are ignored
    ///   meanwhile so that an outage does not cause a retry storm. The actions stay pending in the outbox and are sent in order.
    /// - Only this service sends, so that the actions keep their order and the publisher is not used concurrently.
    /// - Each send runs in its own scope, so that every send uses a new database context.
    /// - The service must be initialized with the intervals of the system configuration before the application runs.
    /// </remarks>
    public class AlarmActionOutboxPublisherService : BackgroundService
    {
        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILogger<AlarmActionOutboxPublisherService> _logger;


        /// <summary>
        /// Factory of the scope of each send.
        /// </summary>
        private readonly IServiceScopeFactory _scopeFactory;


        /// <summary>
        /// Signal of new saved alarm actions.
        /// </summary>
        private readonly IAlarmActionOutboxSignal _outboxSignal;


        /// <summary>
        /// Time provider of the retry wait.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// Maximum wait between two sends without a signal.
        /// </summary>
        private TimeSpan _sendInterval;


        /// <summary>
        /// Wait before a failed send is retried.
        /// </summary>
        private TimeSpan _retryInterval;


        /// <summary>
        /// Constructor of <see cref="AlarmActionOutboxPublisherService"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="scopeFactory">Factory of the scope of each send.</param>
        /// <param name="outboxSignal">Signal of new saved alarm actions.</param>
        /// <param name="timeProvider">Time provider of the retry wait.</param>
        public AlarmActionOutboxPublisherService(ILogger<AlarmActionOutboxPublisherService> logger, IServiceScopeFactory scopeFactory,
            IAlarmActionOutboxSignal outboxSignal, TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(scopeFactory);
            ArgumentNullException.ThrowIfNull(outboxSignal);
            ArgumentNullException.ThrowIfNull(timeProvider);

            _logger = logger;
            _scopeFactory = scopeFactory;
            _outboxSignal = outboxSignal;
            _timeProvider = timeProvider;
        }


        /// <summary>
        /// Initializes the service with the configured intervals.
        /// </summary>
        /// <param name="sendIntervalMilliseconds">Maximum wait, in milliseconds, between two sends without a signal.</param>
        /// <param name="retryIntervalMilliseconds">Wait, in milliseconds, before a failed send is retried.</param>
        public void Initialize(int sendIntervalMilliseconds, int retryIntervalMilliseconds)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sendIntervalMilliseconds);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(retryIntervalMilliseconds);

            _logger.LogInformation("Initializing alarm action outbox publisher service with {SendInterval}ms send and {RetryInterval}ms retry interval.",
                sendIntervalMilliseconds, retryIntervalMilliseconds);
            _sendInterval = TimeSpan.FromMilliseconds(sendIntervalMilliseconds);
            _retryInterval = TimeSpan.FromMilliseconds(retryIntervalMilliseconds);
        }


        /// <summary>
        /// Sends the pending alarm actions once.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>When the pending alarm actions are sent next.</returns>
        public async Task<AlarmActionOutboxNextSend> SendPendingAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfNotInitialized();

            try
            {
                await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
                IAlarmActionOutboxSender sender = scope.ServiceProvider.GetRequiredService<IAlarmActionOutboxSender>();
                AlarmActionOutboxSendResult result = await sender.SendPendingAsync(cancellationToken);

                if (result.IsFailed)
                {
                    return AlarmActionOutboxNextSend.AfterRetryInterval;
                }

                if (result.HasMore)
                {
                    return AlarmActionOutboxNextSend.Immediately;
                }

                return AlarmActionOutboxNextSend.OnSignalOrSendInterval;
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                // E.g. the database is not reachable, the actions stay pending and are sent later
                _logger.LogError(exception, "Unexpected error while sending the alarm actions.");

                return AlarmActionOutboxNextSend.AfterRetryInterval;
            }
        }


        /// <summary>
        /// Waits until the pending alarm actions are sent next.
        /// </summary>
        /// <param name="nextSend">When the pending alarm actions are sent next.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task that completes when the next send is due.</returns>
        public async Task WaitForNextSendAsync(AlarmActionOutboxNextSend nextSend, CancellationToken cancellationToken = default)
        {
            ThrowIfNotInitialized();

            switch (nextSend)
            {
                case AlarmActionOutboxNextSend.Immediately:
                    break;
                case AlarmActionOutboxNextSend.OnSignalOrSendInterval:
                    await _outboxSignal.WaitAsync(_sendInterval, cancellationToken);
                    break;
                case AlarmActionOutboxNextSend.AfterRetryInterval:
                    await Task.Delay(_retryInterval, _timeProvider, cancellationToken);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(nextSend), nextSend, "Unknown next send.");
            }
        }


        /// <inheritdoc/>
        /// <remarks>
        /// ExecuteAsync is called only after the application starts running (app.Run).
        /// The first send sends the actions that are pending since the last run of the service.
        /// </remarks>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ThrowIfNotInitialized();

            try
            {
                while (stoppingToken.IsCancellationRequested is false)
                {
                    AlarmActionOutboxNextSend nextSend = await SendPendingAsync(stoppingToken);
                    await WaitForNextSendAsync(nextSend, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Expected when normal shutdown.
            }
        }


        /// <summary>
        /// Throws when the service has not been initialized.
        /// </summary>
        private void ThrowIfNotInitialized()
        {
            if (_sendInterval == TimeSpan.Zero)
            {
                throw new InvalidOperationException("The alarm action outbox publisher service has not been initialized. Call Initialize before starting it.");
            }
        }
    }
}

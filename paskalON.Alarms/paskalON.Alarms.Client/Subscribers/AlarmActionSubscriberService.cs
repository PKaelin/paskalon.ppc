// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using paskalON.Alarms.Dto.Actions;
using paskalON.Messaging.RabbitMq;

namespace paskalON.Alarms.Client.Subscribers
{
    /// <summary>
    /// Subscribes to the alarm actions of the alarm service and passes them to the alarm action handler.
    /// </summary>
    /// <remarks>
    /// - Every instance of the recipient service has its own queue and receives every action (see <see cref="RabbitMqSubscriber"/>).
    /// - The subscription is retried until RabbitMQ is reachable.
    /// - After the subscription was established and after every recovery of a lost connection the handler synchronizes
    ///   its state (e.g. loads the alarm summary), retried until it succeeds. Subscribing first and synchronizing after
    ///   makes sure that no change between the synchronization and the subscription is missed.
    /// </remarks>
    public class AlarmActionSubscriberService : BackgroundService
    {
        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILogger<AlarmActionSubscriberService> _logger;


        /// <summary>
        /// Subscriber of the alarm action exchange.
        /// </summary>
        private readonly IRabbitMqSubscriber _subscriber;


        /// <summary>
        /// Recipient of the received actions.
        /// </summary>
        private readonly IAlarmActionHandler _handler;


        /// <summary>
        /// Interval between the retries of the subscription and the synchronization.
        /// </summary>
        private readonly TimeSpan _retryInterval;


        /// <summary>
        /// Time provider of the retry interval.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// Requests a synchronization after a recovered connection, holds at most one pending request.
        /// </summary>
        private readonly Channel<bool> _synchronizeRequests = Channel.CreateBounded<bool>(new BoundedChannelOptions(1)
        {
            FullMode = BoundedChannelFullMode.DropWrite,
            SingleReader = true
        });


        /// <summary>
        /// Constructor of <see cref="AlarmActionSubscriberService"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="subscriber">Subscriber of the alarm action exchange.</param>
        /// <param name="handler">Recipient of the received actions.</param>
        /// <param name="retryInterval">Interval between the retries of the subscription and the synchronization.</param>
        /// <param name="timeProvider">Time provider of the retry interval.</param>
        public AlarmActionSubscriberService(ILogger<AlarmActionSubscriberService> logger, IRabbitMqSubscriber subscriber, IAlarmActionHandler handler,
            TimeSpan retryInterval, TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(subscriber);
            ArgumentNullException.ThrowIfNull(handler);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(retryInterval, TimeSpan.Zero);
            ArgumentNullException.ThrowIfNull(timeProvider);

            _logger = logger;
            _subscriber = subscriber;
            _handler = handler;
            _retryInterval = retryInterval;
            _timeProvider = timeProvider;
        }


        /// <summary>
        /// Subscribes, then synchronizes the handler after the subscription and after every recovered connection.
        /// </summary>
        /// <param name="stoppingToken">Cancelled when the service stops.</param>
        /// <returns>Task.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await SubscribeAsync(stoppingToken);

                while (stoppingToken.IsCancellationRequested is false)
                {
                    await SynchronizeAsync(stoppingToken);
                    await _synchronizeRequests.Reader.ReadAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Expected when normal shutdown.
            }
        }


        /// <summary>
        /// Subscribes to the actions of the handler, retried until RabbitMQ is reachable.
        /// </summary>
        private async Task SubscribeAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    await _subscriber.SubscribeAsync(_handler.ActionNames, HandleMessageAsync, RequestSynchronization, stoppingToken);

                    return;
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    _logger.LogWarning(exception, "Subscribing to the alarm actions failed, retrying in {RetryInterval}.", _retryInterval);
                }

                await Task.Delay(_retryInterval, _timeProvider, stoppingToken);
            }
        }


        /// <summary>
        /// Synchronizes the state of the handler, retried until it succeeds.
        /// </summary>
        private async Task SynchronizeAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    await _handler.SynchronizeAsync(stoppingToken);

                    return;
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    _logger.LogWarning(exception, "Synchronizing the alarm state failed, retrying in {RetryInterval}.", _retryInterval);
                }

                await Task.Delay(_retryInterval, _timeProvider, stoppingToken);
            }
        }


        /// <summary>
        /// Requests a synchronization after the subscriber recovered a lost connection.
        /// </summary>
        private void RequestSynchronization()
        {
            _synchronizeRequests.Writer.TryWrite(true);
        }


        /// <summary>
        /// Deserializes a received message to the alarm action and passes it to the handler.
        /// </summary>
        /// <exception cref="JsonException">Thrown when the body is not an alarm action, the subscriber rejects the message.</exception>
        private async Task HandleMessageAsync(RabbitMqMessage message, CancellationToken cancellationToken)
        {
            AlarmActionDto? action = JsonSerializer.Deserialize<AlarmActionDto>(message.Body);

            if (action == null)
            {
                throw new JsonException($"The body of message {message.MessageId} is not an alarm action.");
            }

            await _handler.HandleAsync(action, cancellationToken);
        }
    }
}

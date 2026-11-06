// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Alarms.Infrastructure.Storage.Outboxes;
using paskalON.Alarms.Infrastructure.Storage.Repositories;
using paskalON.Messaging.RabbitMq;

namespace paskalON.Alarms.Application.Outboxes
{
    /// <summary>
    /// Sends the pending alarm action messages of the outbox to the message broker.
    /// </summary>
    /// <remarks>
    /// - The messages are sent one after another in creation order. Sending stops at the first failed message,
    ///   so that a later action (e.g. ResetPower) never overtakes an earlier action (e.g. ReducePower).
    /// - A message is marked as sent after the broker confirmed it. When saving the sent state fails, the message
    ///   is sent again later (at least once delivery), recipients ignore duplicated deliveries with the message id.
    /// - The sender is scoped (repository), the publisher is a singleton (connection).
    /// </remarks>
    public class AlarmActionOutboxSender : IAlarmActionOutboxSender
    {
        /// <summary>
        /// Default number of messages sent per batch.
        /// </summary>
        public const int DefaultBatchSize = 50;


        /// <summary>
        /// Maximum length of the stored error (database column length).
        /// </summary>
        private const int MaximumErrorLength = 2000;


        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILogger<AlarmActionOutboxSender> _logger;


        /// <summary>
        /// Repository of the outbox messages.
        /// </summary>
        private readonly IAlarmActionOutboxRepository _outboxRepository;


        /// <summary>
        /// Publisher of the messages to the message broker.
        /// </summary>
        private readonly IRabbitMqPublisher _publisher;


        /// <summary>
        /// Time provider of the sent and failed timestamps.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// Number of messages sent per batch.
        /// </summary>
        private readonly int _batchSize;


        /// <summary>
        /// Constructor of <see cref="AlarmActionOutboxSender"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="outboxRepository">Repository of the outbox messages.</param>
        /// <param name="publisher">Publisher of the messages to the message broker.</param>
        /// <param name="timeProvider">Time provider of the sent and failed timestamps.</param>
        /// <param name="batchSize">Number of messages sent per batch.</param>
        public AlarmActionOutboxSender(ILogger<AlarmActionOutboxSender> logger, IAlarmActionOutboxRepository outboxRepository,
            IRabbitMqPublisher publisher, TimeProvider timeProvider, int batchSize = DefaultBatchSize)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(outboxRepository);
            ArgumentNullException.ThrowIfNull(publisher);
            ArgumentNullException.ThrowIfNull(timeProvider);
            ArgumentOutOfRangeException.ThrowIfLessThan(batchSize, 1);

            _logger = logger;
            _outboxRepository = outboxRepository;
            _publisher = publisher;
            _timeProvider = timeProvider;
            _batchSize = batchSize;
        }


        /// <inheritdoc/>
        public async Task<AlarmActionOutboxSendResult> SendPendingAsync(CancellationToken cancellationToken = default)
        {
            List<AlarmActionOutboxMessage> pendingMessages = await _outboxRepository.GetPendingAsync(_batchSize, cancellationToken);
            int sentCount = 0;

            foreach (AlarmActionOutboxMessage message in pendingMessages)
            {
                bool isSent = await TrySendAsync(message, cancellationToken);

                if (isSent is false)
                {
                    return new AlarmActionOutboxSendResult { SentCount = sentCount, IsFailed = true, HasMore = true };
                }

                sentCount++;
            }

            return new AlarmActionOutboxSendResult { SentCount = sentCount, IsFailed = false, HasMore = pendingMessages.Count == _batchSize };
        }


        /// <summary>
        /// Publishes a message and saves its sent state or its failed attempt.
        /// </summary>
        /// <returns>True if the broker confirmed the message.</returns>
        private async Task<bool> TrySendAsync(AlarmActionOutboxMessage message, CancellationToken cancellationToken)
        {
            DateTimeOffset now;

            try
            {
                await _publisher.PublishAsync(CreateRabbitMqMessage(message), cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                now = _timeProvider.GetUtcNow();
                message.FailedAttempts++;
                message.LastError = Truncate($"{exception.GetType().Name}: {exception.Message}", MaximumErrorLength);
                message.ChangedBy = PpcAlarmEvaluator.SystemUser;
                message.ChangedDate = now;
                await _outboxRepository.SaveChangesAsync(cancellationToken);
                _logger.LogWarning("Sending alarm action {ActionName} message {MessageId} failed (attempt {FailedAttempts}): {Error}",
                    message.ActionName, message.MessageId, message.FailedAttempts, message.LastError);

                return false;
            }

            now = _timeProvider.GetUtcNow();
            message.SentOn = now;
            message.ChangedBy = PpcAlarmEvaluator.SystemUser;
            message.ChangedDate = now;
            await _outboxRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Alarm action {ActionName} message {MessageId} sent.", message.ActionName, message.MessageId);

            return true;
        }


        /// <summary>
        /// Creates the broker message of an outbox message, the action name is the routing key and type.
        /// </summary>
        private RabbitMqMessage CreateRabbitMqMessage(AlarmActionOutboxMessage message)
        {
            return new RabbitMqMessage
            {
                MessageId = message.MessageId,
                RoutingKey = message.ActionName,
                Type = message.ActionName,
                Body = message.Payload,
                CreatedOn = message.CreatedOn
            };
        }


        /// <summary>
        /// Truncates a text to a maximum length.
        /// </summary>
        private string Truncate(string text, int maximumLength)
        {
            if (text.Length <= maximumLength)
            {
                return text;
            }

            return text[..maximumLength];
        }
    }
}

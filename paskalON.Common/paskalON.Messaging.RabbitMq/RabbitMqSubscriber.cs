// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace paskalON.Messaging.RabbitMq
{
    /// <summary>
    /// Subscribes to the messages of a durable topic exchange with a temporary queue of its own.
    /// </summary>
    /// <remarks>
    /// - Every subscriber (e.g. every instance of a service) has its own server named, exclusive and auto delete queue,
    ///   so that every subscriber receives every message. The queue is deleted when the subscriber disconnects,
    ///   messages published while it is not connected are not received.
    /// - The exchange is declared like by <see cref="RabbitMqPublisher"/>, so that the start order does not matter.
    /// - The messages are handled one after another in the order of the queue and acknowledged after the handler completed.
    ///   A message whose handler throws is logged and rejected without requeue, so that it does not block the queue.
    /// - A lost connection is recovered automatically with a new queue, the bindings and the consumer, then the
    ///   recovered callback is called.
    /// </remarks>
    public sealed class RabbitMqSubscriber : IRabbitMqSubscriber, IAsyncDisposable
    {
        /// <summary>
        /// Maximum number of unacknowledged messages delivered to the subscriber.
        /// </summary>
        private const ushort PrefetchCount = 100;


        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILogger<RabbitMqSubscriber> _logger;


        /// <summary>
        /// Factory of the connection with automatic recovery.
        /// </summary>
        private readonly ConnectionFactory _connectionFactory;


        /// <summary>
        /// Name of the topic exchange.
        /// </summary>
        private readonly string _exchangeName;


        /// <summary>
        /// Lock of the subscription.
        /// </summary>
        private readonly SemaphoreSlim _dataLock = new SemaphoreSlim(1, 1);


        /// <summary>
        /// Connection of the subscription, null if not subscribed.
        /// </summary>
        private IConnection? _connection;


        /// <summary>
        /// Channel of the subscription, null if not subscribed.
        /// </summary>
        private IChannel? _channel;


        /// <summary>
        /// Called after a lost connection was recovered.
        /// </summary>
        private Action? _recovered;


        /// <summary>
        /// Constructor of <see cref="RabbitMqSubscriber"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="connectionUri">AMQP connection URI, e.g. amqp://user:password@host:5672/.</param>
        /// <param name="exchangeName">Name of the topic exchange, e.g. ppc:alarms:actions.</param>
        /// <param name="clientName">Name of the client shown in the RabbitMQ management, e.g. the service name.</param>
        public RabbitMqSubscriber(ILogger<RabbitMqSubscriber> logger, Uri connectionUri, string exchangeName, string clientName)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(connectionUri);
            ArgumentException.ThrowIfNullOrWhiteSpace(exchangeName);
            ArgumentException.ThrowIfNullOrWhiteSpace(clientName);

            _logger = logger;
            _exchangeName = exchangeName;
            _connectionFactory = new ConnectionFactory
            {
                Uri = connectionUri,
                ClientProvidedName = clientName,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
            };
        }


        /// <inheritdoc/>
        /// <remarks>
        /// When connecting fails the exception is thrown and the subscription can be tried again.
        /// </remarks>
        public async Task SubscribeAsync(IReadOnlyCollection<string> routingKeys, Func<RabbitMqMessage, CancellationToken, Task> handler,
            Action recovered, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(routingKeys);
            ArgumentOutOfRangeException.ThrowIfZero(routingKeys.Count);
            ArgumentNullException.ThrowIfNull(handler);
            ArgumentNullException.ThrowIfNull(recovered);

            await _dataLock.WaitAsync(cancellationToken);

            try
            {
                if (_connection != null)
                {
                    throw new InvalidOperationException($"The subscriber of exchange {_exchangeName} is already subscribed.");
                }

                IConnection connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

                try
                {
                    IChannel channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
                    await channel.BasicQosAsync(0, PrefetchCount, false, cancellationToken);
                    await channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Topic, durable: true, autoDelete: false, cancellationToken: cancellationToken);
                    QueueDeclareOk queue = await channel.QueueDeclareAsync(string.Empty, durable: false, exclusive: true, autoDelete: true,
                        cancellationToken: cancellationToken);

                    foreach (string routingKey in routingKeys)
                    {
                        await channel.QueueBindAsync(queue.QueueName, _exchangeName, routingKey, cancellationToken: cancellationToken);
                    }

                    AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(channel);
                    consumer.ReceivedAsync += (_, delivery) => HandleDeliveryAsync(channel, delivery, handler);
                    connection.RecoverySucceededAsync += OnRecoverySucceededAsync;
                    await channel.BasicConsumeAsync(queue.QueueName, autoAck: false, consumer, cancellationToken);

                    _connection = connection;
                    _channel = channel;
                    _recovered = recovered;
                    _logger.LogInformation("Subscribed to exchange {ExchangeName} with queue {QueueName} and routing keys {RoutingKeys}.",
                        _exchangeName, queue.QueueName, string.Join(", ", routingKeys));
                }
                catch
                {
                    await connection.DisposeAsync();

                    throw;
                }
            }
            finally
            {
                _dataLock.Release();
            }
        }


        /// <summary>
        /// Closes the connection, the queue of the subscriber is deleted.
        /// </summary>
        /// <returns>Task.</returns>
        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.DisposeAsync();
                _channel = null;
            }

            if (_connection != null)
            {
                _connection.RecoverySucceededAsync -= OnRecoverySucceededAsync;
                await _connection.DisposeAsync();
                _connection = null;
            }

            _dataLock.Dispose();
        }


        /// <summary>
        /// Handles a delivered message and acknowledges it, or rejects it when the handler throws.
        /// </summary>
        private async Task HandleDeliveryAsync(IChannel channel, BasicDeliverEventArgs delivery, Func<RabbitMqMessage, CancellationToken, Task> handler)
        {
            bool isHandled;

            try
            {
                await handler(CreateMessage(delivery), delivery.CancellationToken);
                isHandled = true;
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogError(exception, "Handling message {MessageId} with routing key {RoutingKey} of exchange {ExchangeName} failed, the message is rejected.",
                    delivery.BasicProperties.MessageId, delivery.RoutingKey, _exchangeName);
                isHandled = false;
            }

            try
            {
                if (isHandled)
                {
                    await channel.BasicAckAsync(delivery.DeliveryTag, false, delivery.CancellationToken);
                }
                else
                {
                    await channel.BasicNackAsync(delivery.DeliveryTag, false, false, delivery.CancellationToken);
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                // E.g. the connection was lost, the exclusive queue and its messages are deleted with it
                _logger.LogWarning(exception, "Acknowledging message {MessageId} of exchange {ExchangeName} failed.", delivery.BasicProperties.MessageId, _exchangeName);
            }
        }


        /// <summary>
        /// Notifies the recipient that the connection, the queue and the consumer were recovered.
        /// </summary>
        private Task OnRecoverySucceededAsync(object sender, AsyncEventArgs args)
        {
            _logger.LogWarning("The connection of exchange {ExchangeName} was recovered, messages published while it was lost are not received.", _exchangeName);

            try
            {
                _recovered?.Invoke();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "The recovered callback of exchange {ExchangeName} failed.", _exchangeName);
            }

            return Task.CompletedTask;
        }


        /// <summary>
        /// Creates the message of a delivery, the properties are set by <see cref="RabbitMqPublisher"/>.
        /// </summary>
        /// <exception cref="FormatException">Thrown when the message id is not a GUID.</exception>
        private static RabbitMqMessage CreateMessage(BasicDeliverEventArgs delivery)
        {
            IReadOnlyBasicProperties properties = delivery.BasicProperties;

            if (Guid.TryParse(properties.MessageId, out Guid messageId) is false)
            {
                throw new FormatException($"The message id '{properties.MessageId}' is not a GUID.");
            }

            return new RabbitMqMessage
            {
                MessageId = messageId,
                RoutingKey = delivery.RoutingKey,
                Type = properties.Type ?? delivery.RoutingKey,
                Body = Encoding.UTF8.GetString(delivery.Body.Span),
                CreatedOn = DateTimeOffset.FromUnixTimeSeconds(properties.Timestamp.UnixTime)
            };
        }
    }
}

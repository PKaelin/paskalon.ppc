// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace paskalON.Messaging.RabbitMq
{
    /// <summary>
    /// Publishes JSON messages reliably to a RabbitMQ topic exchange.
    /// </summary>
    /// <remarks>
    /// - The durable topic exchange is declared by the publisher, recipients bind their queues with the routing keys
    ///   of the messages they process.
    /// - Messages are persistent and published mandatory with publisher confirmations: the publish only succeeds
    ///   when the broker routed the message to at least one queue and confirmed it. A message without a bound queue
    ///   is returned and the publish fails, so that e.g. an outbox keeps the message until a recipient is available.
    /// - The connection and channel are created on the first publish and recreated after a failure.
    /// - Not thread safe for concurrent publishes, the messages are published one after another in order.
    /// </remarks>
    public sealed class RabbitMqPublisher : IRabbitMqPublisher, IAsyncDisposable
    {
        /// <summary>
        /// Content type of the message body.
        /// </summary>
        private const string JsonContentType = "application/json";


        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILogger<RabbitMqPublisher> _logger;


        /// <summary>
        /// Factory of the RabbitMQ connections.
        /// </summary>
        private readonly ConnectionFactory _connectionFactory;


        /// <summary>
        /// Name of the exchange the messages are published to.
        /// </summary>
        private readonly string _exchangeName;


        /// <summary>
        /// Lock of the connection and channel creation.
        /// </summary>
        private readonly SemaphoreSlim _dataLock = new SemaphoreSlim(1, 1);


        /// <summary>
        /// Connection to the broker, null until the first publish.
        /// </summary>
        private IConnection? _connection;


        /// <summary>
        /// Channel with publisher confirmations, null until the first publish or after a failure.
        /// </summary>
        private IChannel? _channel;


        /// <summary>
        /// Constructor of <see cref="RabbitMqPublisher"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="connectionUri">AMQP connection URI, e.g. amqp://user:password@paskalon.rabbitmq:5672/.</param>
        /// <param name="exchangeName">Name of the topic exchange, e.g. ppc:alarms:actions.</param>
        /// <param name="clientName">Name of the client shown in the broker management, e.g. paskalON.Alarms.</param>
        public RabbitMqPublisher(ILogger<RabbitMqPublisher> logger, Uri connectionUri, string exchangeName, string clientName)
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
                AutomaticRecoveryEnabled = true
            };
        }


        /// <inheritdoc/>
        public async Task PublishAsync(RabbitMqMessage message, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(message);
            ArgumentException.ThrowIfNullOrWhiteSpace(message.RoutingKey);

            IChannel channel = await GetChannelAsync(cancellationToken);
            BasicProperties properties = new BasicProperties
            {
                MessageId = message.MessageId.ToString(),
                Type = message.Type,
                ContentType = JsonContentType,
                ContentEncoding = Encoding.UTF8.WebName,
                DeliveryMode = DeliveryModes.Persistent,
                Timestamp = new AmqpTimestamp(message.CreatedOn.ToUnixTimeSeconds())
            };
            byte[] body = Encoding.UTF8.GetBytes(message.Body);

            try
            {
                // Waits for the broker confirmation, throws when the message was returned (not routed) or rejected
                await channel.BasicPublishAsync(_exchangeName, message.RoutingKey, true, properties, body, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogWarning(exception, "Publishing message {MessageId} with routing key {RoutingKey} to exchange {ExchangeName} failed.",
                    message.MessageId, message.RoutingKey, _exchangeName);
                await ResetChannelAsync();

                throw;
            }
        }


        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await ResetChannelAsync();

            if (_connection != null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }

            _dataLock.Dispose();
        }


        /// <summary>
        /// Gets the open channel, creates the connection, the channel and declares the exchange if necessary.
        /// </summary>
        private async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken)
        {
            await _dataLock.WaitAsync(cancellationToken);

            try
            {
                if (_channel != null && _channel.IsOpen)
                {
                    return _channel;
                }

                if (_connection == null || _connection.IsOpen is false)
                {
                    if (_connection != null)
                    {
                        await _connection.DisposeAsync();
                    }

                    _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
                }

                CreateChannelOptions options = new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true);
                _channel = await _connection.CreateChannelAsync(options, cancellationToken);
                await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Topic, durable: true, autoDelete: false, cancellationToken: cancellationToken);

                return _channel;
            }
            finally
            {
                _dataLock.Release();
            }
        }


        /// <summary>
        /// Disposes the channel so that the next publish creates a new one.
        /// </summary>
        private async Task ResetChannelAsync()
        {
            IChannel? channel = _channel;
            _channel = null;

            if (channel != null)
            {
                try
                {
                    await channel.DisposeAsync();
                }
                catch (Exception exception)
                {
                    _logger.LogDebug(exception, "Disposing the channel of exchange {ExchangeName} failed.", _exchangeName);
                }
            }
        }
    }
}

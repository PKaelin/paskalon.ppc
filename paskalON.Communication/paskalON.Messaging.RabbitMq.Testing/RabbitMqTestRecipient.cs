// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using RabbitMQ.Client;

namespace paskalON.Messaging.RabbitMq.Testing
{
    /// <summary>
    /// Recipient side in integration tests: binds queues to a topic exchange and reads the received messages.
    /// </summary>
    public sealed class RabbitMqTestRecipient : IAsyncDisposable
    {
        /// <summary>
        /// Connection to the broker.
        /// </summary>
        private readonly IConnection _connection;


        /// <summary>
        /// Channel of the recipient.
        /// </summary>
        private readonly IChannel _channel;


        /// <summary>
        /// Name of the topic exchange.
        /// </summary>
        private readonly string _exchangeName;


        /// <summary>
        /// Constructor of <see cref="RabbitMqTestRecipient"/>.
        /// </summary>
        private RabbitMqTestRecipient(IConnection connection, IChannel channel, string exchangeName)
        {
            _connection = connection;
            _channel = channel;
            _exchangeName = exchangeName;
        }


        /// <summary>
        /// Connects to the broker and declares the topic exchange like the <see cref="RabbitMqPublisher"/>.
        /// </summary>
        /// <param name="connectionUri">AMQP connection URI, e.g. <see cref="RabbitMqFixture.ConnectionUri"/>.</param>
        /// <param name="exchangeName">Name of the topic exchange.</param>
        /// <returns>Connected recipient.</returns>
        public static async Task<RabbitMqTestRecipient> ConnectAsync(Uri connectionUri, string exchangeName)
        {
            ArgumentNullException.ThrowIfNull(connectionUri);
            ArgumentException.ThrowIfNullOrWhiteSpace(exchangeName);

            ConnectionFactory factory = new ConnectionFactory { Uri = connectionUri };
            IConnection connection = await factory.CreateConnectionAsync();
            IChannel channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Topic, durable: true, autoDelete: false);

            return new RabbitMqTestRecipient(connection, channel, exchangeName);
        }


        /// <summary>
        /// Declares a temporary queue bound to the exchange with the routing key (e.g. ReducePower or # for all messages).
        /// </summary>
        /// <param name="routingKey">Routing key of the binding.</param>
        /// <returns>Name of the queue.</returns>
        public async Task<string> BindQueueAsync(string routingKey)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(routingKey);

            QueueDeclareOk queue = await _channel.QueueDeclareAsync();
            await _channel.QueueBindAsync(queue.QueueName, _exchangeName, routingKey);

            return queue.QueueName;
        }


        /// <summary>
        /// Reads all received messages of a queue in order.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>Received messages.</returns>
        public async Task<List<BasicGetResult>> ReceiveAllAsync(string queueName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(queueName);

            List<BasicGetResult> messages = new List<BasicGetResult>();
            BasicGetResult? message = await _channel.BasicGetAsync(queueName, autoAck: true);

            while (message != null)
            {
                messages.Add(message);
                message = await _channel.BasicGetAsync(queueName, autoAck: true);
            }

            return messages;
        }


        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await _channel.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}

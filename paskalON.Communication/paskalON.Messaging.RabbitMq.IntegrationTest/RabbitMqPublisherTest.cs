// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Messaging.RabbitMq.Testing;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;

namespace paskalON.Messaging.RabbitMq.IntegrationTest
{
    /// <summary>
    /// Tests <see cref="RabbitMqPublisher"/> against a RabbitMQ container.
    /// </summary>
    [TestClass]
    public class RabbitMqPublisherTest
    {
        /// <summary>
        /// Time when the messages were created.
        /// </summary>
        private static readonly DateTimeOffset CreatedOn = new DateTimeOffset(2026, 11, 6, 8, 15, 30, TimeSpan.Zero);


        /// <summary>
        /// Exchange of this test, so that the parallel tests do not receive each other's messages.
        /// </summary>
        private readonly string _exchangeName = $"test:messaging:{Guid.NewGuid():N}";


        /// <summary>
        /// The messages are delivered in order with their properties to a bound queue.
        /// </summary>
        [TestMethod]
        public async Task PublishAsyncDeliversToBoundQueueInOrderTest()
        {
            await using RabbitMqTestRecipient recipient = await RabbitMqTestRecipient.ConnectAsync(RabbitMqFixture.ConnectionUri, _exchangeName);
            string queueName = await recipient.BindQueueAsync("#");
            await using RabbitMqPublisher publisher = CreatePublisher(RabbitMqFixture.ConnectionUri);
            RabbitMqMessage reducePower = CreateMessage("ReducePower", "{\"Percentage\":50}");
            RabbitMqMessage resetPower = CreateMessage("ResetPower", "{\"Percentage\":100}");

            await publisher.PublishAsync(reducePower);
            await publisher.PublishAsync(resetPower);

            List<BasicGetResult> received = await recipient.ReceiveAllAsync(queueName);
            Assert.HasCount(2, received);
            Assert.AreEqual("ReducePower", received[0].RoutingKey);
            Assert.AreEqual("ResetPower", received[1].RoutingKey);
            Assert.AreEqual(reducePower.MessageId.ToString(), received[0].BasicProperties.MessageId);
            Assert.AreEqual("ReducePower", received[0].BasicProperties.Type);
            Assert.AreEqual("application/json", received[0].BasicProperties.ContentType);
            Assert.AreEqual(DeliveryModes.Persistent, received[0].BasicProperties.DeliveryMode);
            Assert.AreEqual(CreatedOn.ToUnixTimeSeconds(), received[0].BasicProperties.Timestamp.UnixTime);
            Assert.AreEqual("{\"Percentage\":50}", Encoding.UTF8.GetString(received[0].Body.Span));
        }


        /// <summary>
        /// Only the queue bound with the routing key of the message receives it.
        /// </summary>
        [TestMethod]
        public async Task PublishAsyncRoutesByRoutingKeyTest()
        {
            await using RabbitMqTestRecipient recipient = await RabbitMqTestRecipient.ConnectAsync(RabbitMqFixture.ConnectionUri, _exchangeName);
            string reducePowerQueue = await recipient.BindQueueAsync("ReducePower");
            string resetPowerQueue = await recipient.BindQueueAsync("ResetPower");
            await using RabbitMqPublisher publisher = CreatePublisher(RabbitMqFixture.ConnectionUri);

            await publisher.PublishAsync(CreateMessage("ResetPower", "{\"Percentage\":100}"));

            Assert.IsEmpty(await recipient.ReceiveAllAsync(reducePowerQueue));
            Assert.HasCount(1, await recipient.ReceiveAllAsync(resetPowerQueue));
        }


        /// <summary>
        /// Without a bound queue the broker returns the message and the publish fails.
        /// </summary>
        [TestMethod]
        public async Task PublishAsyncWithoutBoundQueueThrowsTest()
        {
            await using RabbitMqPublisher publisher = CreatePublisher(RabbitMqFixture.ConnectionUri);
            RabbitMqMessage message = CreateMessage("ReducePower", "{\"Percentage\":50}");

            Func<Task> action = () => publisher.PublishAsync(message);

            await Assert.ThrowsAsync<PublishException>(action);
        }


        /// <summary>
        /// After a failed publish the same publisher recovers and publishes once a queue is bound.
        /// </summary>
        [TestMethod]
        public async Task PublishAsyncAfterFailurePublishesTest()
        {
            await using RabbitMqPublisher publisher = CreatePublisher(RabbitMqFixture.ConnectionUri);
            RabbitMqMessage message = CreateMessage("ReducePower", "{\"Percentage\":50}");
            await Assert.ThrowsAsync<PublishException>(() => publisher.PublishAsync(message));
            await using RabbitMqTestRecipient recipient = await RabbitMqTestRecipient.ConnectAsync(RabbitMqFixture.ConnectionUri, _exchangeName);
            string queueName = await recipient.BindQueueAsync("ReducePower");

            await publisher.PublishAsync(message);

            Assert.HasCount(1, await recipient.ReceiveAllAsync(queueName));
        }


        /// <summary>
        /// When the broker is not reachable the publish fails.
        /// </summary>
        [TestMethod]
        public async Task PublishAsyncBrokerNotReachableThrowsTest()
        {
            await using RabbitMqPublisher publisher = CreatePublisher(new Uri("amqp://guest:guest@127.0.0.1:1/"));
            RabbitMqMessage message = CreateMessage("ReducePower", "{\"Percentage\":50}");

            Func<Task> action = () => publisher.PublishAsync(message);

            await Assert.ThrowsAsync<BrokerUnreachableException>(action);
        }


        /// <summary>
        /// An empty exchange or client name is rejected.
        /// </summary>
        [TestMethod]
        [DataRow("", "paskalON.Alarms")]
        [DataRow(" ", "paskalON.Alarms")]
        [DataRow("ppc:alarms:actions", "")]
        [DataRow("ppc:alarms:actions", " ")]
        public void ConstructorEmptyNameThrowsTest(string exchangeName, string clientName)
        {
            Uri connectionUri = new Uri("amqp://guest:guest@localhost:5672/");

            Action action = () => new RabbitMqPublisher(NullLogger<RabbitMqPublisher>.Instance, connectionUri, exchangeName, clientName);

            Assert.Throws<ArgumentException>(action);
        }


        /// <summary>
        /// Creates the publisher to the exchange of this test.
        /// </summary>
        private RabbitMqPublisher CreatePublisher(Uri connectionUri)
        {
            return new RabbitMqPublisher(NullLogger<RabbitMqPublisher>.Instance, connectionUri, _exchangeName, "paskalON.Messaging.RabbitMq.IntegrationTest");
        }


        /// <summary>
        /// Creates a message with the action name as routing key and type.
        /// </summary>
        private RabbitMqMessage CreateMessage(string actionName, string body)
        {
            return new RabbitMqMessage
            {
                MessageId = Guid.CreateVersion7(),
                RoutingKey = actionName,
                Type = actionName,
                Body = body,
                CreatedOn = CreatedOn
            };
        }
    }
}

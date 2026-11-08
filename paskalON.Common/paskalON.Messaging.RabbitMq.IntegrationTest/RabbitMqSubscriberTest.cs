// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Threading.Channels;
using Microsoft.Extensions.Logging.Abstractions;
using paskalON.Messaging.RabbitMq.Testing;
using RabbitMQ.Client.Exceptions;

namespace paskalON.Messaging.RabbitMq.IntegrationTest
{
    /// <summary>
    /// Tests <see cref="RabbitMqSubscriber"/> with <see cref="RabbitMqPublisher"/> against a RabbitMQ container.
    /// </summary>
    [TestClass]
    public class RabbitMqSubscriberTest
    {
        /// <summary>
        /// Time when the messages were created, whole seconds like the AMQP timestamp.
        /// </summary>
        private static readonly DateTimeOffset CreatedOn = new DateTimeOffset(2026, 11, 7, 9, 30, 15, TimeSpan.Zero);


        /// <summary>
        /// Maximum real time to wait for a received message.
        /// </summary>
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);


        /// <summary>
        /// Maximum real time to wait for the recovery of a lost connection (the client retries every 5 seconds).
        /// </summary>
        private static readonly TimeSpan RecoveryTimeout = TimeSpan.FromSeconds(30);


        /// <summary>
        /// Exchange of this test, so that the parallel tests do not receive each other's messages.
        /// </summary>
        private readonly string _exchangeName = $"test:messaging:{Guid.NewGuid():N}";


        /// <summary>
        /// The published messages are received in order with their properties.
        /// </summary>
        [TestMethod]
        public async Task SubscribeAsyncReceivesPublishedMessagesInOrderTest()
        {
            Channel<RabbitMqMessage> received = Channel.CreateUnbounded<RabbitMqMessage>();
            await using RabbitMqSubscriber subscriber = CreateSubscriber(RabbitMqFixture.ConnectionUri);
            await subscriber.SubscribeAsync(new string[] { "RaiseAlarm", "ClearAlarm" }, (message, _) => Collect(received, message), () => { });
            await using RabbitMqPublisher publisher = CreatePublisher();
            RabbitMqMessage showAlarm = CreateMessage("RaiseAlarm", "{\"AlarmId\":\"PCS-COMM\"}");
            RabbitMqMessage clearAlarm = CreateMessage("ClearAlarm", "{\"AlarmId\":\"PCS-COMM\"}");

            await publisher.PublishAsync(showAlarm);
            await publisher.PublishAsync(clearAlarm);

            Assert.AreEqual(showAlarm, await ReceiveAsync(received));
            Assert.AreEqual(clearAlarm, await ReceiveAsync(received));
        }


        /// <summary>
        /// Every subscriber (e.g. every service instance) receives every message.
        /// </summary>
        [TestMethod]
        public async Task SubscribeAsyncEverySubscriberReceivesEveryMessageTest()
        {
            Channel<RabbitMqMessage> firstReceived = Channel.CreateUnbounded<RabbitMqMessage>();
            Channel<RabbitMqMessage> secondReceived = Channel.CreateUnbounded<RabbitMqMessage>();
            await using RabbitMqSubscriber firstSubscriber = CreateSubscriber(RabbitMqFixture.ConnectionUri);
            await using RabbitMqSubscriber secondSubscriber = CreateSubscriber(RabbitMqFixture.ConnectionUri);
            await firstSubscriber.SubscribeAsync(new string[] { "RaiseAlarm" }, (message, _) => Collect(firstReceived, message), () => { });
            await secondSubscriber.SubscribeAsync(new string[] { "RaiseAlarm" }, (message, _) => Collect(secondReceived, message), () => { });
            await using RabbitMqPublisher publisher = CreatePublisher();
            RabbitMqMessage showAlarm = CreateMessage("RaiseAlarm", "{\"AlarmId\":\"BB-COMM\"}");

            await publisher.PublishAsync(showAlarm);

            Assert.AreEqual(showAlarm.MessageId, (await ReceiveAsync(firstReceived)).MessageId);
            Assert.AreEqual(showAlarm.MessageId, (await ReceiveAsync(secondReceived)).MessageId);
        }


        /// <summary>
        /// Only the messages of the subscribed routing keys are received.
        /// </summary>
        [TestMethod]
        public async Task SubscribeAsyncReceivesOnlySubscribedRoutingKeysTest()
        {
            Channel<RabbitMqMessage> received = Channel.CreateUnbounded<RabbitMqMessage>();
            await using RabbitMqSubscriber subscriber = CreateSubscriber(RabbitMqFixture.ConnectionUri);
            await subscriber.SubscribeAsync(new string[] { "RaiseAlarm" }, (message, _) => Collect(received, message), () => { });
            // Another recipient of all messages, so that the message of the not subscribed routing key is routed
            await using RabbitMqTestRecipient otherRecipient = await RabbitMqTestRecipient.ConnectAsync(RabbitMqFixture.ConnectionUri, _exchangeName);
            await otherRecipient.BindQueueAsync("#");
            await using RabbitMqPublisher publisher = CreatePublisher();

            await publisher.PublishAsync(CreateMessage("ReducePower", "{\"Percentage\":50}"));
            await publisher.PublishAsync(CreateMessage("RaiseAlarm", "{\"AlarmId\":\"PV-COMM\"}"));

            Assert.AreEqual("RaiseAlarm", (await ReceiveAsync(received)).RoutingKey);
        }


        /// <summary>
        /// A message whose handler throws is rejected and not delivered again, the following messages are still handled.
        /// </summary>
        [TestMethod]
        public async Task SubscribeAsyncHandlerFailsRejectsMessageAndContinuesTest()
        {
            Channel<RabbitMqMessage> received = Channel.CreateUnbounded<RabbitMqMessage>();
            int handledCount = 0;
            await using RabbitMqSubscriber subscriber = CreateSubscriber(RabbitMqFixture.ConnectionUri);
            await subscriber.SubscribeAsync(new string[] { "RaiseAlarm" }, (message, _) =>
            {
                handledCount++;

                if (message.Body == "invalid")
                {
                    throw new FormatException("The body is not valid JSON.");
                }

                return Collect(received, message);
            }, () => { });
            await using RabbitMqPublisher publisher = CreatePublisher();

            await publisher.PublishAsync(CreateMessage("RaiseAlarm", "invalid"));
            await publisher.PublishAsync(CreateMessage("RaiseAlarm", "{\"AlarmId\":\"PCS-COMM\"}"));

            Assert.AreEqual("{\"AlarmId\":\"PCS-COMM\"}", (await ReceiveAsync(received)).Body);
            Assert.AreEqual(2, handledCount);
        }


        /// <summary>
        /// The queue of a disposed subscriber is deleted, a mandatory message without another recipient is not routed.
        /// </summary>
        [TestMethod]
        public async Task DisposeAsyncDeletesQueueTest()
        {
            RabbitMqSubscriber subscriber = CreateSubscriber(RabbitMqFixture.ConnectionUri);
            await subscriber.SubscribeAsync(new string[] { "RaiseAlarm" }, (_, _) => Task.CompletedTask, () => { });
            await using RabbitMqPublisher publisher = CreatePublisher();
            await publisher.PublishAsync(CreateMessage("RaiseAlarm", "{\"AlarmId\":\"PCS-COMM\"}"));

            await subscriber.DisposeAsync();

            await Assert.ThrowsAsync<PublishException>(() => publisher.PublishAsync(CreateMessage("RaiseAlarm", "{\"AlarmId\":\"PCS-COMM\"}")));
        }


        /// <summary>
        /// After the broker closed the connection the subscriber recovers it with a new queue, calls the recovered callback
        /// and receives the messages published after the recovery.
        /// </summary>
        [TestMethod]
        [DoNotParallelize]
        public async Task SubscribeAsyncConnectionLostRecoversAndCallsRecoveredTest()
        {
            Channel<RabbitMqMessage> received = Channel.CreateUnbounded<RabbitMqMessage>();
            TaskCompletionSource recovered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            await using RabbitMqSubscriber subscriber = CreateSubscriber(RabbitMqFixture.ConnectionUri);
            await subscriber.SubscribeAsync(new string[] { "RaiseAlarm" }, (message, _) => Collect(received, message), () => recovered.TrySetResult());
            await RabbitMqFixture.CloseAllConnectionsAsync();
            await recovered.Task.WaitAsync(RecoveryTimeout);
            await using RabbitMqPublisher publisher = CreatePublisher();
            RabbitMqMessage showAlarm = CreateMessage("RaiseAlarm", "{\"AlarmId\":\"PCS-COMM\"}");

            await publisher.PublishAsync(showAlarm);

            Assert.AreEqual(showAlarm.MessageId, (await ReceiveAsync(received)).MessageId);
        }


        /// <summary>
        /// A subscriber subscribes only once.
        /// </summary>
        [TestMethod]
        public async Task SubscribeAsyncTwiceThrowsTest()
        {
            await using RabbitMqSubscriber subscriber = CreateSubscriber(RabbitMqFixture.ConnectionUri);
            await subscriber.SubscribeAsync(new string[] { "RaiseAlarm" }, (_, _) => Task.CompletedTask, () => { });

            Func<Task> action = () => subscriber.SubscribeAsync(new string[] { "ClearAlarm" }, (_, _) => Task.CompletedTask, () => { });

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(action);
        }


        /// <summary>
        /// When the broker is not reachable the subscription fails and can be tried again.
        /// </summary>
        [TestMethod]
        public async Task SubscribeAsyncBrokerNotReachableThrowsTest()
        {
            await using RabbitMqSubscriber subscriber = CreateSubscriber(new Uri("amqp://guest:guest@127.0.0.1:1/"));

            Func<Task> action = () => subscriber.SubscribeAsync(new string[] { "RaiseAlarm" }, (_, _) => Task.CompletedTask, () => { });

            await Assert.ThrowsAsync<BrokerUnreachableException>(action);
        }


        /// <summary>
        /// Adds a received message to the channel of the test.
        /// </summary>
        private static Task Collect(Channel<RabbitMqMessage> received, RabbitMqMessage message)
        {
            received.Writer.TryWrite(message);

            return Task.CompletedTask;
        }


        /// <summary>
        /// Waits for the next received message.
        /// </summary>
        private static async Task<RabbitMqMessage> ReceiveAsync(Channel<RabbitMqMessage> received)
        {
            return await received.Reader.ReadAsync().AsTask().WaitAsync(TestTimeout);
        }


        /// <summary>
        /// Creates the subscriber of the exchange of this test.
        /// </summary>
        private RabbitMqSubscriber CreateSubscriber(Uri connectionUri)
        {
            return new RabbitMqSubscriber(NullLogger<RabbitMqSubscriber>.Instance, connectionUri, _exchangeName, "paskalON.Messaging.RabbitMq.IntegrationTest");
        }


        /// <summary>
        /// Creates the publisher to the exchange of this test.
        /// </summary>
        private RabbitMqPublisher CreatePublisher()
        {
            return new RabbitMqPublisher(NullLogger<RabbitMqPublisher>.Instance, RabbitMqFixture.ConnectionUri, _exchangeName, "paskalON.Messaging.RabbitMq.IntegrationTest");
        }


        /// <summary>
        /// Creates a message with the action name as routing key and type.
        /// </summary>
        private static RabbitMqMessage CreateMessage(string actionName, string body)
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

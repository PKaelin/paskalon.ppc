// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.Messaging.Redis;
using paskalON.Messaging.UnitTest.TestDoubles;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace paskalON.Messaging.UnitTest.Redis
{
    [TestClass]
    public sealed class RedisMessageSubscriberTest
    {
        private const string Topic = "devices.batterybank01.measurements";
        private const string Json = "{\"deviceId\":\"BatteryBank01\",\"stateOfCharge\":78.5,\"activePower\":-1250.0,\"temperature\":\"24.3 °C\"}";
        private static readonly TimeSpan DeliveryTimeout = TimeSpan.FromSeconds(10);


        [TestMethod]
        public void ConstructorNullRedisThrowsTest()
        {
            Func<RedisMessageSubscriber> act = () => new RedisMessageSubscriber(null!);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }


        [TestMethod]
        public void ConstructorRetrievesSubscriberOnceTest()
        {
            Mock<ISubscriber> subscriberMock = new Mock<ISubscriber>(MockBehavior.Loose);
            Mock<IConnectionMultiplexer> redisMock = CreateRedisMock(subscriberMock);

            RedisMessageSubscriber subscriber = new RedisMessageSubscriber(redisMock.Object);

            Assert.IsNotNull(subscriber);
            redisMock.Verify(x => x.GetSubscriber(It.IsAny<object?>()), Times.Once());
        }


        [TestMethod]
        public void SubscribeNullTopicThrowsAndDoesNotSubscribeTest()
        {
            Mock<ISubscriber> subscriberMock = new Mock<ISubscriber>(MockBehavior.Loose);
            RedisMessageSubscriber subscriber = new RedisMessageSubscriber(CreateRedisMock(subscriberMock).Object);

            Action act = () => subscriber.Subscribe(null!, _ => { });

            Assert.ThrowsExactly<ArgumentNullException>(act);
            VerifyNeverSubscribed(subscriberMock);
        }


        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("\t\r\n")]
        public void SubscribeEmptyOrWhiteSpaceTopicThrowsAndDoesNotSubscribeTest(string topic)
        {
            Mock<ISubscriber> subscriberMock = new Mock<ISubscriber>(MockBehavior.Loose);
            RedisMessageSubscriber subscriber = new RedisMessageSubscriber(CreateRedisMock(subscriberMock).Object);

            Action act = () => subscriber.Subscribe(topic, _ => { });

            Assert.ThrowsExactly<ArgumentException>(act);
            VerifyNeverSubscribed(subscriberMock);
        }


        [TestMethod]
        public void SubscribeNullCallbackThrowsAndDoesNotSubscribeTest()
        {
            Mock<ISubscriber> subscriberMock = new Mock<ISubscriber>(MockBehavior.Loose);
            RedisMessageSubscriber subscriber = new RedisMessageSubscriber(CreateRedisMock(subscriberMock).Object);

            Action act = () => subscriber.Subscribe(Topic, null!);

            Assert.ThrowsExactly<ArgumentNullException>(act);
            VerifyNeverSubscribed(subscriberMock);
        }


        [TestMethod]
        [DataRow(Topic)]
        [DataRow("devices.*")]
        [DataRow("devices.batterybank0?.measurements")]
        public void SubscribeSubscribesToLiteralChannelTest(string topic)
        {
            using TestChannelMessageQueue queue = new TestChannelMessageQueue(RedisChannel.Literal(topic));
            RedisChannel subscribedChannel = default;
            Mock<ISubscriber> subscriberMock = new Mock<ISubscriber>(MockBehavior.Loose);
            subscriberMock
                .Setup(x => x.Subscribe(It.IsAny<RedisChannel>(), It.IsAny<CommandFlags>()))
                .Callback<RedisChannel, CommandFlags>((channel, _) => subscribedChannel = channel)
                .Returns(queue.Queue);
            RedisMessageSubscriber subscriber = new RedisMessageSubscriber(CreateRedisMock(subscriberMock).Object);

            subscriber.Subscribe(topic, _ => { });

            subscriberMock.Verify(x => x.Subscribe(It.IsAny<RedisChannel>(), It.IsAny<CommandFlags>()), Times.Once());
            Assert.AreEqual(topic, subscribedChannel.ToString());
            Assert.IsFalse(subscribedChannel.IsPattern);
        }


        [TestMethod]
        public void SubscribeRedisConnectionFailurePropagatesTest()
        {
            Mock<ISubscriber> subscriberMock = new Mock<ISubscriber>(MockBehavior.Loose);
            subscriberMock
                .Setup(x => x.Subscribe(It.IsAny<RedisChannel>(), It.IsAny<CommandFlags>()))
                .Throws(new RedisConnectionException(ConnectionFailureType.UnableToConnect, CommandFlags.None, "No connection is available.", null, CommandStatus.Unknown));
            RedisMessageSubscriber subscriber = new RedisMessageSubscriber(CreateRedisMock(subscriberMock).Object);

            Action act = () => subscriber.Subscribe(Topic, _ => { });

            RedisConnectionException exception = Assert.ThrowsExactly<RedisConnectionException>(act);
            Assert.AreEqual(ConnectionFailureType.UnableToConnect, exception.FailureType);
        }


        [TestMethod]
        public async Task SubscribeForwardsReceivedJsonToCallbackTest()
        {
            using TestChannelMessageQueue queue = new TestChannelMessageQueue(RedisChannel.Literal(Topic));
            RedisMessageSubscriber subscriber = CreateSubscriber(queue);
            TaskCompletionSource<string> received = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            subscriber.Subscribe(Topic, json => received.TrySetResult(json));
            queue.Receive(Json);
            string receivedJson = await received.Task.WaitAsync(DeliveryTimeout);

            Assert.AreEqual(Json, receivedJson);
        }


        [TestMethod]
        public async Task SubscribeForwardsUtf8BytePayloadAsStringTest()
        {
            using TestChannelMessageQueue queue = new TestChannelMessageQueue(RedisChannel.Literal(Topic));
            RedisMessageSubscriber subscriber = CreateSubscriber(queue);
            TaskCompletionSource<string> received = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            byte[] payload = System.Text.Encoding.UTF8.GetBytes(Json);

            subscriber.Subscribe(Topic, json => received.TrySetResult(json));
            queue.Receive(payload);
            string receivedJson = await received.Task.WaitAsync(DeliveryTimeout);

            Assert.AreEqual(Json, receivedJson);
        }


        [TestMethod]
        public async Task SubscribeSkipsNullAndEmptyMessagesTest()
        {
            using TestChannelMessageQueue queue = new TestChannelMessageQueue(RedisChannel.Literal(Topic));
            RedisMessageSubscriber subscriber = CreateSubscriber(queue);
            ConcurrentQueue<string> receivedMessages = new ConcurrentQueue<string>();
            TaskCompletionSource lastReceived = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            subscriber.Subscribe(Topic, json =>
            {
                receivedMessages.Enqueue(json);

                if (json == Json)
                {
                    lastReceived.TrySetResult();
                }
            });
            queue.Receive(RedisValue.Null);
            queue.Receive(RedisValue.EmptyString);
            queue.Receive(Array.Empty<byte>());
            queue.Receive(Json);
            await lastReceived.Task.WaitAsync(DeliveryTimeout);

            CollectionAssert.AreEqual(new string[] { Json }, receivedMessages.ToArray());
        }


        [TestMethod]
        [DataRow(" ")]
        [DataRow("0")]
        [DataRow("{}")]
        public async Task SubscribeForwardsShortNonEmptyMessagesTest(string message)
        {
            using TestChannelMessageQueue queue = new TestChannelMessageQueue(RedisChannel.Literal(Topic));
            RedisMessageSubscriber subscriber = CreateSubscriber(queue);
            TaskCompletionSource<string> received = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            subscriber.Subscribe(Topic, json => received.TrySetResult(json));
            queue.Receive(message);
            string receivedMessage = await received.Task.WaitAsync(DeliveryTimeout);

            Assert.AreEqual(message, receivedMessage);
        }


        [TestMethod]
        public async Task SubscribeDeliversMessagesInOrderTest()
        {
            const int MessageCount = 500;
            using TestChannelMessageQueue queue = new TestChannelMessageQueue(RedisChannel.Literal(Topic));
            RedisMessageSubscriber subscriber = CreateSubscriber(queue);
            List<string> receivedMessages = new List<string>();
            TaskCompletionSource allReceived = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            string[] expectedMessages = Enumerable.Range(0, MessageCount).Select(i => $"{{\"sequence\":{i}}}").ToArray();

            subscriber.Subscribe(Topic, json =>
            {
                receivedMessages.Add(json);

                if (receivedMessages.Count == MessageCount)
                {
                    allReceived.TrySetResult();
                }
            });

            foreach (string message in expectedMessages)
            {
                queue.Receive(message);
            }

            await allReceived.Task.WaitAsync(DeliveryTimeout);

            CollectionAssert.AreEqual(expectedMessages, receivedMessages);
        }


        [TestMethod]
        public async Task SubscribeNeverInvokesCallbackConcurrentlyTest()
        {
            const int MessageCount = 200;
            const int ProducerCount = 4;
            using TestChannelMessageQueue queue = new TestChannelMessageQueue(RedisChannel.Literal(Topic));
            RedisMessageSubscriber subscriber = CreateSubscriber(queue);
            int activeCallbacks = 0;
            int maxActiveCallbacks = 0;
            int receivedCount = 0;
            TaskCompletionSource allReceived = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            subscriber.Subscribe(Topic, json =>
            {
                int active = Interlocked.Increment(ref activeCallbacks);
                InterlockedMax(ref maxActiveCallbacks, active);
                Thread.SpinWait(1000);
                Interlocked.Decrement(ref activeCallbacks);

                if (Interlocked.Increment(ref receivedCount) == MessageCount * ProducerCount)
                {
                    allReceived.TrySetResult();
                }
            });
            IEnumerable<Task> producers = Enumerable.Range(0, ProducerCount)
                .Select(producer => Task.Run(() =>
                {
                    for (int i = 0; i < MessageCount; i++)
                    {
                        queue.Receive($"{{\"producer\":{producer},\"sequence\":{i}}}");
                    }
                }));
            await Task.WhenAll(producers);
            await allReceived.Task.WaitAsync(DeliveryTimeout);

            Assert.AreEqual(MessageCount * ProducerCount, receivedCount);
            Assert.AreEqual(1, maxActiveCallbacks);
        }


        [TestMethod]
        public async Task SubscribeMultipleTopicsRoutesMessagesToMatchingCallbackTest()
        {
            const string OtherTopic = "devices.inverter01.measurements";
            const string OtherJson = "{\"deviceId\":\"Inverter01\",\"activePower\":3200.0}";
            using TestChannelMessageQueue queue = new TestChannelMessageQueue(RedisChannel.Literal(Topic));
            using TestChannelMessageQueue otherQueue = new TestChannelMessageQueue(RedisChannel.Literal(OtherTopic));
            Mock<ISubscriber> subscriberMock = new Mock<ISubscriber>(MockBehavior.Strict);
            subscriberMock
                .Setup(x => x.Subscribe(RedisChannel.Literal(Topic), It.IsAny<CommandFlags>()))
                .Returns(queue.Queue);
            subscriberMock
                .Setup(x => x.Subscribe(RedisChannel.Literal(OtherTopic), It.IsAny<CommandFlags>()))
                .Returns(otherQueue.Queue);
            RedisMessageSubscriber subscriber = new RedisMessageSubscriber(CreateRedisMock(subscriberMock).Object);
            ConcurrentQueue<string> received = new ConcurrentQueue<string>();
            ConcurrentQueue<string> otherReceived = new ConcurrentQueue<string>();
            TaskCompletionSource receivedDone = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            TaskCompletionSource otherReceivedDone = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            subscriber.Subscribe(Topic, json =>
            {
                received.Enqueue(json);
                receivedDone.TrySetResult();
            });
            subscriber.Subscribe(OtherTopic, json =>
            {
                otherReceived.Enqueue(json);
                otherReceivedDone.TrySetResult();
            });
            queue.Receive(Json);
            otherQueue.Receive(OtherJson);
            await Task.WhenAll(receivedDone.Task, otherReceivedDone.Task).WaitAsync(DeliveryTimeout);

            CollectionAssert.AreEqual(new string[] { Json }, received.ToArray());
            CollectionAssert.AreEqual(new string[] { OtherJson }, otherReceived.ToArray());
        }


        [TestMethod]
        public async Task SubscribeCallbackExceptionDoesNotStopSubsequentDeliveryTest()
        {
            const string FaultyJson = "{\"deviceId\":";
            using TestChannelMessageQueue queue = new TestChannelMessageQueue(RedisChannel.Literal(Topic));
            RedisMessageSubscriber subscriber = CreateSubscriber(queue);
            TaskCompletionSource<string> received = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            subscriber.Subscribe(Topic, json =>
            {
                if (json == FaultyJson)
                {
                    throw new FormatException("Invalid json payload.");
                }

                received.TrySetResult(json);
            });
            queue.Receive(FaultyJson);
            queue.Receive(Json);
            string receivedJson = await received.Task.WaitAsync(DeliveryTimeout);

            Assert.AreEqual(Json, receivedJson);
        }


        private static RedisMessageSubscriber CreateSubscriber(TestChannelMessageQueue queue)
        {
            Mock<ISubscriber> subscriberMock = new Mock<ISubscriber>(MockBehavior.Strict);
            subscriberMock
                .Setup(x => x.Subscribe(queue.Channel, It.IsAny<CommandFlags>()))
                .Returns(queue.Queue);

            return new RedisMessageSubscriber(CreateRedisMock(subscriberMock).Object);
        }


        private static Mock<IConnectionMultiplexer> CreateRedisMock(Mock<ISubscriber> subscriberMock)
        {
            Mock<IConnectionMultiplexer> redisMock = new Mock<IConnectionMultiplexer>(MockBehavior.Loose);
            redisMock
                .Setup(x => x.GetSubscriber(It.IsAny<object?>()))
                .Returns(subscriberMock.Object);

            return redisMock;
        }


        private static void VerifyNeverSubscribed(Mock<ISubscriber> subscriberMock)
        {
            subscriberMock.Verify(x => x.Subscribe(It.IsAny<RedisChannel>(), It.IsAny<CommandFlags>()), Times.Never());
        }


        private static void InterlockedMax(ref int target, int value)
        {
            int current = Volatile.Read(ref target);

            while (value > current)
            {
                int previous = Interlocked.CompareExchange(ref target, value, current);

                if (previous == current)
                {
                    return;
                }

                current = previous;
            }
        }
    }
}

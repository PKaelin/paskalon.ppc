// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.Messaging.Redis;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace paskalON.Messaging.UnitTest.Redis
{
    [TestClass]
    public sealed class RedisMessagePublisherTest
    {
        private const string Topic = "devices.batterybank01.measurements";
        private const string Json = "{\"deviceId\":\"BatteryBank01\",\"stateOfCharge\":78.5,\"activePower\":-1250.0,\"temperature\":\"24.3 °C\"}";


        [TestMethod]
        public void ConstructorNullRedisThrowsTest()
        {
            Func<RedisMessagePublisher> act = () => new RedisMessagePublisher(null!);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }


        [TestMethod]
        public async Task PublishAsyncPublishesJsonToTopicTest()
        {
            Mock<ISubscriber> subscriberMock = CreateSubscriberMock();
            RedisMessagePublisher publisher = new RedisMessagePublisher(CreateRedisMock(subscriberMock).Object);

            await publisher.PublishAsync(Topic, Json);

            subscriberMock.Verify(
                x => x.PublishAsync(RedisChannel.Literal(Topic), (RedisValue)Json, It.IsAny<CommandFlags>()),
                Times.Once());
        }


        [TestMethod]
        [DataRow("devices.*")]
        [DataRow("devices.batterybank0?.measurements")]
        [DataRow("devices.[ab]*")]
        public async Task PublishAsyncTopicWithGlobCharactersIsPublishedLiterallyTest(string topic)
        {
            RedisChannel publishedChannel = default;
            Mock<ISubscriber> subscriberMock = CreateSubscriberMock();
            subscriberMock
                .Setup(x => x.PublishAsync(It.IsAny<RedisChannel>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
                .Callback<RedisChannel, RedisValue, CommandFlags>((channel, _, _) => publishedChannel = channel)
                .ReturnsAsync(1L);
            RedisMessagePublisher publisher = new RedisMessagePublisher(CreateRedisMock(subscriberMock).Object);

            await publisher.PublishAsync(topic, Json);

            Assert.AreEqual(topic, publishedChannel.ToString());
            Assert.IsFalse(publishedChannel.IsPattern);
        }


        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("   ")]
        [DataRow("\t\r\n")]
        public async Task PublishAsyncEmptyOrWhiteSpaceJsonIsSkippedTest(string json)
        {
            Mock<ISubscriber> subscriberMock = CreateSubscriberMock();
            Mock<IConnectionMultiplexer> redisMock = CreateRedisMock(subscriberMock);
            RedisMessagePublisher publisher = new RedisMessagePublisher(redisMock.Object);

            await publisher.PublishAsync(Topic, json);

            VerifyNothingPublished(subscriberMock);
            redisMock.Verify(x => x.GetSubscriber(It.IsAny<object?>()), Times.Never());
        }


        [TestMethod]
        [DataRow("{}")]
        [DataRow("0")]
        [DataRow(" {} ")]
        public async Task PublishAsyncShortNonWhiteSpaceJsonIsPublishedTest(string json)
        {
            Mock<ISubscriber> subscriberMock = CreateSubscriberMock();
            RedisMessagePublisher publisher = new RedisMessagePublisher(CreateRedisMock(subscriberMock).Object);

            await publisher.PublishAsync(Topic, json);

            subscriberMock.Verify(
                x => x.PublishAsync(RedisChannel.Literal(Topic), (RedisValue)json, It.IsAny<CommandFlags>()),
                Times.Once());
        }


        [TestMethod]
        public async Task PublishAsyncNullTopicThrowsAndDoesNotPublishTest()
        {
            Mock<ISubscriber> subscriberMock = CreateSubscriberMock();
            RedisMessagePublisher publisher = new RedisMessagePublisher(CreateRedisMock(subscriberMock).Object);

            Func<Task> act = () => publisher.PublishAsync(null!, Json);

            await Assert.ThrowsExactlyAsync<ArgumentNullException>(act);
            VerifyNothingPublished(subscriberMock);
        }


        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("\t\r\n")]
        public async Task PublishAsyncEmptyOrWhiteSpaceTopicThrowsAndDoesNotPublishTest(string topic)
        {
            Mock<ISubscriber> subscriberMock = CreateSubscriberMock();
            RedisMessagePublisher publisher = new RedisMessagePublisher(CreateRedisMock(subscriberMock).Object);

            Func<Task> act = () => publisher.PublishAsync(topic, Json);

            await Assert.ThrowsExactlyAsync<ArgumentException>(act);
            VerifyNothingPublished(subscriberMock);
        }


        [TestMethod]
        public async Task PublishAsyncNullJsonThrowsAndDoesNotPublishTest()
        {
            Mock<ISubscriber> subscriberMock = CreateSubscriberMock();
            RedisMessagePublisher publisher = new RedisMessagePublisher(CreateRedisMock(subscriberMock).Object);

            Func<Task> act = () => publisher.PublishAsync(Topic, null!);

            await Assert.ThrowsExactlyAsync<ArgumentNullException>(act);
            VerifyNothingPublished(subscriberMock);
        }


        [TestMethod]
        public async Task PublishAsyncRedisConnectionFailurePropagatesTest()
        {
            Mock<ISubscriber> subscriberMock = CreateSubscriberMock();
            subscriberMock
                .Setup(x => x.PublishAsync(It.IsAny<RedisChannel>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
                .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.UnableToConnect, CommandFlags.None, "No connection is available.", null, CommandStatus.Unknown));
            RedisMessagePublisher publisher = new RedisMessagePublisher(CreateRedisMock(subscriberMock).Object);

            Func<Task> act = () => publisher.PublishAsync(Topic, Json);

            RedisConnectionException exception = await Assert.ThrowsExactlyAsync<RedisConnectionException>(act);
            Assert.AreEqual(ConnectionFailureType.UnableToConnect, exception.FailureType);
        }


        [TestMethod]
        public async Task PublishAsyncDoesNotCompleteBeforeRedisPublishCompletesTest()
        {
            TaskCompletionSource<long> redisPublish = new TaskCompletionSource<long>(TaskCreationOptions.RunContinuationsAsynchronously);
            Mock<ISubscriber> subscriberMock = CreateSubscriberMock();
            subscriberMock
                .Setup(x => x.PublishAsync(It.IsAny<RedisChannel>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
                .Returns(redisPublish.Task);
            RedisMessagePublisher publisher = new RedisMessagePublisher(CreateRedisMock(subscriberMock).Object);

            Task publishTask = publisher.PublishAsync(Topic, Json);
            bool completedBeforeRedis = publishTask.IsCompleted;
            redisPublish.SetResult(1L);
            await publishTask;

            Assert.IsFalse(completedBeforeRedis);
            Assert.IsTrue(publishTask.IsCompletedSuccessfully);
        }


        [TestMethod]
        public async Task PublishAsyncConcurrentCallsPublishEveryMessageOnceTest()
        {
            const int MessageCount = 200;
            ConcurrentBag<(string Channel, string Message)> published = new ConcurrentBag<(string Channel, string Message)>();
            Mock<ISubscriber> subscriberMock = CreateSubscriberMock();
            subscriberMock
                .Setup(x => x.PublishAsync(It.IsAny<RedisChannel>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
                .Callback<RedisChannel, RedisValue, CommandFlags>((channel, message, _) => published.Add((channel.ToString(), message.ToString())))
                .ReturnsAsync(1L);
            RedisMessagePublisher publisher = new RedisMessagePublisher(CreateRedisMock(subscriberMock).Object);

            IEnumerable<Task> publishTasks = Enumerable.Range(0, MessageCount)
                .Select(i => Task.Run(() => publisher.PublishAsync($"devices.batterybank{i:D3}.measurements", $"{{\"sequence\":{i}}}")));
            await Task.WhenAll(publishTasks);

            Assert.HasCount(MessageCount, published);

            for (int i = 0; i < MessageCount; i++)
            {
                Assert.Contains(($"devices.batterybank{i:D3}.measurements", $"{{\"sequence\":{i}}}"), published);
            }
        }


        private Mock<ISubscriber> CreateSubscriberMock()
        {
            Mock<ISubscriber> subscriberMock = new Mock<ISubscriber>(MockBehavior.Loose);
            subscriberMock
                .Setup(x => x.PublishAsync(It.IsAny<RedisChannel>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(1L);

            return subscriberMock;
        }


        private Mock<IConnectionMultiplexer> CreateRedisMock(Mock<ISubscriber> subscriberMock)
        {
            Mock<IConnectionMultiplexer> redisMock = new Mock<IConnectionMultiplexer>(MockBehavior.Loose);
            redisMock
                .Setup(x => x.GetSubscriber(It.IsAny<object?>()))
                .Returns(subscriberMock.Object);

            return redisMock;
        }


        private void VerifyNothingPublished(Mock<ISubscriber> subscriberMock)
        {
            subscriberMock.Verify(
                x => x.PublishAsync(It.IsAny<RedisChannel>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()),
                Times.Never());
        }
    }
}

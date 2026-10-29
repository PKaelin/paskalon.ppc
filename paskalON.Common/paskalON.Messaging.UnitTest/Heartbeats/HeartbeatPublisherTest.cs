// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.Messaging.Heartbeats;
using paskalON.Messaging.Sequences;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text.Json;

namespace paskalON.Messaging.UnitTest.Heartbeats
{
    [TestClass]
    public sealed class HeartbeatPublisherTest
    {
        private const string ServiceName = "PowerControl";
        private const string Topic = "ppc:pc:heartbeat";
        private const string SourceInstance = "PowerControl-ppc-core-node1-3f2b9c1d8e7a4f60b5c2d1e0a9b8c7d6";
        private static readonly TimeSpan Interval = TimeSpan.FromMilliseconds(200);
        private static readonly TimeSpan DeliveryTimeout = TimeSpan.FromSeconds(10);


        [TestMethod]
        [DataRow(0)]
        [DataRow(-200)]
        public void ConstructorInvalidIntervalThrowsTest(int intervalMilliseconds)
        {
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>(MockBehavior.Strict);

            Func<HeartbeatPublisher> act = () => new HeartbeatPublisher(NullLogger<HeartbeatPublisher>.Instance, publisherMock.Object,
                new MessageSequencer(SourceInstance, TimeProvider.System), ServiceName, Topic, TimeSpan.FromMilliseconds(intervalMilliseconds), TimeProvider.System);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
        }


        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void ConstructorInvalidTopicThrowsTest(string? topic)
        {
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>(MockBehavior.Strict);

            Func<HeartbeatPublisher> act = () => new HeartbeatPublisher(NullLogger<HeartbeatPublisher>.Instance, publisherMock.Object,
                new MessageSequencer(SourceInstance, TimeProvider.System), ServiceName, topic!, Interval, TimeProvider.System);

            Assert.Throws<ArgumentException>(act);
        }


        [TestMethod]
        public async Task PublishAsyncPublishesSequencedHeartbeatTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 10, 29, 8, 15, 0, TimeSpan.Zero));
            List<string> messages = new List<string>();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>(MockBehavior.Strict);
            publisherMock
                .Setup(x => x.PublishAsync(Topic, It.IsAny<string>()))
                .Callback<string, string>((_, json) => messages.Add(json))
                .Returns(Task.CompletedTask);
            HeartbeatPublisher publisher = CreatePublisher(publisherMock.Object, timeProvider);

            bool isFirstPublished = await publisher.PublishAsync();
            bool isSecondPublished = await publisher.PublishAsync();

            Assert.IsTrue(isFirstPublished);
            Assert.IsTrue(isSecondPublished);
            Assert.HasCount(2, messages);
            HeartbeatDto? first = JsonSerializer.Deserialize<HeartbeatDto>(messages[0]);
            HeartbeatDto? second = JsonSerializer.Deserialize<HeartbeatDto>(messages[1]);
            Assert.IsNotNull(first);
            Assert.IsNotNull(second);
            Assert.AreEqual(ServiceName, first.ServiceName);
            Assert.AreEqual(SourceInstance, first.SourceInstance);
            Assert.AreEqual(1, first.Sequence);
            Assert.AreEqual(2, second.Sequence);
            Assert.AreEqual(new DateTimeOffset(2026, 10, 29, 8, 15, 0, TimeSpan.Zero), first.Timestamp);
        }


        [TestMethod]
        public async Task PublishAsyncFailureReturnsFalseAndLogsOnceTest()
        {
            FakeLogger<HeartbeatPublisher> logger = new FakeLogger<HeartbeatPublisher>();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>(MockBehavior.Strict);
            publisherMock
                .Setup(x => x.PublishAsync(Topic, It.IsAny<string>()))
                .ThrowsAsync(CreateConnectionError());
            HeartbeatPublisher publisher = CreatePublisher(publisherMock.Object, new FakeTimeProvider(), logger);

            bool isFirstPublished = await publisher.PublishAsync();
            bool isSecondPublished = await publisher.PublishAsync();
            bool isThirdPublished = await publisher.PublishAsync();

            Assert.IsFalse(isFirstPublished);
            Assert.IsFalse(isSecondPublished);
            Assert.IsFalse(isThirdPublished);
            IReadOnlyList<FakeLogRecord> records = logger.Collector.GetSnapshot();
            Assert.HasCount(1, records);
            Assert.AreEqual(LogLevel.Warning, records[0].Level);
        }


        [TestMethod]
        public async Task PublishAsyncRecoveryLogsInformationOnceTest()
        {
            FakeLogger<HeartbeatPublisher> logger = new FakeLogger<HeartbeatPublisher>();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>(MockBehavior.Strict);
            publisherMock
                .SetupSequence(x => x.PublishAsync(Topic, It.IsAny<string>()))
                .ThrowsAsync(CreateConnectionError())
                .Returns(Task.CompletedTask)
                .Returns(Task.CompletedTask);
            HeartbeatPublisher publisher = CreatePublisher(publisherMock.Object, new FakeTimeProvider(), logger);

            await publisher.PublishAsync();
            bool isRecovered = await publisher.PublishAsync();
            await publisher.PublishAsync();

            Assert.IsTrue(isRecovered);
            IReadOnlyList<FakeLogRecord> records = logger.Collector.GetSnapshot();
            Assert.HasCount(2, records);
            Assert.AreEqual(LogLevel.Warning, records[0].Level);
            Assert.AreEqual(LogLevel.Information, records[1].Level);
        }


        [TestMethod]
        public async Task RunAsyncPublishesImmediatelyAndEveryIntervalTest()
        {
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            ConcurrentQueue<string> messages = new ConcurrentQueue<string>();
            using SemaphoreSlim published = new SemaphoreSlim(0);
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>(MockBehavior.Strict);
            publisherMock
                .Setup(x => x.PublishAsync(Topic, It.IsAny<string>()))
                .Callback<string, string>((_, json) =>
                {
                    messages.Enqueue(json);
                    published.Release();
                })
                .Returns(Task.CompletedTask);
            HeartbeatPublisher publisher = CreatePublisher(publisherMock.Object, timeProvider);

            Task run = publisher.RunAsync(cancellation.Token);
            bool isImmediatePublished = await published.WaitAsync(DeliveryTimeout);
            timeProvider.Advance(Interval);
            bool isFirstIntervalPublished = await published.WaitAsync(DeliveryTimeout);
            timeProvider.Advance(Interval);
            bool isSecondIntervalPublished = await published.WaitAsync(DeliveryTimeout);
            await cancellation.CancelAsync();
            await run;

            Assert.IsTrue(isImmediatePublished);
            Assert.IsTrue(isFirstIntervalPublished);
            Assert.IsTrue(isSecondIntervalPublished);
            Assert.HasCount(3, messages);
            List<long> sequences = messages.Select(json => JsonSerializer.Deserialize<HeartbeatDto>(json)!.Sequence).ToList();
            CollectionAssert.AreEqual(new List<long> { 1, 2, 3 }, sequences);
        }


        private HeartbeatPublisher CreatePublisher(IMessagePublisher messagePublisher, TimeProvider timeProvider, ILogger<HeartbeatPublisher>? logger = null)
        {
            return new HeartbeatPublisher(logger ?? NullLogger<HeartbeatPublisher>.Instance, messagePublisher, new MessageSequencer(SourceInstance, timeProvider),
                ServiceName, Topic, Interval, timeProvider);
        }


        private RedisConnectionException CreateConnectionError()
        {
            return new RedisConnectionException(ConnectionFailureType.UnableToConnect, CommandFlags.None, "No connection is available to service this operation: PUBLISH ppc:pc:heartbeat", null, CommandStatus.Unknown);
        }
    }
}

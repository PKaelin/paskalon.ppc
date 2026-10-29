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

namespace paskalON.Messaging.UnitTest.Heartbeats
{
    [TestClass]
    public sealed class HeartbeatServiceTest
    {
        private const string Topic = "ppc:om:heartbeat";
        private static readonly TimeSpan DeliveryTimeout = TimeSpan.FromSeconds(10);


        [TestMethod]
        public void ConstructorNullLoggerThrowsTest()
        {
            Func<HeartbeatService> act = () => new HeartbeatService(null!);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }


        [TestMethod]
        public void InitializeNullPublisherThrowsTest()
        {
            HeartbeatService service = new HeartbeatService(NullLogger<HeartbeatService>.Instance);

            Action act = () => service.Initialize(null!);

            Assert.ThrowsExactly<ArgumentNullException>(act);
        }


        [TestMethod]
        public async Task ExecuteAsyncNotInitializedCompletesAndLogsWarningTest()
        {
            FakeLogger<HeartbeatService> logger = new FakeLogger<HeartbeatService>();
            using HeartbeatService service = new HeartbeatService(logger);

            await service.StartAsync(CancellationToken.None);
            await service.ExecuteTask!;

            Assert.IsTrue(service.ExecuteTask!.IsCompletedSuccessfully);
            Assert.AreEqual(1, logger.Collector.GetSnapshot().Count(r => r.Level == LogLevel.Warning));
        }


        [TestMethod]
        public async Task ExecuteAsyncInitializedPublishesUntilStoppedTest()
        {
            using SemaphoreSlim published = new SemaphoreSlim(0);
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>(MockBehavior.Strict);
            publisherMock
                .Setup(x => x.PublishAsync(Topic, It.IsAny<string>()))
                .Callback(() => published.Release())
                .Returns(Task.CompletedTask);
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            HeartbeatPublisher heartbeatPublisher = new HeartbeatPublisher(NullLogger<HeartbeatPublisher>.Instance, publisherMock.Object,
                new MessageSequencer("OperatingMode-ppc-core-node1-5d4c3b2a19f84e7d8c6b5a4f3e2d1c0b", timeProvider), "OperatingMode", Topic,
                TimeSpan.FromMilliseconds(500), timeProvider);
            using HeartbeatService service = new HeartbeatService(NullLogger<HeartbeatService>.Instance);
            service.Initialize(heartbeatPublisher);

            await service.StartAsync(CancellationToken.None);
            bool isPublished = await published.WaitAsync(DeliveryTimeout);
            await service.StopAsync(CancellationToken.None);

            Assert.IsTrue(isPublished);
            Assert.IsTrue(service.ExecuteTask!.IsCompleted);
            publisherMock.Verify(x => x.PublishAsync(Topic, It.IsAny<string>()), Times.Once());
        }
    }
}

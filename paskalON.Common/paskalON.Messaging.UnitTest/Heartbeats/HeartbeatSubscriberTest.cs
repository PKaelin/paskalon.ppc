// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.Messaging.Health;
using paskalON.Messaging.Heartbeats;
using System.Text.Json;

namespace paskalON.Messaging.UnitTest.Heartbeats
{
    [TestClass]
    public sealed class HeartbeatSubscriberTest
    {
        private const string Topic = "ppc:pc:heartbeat";
        private const string DependencyName = "PowerControl";
        private const string SourceInstance = "PowerControl-ppc-core-node1-3f2b9c1d8e7a4f60b5c2d1e0a9b8c7d6";


        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void ConstructorInvalidDependencyNameThrowsTest(string? dependencyName)
        {
            Mock<IMessageSubscriber> subscriberMock = new Mock<IMessageSubscriber>(MockBehavior.Strict);
            Mock<IDependencyHealthMonitor> monitorMock = new Mock<IDependencyHealthMonitor>(MockBehavior.Strict);

            Func<HeartbeatSubscriber> act = () => new HeartbeatSubscriber(NullLogger<HeartbeatSubscriber>.Instance, subscriberMock.Object, monitorMock.Object,
                Topic, dependencyName!);

            Assert.Throws<ArgumentException>(act);
        }


        [TestMethod]
        public void SubscribeSubscribesToTopicTest()
        {
            Mock<IMessageSubscriber> subscriberMock = new Mock<IMessageSubscriber>(MockBehavior.Strict);
            subscriberMock.Setup(x => x.Subscribe(Topic, It.IsAny<Action<string>>()));
            Mock<IDependencyHealthMonitor> monitorMock = new Mock<IDependencyHealthMonitor>(MockBehavior.Strict);
            HeartbeatSubscriber subscriber = CreateSubscriber(subscriberMock.Object, monitorMock.Object);

            subscriber.Subscribe();

            subscriberMock.Verify(x => x.Subscribe(Topic, It.IsAny<Action<string>>()), Times.Once());
        }


        [TestMethod]
        public void HandleValidHeartbeatSignalsDependencyTest()
        {
            Mock<IMessageSubscriber> subscriberMock = new Mock<IMessageSubscriber>(MockBehavior.Strict);
            Mock<IDependencyHealthMonitor> monitorMock = new Mock<IDependencyHealthMonitor>(MockBehavior.Strict);
            monitorMock.Setup(x => x.Signal(DependencyName));
            HeartbeatSubscriber subscriber = CreateSubscriber(subscriberMock.Object, monitorMock.Object);

            subscriber.Handle(CreateHeartbeatJson(1));
            subscriber.Handle(CreateHeartbeatJson(2));

            monitorMock.Verify(x => x.Signal(DependencyName), Times.Exactly(2));
        }


        [TestMethod]
        [DataRow(7L)]
        [DataRow(3L)]
        public void HandleOutdatedHeartbeatDoesNotSignalTest(long outdatedSequence)
        {
            Mock<IMessageSubscriber> subscriberMock = new Mock<IMessageSubscriber>(MockBehavior.Strict);
            Mock<IDependencyHealthMonitor> monitorMock = new Mock<IDependencyHealthMonitor>(MockBehavior.Strict);
            monitorMock.Setup(x => x.Signal(DependencyName));
            HeartbeatSubscriber subscriber = CreateSubscriber(subscriberMock.Object, monitorMock.Object);
            subscriber.Handle(CreateHeartbeatJson(7));

            subscriber.Handle(CreateHeartbeatJson(outdatedSequence));

            monitorMock.Verify(x => x.Signal(DependencyName), Times.Once());
        }


        [TestMethod]
        [DataRow("{\"ServiceName\":\"PowerControl\",\"Sequence\":")]
        [DataRow("{\"ServiceName\":\"PowerControl\",\"Sequence\":1,\"Timestamp\":\"2026-10-29T08:15:00+00:00\"}")]
        public void HandleInvalidHeartbeatDoesNotSignalAndLogsWarningTest(string json)
        {
            FakeLogger<HeartbeatSubscriber> logger = new FakeLogger<HeartbeatSubscriber>();
            Mock<IMessageSubscriber> subscriberMock = new Mock<IMessageSubscriber>(MockBehavior.Strict);
            Mock<IDependencyHealthMonitor> monitorMock = new Mock<IDependencyHealthMonitor>(MockBehavior.Strict);
            HeartbeatSubscriber subscriber = CreateSubscriber(subscriberMock.Object, monitorMock.Object, logger);

            subscriber.Handle(json);

            monitorMock.Verify(x => x.Signal(It.IsAny<string>()), Times.Never());
            Assert.AreEqual(1, logger.Collector.GetSnapshot().Count(r => r.Level == LogLevel.Warning));
        }


        [TestMethod]
        public void HandleNullHeartbeatDoesNotSignalTest()
        {
            Mock<IMessageSubscriber> subscriberMock = new Mock<IMessageSubscriber>(MockBehavior.Strict);
            Mock<IDependencyHealthMonitor> monitorMock = new Mock<IDependencyHealthMonitor>(MockBehavior.Strict);
            HeartbeatSubscriber subscriber = CreateSubscriber(subscriberMock.Object, monitorMock.Object);

            subscriber.Handle("null");

            monitorMock.Verify(x => x.Signal(It.IsAny<string>()), Times.Never());
        }


        private HeartbeatSubscriber CreateSubscriber(IMessageSubscriber messageSubscriber, IDependencyHealthMonitor monitor, ILogger<HeartbeatSubscriber>? logger = null)
        {
            return new HeartbeatSubscriber(logger ?? NullLogger<HeartbeatSubscriber>.Instance, messageSubscriber, monitor, Topic, DependencyName);
        }


        private string CreateHeartbeatJson(long sequence)
        {
            HeartbeatDto dto = new HeartbeatDto
            {
                ServiceName = DependencyName,
                SourceInstance = SourceInstance,
                Sequence = sequence,
                Timestamp = new DateTimeOffset(2026, 10, 29, 8, 15, 0, TimeSpan.Zero)
            };

            return JsonSerializer.Serialize(dto);
        }
    }
}

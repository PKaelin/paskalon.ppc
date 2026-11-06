// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.Alarms.Application.Outboxes;
using paskalON.Alarms.Application.UnitTest.SampleData;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Infrastructure.Storage.Outboxes;
using paskalON.Alarms.Infrastructure.Storage.Repositories;
using paskalON.Messaging.RabbitMq;

namespace paskalON.Alarms.Application.UnitTest.Outboxes
{
    /// <summary>
    /// Tests <see cref="AlarmActionOutboxSender"/> with a mocked publisher and repository.
    /// </summary>
    [TestClass]
    public class AlarmActionOutboxSenderTest
    {
        /// <summary>
        /// Time when the messages are sent.
        /// </summary>
        private static readonly DateTimeOffset Now = new DateTimeOffset(2026, 11, 6, 9, 0, 0, TimeSpan.Zero);


        /// <summary>
        /// Mocked outbox repository.
        /// </summary>
        private readonly Mock<IAlarmActionOutboxRepository> _outboxRepository = new Mock<IAlarmActionOutboxRepository>();


        /// <summary>
        /// Mocked publisher.
        /// </summary>
        private readonly Mock<IRabbitMqPublisher> _publisher = new Mock<IRabbitMqPublisher>();


        /// <summary>
        /// Published broker messages in publish order.
        /// </summary>
        private readonly List<RabbitMqMessage> _publishedMessages = new List<RabbitMqMessage>();


        /// <summary>
        /// Sets up the publisher to record the published messages.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _publisher.Setup(publisher => publisher.PublishAsync(It.IsAny<RabbitMqMessage>(), It.IsAny<CancellationToken>()))
                .Callback<RabbitMqMessage, CancellationToken>((message, _) => _publishedMessages.Add(message))
                .Returns(Task.CompletedTask);
            _outboxRepository.Setup(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        }


        /// <summary>
        /// The pending messages are published in order and marked as sent.
        /// </summary>
        [TestMethod]
        public async Task SendPendingAsyncPublishesInOrderAndMarksSentTest()
        {
            List<AlarmActionOutboxMessage> messages = CreateMessages("ReducePower", "ResetPower");
            SetupPending(messages);
            AlarmActionOutboxSender sender = CreateSender(batchSize: 10);

            AlarmActionOutboxSendResult result = await sender.SendPendingAsync();

            Assert.AreEqual(2, result.SentCount);
            Assert.IsFalse(result.IsFailed);
            Assert.IsFalse(result.HasMore);
            CollectionAssert.AreEqual(new List<string> { "ReducePower", "ResetPower" }, _publishedMessages.Select(message => message.RoutingKey).ToList());
            Assert.IsTrue(messages.All(message => message.SentOn == Now && message.FailedAttempts == 0));
            _outboxRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }


        /// <summary>
        /// A failing message stops the batch, records the failed attempt and later messages are not published (order is kept).
        /// </summary>
        [TestMethod]
        public async Task SendPendingAsyncPublishFailsStopsAndRecordsFailureTest()
        {
            List<AlarmActionOutboxMessage> messages = CreateMessages("ReducePower", "ResetPower", "ReducePower");
            SetupPending(messages);
            _publisher.Setup(publisher => publisher.PublishAsync(It.Is<RabbitMqMessage>(message => message.MessageId == messages[1].MessageId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("No queue bound for ResetPower."));
            AlarmActionOutboxSender sender = CreateSender(batchSize: 10);

            AlarmActionOutboxSendResult result = await sender.SendPendingAsync();

            Assert.AreEqual(1, result.SentCount);
            Assert.IsTrue(result.IsFailed);
            Assert.AreEqual(Now, messages[0].SentOn);
            Assert.IsNull(messages[1].SentOn);
            Assert.AreEqual(1, messages[1].FailedAttempts);
            Assert.AreEqual("InvalidOperationException: No queue bound for ResetPower.", messages[1].LastError);
            Assert.AreEqual(Now, messages[1].ChangedDate);
            Assert.IsNull(messages[2].SentOn);
            Assert.AreEqual(0, messages[2].FailedAttempts);
            _publisher.Verify(publisher => publisher.PublishAsync(It.Is<RabbitMqMessage>(message => message.MessageId == messages[2].MessageId),
                It.IsAny<CancellationToken>()), Times.Never);
        }


        /// <summary>
        /// The outbox message is published with the action name as routing key and type, its message id, payload and creation time.
        /// </summary>
        [TestMethod]
        public async Task SendPendingAsyncMapsOutboxMessageToBrokerMessageTest()
        {
            List<AlarmActionOutboxMessage> messages = CreateMessages("ReducePower");
            SetupPending(messages);
            AlarmActionOutboxSender sender = CreateSender(batchSize: 10);

            await sender.SendPendingAsync();

            RabbitMqMessage published = _publishedMessages.Single();
            Assert.AreEqual(messages[0].MessageId, published.MessageId);
            Assert.AreEqual("ReducePower", published.RoutingKey);
            Assert.AreEqual("ReducePower", published.Type);
            Assert.AreEqual(messages[0].Payload, published.Body);
            Assert.AreEqual(messages[0].CreatedOn, published.CreatedOn);
        }


        /// <summary>
        /// A full batch indicates that more pending messages might exist.
        /// </summary>
        [TestMethod]
        [DataRow(2, true)]
        [DataRow(3, false)]
        public async Task SendPendingAsyncFullBatchHasMoreTest(int batchSize, bool expectedHasMore)
        {
            SetupPending(CreateMessages("ReducePower", "ResetPower"));
            AlarmActionOutboxSender sender = CreateSender(batchSize);

            AlarmActionOutboxSendResult result = await sender.SendPendingAsync();

            Assert.AreEqual(expectedHasMore, result.HasMore);
        }


        /// <summary>
        /// Without pending messages nothing is published or saved.
        /// </summary>
        [TestMethod]
        public async Task SendPendingAsyncNoPendingMessagesSendsNothingTest()
        {
            SetupPending(new List<AlarmActionOutboxMessage>());
            AlarmActionOutboxSender sender = CreateSender(batchSize: 10);

            AlarmActionOutboxSendResult result = await sender.SendPendingAsync();

            Assert.AreEqual(0, result.SentCount);
            Assert.IsFalse(result.IsFailed);
            _outboxRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }


        /// <summary>
        /// Sets up the repository to return the pending messages.
        /// </summary>
        private void SetupPending(List<AlarmActionOutboxMessage> messages)
        {
            _outboxRepository.Setup(repository => repository.GetPendingAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(messages);
        }


        /// <summary>
        /// Creates the sender with the mocks and a fake time provider.
        /// </summary>
        private AlarmActionOutboxSender CreateSender(int batchSize)
        {
            return new AlarmActionOutboxSender(NullLogger<AlarmActionOutboxSender>.Instance, _outboxRepository.Object,
                _publisher.Object, new FakeTimeProvider(Now), batchSize);
        }


        /// <summary>
        /// Creates pending messages of a PCS 7 over temperature alarm in creation order.
        /// </summary>
        private List<AlarmActionOutboxMessage> CreateMessages(params string[] actionNames)
        {
            PpcAlarm alarm = PcsAlarmConfigSampleData.CreateActiveAlarm(PcsAlarmConfigSampleData.CreatePcsFaultConfig(), 7, "FaultStates.OverTemperature");

            return actionNames
                .Select((actionName, index) => new AlarmActionOutboxMessage
                {
                    Id = index + 1,
                    ChangedBy = "System",
                    ChangedDate = PcsAlarmConfigSampleData.ChangedOn,
                    MessageId = Guid.CreateVersion7(),
                    PpcAlarm = alarm,
                    ActionName = actionName,
                    Payload = $"{{\"Name\":\"{actionName}\"}}",
                    CreatedOn = PcsAlarmConfigSampleData.ChangedOn.AddSeconds(index)
                })
                .ToList();
        }
    }
}

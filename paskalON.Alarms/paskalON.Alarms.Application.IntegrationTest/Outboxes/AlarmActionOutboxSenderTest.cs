// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using paskalON.Alarms.Application.Outboxes;
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Infrastructure.Storage;
using paskalON.Alarms.Infrastructure.Storage.Outboxes;
using paskalON.Alarms.Infrastructure.Storage.Repositories;
using paskalON.Messaging.RabbitMq;
using paskalON.Messaging.RabbitMq.Testing;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace paskalON.Alarms.Application.IntegrationTest.Outboxes
{
    /// <summary>
    /// Tests <see cref="AlarmActionOutboxSender"/> with the outbox in PostgreSQL and the <see cref="RabbitMqPublisher"/> to RabbitMQ.
    /// </summary>
    [TestClass]
    public class AlarmActionOutboxSenderTest : DatabaseTestBase
    {
        /// <summary>
        /// Time when the alarm changed and the messages were created.
        /// </summary>
        private static readonly DateTimeOffset CreatedOn = new DateTimeOffset(2026, 11, 6, 8, 15, 30, TimeSpan.Zero);


        /// <summary>
        /// Time when the messages are sent.
        /// </summary>
        private static readonly DateTimeOffset SentOn = CreatedOn.AddSeconds(2);


        /// <summary>
        /// Exchange of this test, so that the parallel tests do not receive each other's messages.
        /// </summary>
        private readonly string _exchangeName = $"test:alarms:actions:{Guid.NewGuid():N}";


        /// <summary>
        /// Seeds a PCS 7 over temperature alarm with a pending ReducePower and a later pending ResetPower message.
        /// </summary>
        protected override async Task SeedAsync(AlarmContext db)
        {
            PpcAlarmConfig config = new PpcAlarmConfig
            {
                ChangedBy = "Homer",
                ChangedDate = CreatedOn,
                IsEnabled = true,
                AlarmId = "PCS-FAULT",
                Name = "{{PCS.Fault.Name}}",
                SeverityLevel = AlarmSeverity.Error,
                Description = "{{PCS.Fault.Desc}}",
                PpcAlarmType = PpcAlarmType.PcsAlarm,
                PropertyName = "FaultStates",
                PropertyOperator = PropertyOperator.Equal,
                PropertyValue = 1
            };

            PpcAlarm alarm = new PpcAlarm
            {
                ChangedBy = "System",
                ChangedDate = CreatedOn,
                CreatedOn = CreatedOn,
                DeviceId = 7,
                PropertyName = "FaultStates.OverTemperature",
                PpcAlarmConfig = config
            };

            db.PpcAlarms.Add(alarm);
            db.AlarmActionOutboxMessages.Add(CreateMessage(alarm, "ReducePower", CreatedOn, "{\"Key\":\"Percentage\",\"Value\":\"50\"}"));
            db.AlarmActionOutboxMessages.Add(CreateMessage(alarm, "ResetPower", CreatedOn.AddSeconds(1), "{\"Key\":\"Percentage\",\"Value\":\"100\"}"));
            await db.SaveChangesAsync();
        }


        /// <summary>
        /// The pending messages are delivered persistent and in order to a bound queue and marked as sent.
        /// </summary>
        [TestMethod]
        public async Task SendPendingAsyncDeliversToBoundQueueAndMarksSentTest()
        {
            await using RabbitMqTestRecipient recipient = await RabbitMqTestRecipient.ConnectAsync(RabbitMqFixture.ConnectionUri, _exchangeName);
            string queueName = await recipient.BindQueueAsync("#");
            await using RabbitMqPublisher publisher = CreatePublisher(RabbitMqFixture.ConnectionUri);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            AlarmActionOutboxSender sender = CreateSender(context, publisher);

            AlarmActionOutboxSendResult result = await sender.SendPendingAsync();

            Assert.AreEqual(2, result.SentCount);
            Assert.IsFalse(result.IsFailed);
            List<BasicGetResult> received = await recipient.ReceiveAllAsync(queueName);
            Assert.HasCount(2, received);
            Assert.AreEqual("ReducePower", received[0].RoutingKey);
            Assert.AreEqual("ResetPower", received[1].RoutingKey);
            Assert.AreEqual(DeliveryModes.Persistent, received[0].BasicProperties.DeliveryMode);
            Assert.AreEqual("application/json", received[0].BasicProperties.ContentType);
            // The payload is stored as jsonb, hence the JSON is semantically equal but not formatted the same
            using JsonDocument body = JsonDocument.Parse(Encoding.UTF8.GetString(received[0].Body.Span));
            Assert.AreEqual("ReducePower", body.RootElement.GetProperty("Name").GetString());
            Assert.AreEqual("50", body.RootElement.GetProperty("AlarmActionParameters")[0].GetProperty("Value").GetString());
            List<AlarmActionOutboxMessage> messages = await GetMessagesAsync();
            Assert.AreEqual(messages[0].MessageId.ToString(), received[0].BasicProperties.MessageId);
            Assert.IsTrue(messages.All(message => message.SentOn == SentOn && message.FailedAttempts == 0));
        }


        /// <summary>
        /// Without a bound queue (no recipient) the broker returns the message: it stays pending with a failed attempt
        /// and the later message is not sent.
        /// </summary>
        [TestMethod]
        public async Task SendPendingAsyncWithoutBoundQueueKeepsMessagesPendingTest()
        {
            await using RabbitMqPublisher publisher = CreatePublisher(RabbitMqFixture.ConnectionUri);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            AlarmActionOutboxSender sender = CreateSender(context, publisher);

            AlarmActionOutboxSendResult result = await sender.SendPendingAsync();

            Assert.AreEqual(0, result.SentCount);
            Assert.IsTrue(result.IsFailed);
            List<AlarmActionOutboxMessage> messages = await GetMessagesAsync();
            Assert.IsTrue(messages.All(message => message.SentOn == null));
            Assert.AreEqual(1, messages[0].FailedAttempts);
            Assert.IsFalse(string.IsNullOrWhiteSpace(messages[0].LastError));
            Assert.AreEqual(0, messages[1].FailedAttempts);
        }


        /// <summary>
        /// Only the later action has a recipient: the earlier action is not routable and the later action does not overtake it.
        /// </summary>
        [TestMethod]
        public async Task SendPendingAsyncFirstActionWithoutRecipientKeepsOrderTest()
        {
            await using RabbitMqTestRecipient recipient = await RabbitMqTestRecipient.ConnectAsync(RabbitMqFixture.ConnectionUri, _exchangeName);
            string queueName = await recipient.BindQueueAsync("ResetPower");
            await using RabbitMqPublisher publisher = CreatePublisher(RabbitMqFixture.ConnectionUri);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            AlarmActionOutboxSender sender = CreateSender(context, publisher);

            AlarmActionOutboxSendResult result = await sender.SendPendingAsync();

            Assert.IsTrue(result.IsFailed);
            Assert.IsEmpty(await recipient.ReceiveAllAsync(queueName));
        }


        /// <summary>
        /// After a failed send the same publisher recovers and sends the pending messages once a recipient is available.
        /// </summary>
        [TestMethod]
        public async Task SendPendingAsyncAfterFailureSendsPendingMessagesTest()
        {
            await using RabbitMqPublisher publisher = CreatePublisher(RabbitMqFixture.ConnectionUri);
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            AlarmActionOutboxSender sender = CreateSender(context, publisher);
            AlarmActionOutboxSendResult failedResult = await sender.SendPendingAsync();
            Assert.IsTrue(failedResult.IsFailed);
            await using RabbitMqTestRecipient recipient = await RabbitMqTestRecipient.ConnectAsync(RabbitMqFixture.ConnectionUri, _exchangeName);
            string queueName = await recipient.BindQueueAsync("#");

            AlarmActionOutboxSendResult result = await sender.SendPendingAsync();

            Assert.AreEqual(2, result.SentCount);
            Assert.IsFalse(result.IsFailed);
            Assert.HasCount(2, await recipient.ReceiveAllAsync(queueName));
            List<AlarmActionOutboxMessage> messages = await GetMessagesAsync();
            Assert.AreEqual(1, messages[0].FailedAttempts);
            Assert.AreEqual(SentOn, messages[0].SentOn);
        }


        /// <summary>
        /// When the broker is not reachable the message stays pending with a failed attempt.
        /// </summary>
        [TestMethod]
        public async Task SendPendingAsyncBrokerNotReachableRecordsFailureTest()
        {
            await using RabbitMqPublisher publisher = CreatePublisher(new Uri("amqp://guest:guest@127.0.0.1:1/"));
            await using AlarmContext context = CreateDbContext<AlarmContext>();
            AlarmActionOutboxSender sender = CreateSender(context, publisher);

            AlarmActionOutboxSendResult result = await sender.SendPendingAsync();

            Assert.IsTrue(result.IsFailed);
            List<AlarmActionOutboxMessage> messages = await GetMessagesAsync();
            Assert.IsNull(messages[0].SentOn);
            Assert.AreEqual(1, messages[0].FailedAttempts);
            Assert.IsFalse(string.IsNullOrWhiteSpace(messages[0].LastError));
        }


        /// <summary>
        /// Creates the publisher to the exchange of this test.
        /// </summary>
        private RabbitMqPublisher CreatePublisher(Uri connectionUri)
        {
            return new RabbitMqPublisher(NullLogger<RabbitMqPublisher>.Instance, connectionUri, _exchangeName, "paskalON.Alarms.Application.IntegrationTest");
        }


        /// <summary>
        /// Creates the sender with the real repository and publisher.
        /// </summary>
        private AlarmActionOutboxSender CreateSender(AlarmContext context, IRabbitMqPublisher publisher)
        {
            AlarmActionOutboxRepository repository = new AlarmActionOutboxRepository(NullLogger<AlarmActionOutboxRepository>.Instance, context);

            return new AlarmActionOutboxSender(NullLogger<AlarmActionOutboxSender>.Instance, repository, publisher, new FakeTimeProvider(SentOn));
        }


        /// <summary>
        /// Gets the persisted messages in creation order from a new context.
        /// </summary>
        private async Task<List<AlarmActionOutboxMessage>> GetMessagesAsync()
        {
            await using AlarmContext context = CreateDbContext<AlarmContext>();

            return await context.AlarmActionOutboxMessages.OrderBy(message => message.CreatedOn).ToListAsync();
        }


        /// <summary>
        /// Creates a pending message of the alarm.
        /// </summary>
        private AlarmActionOutboxMessage CreateMessage(PpcAlarm alarm, string actionName, DateTimeOffset createdOn, string parameter)
        {
            Guid messageId = Guid.CreateVersion7();

            return new AlarmActionOutboxMessage
            {
                ChangedBy = "System",
                ChangedDate = createdOn,
                MessageId = messageId,
                PpcAlarm = alarm,
                ActionName = actionName,
                Payload = $"{{\"MessageId\":\"{messageId}\",\"Name\":\"{actionName}\",\"AlarmActionParameters\":[{parameter}]}}",
                CreatedOn = createdOn
            };
        }
    }
}

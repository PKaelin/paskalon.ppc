// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.Alarms.Client.Subscribers;
using paskalON.Alarms.Dto.Actions;
using paskalON.Alarms.Dto.Summaries;
using paskalON.Messaging.RabbitMq;
using RabbitMQ.Client.Exceptions;

namespace paskalON.Alarms.Client.UnitTest.Subscribers
{
    /// <summary>
    /// Tests <see cref="AlarmActionSubscriberService"/> with a mocked RabbitMQ subscriber and a mocked alarm action handler.
    /// </summary>
    [TestClass]
    public class AlarmActionSubscriberServiceTest
    {
        /// <summary>
        /// Interval between the retries.
        /// </summary>
        private static readonly TimeSpan RetryInterval = TimeSpan.FromSeconds(5);


        /// <summary>
        /// Maximum real time to wait for a completion in the tests.
        /// </summary>
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);


        /// <summary>
        /// Names of the actions of the handler.
        /// </summary>
        private static readonly string[] ActionNames = new string[] { "RaiseAlarm", "ClearAlarm" };


        /// <summary>
        /// Mocked RabbitMQ subscriber.
        /// </summary>
        private readonly Mock<IRabbitMqSubscriber> _subscriber = new Mock<IRabbitMqSubscriber>();


        /// <summary>
        /// Mocked alarm action handler.
        /// </summary>
        private readonly Mock<IAlarmActionHandler> _handler = new Mock<IAlarmActionHandler>();


        /// <summary>
        /// Fake time of the retry interval.
        /// </summary>
        private readonly FakeTimeProvider _timeProvider = new FakeTimeProvider(new DateTimeOffset(2026, 11, 7, 12, 0, 0, TimeSpan.Zero));


        /// <summary>
        /// Message handler passed to the subscriber.
        /// </summary>
        private Func<RabbitMqMessage, CancellationToken, Task>? _messageHandler;


        /// <summary>
        /// Recovered callback passed to the subscriber.
        /// </summary>
        private Action? _recovered;


        /// <summary>
        /// Sets up the handler with its action names and the subscriber to capture the message handler and the recovered callback.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _handler.Setup(handler => handler.ActionNames).Returns(ActionNames);
            _subscriber.Setup(subscriber => subscriber.SubscribeAsync(It.IsAny<IReadOnlyCollection<string>>(),
                    It.IsAny<Func<RabbitMqMessage, CancellationToken, Task>>(), It.IsAny<Action>(), It.IsAny<CancellationToken>()))
                .Callback<IReadOnlyCollection<string>, Func<RabbitMqMessage, CancellationToken, Task>, Action, CancellationToken>(
                    (_, messageHandler, recovered, _) =>
                    {
                        _messageHandler = messageHandler;
                        _recovered = recovered;
                    })
                .Returns(Task.CompletedTask);
        }


        /// <summary>
        /// The service subscribes to the actions of the handler and then synchronizes the handler.
        /// </summary>
        [TestMethod]
        public async Task ExecuteAsyncSubscribesThenSynchronizesTest()
        {
            TaskCompletionSource synchronized = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _handler.Setup(handler => handler.SynchronizeAsync(It.IsAny<CancellationToken>()))
                .Callback(() => synchronized.TrySetResult())
                .Returns(Task.CompletedTask);
            AlarmActionSubscriberService service = CreateService();

            await service.StartAsync(CancellationToken.None);
            await synchronized.Task.WaitAsync(TestTimeout);
            await service.StopAsync(CancellationToken.None);

            _subscriber.Verify(subscriber => subscriber.SubscribeAsync(ActionNames, It.IsAny<Func<RabbitMqMessage, CancellationToken, Task>>(),
                It.IsAny<Action>(), It.IsAny<CancellationToken>()), Times.Once);
            _handler.Verify(handler => handler.SynchronizeAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        /// <summary>
        /// A failed subscription (RabbitMQ not reachable) is retried after the retry interval.
        /// </summary>
        [TestMethod]
        public async Task ExecuteAsyncSubscribeFailsRetriesTest()
        {
            TaskCompletionSource synchronized = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _subscriber.SetupSequence(subscriber => subscriber.SubscribeAsync(It.IsAny<IReadOnlyCollection<string>>(),
                    It.IsAny<Func<RabbitMqMessage, CancellationToken, Task>>(), It.IsAny<Action>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BrokerUnreachableException(new IOException("Connection refused.")))
                .Returns(Task.CompletedTask);
            _handler.Setup(handler => handler.SynchronizeAsync(It.IsAny<CancellationToken>()))
                .Callback(() => synchronized.TrySetResult())
                .Returns(Task.CompletedTask);
            AlarmActionSubscriberService service = CreateService();

            await service.StartAsync(CancellationToken.None);
            await AdvanceUntilAsync(synchronized.Task);
            await service.StopAsync(CancellationToken.None);

            _subscriber.Verify(subscriber => subscriber.SubscribeAsync(It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<Func<RabbitMqMessage, CancellationToken, Task>>(), It.IsAny<Action>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }


        /// <summary>
        /// A failed synchronization (alarm service not reachable) is retried after the retry interval.
        /// </summary>
        [TestMethod]
        public async Task ExecuteAsyncSynchronizeFailsRetriesTest()
        {
            TaskCompletionSource synchronized = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _handler.SetupSequence(handler => handler.SynchronizeAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Alarm service not reachable."))
                .Returns(() =>
                {
                    synchronized.TrySetResult();

                    return Task.CompletedTask;
                });
            AlarmActionSubscriberService service = CreateService();

            await service.StartAsync(CancellationToken.None);
            await AdvanceUntilAsync(synchronized.Task);
            await service.StopAsync(CancellationToken.None);

            _handler.Verify(handler => handler.SynchronizeAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }


        /// <summary>
        /// After the subscriber recovered a lost connection the handler synchronizes again, the actions sent meanwhile are lost.
        /// </summary>
        [TestMethod]
        public async Task ExecuteAsyncRecoveredSynchronizesAgainTest()
        {
            int synchronizeCount = 0;
            TaskCompletionSource firstSynchronized = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            TaskCompletionSource secondSynchronized = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _handler.Setup(handler => handler.SynchronizeAsync(It.IsAny<CancellationToken>()))
                .Callback(() => (Interlocked.Increment(ref synchronizeCount) == 1 ? firstSynchronized : secondSynchronized).TrySetResult())
                .Returns(Task.CompletedTask);
            AlarmActionSubscriberService service = CreateService();
            await service.StartAsync(CancellationToken.None);
            await firstSynchronized.Task.WaitAsync(TestTimeout);

            _recovered!();

            await secondSynchronized.Task.WaitAsync(TestTimeout);
            await service.StopAsync(CancellationToken.None);
            Assert.AreEqual(2, synchronizeCount);
        }


        /// <summary>
        /// A received message is deserialized to the alarm action with its alarm summary and passed to the handler.
        /// </summary>
        [TestMethod]
        public async Task ReceivedMessagePassesActionToHandlerTest()
        {
            AlarmActionDto? handledAction = null;
            _handler.Setup(handler => handler.HandleAsync(It.IsAny<AlarmActionDto>(), It.IsAny<CancellationToken>()))
                .Callback<AlarmActionDto, CancellationToken>((action, _) => handledAction = action)
                .Returns(Task.CompletedTask);
            AlarmActionDto showAlarm = CreateRaiseAlarm();
            Func<RabbitMqMessage, CancellationToken, Task> messageHandler = await StartAndGetMessageHandlerAsync();

            await messageHandler(CreateMessage(JsonSerializer.Serialize(showAlarm)), CancellationToken.None);

            Assert.IsNotNull(handledAction);
            Assert.AreEqual(showAlarm.MessageId, handledAction.MessageId);
            Assert.AreEqual("RaiseAlarm", handledAction.Name);
            Assert.AreEqual("PCS-COMM", handledAction.AlarmActionParameters.Single(parameter => parameter.Key == "AlarmId").Value);
            Assert.AreEqual(showAlarm.AlarmSummary, handledAction.AlarmSummary);
        }


        /// <summary>
        /// A message that is not an alarm action throws, so that the subscriber rejects it.
        /// </summary>
        [TestMethod]
        [DataRow("not json")]
        [DataRow("null")]
        public async Task ReceivedInvalidMessageThrowsTest(string body)
        {
            Func<RabbitMqMessage, CancellationToken, Task> messageHandler = await StartAndGetMessageHandlerAsync();

            Func<Task> handle = () => messageHandler(CreateMessage(body), CancellationToken.None);

            await Assert.ThrowsAsync<JsonException>(handle);
            _handler.Verify(handler => handler.HandleAsync(It.IsAny<AlarmActionDto>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        /// <summary>
        /// Starts the service, waits for the synchronization and returns the message handler passed to the subscriber.
        /// </summary>
        private async Task<Func<RabbitMqMessage, CancellationToken, Task>> StartAndGetMessageHandlerAsync()
        {
            TaskCompletionSource synchronized = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _handler.Setup(handler => handler.SynchronizeAsync(It.IsAny<CancellationToken>()))
                .Callback(() => synchronized.TrySetResult())
                .Returns(Task.CompletedTask);
            AlarmActionSubscriberService service = CreateService();
            await service.StartAsync(CancellationToken.None);
            await synchronized.Task.WaitAsync(TestTimeout);
            await service.StopAsync(CancellationToken.None);

            return _messageHandler!;
        }


        /// <summary>
        /// Advances the fake time by the retry interval until the task completed, the retry delay might not be registered yet.
        /// </summary>
        private async Task AdvanceUntilAsync(Task task)
        {
            using CancellationTokenSource timeout = new CancellationTokenSource(TestTimeout);

            while (task.IsCompleted is false)
            {
                timeout.Token.ThrowIfCancellationRequested();
                _timeProvider.Advance(RetryInterval);
                await Task.WhenAny(task, Task.Delay(TimeSpan.FromMilliseconds(10), TimeProvider.System));
            }

            await task;
        }


        /// <summary>
        /// Creates the service.
        /// </summary>
        private AlarmActionSubscriberService CreateService()
        {
            return new AlarmActionSubscriberService(NullLogger<AlarmActionSubscriberService>.Instance, _subscriber.Object, _handler.Object,
                RetryInterval, _timeProvider);
        }


        /// <summary>
        /// Creates a show alarm action of a PCS communication error like sent by the alarm service.
        /// </summary>
        private static AlarmActionDto CreateRaiseAlarm()
        {
            return new AlarmActionDto
            {
                MessageId = Guid.CreateVersion7(),
                Name = "RaiseAlarm",
                AlarmActionParameters = new List<AlarmActionParameterDto>
                {
                    new AlarmActionParameterDto { Key = "AlarmId", Value = "PCS-COMM" },
                    new AlarmActionParameterDto { Key = "DeviceId", Value = "1" }
                },
                AlarmSummary = new AlarmSummaryDto
                {
                    CalculatedOn = new DateTimeOffset(2026, 11, 7, 12, 0, 1, TimeSpan.Zero),
                    InfoCount = 0,
                    WarningCount = 0,
                    ErrorCount = 1,
                    CriticalCount = 0,
                    AcknowledgedInfoCount = 0,
                    AcknowledgedWarningCount = 0,
                    AcknowledgedErrorCount = 0,
                    AcknowledgedCriticalCount = 0
                }
            };
        }


        /// <summary>
        /// Creates a received message with the body.
        /// </summary>
        private static RabbitMqMessage CreateMessage(string body)
        {
            return new RabbitMqMessage
            {
                MessageId = Guid.CreateVersion7(),
                RoutingKey = "RaiseAlarm",
                Type = "RaiseAlarm",
                Body = body,
                CreatedOn = new DateTimeOffset(2026, 11, 7, 12, 0, 1, TimeSpan.Zero)
            };
        }
    }
}

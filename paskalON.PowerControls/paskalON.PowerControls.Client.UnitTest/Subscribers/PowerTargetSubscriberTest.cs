// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.Messaging;
using paskalON.PowerControls.Client.Subscribers;
using paskalON.PowerControls.Dto.Powers;
using System.Text.Json;

namespace paskalON.PowerControls.Client.UnitTest.Subscribers
{
    [TestClass]
    public sealed class PowerTargetSubscriberTest
    {
        private const string PowerTargetTopic = "ppc:pc:targets";
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

        private Mock<IPowerTargetHandler> _powerTargetHandler = null!;
        private Mock<IMessageSubscriber> _messageSubscriber = null!;
        private Action<string>? _callback;


        [TestInitialize]
        public void Initialize()
        {
            _callback = null;
            _powerTargetHandler = new Mock<IPowerTargetHandler>();
            _messageSubscriber = new Mock<IMessageSubscriber>();
            _messageSubscriber.Setup(s => s.Subscribe(PowerTargetTopic, It.IsAny<Action<string>>()))
                .Callback<string, Action<string>>((topic, callback) => _callback = callback);
        }


        [TestMethod]
        public void PowerTargetSubscriberNullLoggerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerTargetSubscriber(null!, _powerTargetHandler.Object, _messageSubscriber.Object));
        }


        [TestMethod]
        public void PowerTargetSubscriberNullPowerTargetHandlerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerTargetSubscriber(NullLogger<PowerTargetSubscriber>.Instance, null!,
                _messageSubscriber.Object));
        }


        [TestMethod]
        public void PowerTargetSubscriberNullMessageSubscriberTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerTargetSubscriber(NullLogger<PowerTargetSubscriber>.Instance,
                _powerTargetHandler.Object, null!));
        }


        [TestMethod]
        public void SubscribeRegistersOnTopicTest()
        {
            PowerTargetSubscriber subscriber = CreateSubscriber();

            subscriber.Subscribe(PowerTargetTopic);

            _messageSubscriber.Verify(s => s.Subscribe(PowerTargetTopic, It.IsAny<Action<string>>()), Times.Once);
        }


        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void SubscribeWithoutTopicLogsWarningTest(string? topic)
        {
            FakeLogger<PowerTargetSubscriber> logger = new FakeLogger<PowerTargetSubscriber>();
            PowerTargetSubscriber subscriber = new PowerTargetSubscriber(logger, _powerTargetHandler.Object, _messageSubscriber.Object);

            subscriber.Subscribe(topic);

            Assert.AreEqual(LogLevel.Warning, logger.LatestRecord.Level);
            _messageSubscriber.Verify(s => s.Subscribe(It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Never);
        }


        [TestMethod]
        public async Task RunHandlesReceivedTargetsInOrderTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            List<CurrentTargetsDto> handled = new List<CurrentTargetsDto>();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _powerTargetHandler.Setup(h => h.HandleAsync(It.IsAny<CurrentTargetsDto>()))
                .Callback<CurrentTargetsDto>(targets =>
                {
                    handled.Add(targets);

                    if (handled.Count == 2)
                    {
                        completed.TrySetResult();
                    }
                })
                .Returns(Task.CompletedTask);

            CurrentTargetsDto first = new CurrentTargetsDto { DeviceId = 7, ActivePowerWatt = 25000.5, ReactivePowerVar = -4000.25 };
            CurrentTargetsDto second = new CurrentTargetsDto { DeviceId = 7, ActivePowerWatt = 12000, ReactivePowerVar = 3000 };
            PowerTargetSubscriber subscriber = CreateSubscriber();
            subscriber.Subscribe(PowerTargetTopic);
            Task run = subscriber.RunAsync(cancellation.Token);

            _callback!(JsonSerializer.Serialize(first));
            _callback!(JsonSerializer.Serialize(second));
            await completed.Task.WaitAsync(TestTimeout);
            await cancellation.CancelAsync();
            await run.WaitAsync(TestTimeout);

            CollectionAssert.AreEqual(new List<CurrentTargetsDto> { first, second }, handled);
        }


        [TestMethod]
        [DataRow("not json")]
        [DataRow("null")]
        public async Task RunSkipsInvalidMessageAndHandlesNextTargetsTest(string invalidJson)
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            CurrentTargetsDto valid = new CurrentTargetsDto { DeviceId = 5, ActivePowerWatt = 12000, ReactivePowerVar = 3000 };
            _powerTargetHandler.Setup(h => h.HandleAsync(valid))
                .Callback(() => completed.TrySetResult())
                .Returns(Task.CompletedTask);
            PowerTargetSubscriber subscriber = CreateSubscriber();
            subscriber.Subscribe(PowerTargetTopic);
            Task run = subscriber.RunAsync(cancellation.Token);

            _callback!(invalidJson);
            _callback!(JsonSerializer.Serialize(valid));
            await completed.Task.WaitAsync(TestTimeout);
            await cancellation.CancelAsync();
            await run.WaitAsync(TestTimeout);

            _powerTargetHandler.Verify(h => h.HandleAsync(It.IsAny<CurrentTargetsDto>()), Times.Once);
        }


        [TestMethod]
        public async Task RunFailedHandlingContinuesWithNextTargetsTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            CurrentTargetsDto failing = new CurrentTargetsDto { DeviceId = 99, ActivePowerWatt = 1000, ReactivePowerVar = 0 };
            CurrentTargetsDto valid = new CurrentTargetsDto { DeviceId = 5, ActivePowerWatt = 12000, ReactivePowerVar = 3000 };
            _powerTargetHandler.Setup(h => h.HandleAsync(failing))
                .ThrowsAsync(new InvalidOperationException("Device Manager cannot find PCS with Device ID: 99"));
            _powerTargetHandler.Setup(h => h.HandleAsync(valid))
                .Callback(() => completed.TrySetResult())
                .Returns(Task.CompletedTask);
            PowerTargetSubscriber subscriber = CreateSubscriber();
            subscriber.Subscribe(PowerTargetTopic);
            Task run = subscriber.RunAsync(cancellation.Token);

            _callback!(JsonSerializer.Serialize(failing));
            _callback!(JsonSerializer.Serialize(valid));
            await completed.Task.WaitAsync(TestTimeout);
            await cancellation.CancelAsync();
            await run.WaitAsync(TestTimeout);

            _powerTargetHandler.Verify(h => h.HandleAsync(failing), Times.Once);
            _powerTargetHandler.Verify(h => h.HandleAsync(valid), Times.Once);
        }


        [TestMethod]
        public async Task RunStopsWhenCancelledTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            PowerTargetSubscriber subscriber = CreateSubscriber();
            Task run = subscriber.RunAsync(cancellation.Token);

            await cancellation.CancelAsync();
            await run.WaitAsync(TestTimeout);

            Assert.IsTrue(run.IsCompletedSuccessfully);
        }


        private PowerTargetSubscriber CreateSubscriber()
        {
            return new PowerTargetSubscriber(NullLogger<PowerTargetSubscriber>.Instance, _powerTargetHandler.Object, _messageSubscriber.Object);
        }
    }
}

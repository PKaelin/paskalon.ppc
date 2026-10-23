// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.Messaging;
using paskalON.OperatingModes.Client.Subscribers;
using paskalON.OperatingModes.Dto.Powers;
using System.Text.Json;

namespace paskalON.OperatingModes.Client.UnitTest.Subscribers
{
    [TestClass]
    public sealed class PowerSetpointSubscriberTest
    {
        private const string SetpointTopic = "ppc:om:setpoints";
        private const string DerUnitSetpointTopic = "ppc:om:setpoints:unit";
        private const string DerUnitName = "BESS-Unit 1";
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

        private Mock<IPowerSetpointHandler> _powerSetpointHandler = null!;
        private Mock<IMessageSubscriber> _messageSubscriber = null!;
        private Action<string>? _callback;
        private Action<string>? _derUnitCallback;


        [TestInitialize]
        public void Initialize()
        {
            _callback = null;
            _derUnitCallback = null;
            _powerSetpointHandler = new Mock<IPowerSetpointHandler>();
            _messageSubscriber = new Mock<IMessageSubscriber>();
            _messageSubscriber.Setup(s => s.Subscribe(SetpointTopic, It.IsAny<Action<string>>()))
                .Callback<string, Action<string>>((topic, callback) => _callback = callback);
            _messageSubscriber.Setup(s => s.Subscribe(DerUnitSetpointTopic, It.IsAny<Action<string>>()))
                .Callback<string, Action<string>>((topic, callback) => _derUnitCallback = callback);
        }


        [TestMethod]
        public void PowerSetpointSubscriberNullLoggerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerSetpointSubscriber(null!, _powerSetpointHandler.Object, _messageSubscriber.Object));
        }


        [TestMethod]
        public void PowerSetpointSubscriberNullPowerSetpointHandlerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerSetpointSubscriber(NullLogger<PowerSetpointSubscriber>.Instance, null!,
                _messageSubscriber.Object));
        }


        [TestMethod]
        public void PowerSetpointSubscriberNullMessageSubscriberTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerSetpointSubscriber(NullLogger<PowerSetpointSubscriber>.Instance,
                _powerSetpointHandler.Object, null!));
        }


        [TestMethod]
        public void SubscribeRegistersOnTopicTest()
        {
            PowerSetpointSubscriber subscriber = CreateSubscriber();

            subscriber.Subscribe(SetpointTopic);

            _messageSubscriber.Verify(s => s.Subscribe(SetpointTopic, It.IsAny<Action<string>>()), Times.Once);
            Assert.IsNotNull(_callback);
        }


        [TestMethod]
        public void SubscribeLogsInformationWithTopicTest()
        {
            FakeLogger<PowerSetpointSubscriber> logger = new FakeLogger<PowerSetpointSubscriber>();
            PowerSetpointSubscriber subscriber = new PowerSetpointSubscriber(logger, _powerSetpointHandler.Object, _messageSubscriber.Object);

            subscriber.Subscribe(SetpointTopic);

            Assert.AreEqual(LogLevel.Information, logger.LatestRecord.Level);
            StringAssert.Contains(logger.LatestRecord.Message, SetpointTopic);
        }


        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void SubscribeWithoutTopicLogsWarningTest(string? topic)
        {
            FakeLogger<PowerSetpointSubscriber> logger = new FakeLogger<PowerSetpointSubscriber>();
            PowerSetpointSubscriber subscriber = new PowerSetpointSubscriber(logger, _powerSetpointHandler.Object, _messageSubscriber.Object);

            subscriber.Subscribe(topic);

            Assert.AreEqual(LogLevel.Warning, logger.LatestRecord.Level);
            _messageSubscriber.Verify(s => s.Subscribe(It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Never);
        }


        [TestMethod]
        public async Task RunHandlesReceivedSetpointsInOrderTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            List<CurrentSetpointsDto> handled = new List<CurrentSetpointsDto>();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _powerSetpointHandler.Setup(h => h.HandleSetpointsAsync(It.IsAny<CurrentSetpointsDto>()))
                .Callback<CurrentSetpointsDto>(setpoints =>
                {
                    handled.Add(setpoints);

                    if (handled.Count == 2)
                    {
                        completed.TrySetResult();
                    }
                })
                .Returns(Task.CompletedTask);

            CurrentSetpointsDto first = new CurrentSetpointsDto { SetpointActivePowerWatts = 25000.5, SetpointReactivePowerVars = -4000.25 };
            CurrentSetpointsDto second = new CurrentSetpointsDto { SetpointActivePowerWatts = -12000, SetpointReactivePowerVars = 3000 };
            PowerSetpointSubscriber subscriber = CreateSubscriber();
            subscriber.Subscribe(SetpointTopic);
            Task run = subscriber.RunAsync(cancellation.Token);

            _callback!(JsonSerializer.Serialize(first));
            _callback!(JsonSerializer.Serialize(second));
            await completed.Task.WaitAsync(TestTimeout);
            await cancellation.CancelAsync();
            await run.WaitAsync(TestTimeout);

            Assert.HasCount(2, handled);
            Assert.AreEqual(25000.5, handled[0].SetpointActivePowerWatts);
            Assert.AreEqual(-4000.25, handled[0].SetpointReactivePowerVars);
            Assert.AreEqual(-12000, handled[1].SetpointActivePowerWatts);
            Assert.AreEqual(3000, handled[1].SetpointReactivePowerVars);
        }


        [TestMethod]
        [DataRow("not json")]
        [DataRow("null")]
        public async Task RunSkipsInvalidMessageAndHandlesNextSetpointsTest(string invalidJson)
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _powerSetpointHandler.Setup(h => h.HandleSetpointsAsync(It.Is<CurrentSetpointsDto>(s => s.SetpointActivePowerWatts == 12000)))
                .Callback(() => completed.TrySetResult())
                .Returns(Task.CompletedTask);
            PowerSetpointSubscriber subscriber = CreateSubscriber();
            subscriber.Subscribe(SetpointTopic);
            Task run = subscriber.RunAsync(cancellation.Token);

            _callback!(invalidJson);
            _callback!(JsonSerializer.Serialize(new CurrentSetpointsDto { SetpointActivePowerWatts = 12000, SetpointReactivePowerVars = 3000 }));
            await completed.Task.WaitAsync(TestTimeout);
            await cancellation.CancelAsync();
            await run.WaitAsync(TestTimeout);

            _powerSetpointHandler.Verify(h => h.HandleSetpointsAsync(It.IsAny<CurrentSetpointsDto>()), Times.Once);
        }


        [TestMethod]
        public async Task RunFailedHandlingContinuesWithNextSetpointsTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _powerSetpointHandler.Setup(h => h.HandleSetpointsAsync(It.Is<CurrentSetpointsDto>(s => s.SetpointActivePowerWatts == 1000)))
                .ThrowsAsync(new InvalidOperationException("System power control is not initialized"));
            _powerSetpointHandler.Setup(h => h.HandleSetpointsAsync(It.Is<CurrentSetpointsDto>(s => s.SetpointActivePowerWatts == 12000)))
                .Callback(() => completed.TrySetResult())
                .Returns(Task.CompletedTask);
            PowerSetpointSubscriber subscriber = CreateSubscriber();
            subscriber.Subscribe(SetpointTopic);
            Task run = subscriber.RunAsync(cancellation.Token);

            _callback!(JsonSerializer.Serialize(new CurrentSetpointsDto { SetpointActivePowerWatts = 1000, SetpointReactivePowerVars = 0 }));
            _callback!(JsonSerializer.Serialize(new CurrentSetpointsDto { SetpointActivePowerWatts = 12000, SetpointReactivePowerVars = 3000 }));
            await completed.Task.WaitAsync(TestTimeout);
            await cancellation.CancelAsync();
            await run.WaitAsync(TestTimeout);

            _powerSetpointHandler.Verify(h => h.HandleSetpointsAsync(It.IsAny<CurrentSetpointsDto>()), Times.Exactly(2));
        }


        [TestMethod]
        public async Task RunStopsWhenCancelledTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            PowerSetpointSubscriber subscriber = CreateSubscriber();
            Task run = subscriber.RunAsync(cancellation.Token);

            await cancellation.CancelAsync();
            await run.WaitAsync(TestTimeout);

            Assert.IsTrue(run.IsCompletedSuccessfully);
        }


        [TestMethod]
        public void SubscribeDerUnitRegistersOnTopicTest()
        {
            PowerSetpointSubscriber subscriber = CreateSubscriber();

            subscriber.SubscribeDerUnit(DerUnitSetpointTopic);

            _messageSubscriber.Verify(s => s.Subscribe(DerUnitSetpointTopic, It.IsAny<Action<string>>()), Times.Once);
            Assert.IsNotNull(_derUnitCallback);
        }


        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void SubscribeDerUnitWithoutTopicLogsWarningTest(string? topic)
        {
            FakeLogger<PowerSetpointSubscriber> logger = new FakeLogger<PowerSetpointSubscriber>();
            PowerSetpointSubscriber subscriber = new PowerSetpointSubscriber(logger, _powerSetpointHandler.Object, _messageSubscriber.Object);

            subscriber.SubscribeDerUnit(topic);

            Assert.AreEqual(LogLevel.Warning, logger.LatestRecord.Level);
            _messageSubscriber.Verify(s => s.Subscribe(It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Never);
        }


        [TestMethod]
        public async Task RunHandlesReceivedDerUnitSetpointsTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            CurrentDerUnitSetpointsDto setpoint = new CurrentDerUnitSetpointsDto
            {
                DerUnitName = DerUnitName,
                SetpointActivePowerWatts = -30000,
                SetpointReactivePowerVars = 5000
            };
            _powerSetpointHandler.Setup(h => h.HandleDerUnitSetpointsAsync(setpoint))
                .Callback(() => completed.TrySetResult())
                .Returns(Task.CompletedTask);
            PowerSetpointSubscriber subscriber = CreateSubscriber();
            subscriber.SubscribeDerUnit(DerUnitSetpointTopic);
            Task run = subscriber.RunAsync(cancellation.Token);

            _derUnitCallback!(JsonSerializer.Serialize(setpoint));
            await completed.Task.WaitAsync(TestTimeout);
            await cancellation.CancelAsync();
            await run.WaitAsync(TestTimeout);

            _powerSetpointHandler.Verify(h => h.HandleDerUnitSetpointsAsync(setpoint), Times.Once);
            _powerSetpointHandler.Verify(h => h.HandleSetpointsAsync(It.IsAny<CurrentSetpointsDto>()), Times.Never);
        }


        [TestMethod]
        public async Task RunHandlesSystemAndDerUnitSetpointsInOrderTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            List<string> handled = new List<string>();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _powerSetpointHandler.Setup(h => h.HandleSetpointsAsync(It.IsAny<CurrentSetpointsDto>()))
                .Callback(() => handled.Add("System"))
                .Returns(Task.CompletedTask);
            _powerSetpointHandler.Setup(h => h.HandleDerUnitSetpointsAsync(It.IsAny<CurrentDerUnitSetpointsDto>()))
                .Callback<CurrentDerUnitSetpointsDto>(setpoints =>
                {
                    handled.Add(setpoints.DerUnitName);

                    if (handled.Count == 3)
                    {
                        completed.TrySetResult();
                    }
                })
                .Returns(Task.CompletedTask);
            PowerSetpointSubscriber subscriber = CreateSubscriber();
            subscriber.Subscribe(SetpointTopic);
            subscriber.SubscribeDerUnit(DerUnitSetpointTopic);
            Task run = subscriber.RunAsync(cancellation.Token);

            _derUnitCallback!(JsonSerializer.Serialize(new CurrentDerUnitSetpointsDto { DerUnitName = "BESS-Unit 1", SetpointActivePowerWatts = 1000, SetpointReactivePowerVars = 0 }));
            _callback!(JsonSerializer.Serialize(new CurrentSetpointsDto { SetpointActivePowerWatts = 100000, SetpointReactivePowerVars = 20000 }));
            _derUnitCallback!(JsonSerializer.Serialize(new CurrentDerUnitSetpointsDto { DerUnitName = "BESS-Unit 2", SetpointActivePowerWatts = 2000, SetpointReactivePowerVars = 0 }));
            await completed.Task.WaitAsync(TestTimeout);
            await cancellation.CancelAsync();
            await run.WaitAsync(TestTimeout);

            CollectionAssert.AreEqual(new List<string> { "BESS-Unit 1", "System", "BESS-Unit 2" }, handled);
        }


        [TestMethod]
        public async Task RunRejectedDerUnitSetpointContinuesWithNextSetpointsTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            CurrentDerUnitSetpointsDto rejected = new CurrentDerUnitSetpointsDto { DerUnitName = "BESS-Unit 9", SetpointActivePowerWatts = 1000, SetpointReactivePowerVars = 0 };
            CurrentDerUnitSetpointsDto valid = new CurrentDerUnitSetpointsDto { DerUnitName = DerUnitName, SetpointActivePowerWatts = 12000, SetpointReactivePowerVars = 3000 };
            _powerSetpointHandler.Setup(h => h.HandleDerUnitSetpointsAsync(rejected))
                .ThrowsAsync(new KeyNotFoundException("DER unit BESS-Unit 9 does not exist"));
            _powerSetpointHandler.Setup(h => h.HandleDerUnitSetpointsAsync(valid))
                .Callback(() => completed.TrySetResult())
                .Returns(Task.CompletedTask);
            PowerSetpointSubscriber subscriber = CreateSubscriber();
            subscriber.SubscribeDerUnit(DerUnitSetpointTopic);
            Task run = subscriber.RunAsync(cancellation.Token);

            _derUnitCallback!(JsonSerializer.Serialize(rejected));
            _derUnitCallback!(JsonSerializer.Serialize(valid));
            await completed.Task.WaitAsync(TestTimeout);
            await cancellation.CancelAsync();
            await run.WaitAsync(TestTimeout);

            _powerSetpointHandler.Verify(h => h.HandleDerUnitSetpointsAsync(It.IsAny<CurrentDerUnitSetpointsDto>()), Times.Exactly(2));
        }


        private PowerSetpointSubscriber CreateSubscriber()
        {
            return new PowerSetpointSubscriber(NullLogger<PowerSetpointSubscriber>.Instance, _powerSetpointHandler.Object, _messageSubscriber.Object);
        }
    }
}

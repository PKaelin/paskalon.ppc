// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.Messaging;
using paskalON.OperatingModes.Dto.Powers;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Application.Subscribers;
using System.Text.Json;

namespace paskalON.PowerControls.Application.UnitTest.Subscribers
{
    [TestClass]
    public sealed class PowerSetpointSubscriberTest
    {
        private const string SetpointTopic = "ppc:om:setpoints";
        private const string DerUnitSetpointTopic = "ppc:om:setpoints:unit";
        private const string DerUnitName = "BESS-Unit 1";
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

        private Mock<IPowerControlManager> _powerControlManager = null!;
        private Mock<IMessageSubscriber> _messageSubscriber = null!;
        private Action<string>? _callback;
        private Action<string>? _derUnitCallback;


        [TestInitialize]
        public void Initialize()
        {
            _callback = null;
            _derUnitCallback = null;
            _powerControlManager = new Mock<IPowerControlManager>();
            _messageSubscriber = new Mock<IMessageSubscriber>();
            _messageSubscriber.Setup(s => s.Subscribe(SetpointTopic, It.IsAny<Action<string>>()))
                .Callback<string, Action<string>>((topic, callback) => _callback = callback);
            _messageSubscriber.Setup(s => s.Subscribe(DerUnitSetpointTopic, It.IsAny<Action<string>>()))
                .Callback<string, Action<string>>((topic, callback) => _derUnitCallback = callback);
        }


        [TestMethod]
        public void PowerSetpointSubscriberNullLoggerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerSetpointSubscriber(null!, _powerControlManager.Object, _messageSubscriber.Object));
        }


        [TestMethod]
        public void PowerSetpointSubscriberNullPowerControlManagerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerSetpointSubscriber(NullLogger<PowerSetpointSubscriber>.Instance, null!,
                _messageSubscriber.Object));
        }


        [TestMethod]
        public void PowerSetpointSubscriberNullMessageSubscriberTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerSetpointSubscriber(NullLogger<PowerSetpointSubscriber>.Instance,
                _powerControlManager.Object, null!));
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
            PowerSetpointSubscriber subscriber = new PowerSetpointSubscriber(logger, _powerControlManager.Object, _messageSubscriber.Object);

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
            PowerSetpointSubscriber subscriber = new PowerSetpointSubscriber(logger, _powerControlManager.Object, _messageSubscriber.Object);

            subscriber.Subscribe(topic);

            Assert.AreEqual(LogLevel.Warning, logger.LatestRecord.Level);
            _messageSubscriber.Verify(s => s.Subscribe(It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Never);
        }


        [TestMethod]
        public async Task RunAppliesReceivedSetpointsInOrderTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            List<(ActivePower ActivePower, ReactivePower ReactivePower)> applied = new List<(ActivePower ActivePower, ReactivePower ReactivePower)>();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _powerControlManager.Setup(m => m.SetSystemPowerTarget(It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()))
                .Callback<ActivePower, ReactivePower>((activePower, reactivePower) =>
                {
                    applied.Add((activePower, reactivePower));

                    if (applied.Count == 2)
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

            Assert.HasCount(2, applied);
            Assert.AreEqual(new ActivePower(25000.5), applied[0].ActivePower);
            Assert.AreEqual(new ReactivePower(-4000.25), applied[0].ReactivePower);
            Assert.AreEqual(new ActivePower(-12000), applied[1].ActivePower);
            Assert.AreEqual(new ReactivePower(3000), applied[1].ReactivePower);
        }


        [TestMethod]
        [DataRow("not json")]
        [DataRow("null")]
        public async Task RunSkipsInvalidMessageAndAppliesNextSetpointsTest(string invalidJson)
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _powerControlManager.Setup(m => m.SetSystemPowerTarget(new ActivePower(12000), new ReactivePower(3000)))
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

            _powerControlManager.Verify(m => m.SetSystemPowerTarget(It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()), Times.Once);
        }


        [TestMethod]
        public async Task RunFailedApplyContinuesWithNextSetpointsTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _powerControlManager.Setup(m => m.SetSystemPowerTarget(new ActivePower(1000), new ReactivePower(0)))
                .ThrowsAsync(new InvalidOperationException("System power control is not initialized"));
            _powerControlManager.Setup(m => m.SetSystemPowerTarget(new ActivePower(12000), new ReactivePower(3000)))
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

            _powerControlManager.Verify(m => m.SetSystemPowerTarget(new ActivePower(1000), new ReactivePower(0)), Times.Once);
            _powerControlManager.Verify(m => m.SetSystemPowerTarget(new ActivePower(12000), new ReactivePower(3000)), Times.Once);
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
            PowerSetpointSubscriber subscriber = new PowerSetpointSubscriber(logger, _powerControlManager.Object, _messageSubscriber.Object);

            subscriber.SubscribeDerUnit(topic);

            Assert.AreEqual(LogLevel.Warning, logger.LatestRecord.Level);
            _messageSubscriber.Verify(s => s.Subscribe(It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Never);
        }


        [TestMethod]
        public async Task RunAppliesReceivedDerUnitSetpointsTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _powerControlManager.Setup(m => m.SetDerUnitPowerTarget(DerUnitName, new ActivePower(-30000), new ReactivePower(5000)))
                .Callback(() => completed.TrySetResult())
                .Returns(Task.CompletedTask);
            CurrentDerUnitSetpointsDto setpoint = new CurrentDerUnitSetpointsDto
            {
                DerUnitName = DerUnitName,
                SetpointActivePowerWatts = -30000,
                SetpointReactivePowerVars = 5000
            };
            PowerSetpointSubscriber subscriber = CreateSubscriber();
            subscriber.SubscribeDerUnit(DerUnitSetpointTopic);
            Task run = subscriber.RunAsync(cancellation.Token);

            _derUnitCallback!(JsonSerializer.Serialize(setpoint));
            await completed.Task.WaitAsync(TestTimeout);
            await cancellation.CancelAsync();
            await run.WaitAsync(TestTimeout);

            _powerControlManager.Verify(m => m.SetDerUnitPowerTarget(DerUnitName, new ActivePower(-30000), new ReactivePower(5000)), Times.Once);
            _powerControlManager.Verify(m => m.SetSystemPowerTarget(It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()), Times.Never);
        }


        [TestMethod]
        public async Task RunAppliesSystemAndDerUnitSetpointsInOrderTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            List<string> applied = new List<string>();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _powerControlManager.Setup(m => m.SetSystemPowerTarget(It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()))
                .Callback(() => applied.Add("System"))
                .Returns(Task.CompletedTask);
            _powerControlManager.Setup(m => m.SetDerUnitPowerTarget(It.IsAny<string>(), It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()))
                .Callback<string, ActivePower, ReactivePower>((name, activePower, reactivePower) =>
                {
                    applied.Add(name);

                    if (applied.Count == 3)
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

            CollectionAssert.AreEqual(new List<string> { "BESS-Unit 1", "System", "BESS-Unit 2" }, applied);
        }


        [TestMethod]
        public async Task RunRejectedDerUnitSetpointContinuesWithNextSetpointsTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _powerControlManager.Setup(m => m.SetDerUnitPowerTarget("BESS-Unit 9", It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()))
                .ThrowsAsync(new KeyNotFoundException("DER unit BESS-Unit 9 does not exist"));
            _powerControlManager.Setup(m => m.SetDerUnitPowerTarget(DerUnitName, new ActivePower(12000), new ReactivePower(3000)))
                .Callback(() => completed.TrySetResult())
                .Returns(Task.CompletedTask);
            PowerSetpointSubscriber subscriber = CreateSubscriber();
            subscriber.SubscribeDerUnit(DerUnitSetpointTopic);
            Task run = subscriber.RunAsync(cancellation.Token);

            _derUnitCallback!(JsonSerializer.Serialize(new CurrentDerUnitSetpointsDto { DerUnitName = "BESS-Unit 9", SetpointActivePowerWatts = 1000, SetpointReactivePowerVars = 0 }));
            _derUnitCallback!(JsonSerializer.Serialize(new CurrentDerUnitSetpointsDto { DerUnitName = DerUnitName, SetpointActivePowerWatts = 12000, SetpointReactivePowerVars = 3000 }));
            await completed.Task.WaitAsync(TestTimeout);
            await cancellation.CancelAsync();
            await run.WaitAsync(TestTimeout);

            _powerControlManager.Verify(m => m.SetDerUnitPowerTarget(It.IsAny<string>(), It.IsAny<ActivePower>(), It.IsAny<ReactivePower>()), Times.Exactly(2));
        }


        private PowerSetpointSubscriber CreateSubscriber()
        {
            return new PowerSetpointSubscriber(NullLogger<PowerSetpointSubscriber>.Instance, _powerControlManager.Object, _messageSubscriber.Object);
        }
    }
}

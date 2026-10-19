// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.Devices.Application.Subscribers;
using paskalON.Messaging;
using paskalON.PowerControls.Dto.Powers;
using System.Text.Json;

namespace paskalON.Devices.Application.UnitTest.Subscribers
{
    [TestClass]
    public sealed class PowerTargetSubscriberTest
    {
        private const string PowerTargetTopic = "ppc:pc:targets";
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

        private Mock<IDeviceManager> _deviceManager = null!;
        private Mock<IMessageSubscriber> _messageSubscriber = null!;
        private Action<string>? _callback;


        [TestInitialize]
        public void Initialize()
        {
            _callback = null;
            _deviceManager = new Mock<IDeviceManager>();
            _messageSubscriber = new Mock<IMessageSubscriber>();
            _messageSubscriber.Setup(s => s.Subscribe(PowerTargetTopic, It.IsAny<Action<string>>()))
                .Callback<string, Action<string>>((topic, callback) => _callback = callback);
        }


        [TestMethod]
        public void PowerTargetSubscriberNullLoggerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerTargetSubscriber(null!, _deviceManager.Object, _messageSubscriber.Object));
        }


        [TestMethod]
        public void PowerTargetSubscriberNullDeviceManagerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerTargetSubscriber(NullLogger<PowerTargetSubscriber>.Instance, null!,
                _messageSubscriber.Object));
        }


        [TestMethod]
        public void PowerTargetSubscriberNullMessageSubscriberTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new PowerTargetSubscriber(NullLogger<PowerTargetSubscriber>.Instance,
                _deviceManager.Object, null!));
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
            PowerTargetSubscriber subscriber = new PowerTargetSubscriber(logger, _deviceManager.Object, _messageSubscriber.Object);

            subscriber.Subscribe(topic);

            Assert.AreEqual(LogLevel.Warning, logger.LatestRecord.Level);
            _messageSubscriber.Verify(s => s.Subscribe(It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Never);
        }


        [TestMethod]
        public async Task RunAppliesReceivedTargetsInOrderTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            List<CurrentTargetsDto> applied = new List<CurrentTargetsDto>();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _deviceManager.Setup(m => m.SetPcsPowerTarget(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>()))
                .Callback<int, double, double>((deviceId, activePower, reactivePower) =>
                {
                    applied.Add(new CurrentTargetsDto { DeviceId = deviceId, ActivePowerWatt = activePower, ReactivePowerVar = reactivePower });

                    if (applied.Count == 2)
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

            CollectionAssert.AreEqual(new List<CurrentTargetsDto> { first, second }, applied);
        }


        [TestMethod]
        [DataRow("not json")]
        [DataRow("null")]
        public async Task RunSkipsInvalidMessageAndAppliesNextTargetsTest(string invalidJson)
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _deviceManager.Setup(m => m.SetPcsPowerTarget(5, 12000, 3000))
                .Callback(() => completed.TrySetResult())
                .Returns(Task.CompletedTask);
            PowerTargetSubscriber subscriber = CreateSubscriber();
            subscriber.Subscribe(PowerTargetTopic);
            Task run = subscriber.RunAsync(cancellation.Token);

            _callback!(invalidJson);
            _callback!(JsonSerializer.Serialize(new CurrentTargetsDto { DeviceId = 5, ActivePowerWatt = 12000, ReactivePowerVar = 3000 }));
            await completed.Task.WaitAsync(TestTimeout);
            await cancellation.CancelAsync();
            await run.WaitAsync(TestTimeout);

            _deviceManager.Verify(m => m.SetPcsPowerTarget(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>()), Times.Once);
        }


        [TestMethod]
        public async Task RunFailedApplyContinuesWithNextTargetsTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            TaskCompletionSource completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _deviceManager.Setup(m => m.SetPcsPowerTarget(99, 1000, 0))
                .ThrowsAsync(new InvalidOperationException("Device Manager cannot find PCS with Device ID: 99"));
            _deviceManager.Setup(m => m.SetPcsPowerTarget(5, 12000, 3000))
                .Callback(() => completed.TrySetResult())
                .Returns(Task.CompletedTask);
            PowerTargetSubscriber subscriber = CreateSubscriber();
            subscriber.Subscribe(PowerTargetTopic);
            Task run = subscriber.RunAsync(cancellation.Token);

            _callback!(JsonSerializer.Serialize(new CurrentTargetsDto { DeviceId = 99, ActivePowerWatt = 1000, ReactivePowerVar = 0 }));
            _callback!(JsonSerializer.Serialize(new CurrentTargetsDto { DeviceId = 5, ActivePowerWatt = 12000, ReactivePowerVar = 3000 }));
            await completed.Task.WaitAsync(TestTimeout);
            await cancellation.CancelAsync();
            await run.WaitAsync(TestTimeout);

            _deviceManager.Verify(m => m.SetPcsPowerTarget(99, 1000, 0), Times.Once);
            _deviceManager.Verify(m => m.SetPcsPowerTarget(5, 12000, 3000), Times.Once);
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
            return new PowerTargetSubscriber(NullLogger<PowerTargetSubscriber>.Instance, _deviceManager.Object, _messageSubscriber.Object);
        }
    }
}

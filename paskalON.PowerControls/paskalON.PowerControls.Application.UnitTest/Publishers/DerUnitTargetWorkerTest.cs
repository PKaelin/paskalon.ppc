// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.Devices.Client;
using paskalON.Messaging;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Application.Publishers;
using paskalON.PowerControls.Domain.Ders;
using paskalON.PowerControls.Dto.Powers;
using System.Text.Json;

namespace paskalON.PowerControls.Application.UnitTest.Publishers
{
    [TestClass]
    public sealed class DerUnitTargetWorkerTest
    {
        private const int PcsDeviceId = 7;
        private const string PowerTargetTopic = "ppc:pc:targets";
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

        private readonly DerUnitTargetPublisherOptions _options = new DerUnitTargetPublisherOptions
        {
            StartTimeout = TimeSpan.FromSeconds(30),
            StatePollInterval = TimeSpan.FromMilliseconds(500),
            RetryDelay = TimeSpan.FromSeconds(5)
        };

        private Mock<IDerUnitPowerControl> _unit = null!;
        private Mock<IDeviceServer> _deviceServer = null!;
        private Mock<IMessagePublisher> _messagePublisher = null!;
        private FakeTimeProvider _timeProvider = null!;


        [TestInitialize]
        public void Initialize()
        {
            _unit = new Mock<IDerUnitPowerControl>();
            _unit.SetupGet(u => u.PcsDeviceId).Returns(PcsDeviceId);
            _unit.SetupGet(u => u.TargetActivePower).Returns(new ActivePower(25000));
            _unit.SetupGet(u => u.TargetReactivePower).Returns(new ReactivePower(-4000));
            _deviceServer = new Mock<IDeviceServer>();
            _messagePublisher = new Mock<IMessagePublisher>();
            _messagePublisher.Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            _timeProvider = new FakeTimeProvider();
        }


        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void DerUnitTargetWorkerInvalidTopicTest(string? topic)
        {
            Assert.Throws<ArgumentException>(() => new DerUnitTargetWorker(NullLogger.Instance, _unit.Object, _deviceServer.Object,
                _messagePublisher.Object, topic!, _options, _timeProvider));
        }


        [TestMethod]
        public void DerUnitTargetWorkerNullMessagePublisherTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitTargetWorker(NullLogger.Instance, _unit.Object, _deviceServer.Object,
                null!, PowerTargetTopic, _options, _timeProvider));
        }


        [TestMethod]
        public async Task DeliverStartedUnitPublishesTargetsWithoutStartTest()
        {
            string? publishedJson = null;
            _unit.SetupGet(u => u.State).Returns(DerState.Started);
            _messagePublisher.Setup(p => p.PublishAsync(PowerTargetTopic, It.IsAny<string>()))
                .Callback<string, string>((topic, json) => publishedJson = json)
                .Returns(Task.CompletedTask);
            DerUnitTargetWorker worker = CreateWorker();

            bool isDelivered = await worker.DeliverAsync(CancellationToken.None);

            Assert.IsTrue(isDelivered);
            _deviceServer.Verify(s => s.StartPcs(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _messagePublisher.Verify(p => p.PublishAsync(PowerTargetTopic, It.IsAny<string>()), Times.Once);
            Assert.IsNotNull(publishedJson);
            CurrentTargetsDto? dto = JsonSerializer.Deserialize<CurrentTargetsDto>(publishedJson);
            Assert.AreEqual(new CurrentTargetsDto { DeviceId = PcsDeviceId, ActivePowerWatt = 25000, ReactivePowerVar = -4000 }, dto);
        }


        [TestMethod]
        [DataRow(DerState.Stopped)]
        [DataRow(DerState.Standby)]
        public async Task DeliverNotStartedUnitStartsAndPublishesTargetsWhenStartedTest(DerState initialState)
        {
            _unit.SetupSequence(u => u.State)
                .Returns(initialState)
                .Returns(initialState)
                .Returns(DerState.Started);
            DerUnitTargetWorker worker = CreateWorker();

            Task<bool> delivery = worker.DeliverAsync(CancellationToken.None);
            _messagePublisher.Verify(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _timeProvider.Advance(_options.StatePollInterval);
            bool isDelivered = await delivery.WaitAsync(TestTimeout);

            Assert.IsTrue(isDelivered);
            _deviceServer.Verify(s => s.StartPcs(PcsDeviceId, It.IsAny<CancellationToken>()), Times.Once);
            _messagePublisher.Verify(p => p.PublishAsync(PowerTargetTopic, It.IsAny<string>()), Times.Once);
        }


        [TestMethod]
        public async Task DeliverUnitInMaintenanceSkipsStartAndTargetsTest()
        {
            _unit.SetupGet(u => u.State).Returns(DerState.Maintenance);
            DerUnitTargetWorker worker = CreateWorker();

            bool isDelivered = await worker.DeliverAsync(CancellationToken.None);

            Assert.IsTrue(isDelivered);
            _deviceServer.Verify(s => s.StartPcs(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _messagePublisher.Verify(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [TestMethod]
        public async Task DeliverUnitInMaintenancePublishesMaintenanceTargetWithoutStartTest()
        {
            string? publishedJson = null;
            _unit.SetupGet(u => u.State).Returns(DerState.Maintenance);
            _unit.SetupGet(u => u.IsMaintenanceTargetSet).Returns(true);
            _unit.SetupGet(u => u.MaintenanceTargetActivePower).Returns(new ActivePower(-15000));
            _unit.SetupGet(u => u.MaintenanceTargetReactivePower).Returns(new ReactivePower(2000));
            _messagePublisher.Setup(p => p.PublishAsync(PowerTargetTopic, It.IsAny<string>()))
                .Callback<string, string>((topic, json) => publishedJson = json)
                .Returns(Task.CompletedTask);
            DerUnitTargetWorker worker = CreateWorker();

            bool isDelivered = await worker.DeliverAsync(CancellationToken.None);

            Assert.IsTrue(isDelivered);
            _deviceServer.Verify(s => s.StartPcs(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _messagePublisher.Verify(p => p.PublishAsync(PowerTargetTopic, It.IsAny<string>()), Times.Once);
            Assert.IsNotNull(publishedJson);
            CurrentTargetsDto? dto = JsonSerializer.Deserialize<CurrentTargetsDto>(publishedJson);
            Assert.AreEqual(new CurrentTargetsDto { DeviceId = PcsDeviceId, ActivePowerWatt = -15000, ReactivePowerVar = 2000 }, dto);
        }


        [TestMethod]
        public async Task DeliverStartedUnitIgnoresMaintenanceTargetTest()
        {
            string? publishedJson = null;
            _unit.SetupGet(u => u.State).Returns(DerState.Started);
            _unit.SetupGet(u => u.IsMaintenanceTargetSet).Returns(true);
            _unit.SetupGet(u => u.MaintenanceTargetActivePower).Returns(new ActivePower(-15000));
            _unit.SetupGet(u => u.MaintenanceTargetReactivePower).Returns(new ReactivePower(2000));
            _messagePublisher.Setup(p => p.PublishAsync(PowerTargetTopic, It.IsAny<string>()))
                .Callback<string, string>((topic, json) => publishedJson = json)
                .Returns(Task.CompletedTask);
            DerUnitTargetWorker worker = CreateWorker();

            bool isDelivered = await worker.DeliverAsync(CancellationToken.None);

            Assert.IsTrue(isDelivered);
            Assert.IsNotNull(publishedJson);
            CurrentTargetsDto? dto = JsonSerializer.Deserialize<CurrentTargetsDto>(publishedJson);
            Assert.AreEqual(new CurrentTargetsDto { DeviceId = PcsDeviceId, ActivePowerWatt = 25000, ReactivePowerVar = -4000 }, dto);
        }


        [TestMethod]
        public async Task DeliverStartTimeoutReturnsFalseWithoutPublishingTargetsTest()
        {
            DerUnitTargetPublisherOptions options = new DerUnitTargetPublisherOptions
            {
                StartTimeout = TimeSpan.FromSeconds(30),
                StatePollInterval = TimeSpan.FromSeconds(30)
            };

            _unit.SetupGet(u => u.State).Returns(DerState.Stopped);
            DerUnitTargetWorker worker = CreateWorker(options);

            Task<bool> delivery = worker.DeliverAsync(CancellationToken.None);
            _timeProvider.Advance(options.StartTimeout);
            bool isDelivered = await delivery.WaitAsync(TestTimeout);

            Assert.IsFalse(isDelivered);
            _deviceServer.Verify(s => s.StartPcs(PcsDeviceId, It.IsAny<CancellationToken>()), Times.Once);
            _messagePublisher.Verify(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [TestMethod]
        public async Task DeliverMessagePublisherFailureReturnsFalseTest()
        {
            _unit.SetupGet(u => u.State).Returns(DerState.Started);
            _messagePublisher.Setup(p => p.PublishAsync(PowerTargetTopic, It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Message broker unavailable"));
            DerUnitTargetWorker worker = CreateWorker();

            bool isDelivered = await worker.DeliverAsync(CancellationToken.None);

            Assert.IsFalse(isDelivered);
        }


        [TestMethod]
        public async Task DeliverCancelledWhileStartingThrowsOperationCanceledTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            _unit.SetupGet(u => u.State).Returns(DerState.Stopped);
            DerUnitTargetWorker worker = CreateWorker();
            Task<bool> delivery = worker.DeliverAsync(cancellation.Token);

            await cancellation.CancelAsync();

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await delivery.WaitAsync(TestTimeout));
        }


        [TestMethod]
        public async Task RunFailedDeliveryIsRetriedAfterRetryDelayTest()
        {
            using CancellationTokenSource cancellation = new CancellationTokenSource();
            TaskCompletionSource retried = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _unit.SetupGet(u => u.State).Returns(DerState.Started);
            _messagePublisher.SetupSequence(p => p.PublishAsync(PowerTargetTopic, It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Message broker unavailable"))
                .Returns(() =>
                {
                    retried.TrySetResult();

                    return Task.CompletedTask;
                });
            DerUnitTargetWorker worker = CreateWorker();
            worker.Notify();
            Task run = worker.RunAsync(cancellation.Token);

            _timeProvider.Advance(_options.RetryDelay);
            await retried.Task.WaitAsync(TestTimeout);
            await cancellation.CancelAsync();
            await run.WaitAsync(TestTimeout);

            _messagePublisher.Verify(p => p.PublishAsync(PowerTargetTopic, It.IsAny<string>()), Times.Exactly(2));
        }


        [TestMethod]
        public async Task RunCompletedWorkerStopsWithoutDeliveryTest()
        {
            DerUnitTargetWorker worker = CreateWorker();
            Task run = worker.RunAsync(CancellationToken.None);

            worker.Complete();
            worker.Notify();
            await run.WaitAsync(TestTimeout);

            Assert.IsTrue(run.IsCompletedSuccessfully);
            _messagePublisher.Verify(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        private DerUnitTargetWorker CreateWorker(DerUnitTargetPublisherOptions? options = null)
        {
            return new DerUnitTargetWorker(NullLogger.Instance, _unit.Object, _deviceServer.Object, _messagePublisher.Object, PowerTargetTopic,
                options ?? _options, _timeProvider);
        }
    }
}

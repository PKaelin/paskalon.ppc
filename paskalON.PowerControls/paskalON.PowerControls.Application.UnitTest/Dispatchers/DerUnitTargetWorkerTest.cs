// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using paskalON.Devices.Client;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Application.Dispatchers;
using paskalON.PowerControls.Domain.Ders;

namespace paskalON.PowerControls.Application.UnitTest.Dispatchers
{
    [TestClass]
    public sealed class DerUnitTargetWorkerTest
    {
        private const int PcsDeviceId = 7;
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

        private readonly DerUnitTargetDispatcherOptions _options = new DerUnitTargetDispatcherOptions
        {
            StartTimeout = TimeSpan.FromSeconds(30),
            StatePollInterval = TimeSpan.FromMilliseconds(500),
            RetryDelay = TimeSpan.FromSeconds(5)
        };

        private Mock<IDerUnitPowerControl> _unit = null!;
        private Mock<IDeviceServer> _deviceServer = null!;
        private FakeTimeProvider _timeProvider = null!;


        [TestInitialize]
        public void Initialize()
        {
            _unit = new Mock<IDerUnitPowerControl>();
            _unit.SetupGet(u => u.PcsDeviceId).Returns(PcsDeviceId);
            _unit.SetupGet(u => u.TargetActivePower).Returns(new ActivePower(25000));
            _unit.SetupGet(u => u.TargetReactivePower).Returns(new ReactivePower(-4000));
            _deviceServer = new Mock<IDeviceServer>();
            _timeProvider = new FakeTimeProvider();
        }


        [TestMethod]
        public async Task DeliverStartedUnitSendsTargetsWithoutStartTest()
        {
            _unit.SetupGet(u => u.State).Returns(DerState.Started);
            DerUnitTargetWorker worker = CreateWorker();

            bool isDelivered = await worker.DeliverAsync(CancellationToken.None);

            Assert.IsTrue(isDelivered);
            _deviceServer.Verify(s => s.StartPcs(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _deviceServer.Verify(s => s.SetPcsPowerTarget(PcsDeviceId, 25000, -4000, It.IsAny<CancellationToken>()), Times.Once);
        }


        [TestMethod]
        [DataRow(DerState.Stopped)]
        [DataRow(DerState.Standby)]
        public async Task DeliverNotStartedUnitStartsAndSendsTargetsWhenStartedTest(DerState initialState)
        {
            _unit.SetupSequence(u => u.State)
                .Returns(initialState)
                .Returns(initialState)
                .Returns(DerState.Started);
            DerUnitTargetWorker worker = CreateWorker();

            Task<bool> delivery = worker.DeliverAsync(CancellationToken.None);
            _deviceServer.Verify(s => s.SetPcsPowerTarget(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<CancellationToken>()), Times.Never);
            _timeProvider.Advance(_options.StatePollInterval);
            bool isDelivered = await delivery.WaitAsync(TestTimeout);

            Assert.IsTrue(isDelivered);
            _deviceServer.Verify(s => s.StartPcs(PcsDeviceId, It.IsAny<CancellationToken>()), Times.Once);
            _deviceServer.Verify(s => s.SetPcsPowerTarget(PcsDeviceId, 25000, -4000, It.IsAny<CancellationToken>()), Times.Once);
        }


        [TestMethod]
        public async Task DeliverUnitInMaintenanceSkipsStartAndTargetsTest()
        {
            _unit.SetupGet(u => u.State).Returns(DerState.Maintenance);
            DerUnitTargetWorker worker = CreateWorker();

            bool isDelivered = await worker.DeliverAsync(CancellationToken.None);

            Assert.IsTrue(isDelivered);
            _deviceServer.Verify(s => s.StartPcs(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _deviceServer.Verify(s => s.SetPcsPowerTarget(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [TestMethod]
        public async Task DeliverStartTimeoutReturnsFalseWithoutSendingTargetsTest()
        {
            DerUnitTargetDispatcherOptions options = new DerUnitTargetDispatcherOptions
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
            _deviceServer.Verify(s => s.SetPcsPowerTarget(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [TestMethod]
        public async Task DeliverDeviceServerFailureReturnsFalseTest()
        {
            _unit.SetupGet(u => u.State).Returns(DerState.Started);
            _deviceServer.Setup(s => s.SetPcsPowerTarget(PcsDeviceId, 25000, -4000, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Device service unavailable"));
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
            _deviceServer.SetupSequence(s => s.SetPcsPowerTarget(PcsDeviceId, 25000, -4000, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Device service unavailable"))
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

            _deviceServer.Verify(s => s.SetPcsPowerTarget(PcsDeviceId, 25000, -4000, It.IsAny<CancellationToken>()), Times.Exactly(2));
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
            _deviceServer.Verify(s => s.SetPcsPowerTarget(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        private DerUnitTargetWorker CreateWorker(DerUnitTargetDispatcherOptions? options = null)
        {
            return new DerUnitTargetWorker(NullLogger.Instance, _unit.Object, _deviceServer.Object, options ?? _options, _timeProvider);
        }
    }
}

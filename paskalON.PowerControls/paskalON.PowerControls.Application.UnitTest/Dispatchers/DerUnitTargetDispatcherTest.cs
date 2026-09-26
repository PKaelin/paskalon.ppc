// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Devices.Client;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Application.Dispatchers;
using paskalON.PowerControls.Domain.Ders;

namespace paskalON.PowerControls.Application.UnitTest.Dispatchers
{
    [TestClass]
    public sealed class DerUnitTargetDispatcherTest
    {
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

        private readonly DerUnitTargetDispatcherOptions _options = new DerUnitTargetDispatcherOptions();
        private Mock<IDeviceServer> _deviceServer = null!;


        [TestInitialize]
        public void Initialize()
        {
            _deviceServer = new Mock<IDeviceServer>();
        }


        [TestMethod]
        public void DerUnitTargetDispatcherNullLoggerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitTargetDispatcher(null!, _deviceServer.Object, _options, TimeProvider.System));
        }


        [TestMethod]
        public void DerUnitTargetDispatcherNullDeviceServerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitTargetDispatcher(NullLogger<DerUnitTargetDispatcher>.Instance, null!, _options, TimeProvider.System));
        }


        [TestMethod]
        public void DerUnitTargetDispatcherNullOptionsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitTargetDispatcher(NullLogger<DerUnitTargetDispatcher>.Instance, _deviceServer.Object, null!, TimeProvider.System));
        }


        [TestMethod]
        public void DerUnitTargetDispatcherNullTimeProviderTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitTargetDispatcher(NullLogger<DerUnitTargetDispatcher>.Instance, _deviceServer.Object, _options, null!));
        }


        [TestMethod]
        [DataRow(0, 500, 5000)]
        [DataRow(60000, 0, 5000)]
        [DataRow(60000, 500, 0)]
        public void DerUnitTargetDispatcherInvalidOptionsTest(int startTimeoutMs, int statePollIntervalMs, int retryDelayMs)
        {
            DerUnitTargetDispatcherOptions options = new DerUnitTargetDispatcherOptions
            {
                StartTimeout = TimeSpan.FromMilliseconds(startTimeoutMs),
                StatePollInterval = TimeSpan.FromMilliseconds(statePollIntervalMs),
                RetryDelay = TimeSpan.FromMilliseconds(retryDelayMs)
            };

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DerUnitTargetDispatcher(NullLogger<DerUnitTargetDispatcher>.Instance, _deviceServer.Object, options, TimeProvider.System));
        }


        [TestMethod]
        public async Task RegisterNullUnitTest()
        {
            await using DerUnitTargetDispatcher dispatcher = CreateDispatcher();

            Assert.ThrowsExactly<ArgumentNullException>(() => dispatcher.Register(null!));
        }


        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task RegisterUnitWithoutPcsTest(int pcsDeviceId)
        {
            await using DerUnitTargetDispatcher dispatcher = CreateDispatcher();
            Mock<IDerUnitPowerControl> unit = CreateUnit(pcsDeviceId, DerState.Started);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => dispatcher.Register(unit.Object));
        }


        [TestMethod]
        public async Task RegisterSamePcsTwiceTest()
        {
            await using DerUnitTargetDispatcher dispatcher = CreateDispatcher();
            dispatcher.Register(CreateUnit(3, DerState.Started).Object);

            Assert.ThrowsExactly<InvalidOperationException>(() => dispatcher.Register(CreateUnit(3, DerState.Started).Object));
        }


        [TestMethod]
        public async Task RegisterAfterDisposeTest()
        {
            DerUnitTargetDispatcher dispatcher = CreateDispatcher();

            await dispatcher.DisposeAsync();

            Assert.ThrowsExactly<ObjectDisposedException>(() => dispatcher.Register(CreateUnit(3, DerState.Started).Object));
        }


        [TestMethod]
        public async Task RegisterTargetChangeSendsTargetsToUnitPcsTest()
        {
            TaskCompletionSource sent = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _deviceServer.Setup(s => s.SetPcsPowerTarget(5, 12000, 3000, It.IsAny<CancellationToken>()))
                .Callback(() => sent.TrySetResult())
                .Returns(Task.CompletedTask);
            Mock<IDerUnitPowerControl> unit1 = CreateUnit(4, DerState.Started);
            Mock<IDerUnitPowerControl> unit2 = CreateUnit(5, DerState.Started);
            await using DerUnitTargetDispatcher dispatcher = CreateDispatcher();
            dispatcher.Register(unit1.Object);
            dispatcher.Register(unit2.Object);

            unit2.Raise(u => u.TargetPowerChanged += null, unit2.Object, EventArgs.Empty);
            await sent.Task.WaitAsync(TestTimeout);

            _deviceServer.Verify(s => s.SetPcsPowerTarget(5, 12000, 3000, It.IsAny<CancellationToken>()), Times.Once);
            _deviceServer.Verify(s => s.SetPcsPowerTarget(4, It.IsAny<double>(), It.IsAny<double>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        private DerUnitTargetDispatcher CreateDispatcher()
        {
            return new DerUnitTargetDispatcher(NullLogger<DerUnitTargetDispatcher>.Instance, _deviceServer.Object, _options, TimeProvider.System);
        }


        private static Mock<IDerUnitPowerControl> CreateUnit(int pcsDeviceId, DerState state)
        {
            Mock<IDerUnitPowerControl> unit = new Mock<IDerUnitPowerControl>();
            unit.SetupGet(u => u.PcsDeviceId).Returns(pcsDeviceId);
            unit.SetupGet(u => u.State).Returns(state);
            unit.SetupGet(u => u.TargetActivePower).Returns(new ActivePower(12000));
            unit.SetupGet(u => u.TargetReactivePower).Returns(new ReactivePower(3000));

            return unit;
        }
    }
}

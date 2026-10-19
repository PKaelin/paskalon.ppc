// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
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
    public sealed class DerUnitTargetPublisherTest
    {
        private const string PowerTargetTopic = "ppc:pc:targets";
        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

        private readonly DerUnitTargetPublisherOptions _options = new DerUnitTargetPublisherOptions();
        private Mock<IDeviceServer> _deviceServer = null!;
        private Mock<IMessagePublisher> _messagePublisher = null!;


        [TestInitialize]
        public void Initialize()
        {
            _deviceServer = new Mock<IDeviceServer>();
            _messagePublisher = new Mock<IMessagePublisher>();
            _messagePublisher.Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        }


        [TestMethod]
        public void DerUnitTargetPublisherNullLoggerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitTargetPublisher(null!, _deviceServer.Object, _messagePublisher.Object,
                _options, TimeProvider.System));
        }


        [TestMethod]
        public void DerUnitTargetPublisherNullDeviceServerTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitTargetPublisher(NullLogger<DerUnitTargetPublisher>.Instance, null!,
                _messagePublisher.Object, _options, TimeProvider.System));
        }


        [TestMethod]
        public void DerUnitTargetPublisherNullMessagePublisherTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitTargetPublisher(NullLogger<DerUnitTargetPublisher>.Instance, _deviceServer.Object,
                null!, _options, TimeProvider.System));
        }


        [TestMethod]
        public void DerUnitTargetPublisherNullOptionsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitTargetPublisher(NullLogger<DerUnitTargetPublisher>.Instance, _deviceServer.Object,
                _messagePublisher.Object, null!, TimeProvider.System));
        }


        [TestMethod]
        public void DerUnitTargetPublisherNullTimeProviderTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new DerUnitTargetPublisher(NullLogger<DerUnitTargetPublisher>.Instance, _deviceServer.Object,
                _messagePublisher.Object, _options, null!));
        }


        [TestMethod]
        [DataRow(0, 500, 5000)]
        [DataRow(60000, 0, 5000)]
        [DataRow(60000, 500, 0)]
        public void DerUnitTargetPublisherInvalidOptionsTest(int startTimeoutMs, int statePollIntervalMs, int retryDelayMs)
        {
            DerUnitTargetPublisherOptions options = new DerUnitTargetPublisherOptions
            {
                StartTimeout = TimeSpan.FromMilliseconds(startTimeoutMs),
                StatePollInterval = TimeSpan.FromMilliseconds(statePollIntervalMs),
                RetryDelay = TimeSpan.FromMilliseconds(retryDelayMs)
            };

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DerUnitTargetPublisher(NullLogger<DerUnitTargetPublisher>.Instance,
                _deviceServer.Object, _messagePublisher.Object, options, TimeProvider.System));
        }


        [TestMethod]
        public async Task InitializeNullTopicsTest()
        {
            await using DerUnitTargetPublisher publisher = CreatePublisher();

            Assert.ThrowsExactly<ArgumentNullException>(() => publisher.Initialize(null!));
        }


        [TestMethod]
        public async Task RegisterNotInitializedTest()
        {
            await using DerUnitTargetPublisher publisher = CreatePublisher();

            Assert.ThrowsExactly<InvalidOperationException>(() => publisher.Register(CreateUnit(3, DerState.Started).Object));
        }


        [TestMethod]
        public async Task RegisterNullUnitTest()
        {
            await using DerUnitTargetPublisher publisher = CreateInitializedPublisher();

            Assert.ThrowsExactly<ArgumentNullException>(() => publisher.Register(null!));
        }


        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task RegisterUnitWithoutPcsTest(int pcsDeviceId)
        {
            await using DerUnitTargetPublisher publisher = CreateInitializedPublisher();
            Mock<IDerUnitPowerControl> unit = CreateUnit(pcsDeviceId, DerState.Started);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => publisher.Register(unit.Object));
        }


        [TestMethod]
        public async Task RegisterSamePcsTwiceTest()
        {
            await using DerUnitTargetPublisher publisher = CreateInitializedPublisher();
            publisher.Register(CreateUnit(3, DerState.Started).Object);

            Assert.ThrowsExactly<InvalidOperationException>(() => publisher.Register(CreateUnit(3, DerState.Started).Object));
        }


        [TestMethod]
        public async Task RegisterAfterDisposeTest()
        {
            DerUnitTargetPublisher publisher = CreateInitializedPublisher();

            await publisher.DisposeAsync();

            Assert.ThrowsExactly<ObjectDisposedException>(() => publisher.Register(CreateUnit(3, DerState.Started).Object));
        }


        [TestMethod]
        public async Task RegisterTargetChangePublishesTargetsOfUnitPcsTest()
        {
            TaskCompletionSource<string> published = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            _messagePublisher.Setup(p => p.PublishAsync(PowerTargetTopic, It.IsAny<string>()))
                .Callback<string, string>((topic, json) => published.TrySetResult(json))
                .Returns(Task.CompletedTask);
            Mock<IDerUnitPowerControl> unit1 = CreateUnit(4, DerState.Started);
            Mock<IDerUnitPowerControl> unit2 = CreateUnit(5, DerState.Started);
            await using DerUnitTargetPublisher publisher = CreateInitializedPublisher();
            publisher.Register(unit1.Object);
            publisher.Register(unit2.Object);

            unit2.Raise(u => u.TargetPowerChanged += null, unit2.Object, EventArgs.Empty);
            string json = await published.Task.WaitAsync(TestTimeout);

            CurrentTargetsDto? dto = JsonSerializer.Deserialize<CurrentTargetsDto>(json);
            Assert.AreEqual(new CurrentTargetsDto { DeviceId = 5, ActivePowerWatt = 12000, ReactivePowerVar = 3000 }, dto);
            _messagePublisher.Verify(p => p.PublishAsync(PowerTargetTopic, It.IsAny<string>()), Times.Once);
            _deviceServer.Verify(s => s.SetPcsPowerTarget(It.IsAny<int>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task RegisterWithoutTopicSkipsUnitAndLogsWarningTest(string? topic)
        {
            FakeLogger<DerUnitTargetPublisher> logger = new FakeLogger<DerUnitTargetPublisher>();
            Mock<IDerUnitPowerControl> unit = CreateUnit(3, DerState.Stopped);
            await using DerUnitTargetPublisher publisher = new DerUnitTargetPublisher(logger, _deviceServer.Object, _messagePublisher.Object,
                _options, TimeProvider.System);
            publisher.Initialize(new PublisherTopic { PowerTargetTopic = topic });

            publisher.Register(unit.Object);
            publisher.Register(unit.Object);
            unit.Raise(u => u.TargetPowerChanged += null, unit.Object, EventArgs.Empty);

            Assert.AreEqual(3, logger.Collector.GetSnapshot().Count(r => r.Level == LogLevel.Warning));
            _deviceServer.Verify(s => s.StartPcs(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _messagePublisher.Verify(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        private DerUnitTargetPublisher CreatePublisher()
        {
            return new DerUnitTargetPublisher(NullLogger<DerUnitTargetPublisher>.Instance, _deviceServer.Object, _messagePublisher.Object,
                _options, TimeProvider.System);
        }


        private DerUnitTargetPublisher CreateInitializedPublisher()
        {
            DerUnitTargetPublisher publisher = CreatePublisher();
            publisher.Initialize(new PublisherTopic { PowerTargetTopic = PowerTargetTopic });

            return publisher;
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

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.Dataface.C37s;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Devices.Equipments.Meters.PowerMeters.Simples;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Protocols.C37118;
using paskalON.Telemetry;

namespace paskalON.Devices.Equipments.UnitTest.Meters.PowerMeters.Simples
{
    [TestClass]
    public class SystemPowerMeterSimpleV1ProxyTest
    {
        private SystemPowerMeterConfig? _pmConfig;


        [TestInitialize]
        public void TestInitialize()
        {
            Mock<DerConfig> derConfig = new Mock<DerConfig>();
            derConfig.SetupGet(x => x.Name).Returns("DerConfig");
            Mock<PowerMeterDeviceConfig> device = new Mock<PowerMeterDeviceConfig>();
            device.SetupGet(x => x.Name).Returns("PowerMeterDeviceConfig");

            _pmConfig = new SystemPowerMeterConfig
            {
                ChangedBy = "Test",
                Name = "SystemPowerMeterConfig",
                IsActive = true,
                DeviceId = 1,
                PowerFactorStandard = PowerFactorStandard.IEEE,
                DerConfig = derConfig.Object,
                PowerMeterDeviceConfig = device.Object
            };
        }


        [TestMethod]
        public void CreatePowerMeterWithNullClientTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IC37Dataface> dataface = new Mock<IC37Dataface>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new SystemPowerMeterSimpleV1Proxy(NullLogger.Instance, _pmConfig!, publisher.Object, dataface.Object, null!));
        }


        [TestMethod]
        public void CreatePowerMeterWithMockedClientTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IC37Dataface> dataface = new Mock<IC37Dataface>();
            Mock<IC37Client> client = new Mock<IC37Client>();

            SystemPowerMeterSimpleV1Proxy pm = new SystemPowerMeterSimpleV1Proxy(NullLogger.Instance, _pmConfig!, publisher.Object, dataface.Object, client.Object);

            Assert.AreEqual(_pmConfig!.Name, pm.Name);
        }


        [TestMethod]
        public void PowerMeterWithMockedClientComErrorTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IC37Dataface> dataface = new Mock<IC37Dataface>();
            Mock<IC37Client> client = new Mock<IC37Client>();
            FakeLogger<SystemPowerMeterSimpleV1Proxy> logger = new FakeLogger<SystemPowerMeterSimpleV1Proxy>();

            SystemPowerMeterSimpleV1Proxy pm = new SystemPowerMeterSimpleV1Proxy(logger, _pmConfig!, publisher.Object, dataface.Object, client.Object);
            EventArgs expectedEvent = new EventArgs();
            client.Raise(x => x.OnCommunicationError += null, this, expectedEvent);

            Assert.AreEqual(_pmConfig!.Name, pm.Name);
            Assert.IsTrue(pm.CommunicationError);
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.HasCount(1, logs);
            Assert.IsTrue(logs.First().Message.Contains("CommunicationError state", StringComparison.OrdinalIgnoreCase));
        }


        [TestMethod]
        public async Task PowerMeterConnectTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IC37Dataface> dataface = new Mock<IC37Dataface>();
            Mock<IC37Client> client = new Mock<IC37Client>();
            client.Setup(x => x.SendCommandAsync(C37CommandType.TurnOnTransmission));

            SystemPowerMeterSimpleV1Proxy pm = new SystemPowerMeterSimpleV1Proxy(NullLogger.Instance, _pmConfig!, publisher.Object, dataface.Object, client.Object);

            await pm.ConnectAsync();

            client.Verify(x => x.SendCommandAsync(It.IsAny<C37CommandType>()), Times.Once);
            Assert.AreEqual(PowerMeterState.Connecting, pm.State);
        }


        [TestMethod]
        public async Task PowerMeterDisconnectTest()
        {
            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            Mock<IC37Dataface> dataface = new Mock<IC37Dataface>();
            Mock<IC37Client> client = new Mock<IC37Client>();
            client.Setup(x => x.SendCommandAsync(C37CommandType.TurnOffTransmission));

            SystemPowerMeterSimpleV1Proxy pm = new SystemPowerMeterSimpleV1Proxy(NullLogger.Instance, _pmConfig!, publisher.Object, dataface.Object, client.Object);

            await pm.DisconnectAsync();

            client.Verify(x => x.SendCommandAsync(It.IsAny<C37CommandType>()), Times.Once);
            Assert.AreEqual(PowerMeterState.Disconnecting, pm.State);
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.Dataface;
using paskalON.Devices.Application.Publishers;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyResources.Solars;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Messaging;
using paskalON.Telemetry;

namespace paskalON.Devices.Application.UnitTest.Publishers
{
    [TestClass]
    public class DevicePublisherTest
    {
        [TestMethod]
        public void DevicePublisherConstructorNullLoggerTest()
        {
            Mock<IDeviceManager> deviceManagerMock = CreateDeviceManagerMock();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new DevicePublisher(null!,
                deviceManagerMock.Object, new DeviceMapper(), publisherMock.Object, new PublisherTopic(), 1, 1));
        }


        [TestMethod]
        public void DevicePublisherConstructorNullDeviceManagerTest()
        {
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                null!, new DeviceMapper(), publisherMock.Object, new PublisherTopic(), 1, 1));
        }


        [TestMethod]
        public void DevicePublisherConstructorNullMapperTest()
        {
            Mock<IDeviceManager> deviceManagerMock = CreateDeviceManagerMock();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                deviceManagerMock.Object, null!, publisherMock.Object, new PublisherTopic(), 1, 1));
        }


        [TestMethod]
        public void DevicePublisherConstructorNullPublisherTest()
        {
            Mock<IDeviceManager> deviceManagerMock = CreateDeviceManagerMock();

            Assert.ThrowsExactly<ArgumentNullException>(() => new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                deviceManagerMock.Object, new DeviceMapper(), null!, new PublisherTopic(), 1, 1));
        }


        [TestMethod]
        public void DevicePublisherConstructorNullTopicsTest()
        {
            Mock<IDeviceManager> deviceManagerMock = CreateDeviceManagerMock();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                deviceManagerMock.Object, new DeviceMapper(), publisherMock.Object, null!, 1, 1));
        }


        [TestMethod]
        public void DevicePublisherConstructorTest()
        {
            Mock<IDeviceManager> deviceManagerMock = CreateDeviceManagerMock();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();

            DevicePublisher publisher = new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                deviceManagerMock.Object, new DeviceMapper(), publisherMock.Object, new PublisherTopic(), 1, 1);

            Assert.IsNotNull(publisher);
        }


        [TestMethod]
        public async Task DevicePublisherPublishEmptyCollectionsTest()
        {
            Mock<IDeviceManager> deviceManagerMock = CreateDeviceManagerMock();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();
            DevicePublisher publisher = new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                deviceManagerMock.Object, new DeviceMapper(), publisherMock.Object, CreateTopics(), 1, 1);

            await publisher.Publish(1);

            publisherMock.Verify(messagePublisher => messagePublisher.Publish(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [TestMethod]
        public async Task DevicePublisherPublishCoreIntervalNotReachedTest()
        {
            Mock<IDeviceManager> deviceManagerMock = CreateDeviceManagerMockWithDomains();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();
            DevicePublisher publisher = new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                deviceManagerMock.Object, new DeviceMapper(), publisherMock.Object, CreateTopics(), 2, 3);

            await publisher.Publish(1);

            publisherMock.Verify(messagePublisher => messagePublisher.Publish(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [TestMethod]
        public async Task DevicePublisherPublishDetailIntervalNotReachedTest()
        {
            Mock<IDeviceManager> deviceManagerMock = CreateDeviceManagerMockWithDomains();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();
            DevicePublisher publisher = new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                deviceManagerMock.Object, new DeviceMapper(), publisherMock.Object, CreateTopics(), 3, 2);

            await publisher.Publish(1);

            publisherMock.Verify(messagePublisher => messagePublisher.Publish(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [TestMethod]
        public async Task DevicePublisherPublishCoreAndDetailIntervalsReachedTest()
        {
            Mock<IDeviceManager> deviceManagerMock = CreateDeviceManagerMockWithDomains();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();
            DevicePublisher publisher = new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                deviceManagerMock.Object, new DeviceMapper(), publisherMock.Object, CreateTopics(), 2, 2);

            await publisher.Publish(2);

            publisherMock.Verify(messagePublisher => messagePublisher.Publish(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }


        [TestMethod]
        public async Task DevicePublisherPublishMissingTopicsTest()
        {
            Mock<IDeviceManager> deviceManagerMock = CreateDeviceManagerMockWithDomains();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();
            DevicePublisher publisher = new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                deviceManagerMock.Object, new DeviceMapper(), publisherMock.Object, new PublisherTopic(), 1, 1);

            await publisher.Publish(1);

            publisherMock.Verify(messagePublisher => messagePublisher.Publish(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [TestMethod]
        public async Task DevicePublisherPublishPublisherThrowsExceptionTest()
        {
            FakeLogger<DevicePublisher> logger = new FakeLogger<DevicePublisher>();
            Mock<IDeviceManager> deviceManagerMock = CreateDeviceManagerMockWithDomains();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();
            publisherMock.Setup(messagePublisher => messagePublisher.Publish(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Test exception"));
            DevicePublisher publisher = new DevicePublisher(logger,
                deviceManagerMock.Object, new DeviceMapper(), publisherMock.Object, CreateTopics(), 1, 1);

            await publisher.Publish(1);

            publisherMock.Verify(messagePublisher => messagePublisher.Publish(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            IEnumerable<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.HasCount(2, logs);
            Assert.IsTrue(logs.All(m => m.Message.Contains("error publishing device", StringComparison.OrdinalIgnoreCase)));
        }


        private Mock<IDeviceManager> CreateDeviceManagerMock()
        {
            Mock<IDeviceManager> deviceManagerMock = new Mock<IDeviceManager>();
            deviceManagerMock.SetupGet(deviceManager => deviceManager.PowerConversionSystems)
                .Returns(new List<PowerConversionSystemBase>());
            deviceManagerMock.SetupGet(deviceManager => deviceManager.BatteryBanks)
                .Returns(new List<BatteryBankBase>());
            deviceManagerMock.SetupGet(deviceManager => deviceManager.SolarPanels)
                .Returns(new List<SolarPanelBase>());
            deviceManagerMock.SetupGet(deviceManager => deviceManager.ExternalPowerMeters)
                .Returns(new List<ExternalPowerMeter>());
            deviceManagerMock.SetupGet(deviceManager => deviceManager.AuxiliaryPowerMeters)
                .Returns(new List<AuxiliaryPowerMeter>());
            deviceManagerMock.SetupGet(deviceManager => deviceManager.SystemPowerMeters)
                .Returns(new List<SystemPowerMeter>());
            deviceManagerMock.SetupGet(deviceManager => deviceManager.CircuitPowerMeters)
                .Returns(new List<CircuitPowerMeter>());
            return deviceManagerMock;
        }


        private Mock<IDeviceManager> CreateDeviceManagerMockWithDomains()
        {
            Mock<IDeviceManager> deviceManagerMock = CreateDeviceManagerMock();
            Mock<IMetricsPublisher> metricsPublisherMock = new Mock<IMetricsPublisher>();
            Mock<IDataface> datafaceMock = new Mock<IDataface>();
            // Der
            Mock<DerConfig> derConfig = new Mock<DerConfig>();
            derConfig.SetupGet(x => x.Name).Returns("DerConfig");
            Mock<Der> der = new Mock<Der>(NullLogger.Instance, derConfig.Object);
            // Group
            Mock<DerGroupConfig> groupConfig = new Mock<DerGroupConfig>();
            groupConfig.SetupGet(x => x.Name).Returns("DerGroupConfig");
            Mock<DerGroup> group = new Mock<DerGroup>(NullLogger.Instance, groupConfig.Object, der.Object);
            // Circuit
            Mock<DerCircuitConfig> circuitConfig = new Mock<DerCircuitConfig>();
            circuitConfig.SetupGet(x => x.Name).Returns("DerCircuitConfig");
            Mock<DerCircuit> circuit = new Mock<DerCircuit>(NullLogger.Instance, circuitConfig.Object, group.Object);
            Mock<DerBatteryStorageUnitConfig> unitConfig = new Mock<DerBatteryStorageUnitConfig>();
            unitConfig.SetupGet(x => x.Name).Returns("DerBatteryStorageUnitConfig");
            Mock<DerBatteryStorageUnit> unitMock = new Mock<DerBatteryStorageUnit>(NullLogger.Instance, unitConfig.Object, circuit.Object);
            // Device
            PowerConversionSystemConfig pcsConfig = new PowerConversionSystemConfig
            {
                ChangedBy = "Test",
                IsActive = true,
                DeviceId = 1,
                Name = "PowerConversionSystemConfig",
                PowerConversionSystemDeviceConfig = new Mock<PowerConversionSystemDeviceConfig>().Object,
                ModbusConfig = new Mock<ModbusConfig>().Object,
                DerUnitConfig = unitConfig.Object,
            };
            // Add devices
            deviceManagerMock.Object.PowerConversionSystems.Add(new PcsTest(NullLogger.Instance, pcsConfig, unitMock.Object, metricsPublisherMock.Object, datafaceMock.Object));

            return deviceManagerMock;
        }


        private PublisherTopic CreateTopics()
        {
            PublisherTopic topics = new PublisherTopic
            {
                PowerConversionSystemTopic = CreateTopicEntry(),
                BatteryBankTopic = CreateTopicEntry(),
                SolarPanelTopic = CreateTopicEntry(),
                ExternalPowerMeterTopic = CreateTopicEntry(),
                AuxiliaryPowerMeterTopic = CreateTopicEntry(),
                CircuitPowerMeterTopic = CreateTopicEntry(),
                SystemPowerMeterTopic = CreateTopicEntry()
            };
            return topics;
        }


        private PublisherTopicEntry CreateTopicEntry()
        {
            return new PublisherTopicEntry
            {
                DefinitionTopic = "definition",
                CoreTopic = "core",
                DetailTopic = "detail"
            };
        }

        class PcsTest : PowerConversionSystemBase
        {
            public PcsTest(ILogger logger, PowerConversionSystemConfig config, DerUnit derUnit, IMetricsPublisher publisher, IDataface dataface)
                : base(logger, config, derUnit, publisher, dataface)
            {
            }

            protected override void RegisterDataface()
            {
            }
        }
    }
}

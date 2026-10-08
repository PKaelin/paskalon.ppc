// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.Dataface;
using paskalON.Devices.Application.Publishers;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyResources.Solars;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Messaging;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;
using System.Collections.Concurrent;
using System.Text.Json;

namespace paskalON.Devices.Application.UnitTest.Publishers
{
    [TestClass]
    public class DevicePublisherTest
    {
        private const int PcsDeviceId = 101;
        private const int BatteryBankDeviceId = 102;
        private const int SolarPanelDeviceId = 103;
        private const int ExternalMeterDeviceId = 104;
        private const int AuxiliaryMeterDeviceId = 105;
        private const int CircuitMeterDeviceId = 106;
        private const int SystemMeterDeviceId = 107;

        private const string PcsCoreTopic = "ppc:pcs:core";
        private const string PcsDetailTopic = "ppc:pcs:detail";
        private const string BatteryBankCoreTopic = "ppc:bb:core";
        private const string BatteryBankDetailTopic = "ppc:bb:detail";
        private const string SolarPanelCoreTopic = "ppc:pv:core";
        private const string SolarPanelDetailTopic = "ppc:pv:detail";
        private const string ExternalMeterCoreTopic = "ppc:pm:external:core";
        private const string ExternalMeterDetailTopic = "ppc:pm:external:detail";
        private const string AuxiliaryMeterCoreTopic = "ppc:pm:auxiliary:core";
        private const string AuxiliaryMeterDetailTopic = "ppc:pm:auxiliary:detail";
        private const string CircuitMeterCoreTopic = "ppc:pm:circuit:core";
        private const string CircuitMeterDetailTopic = "ppc:pm:circuit:detail";
        private const string SystemMeterCoreTopic = "ppc:pm:system:core";
        private const string SystemMeterDetailTopic = "ppc:pm:system:detail";

        private static readonly string[] HardCoreTopics = new string[]
        {
            SystemMeterCoreTopic,
            CircuitMeterCoreTopic
        };

        private static readonly string[] CoreTopics = new string[]
        {
            PcsCoreTopic,
            BatteryBankCoreTopic,
            SolarPanelCoreTopic,
            ExternalMeterCoreTopic,
            AuxiliaryMeterCoreTopic
        };

        private static readonly string[] DetailTopics = new string[]
        {
            SystemMeterDetailTopic,
            CircuitMeterDetailTopic,
            PcsDetailTopic,
            BatteryBankDetailTopic,
            SolarPanelDetailTopic,
            ExternalMeterDetailTopic,
            AuxiliaryMeterDetailTopic
        };


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
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void DevicePublisherConstructorInvalidCoreIntervalTest(int coreInterval)
        {
            Mock<IDeviceManager> deviceManagerMock = CreateDeviceManagerMock();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                deviceManagerMock.Object, new DeviceMapper(), publisherMock.Object, new PublisherTopic(), coreInterval, 1));
        }


        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void DevicePublisherConstructorInvalidDetailIntervalTest(int detailInterval)
        {
            Mock<IDeviceManager> deviceManagerMock = CreateDeviceManagerMock();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                deviceManagerMock.Object, new DeviceMapper(), publisherMock.Object, new PublisherTopic(), 1, detailInterval));
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
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public async Task DevicePublisherPublishInvalidCurrentIntervalTest(int currentInterval)
        {
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();
            DevicePublisher publisher = new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                CreateDeviceManagerMockWithDevices().Object, new DeviceMapper(), publisherMock.Object, CreateTopics(), 1, 1);

            await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => publisher.Publish(currentInterval));

            publisherMock.Verify(messagePublisher => messagePublisher.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [TestMethod]
        public async Task DevicePublisherPublishEmptyCollectionsTest()
        {
            Mock<IDeviceManager> deviceManagerMock = CreateDeviceManagerMock();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();
            DevicePublisher publisher = new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                deviceManagerMock.Object, new DeviceMapper(), publisherMock.Object, CreateTopics(), 1, 1);

            await publisher.Publish(1);

            publisherMock.Verify(messagePublisher => messagePublisher.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [TestMethod]
        [DataRow(2, 3, 1, false, false)]
        [DataRow(2, 3, 2, true, false)]
        [DataRow(2, 3, 3, false, true)]
        [DataRow(2, 3, 4, true, false)]
        [DataRow(2, 3, 6, true, true)]
        [DataRow(1, 1, 1, true, true)]
        [DataRow(1, 5, 4, true, false)]
        [DataRow(1, 5, 5, true, true)]
        [DataRow(5, 1, 4, false, true)]
        [DataRow(7, 7, int.MaxValue, false, false)]
        [DataRow(int.MaxValue, int.MaxValue, int.MaxValue, true, true)]
        [DataRow(int.MaxValue, int.MaxValue, int.MaxValue - 1, false, false)]
        public async Task DevicePublisherPublishIntervalScheduleTest(int coreInterval, int detailInterval, int currentInterval,
            bool isCoreExpected, bool isDetailExpected)
        {
            ConcurrentQueue<(string Topic, string Json)> published = new ConcurrentQueue<(string Topic, string Json)>();
            Mock<IMessagePublisher> publisherMock = CreateCapturingPublisherMock(published);
            DevicePublisher publisher = new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                CreateDeviceManagerMockWithDevices().Object, new DeviceMapper(), publisherMock.Object, CreateTopics(), coreInterval, detailInterval);
            List<string> expectedTopics = new List<string>(HardCoreTopics);

            if (isCoreExpected == true)
            {
                expectedTopics.AddRange(CoreTopics);
            }

            if (isDetailExpected == true)
            {
                expectedTopics.AddRange(DetailTopics);
            }

            await publisher.Publish(currentInterval);

            CollectionAssert.AreEquivalent(expectedTopics, published.Select(message => message.Topic).ToList());
        }


        [TestMethod]
        public async Task DevicePublisherPublishHardCoreEveryIntervalTest()
        {
            const int IntervalCount = 6;
            ConcurrentQueue<(string Topic, string Json)> published = new ConcurrentQueue<(string Topic, string Json)>();
            Mock<IMessagePublisher> publisherMock = CreateCapturingPublisherMock(published);
            DevicePublisher publisher = new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                CreateDeviceManagerMockWithDevices().Object, new DeviceMapper(), publisherMock.Object, CreateTopics(), 10, 20);

            for (int interval = 1; interval <= IntervalCount; interval++)
            {
                await publisher.Publish(interval);
            }

            Assert.HasCount(IntervalCount * HardCoreTopics.Length, published);
            Assert.AreEqual(IntervalCount, published.Count(message => message.Topic == SystemMeterCoreTopic));
            Assert.AreEqual(IntervalCount, published.Count(message => message.Topic == CircuitMeterCoreTopic));
        }


        [TestMethod]
        public async Task DevicePublisherPublishIntervalSequenceTest()
        {
            ConcurrentQueue<(string Topic, string Json)> published = new ConcurrentQueue<(string Topic, string Json)>();
            Mock<IMessagePublisher> publisherMock = CreateCapturingPublisherMock(published);
            DevicePublisher publisher = new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                CreateDeviceManagerMockWithDevices().Object, new DeviceMapper(), publisherMock.Object, CreateTopics(), 2, 5);

            for (int interval = 1; interval <= 10; interval++)
            {
                await publisher.Publish(interval);
            }

            Assert.AreEqual(10, published.Count(message => message.Topic == SystemMeterCoreTopic));
            Assert.AreEqual(5, published.Count(message => message.Topic == PcsCoreTopic));
            Assert.AreEqual(2, published.Count(message => message.Topic == PcsDetailTopic));
            Assert.AreEqual(2, published.Count(message => message.Topic == SystemMeterDetailTopic));
        }


        [TestMethod]
        public async Task DevicePublisherPublishPayloadMatchesDeviceTest()
        {
            ConcurrentQueue<(string Topic, string Json)> published = new ConcurrentQueue<(string Topic, string Json)>();
            Mock<IMessagePublisher> publisherMock = CreateCapturingPublisherMock(published);
            DevicePublisher publisher = new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                CreateDeviceManagerMockWithDevices().Object, new DeviceMapper(), publisherMock.Object, CreateTopics(), 1, 1);
            Dictionary<string, int> expectedDeviceIds = new Dictionary<string, int>
            {
                { PcsCoreTopic, PcsDeviceId },
                { PcsDetailTopic, PcsDeviceId },
                { BatteryBankCoreTopic, BatteryBankDeviceId },
                { BatteryBankDetailTopic, BatteryBankDeviceId },
                { SolarPanelCoreTopic, SolarPanelDeviceId },
                { SolarPanelDetailTopic, SolarPanelDeviceId },
                { ExternalMeterCoreTopic, ExternalMeterDeviceId },
                { ExternalMeterDetailTopic, ExternalMeterDeviceId },
                { AuxiliaryMeterCoreTopic, AuxiliaryMeterDeviceId },
                { AuxiliaryMeterDetailTopic, AuxiliaryMeterDeviceId },
                { CircuitMeterCoreTopic, CircuitMeterDeviceId },
                { CircuitMeterDetailTopic, CircuitMeterDeviceId },
                { SystemMeterCoreTopic, SystemMeterDeviceId },
                { SystemMeterDetailTopic, SystemMeterDeviceId }
            };

            await publisher.Publish(1);

            Assert.HasCount(expectedDeviceIds.Count, published);

            foreach ((string topic, string json) in published)
            {
                using JsonDocument document = JsonDocument.Parse(json);
                Assert.AreEqual(expectedDeviceIds[topic], document.RootElement.GetProperty("DeviceId").GetInt32(), topic);
            }
        }


        [TestMethod]
        public async Task DevicePublisherPublishMissingTopicsTest()
        {
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();
            DevicePublisher publisher = new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                CreateDeviceManagerMockWithDevices().Object, new DeviceMapper(), publisherMock.Object, new PublisherTopic(), 1, 1);

            await publisher.Publish(1);

            publisherMock.Verify(messagePublisher => messagePublisher.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        [TestMethod]
        public async Task DevicePublisherPublishPartialTopicsTest()
        {
            ConcurrentQueue<(string Topic, string Json)> published = new ConcurrentQueue<(string Topic, string Json)>();
            Mock<IMessagePublisher> publisherMock = CreateCapturingPublisherMock(published);
            PublisherTopic topics = new PublisherTopic
            {
                PowerConversionSystemTopic = new PublisherTopicEntry(PcsCoreTopic, PcsDetailTopic),
                SystemPowerMeterTopic = new PublisherTopicEntry(SystemMeterCoreTopic, SystemMeterDetailTopic)
            };
            DevicePublisher publisher = new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                CreateDeviceManagerMockWithDevices().Object, new DeviceMapper(), publisherMock.Object, topics, 1, 1);

            await publisher.Publish(1);

            CollectionAssert.AreEquivalent(new string[] { SystemMeterCoreTopic, SystemMeterDetailTopic, PcsCoreTopic, PcsDetailTopic },
                published.Select(message => message.Topic).ToList());
        }


        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("\t\r\n")]
        public async Task DevicePublisherPublishBlankTopicSkippedTest(string blankTopic)
        {
            ConcurrentQueue<(string Topic, string Json)> published = new ConcurrentQueue<(string Topic, string Json)>();
            Mock<IMessagePublisher> publisherMock = CreateCapturingPublisherMock(published);
            PublisherTopic topics = new PublisherTopic
            {
                PowerConversionSystemTopic = new PublisherTopicEntry(blankTopic, PcsDetailTopic),
                SystemPowerMeterTopic = new PublisherTopicEntry(SystemMeterCoreTopic, blankTopic)
            };
            DevicePublisher publisher = new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                CreateDeviceManagerMockWithDevices().Object, new DeviceMapper(), publisherMock.Object, topics, 1, 1);

            await publisher.Publish(1);

            CollectionAssert.AreEquivalent(new string[] { SystemMeterCoreTopic, PcsDetailTopic },
                published.Select(message => message.Topic).ToList());
        }


        [TestMethod]
        public async Task DevicePublisherPublishMultipleDevicesPerTypeTest()
        {
            ConcurrentQueue<(string Topic, string Json)> published = new ConcurrentQueue<(string Topic, string Json)>();
            Mock<IMessagePublisher> publisherMock = CreateCapturingPublisherMock(published);
            Mock<IDeviceManager> deviceManagerMock = CreateDeviceManagerMock();
            deviceManagerMock.Object.SystemPowerMeters.Add(CreateSystemPowerMeter(201));
            deviceManagerMock.Object.SystemPowerMeters.Add(CreateSystemPowerMeter(202));
            deviceManagerMock.Object.SystemPowerMeters.Add(CreateSystemPowerMeter(203));
            DevicePublisher publisher = new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                deviceManagerMock.Object, new DeviceMapper(), publisherMock.Object, CreateTopics(), 1, 1);

            await publisher.Publish(1);

            List<int> coreDeviceIds = published.Where(message => message.Topic == SystemMeterCoreTopic).Select(message => GetDeviceId(message.Json)).ToList();
            List<int> detailDeviceIds = published.Where(message => message.Topic == SystemMeterDetailTopic).Select(message => GetDeviceId(message.Json)).ToList();
            CollectionAssert.AreEqual(new int[] { 201, 202, 203 }, coreDeviceIds);
            CollectionAssert.AreEqual(new int[] { 201, 202, 203 }, detailDeviceIds);
        }


        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task DevicePublisherPublishPublisherThrowsExceptionTest(bool isFaultedTask)
        {
            FakeLogger<DevicePublisher> logger = new FakeLogger<DevicePublisher>();
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();

            if (isFaultedTask == true)
            {
                publisherMock.Setup(messagePublisher => messagePublisher.PublishAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ThrowsAsync(new InvalidOperationException("Broker unavailable"));
            }
            else
            {
                publisherMock.Setup(messagePublisher => messagePublisher.PublishAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .Throws(new InvalidOperationException("Broker unavailable"));
            }

            DevicePublisher publisher = new DevicePublisher(logger,
                CreateDeviceManagerMockWithDevices().Object, new DeviceMapper(), publisherMock.Object, CreateTopics(), 1, 1);
            int expectedCallCount = HardCoreTopics.Length + CoreTopics.Length + DetailTopics.Length;

            await publisher.Publish(1);

            publisherMock.Verify(messagePublisher => messagePublisher.PublishAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(expectedCallCount));
            IReadOnlyList<FakeLogRecord> logs = logger.Collector.GetSnapshot().Where(log => log.Level == LogLevel.Error).ToList();
            Assert.HasCount(expectedCallCount, logs);
            Assert.IsTrue(logs.All(log => log.Message.Contains("error publishing device", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(logs.All(log => log.Message.Contains("Broker unavailable", StringComparison.Ordinal)));
        }


        [TestMethod]
        public async Task DevicePublisherPublishContinuesAfterSingleFailureTest()
        {
            FakeLogger<DevicePublisher> logger = new FakeLogger<DevicePublisher>();
            ConcurrentQueue<(string Topic, string Json)> published = new ConcurrentQueue<(string Topic, string Json)>();
            Mock<IMessagePublisher> publisherMock = CreateCapturingPublisherMock(published);
            publisherMock.Setup(messagePublisher => messagePublisher.PublishAsync(SystemMeterCoreTopic, It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Broker unavailable"));
            DevicePublisher publisher = new DevicePublisher(logger,
                CreateDeviceManagerMockWithDevices().Object, new DeviceMapper(), publisherMock.Object, CreateTopics(), 1, 1);
            List<string> expectedTopics = CoreTopics.Concat(DetailTopics).Append(CircuitMeterCoreTopic).ToList();

            await publisher.Publish(1);

            CollectionAssert.AreEquivalent(expectedTopics, published.Select(message => message.Topic).ToList());
            Assert.HasCount(1, logger.Collector.GetSnapshot().Where(log => log.Level == LogLevel.Error));
        }


        [TestMethod]
        public async Task DevicePublisherPublishConcurrentCallsTest()
        {
            const int CallerCount = 8;
            ConcurrentQueue<(string Topic, string Json)> published = new ConcurrentQueue<(string Topic, string Json)>();
            Mock<IMessagePublisher> publisherMock = CreateCapturingPublisherMock(published);
            DevicePublisher publisher = new DevicePublisher(NullLogger<DevicePublisher>.Instance,
                CreateDeviceManagerMockWithDevices().Object, new DeviceMapper(), publisherMock.Object, CreateTopics(), 1, 1);
            IEnumerable<Task> tasks = Enumerable.Range(0, CallerCount).Select(index => Task.Run(() => publisher.Publish(1)));

            await Task.WhenAll(tasks);

            int topicCount = HardCoreTopics.Length + CoreTopics.Length + DetailTopics.Length;
            Assert.HasCount(CallerCount * topicCount, published);

            foreach (IGrouping<string, (string Topic, string Json)> group in published.GroupBy(message => message.Topic))
            {
                Assert.HasCount(CallerCount, group, group.Key);
            }
        }


        private int GetDeviceId(string json)
        {
            using JsonDocument document = JsonDocument.Parse(json);

            return document.RootElement.GetProperty("DeviceId").GetInt32();
        }


        private Mock<IMessagePublisher> CreateCapturingPublisherMock(ConcurrentQueue<(string Topic, string Json)> published)
        {
            Mock<IMessagePublisher> publisherMock = new Mock<IMessagePublisher>();
            publisherMock.Setup(messagePublisher => messagePublisher.PublishAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((topic, json) => published.Enqueue((topic, json)))
                .Returns(Task.CompletedTask);

            return publisherMock;
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


        private Mock<IDeviceManager> CreateDeviceManagerMockWithDevices()
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
            // Units
            Mock<DerBatteryStorageUnitConfig> batteryUnitConfig = new Mock<DerBatteryStorageUnitConfig>();
            batteryUnitConfig.SetupGet(x => x.Name).Returns("DerBatteryStorageUnitConfig");
            Mock<DerBatteryStorageUnit> batteryUnit = new Mock<DerBatteryStorageUnit>(NullLogger.Instance, batteryUnitConfig.Object, circuit.Object);
            Mock<DerSolarUnitConfig> solarUnitConfig = new Mock<DerSolarUnitConfig>();
            solarUnitConfig.SetupGet(x => x.Name).Returns("DerSolarUnitConfig");
            Mock<DerSolarUnit> solarUnit = new Mock<DerSolarUnit>(NullLogger.Instance, solarUnitConfig.Object, circuit.Object);
            // Power conversion system
            PowerConversionSystemConfig pcsConfig = new PowerConversionSystemConfig
            {
                ChangedBy = "UnitTest",
                IsActive = true,
                DeviceId = PcsDeviceId,
                Name = "PCS-1",
                PowerConversionSystemDeviceConfig = new Mock<PowerConversionSystemDeviceConfig>().Object,
                ModbusConfig = new Mock<ModbusConfig>().Object,
                DerUnitConfig = batteryUnitConfig.Object
            };
            // Battery bank
            BatteryBankConfig batteryBankConfig = new BatteryBankConfig
            {
                ChangedBy = "UnitTest",
                IsActive = true,
                DeviceId = BatteryBankDeviceId,
                Name = "BB-1",
                BatteryBankDeviceConfig = new Mock<BatteryBankDeviceConfig>().Object,
                ModbusConfig = new Mock<ModbusConfig>().Object,
                DerUnitConfig = batteryUnitConfig.Object
            };
            // Solar panel
            SolarPanelConfig solarPanelConfig = new SolarPanelConfig
            {
                ChangedBy = "UnitTest",
                IsActive = true,
                DeviceId = SolarPanelDeviceId,
                Name = "PV-1",
                SolarPanelDeviceConfig = new Mock<SolarPanelDeviceConfig>().Object,
                DerUnitConfig = solarUnitConfig.Object
            };
            // Power meters
            ExternalPowerMeterConfig externalConfig = new ExternalPowerMeterConfig
            {
                ChangedBy = "UnitTest",
                IsActive = true,
                DeviceId = ExternalMeterDeviceId,
                Name = "PM-External-1",
                PowerMeterDeviceConfig = CreatePowerMeterDeviceConfig(),
                PowerFactorStandard = PowerFactorStandard.IEEE,
                DerConfig = derConfig.Object
            };
            AuxiliaryPowerMeterConfig auxiliaryConfig = new AuxiliaryPowerMeterConfig
            {
                ChangedBy = "UnitTest",
                IsActive = true,
                DeviceId = AuxiliaryMeterDeviceId,
                Name = "PM-Auxiliary-1",
                PowerMeterDeviceConfig = CreatePowerMeterDeviceConfig(),
                PowerFactorStandard = PowerFactorStandard.IEEE,
                DerConfig = derConfig.Object
            };
            CircuitPowerMeterConfig circuitMeterConfig = new CircuitPowerMeterConfig
            {
                ChangedBy = "UnitTest",
                IsActive = true,
                DeviceId = CircuitMeterDeviceId,
                Name = "PM-Circuit-1",
                PowerMeterDeviceConfig = CreatePowerMeterDeviceConfig(),
                PowerFactorStandard = PowerFactorStandard.IEEE,
                DerCircuitConfig = circuitConfig.Object
            };
            // Add devices
            deviceManagerMock.Object.PowerConversionSystems.Add(new PcsTest(NullLogger.Instance, pcsConfig, batteryUnit.Object, metricsPublisherMock.Object, datafaceMock.Object));
            deviceManagerMock.Object.BatteryBanks.Add(new BatteryBankTest(NullLogger.Instance, batteryBankConfig, batteryUnit.Object, metricsPublisherMock.Object, datafaceMock.Object));
            deviceManagerMock.Object.SolarPanels.Add(new SolarPanelTest(NullLogger.Instance, solarPanelConfig, solarUnit.Object, metricsPublisherMock.Object, datafaceMock.Object));
            deviceManagerMock.Object.ExternalPowerMeters.Add(new ExternalPowerMeterTest(NullLogger.Instance, externalConfig, metricsPublisherMock.Object, datafaceMock.Object));
            deviceManagerMock.Object.AuxiliaryPowerMeters.Add(new AuxiliaryPowerMeterTest(NullLogger.Instance, auxiliaryConfig, metricsPublisherMock.Object, datafaceMock.Object));
            deviceManagerMock.Object.CircuitPowerMeters.Add(new CircuitPowerMeterTest(NullLogger.Instance, circuitMeterConfig, metricsPublisherMock.Object, datafaceMock.Object));
            deviceManagerMock.Object.SystemPowerMeters.Add(CreateSystemPowerMeter(SystemMeterDeviceId));

            return deviceManagerMock;
        }


        private SystemPowerMeter CreateSystemPowerMeter(int deviceId)
        {
            SystemPowerMeterConfig config = new SystemPowerMeterConfig
            {
                ChangedBy = "UnitTest",
                IsActive = true,
                DeviceId = deviceId,
                Name = $"PM-System-{deviceId}",
                PowerMeterDeviceConfig = CreatePowerMeterDeviceConfig(),
                PowerFactorStandard = PowerFactorStandard.IEEE,
                DerConfig = new Mock<DerConfig>().Object
            };

            return new SystemPowerMeterTest(NullLogger.Instance, config, new Mock<IMetricsPublisher>().Object, new Mock<IDataface>().Object);
        }


        private static PowerMeterDeviceConfig CreatePowerMeterDeviceConfig()
        {
            return new PowerMeterDeviceConfig
            {
                ChangedBy = "UnitTest",
                Name = "PowerMeterDevice",
                ClassName = "PowerMeterTest"
            };
        }


        private PublisherTopic CreateTopics()
        {
            PublisherTopic topics = new PublisherTopic
            {
                PowerConversionSystemTopic = new PublisherTopicEntry(PcsCoreTopic, PcsDetailTopic, "ppc:pcs:definition"),
                BatteryBankTopic = new PublisherTopicEntry(BatteryBankCoreTopic, BatteryBankDetailTopic, "ppc:bb:definition"),
                SolarPanelTopic = new PublisherTopicEntry(SolarPanelCoreTopic, SolarPanelDetailTopic, "ppc:pv:definition"),
                ExternalPowerMeterTopic = new PublisherTopicEntry(ExternalMeterCoreTopic, ExternalMeterDetailTopic, "ppc:pm:external:definition"),
                AuxiliaryPowerMeterTopic = new PublisherTopicEntry(AuxiliaryMeterCoreTopic, AuxiliaryMeterDetailTopic, "ppc:pm:auxiliary:definition"),
                CircuitPowerMeterTopic = new PublisherTopicEntry(CircuitMeterCoreTopic, CircuitMeterDetailTopic, "ppc:pm:circuit:definition"),
                SystemPowerMeterTopic = new PublisherTopicEntry(SystemMeterCoreTopic, SystemMeterDetailTopic, "ppc:pm:system:definition")
            };

            return topics;
        }


        private class PcsTest : PowerConversionSystemBase
        {
            public PcsTest(ILogger logger, PowerConversionSystemConfig config, DerUnit derUnit, IMetricsPublisher publisher, IDataface dataface)
                : base(logger, config, derUnit, publisher, dataface)
            {
            }


            protected override void RegisterDataface()
            {
            }


            public override Task HeartbeatAsync(CancellationToken cancellationToken)
            {
                // Not required for unit tests.
                return Task.CompletedTask;
            }
        }


        private class BatteryBankTest : BatteryBankBase
        {
            public BatteryBankTest(ILogger logger, BatteryBankConfig config, DerBatteryStorageUnit unit, IMetricsPublisher publisher, IDataface dataface)
                : base(logger, config, unit, publisher, dataface)
            {
            }


            protected override void RegisterDataface()
            {
            }


            public override Task HeartbeatAsync(CancellationToken cancellationToken)
            {
                // Not required for unit tests.
                return Task.CompletedTask;
            }
        }


        private class SolarPanelTest : SolarPanelBase
        {
            public SolarPanelTest(ILogger logger, SolarPanelConfig config, DerSolarUnit unit, IMetricsPublisher publisher, IDataface dataface)
                : base(logger, config, unit, publisher, dataface)
            {
            }


            protected override void RegisterDataface()
            {
            }
        }


        private class ExternalPowerMeterTest : ExternalPowerMeter
        {
            public ExternalPowerMeterTest(ILogger logger, ExternalPowerMeterConfig config, IMetricsPublisher publisher, IDataface dataface)
                : base(logger, config, publisher, dataface)
            {
            }


            protected override void RegisterDataface()
            {
            }
        }


        private class AuxiliaryPowerMeterTest : AuxiliaryPowerMeter
        {
            public AuxiliaryPowerMeterTest(ILogger logger, AuxiliaryPowerMeterConfig config, IMetricsPublisher publisher, IDataface dataface)
                : base(logger, config, publisher, dataface)
            {
            }


            protected override void RegisterDataface()
            {
            }
        }


        private class CircuitPowerMeterTest : CircuitPowerMeter
        {
            public CircuitPowerMeterTest(ILogger logger, CircuitPowerMeterConfig config, IMetricsPublisher publisher, IDataface dataface)
                : base(logger, config, publisher, dataface)
            {
            }


            protected override void RegisterDataface()
            {
            }
        }


        private class SystemPowerMeterTest : SystemPowerMeter
        {
            public SystemPowerMeterTest(ILogger logger, SystemPowerMeterConfig config, IMetricsPublisher publisher, IDataface dataface)
                : base(logger, config, publisher, dataface)
            {
            }


            protected override void RegisterDataface()
            {
            }
        }
    }
}

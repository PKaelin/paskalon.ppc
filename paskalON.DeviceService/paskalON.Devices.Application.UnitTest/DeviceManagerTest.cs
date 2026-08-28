// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using paskalON.Dataface;
using paskalON.Devices.Application.Factories;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Infrastructure.Storage.Repositories;
using paskalON.Telemetry;

namespace paskalON.Devices.Application.UnitTest
{
    [TestClass]
    public sealed class DeviceManagerTest
    {
        private FakeLogger<DeviceManagerTestClass> _logger = new FakeLogger<DeviceManagerTestClass>();
        private Mock<IMetricsPublisherFactory> _publisherFactory = new Mock<IMetricsPublisherFactory>();
        private Mock<IModbusDeviceFactory> _deviceFactoryModbus = new Mock<IModbusDeviceFactory>();
        private Mock<IC37DeviceFactory> _deviceFactoryC37 = new Mock<IC37DeviceFactory>();
        private Mock<PowerConversionSystemBase>? _pcsMock;
        private Mock<BatteryBankBase>? _bbMock;


        [TestMethod]
        public void DeviceManagerConstructorNullLoggerTest()
        {
            Mock<IDerRepository> repositoryMock = new Mock<IDerRepository>();
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new DeviceManager(null!, repositoryMock.Object, servicesMock.Object,
                _publisherFactory.Object, _deviceFactoryModbus.Object, _deviceFactoryC37.Object));
        }


        [TestMethod]
        public void DeviceManagerConstructorNullRepositoryTest()
        {
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new DeviceManager(NullLogger<DeviceManager>.Instance, null!,
                servicesMock.Object, _publisherFactory.Object, _deviceFactoryModbus.Object, _deviceFactoryC37.Object));
        }


        [TestMethod]
        public void DeviceManagerConstructorNullServicesTest()
        {
            Mock<IDerRepository> repositoryMock = new Mock<IDerRepository>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new DeviceManager(NullLogger<DeviceManager>.Instance, repositoryMock.Object,
                null!, _publisherFactory.Object, _deviceFactoryModbus.Object, _deviceFactoryC37.Object));
        }


        [TestMethod]
        public void DeviceManagerConstructorNullPublisherFactoryTest()
        {
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();
            Mock<IDerRepository> repositoryMock = new Mock<IDerRepository>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new DeviceManager(NullLogger<DeviceManager>.Instance, repositoryMock.Object,
                servicesMock.Object, null!, _deviceFactoryModbus.Object, _deviceFactoryC37.Object));
        }


        [TestMethod]
        public void DeviceManagerConstructorNullDeviceFactoryModbusTest()
        {
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();
            Mock<IDerRepository> repositoryMock = new Mock<IDerRepository>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new DeviceManager(NullLogger<DeviceManager>.Instance, repositoryMock.Object,
                servicesMock.Object, _publisherFactory.Object, null!, _deviceFactoryC37.Object));
        }


        [TestMethod]
        public void DeviceManagerConstructorNullDeviceFactoryC37Test()
        {
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();
            Mock<IDerRepository> repositoryMock = new Mock<IDerRepository>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new DeviceManager(NullLogger<DeviceManager>.Instance, repositoryMock.Object,
                servicesMock.Object, _publisherFactory.Object, _deviceFactoryModbus.Object, null!));
        }


        [TestMethod]
        public void DeviceManagerConstructorTest()
        {
            Mock<IDerRepository> repositoryMock = new Mock<IDerRepository>();
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();

            DeviceManager manager = new DeviceManager(NullLogger<DeviceManager>.Instance, repositoryMock.Object, servicesMock.Object,
                _publisherFactory.Object, _deviceFactoryModbus.Object, _deviceFactoryC37.Object);

            Assert.IsNotNull(manager.Der);
            Assert.AreEqual("Uninitialized DER", manager.Der.Name);
            Assert.IsNotNull(manager.PowerConversionSystems);
            Assert.IsNotNull(manager.BatteryBanks);
            Assert.IsNotNull(manager.SolarPanels);
            Assert.IsNotNull(manager.SystemPowerMeters);
            Assert.IsNotNull(manager.AuxiliaryPowerMeters);
            Assert.IsNotNull(manager.ExternalPowerMeters);
            Assert.IsNotNull(manager.CircuitPowerMeters);
        }


        [TestMethod]
        public async Task DeviceManagerLoadDerEmptyConfigurationTest()
        {
            DerConfig config = new DerConfig { ChangedBy = "Test", Name = "Test DER" };
            Mock<IDerRepository> repositoryMock = new Mock<IDerRepository>();
            repositoryMock.Setup(repository => repository.GetDer(true)).ReturnsAsync(config);
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();
            DeviceManager manager = new DeviceManager(NullLogger<DeviceManager>.Instance, repositoryMock.Object, servicesMock.Object,
                _publisherFactory.Object, _deviceFactoryModbus.Object, _deviceFactoryC37.Object);

            await manager.LoadDerAsync();

            repositoryMock.Verify(repository => repository.GetDer(true), Times.Once);
            Assert.AreEqual("Test DER", manager.Der.Name);
            Assert.HasCount(0, manager.Der.DerGroups);
            Assert.HasCount(0, manager.Der.GenericModbusDevices);
            Assert.HasCount(0, manager.Der.AutomaticTransferSwitches);
            Assert.HasCount(0, manager.Der.SystemPowerMeters);
            Assert.HasCount(0, manager.Der.AuxiliaryPowerMeters);
            Assert.HasCount(0, manager.Der.ExternalPowerMeters);
            Assert.HasCount(0, manager.PowerConversionSystems);
            Assert.HasCount(0, manager.BatteryBanks);
            Assert.HasCount(0, manager.SolarPanels);
        }


        [TestMethod]
        public async Task DeviceManagerLoadDerRepositoryThrowsExceptionTest()
        {
            InvalidOperationException exception = new InvalidOperationException("Test exception");
            Mock<IDerRepository> repositoryMock = new Mock<IDerRepository>();
            repositoryMock.Setup(repository => repository.GetDer(true)).ThrowsAsync(exception);
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();
            DeviceManager manager = new DeviceManager(NullLogger<DeviceManager>.Instance, repositoryMock.Object, servicesMock.Object,
                _publisherFactory.Object, _deviceFactoryModbus.Object, _deviceFactoryC37.Object);

            InvalidOperationException result = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await manager.LoadDerAsync());

            Assert.AreSame(exception, result);
            repositoryMock.Verify(repository => repository.GetDer(true), Times.Once);
        }


        [TestMethod]
        public async Task DeviceManagerLoadDerBuildsGroupAndCircuitTreeTest()
        {
            DerConfig config = new DerConfig { ChangedBy = "Test", Name = "Configured DER" };
            DerGroupConfig firstGroupConfig = new DerGroupConfig
            {
                ChangedBy = "Test",
                Name = "Group 1",
                DerConfig = config
            };
            DerGroupConfig secondGroupConfig = new DerGroupConfig
            {
                ChangedBy = "Test",
                Name = "Group 2",
                DerConfig = config
            };
            DerCircuitConfig firstCircuitConfig = new DerCircuitConfig
            {
                ChangedBy = "Test",
                Name = "Circuit 1",
                DerGroupConfig = firstGroupConfig
            };
            DerCircuitConfig secondCircuitConfig = new DerCircuitConfig
            {
                ChangedBy = "Test",
                Name = "Circuit 2",
                DerGroupConfig = firstGroupConfig
            };
            firstGroupConfig.DerCircuits.Add(firstCircuitConfig);
            firstGroupConfig.DerCircuits.Add(secondCircuitConfig);
            config.DerGroupConfigs.Add(firstGroupConfig);
            config.DerGroupConfigs.Add(secondGroupConfig);

            Mock<IDerRepository> repositoryMock = new Mock<IDerRepository>();
            repositoryMock.Setup(repository => repository.GetDer(true)).ReturnsAsync(config);
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();
            DeviceManager manager = new DeviceManager(NullLogger<DeviceManager>.Instance, repositoryMock.Object, servicesMock.Object,
                _publisherFactory.Object, _deviceFactoryModbus.Object, _deviceFactoryC37.Object);

            await manager.LoadDerAsync();

            Assert.AreEqual("Configured DER", manager.Der.Name);
            Assert.HasCount(2, manager.Der.DerGroups);
            Assert.AreEqual("Group 1", manager.Der.DerGroups[0].Name);
            Assert.AreEqual("Group 2", manager.Der.DerGroups[1].Name);
            Assert.AreSame(manager.Der, manager.Der.DerGroups[0].Der);
            Assert.HasCount(2, manager.Der.DerGroups[0].DerCircuits);
            Assert.HasCount(0, manager.Der.DerGroups[1].DerCircuits);
            Assert.AreEqual("Circuit 1", manager.Der.DerGroups[0].DerCircuits[0].Name);
            Assert.AreEqual("Circuit 2", manager.Der.DerGroups[0].DerCircuits[1].Name);
            Assert.AreSame(manager.Der.DerGroups[0], manager.Der.DerGroups[0].DerCircuits[0].DerGroup);
            Assert.AreSame(manager.Der.DerGroups[0], manager.Der.DerGroups[0].DerCircuits[1].DerGroup);
        }


        [TestMethod]
        public async Task DeviceManagerLoadDerReplacesPlaceholderDerTest()
        {
            DerConfig config = new DerConfig { ChangedBy = "Test", Name = "Loaded DER" };
            Mock<IDerRepository> repositoryMock = new Mock<IDerRepository>();
            repositoryMock.Setup(repository => repository.GetDer(true)).ReturnsAsync(config);
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();
            DeviceManager manager = new DeviceManager(NullLogger<DeviceManager>.Instance, repositoryMock.Object, servicesMock.Object,
                _publisherFactory.Object, _deviceFactoryModbus.Object, _deviceFactoryC37.Object);
            Der placeholderDer = manager.Der;

            await manager.LoadDerAsync();

            Assert.AreNotSame(placeholderDer, manager.Der);
            Assert.AreEqual("Loaded DER", manager.Der.Name);
        }


        [TestMethod]
        public async Task DeviceManagerLoadDerRejectsUnsupportedUnitConfigurationTest()
        {
            DerConfig config = new DerConfig { ChangedBy = "Test", Name = "Configured DER" };
            DerGroupConfig groupConfig = new DerGroupConfig
            {
                ChangedBy = "Test",
                Name = "Group",
                DerConfig = config
            };
            DerCircuitConfig circuitConfig = new DerCircuitConfig
            {
                ChangedBy = "Test",
                Name = "Circuit",
                DerGroupConfig = groupConfig
            };
            groupConfig.DerCircuits.Add(circuitConfig);
            config.DerGroupConfigs.Add(groupConfig);

            Mock<IDerRepository> repositoryMock = new Mock<IDerRepository>();
            repositoryMock.Setup(repository => repository.GetDer(true)).ReturnsAsync(config);
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();
            DeviceManager manager = new DeviceManager(NullLogger<DeviceManager>.Instance, repositoryMock.Object, servicesMock.Object,
                _publisherFactory.Object, _deviceFactoryModbus.Object, _deviceFactoryC37.Object);

            DerUnitConfig unsupportedConfig = new UnsupportedDerUnitConfig
            {
                ChangedBy = "Test",
                Name = "Unsupported",
                DerCircuitConfig = circuitConfig
            };
            circuitConfig.DerUnitConfigs.Add(unsupportedConfig);

            InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await manager.LoadDerAsync());

            StringAssert.Contains(exception.Message, nameof(UnsupportedDerUnitConfig));
            Assert.AreEqual("Uninitialized DER", manager.Der.Name);
        }


        private sealed class UnsupportedDerUnitConfig : DerUnitConfig
        {
        }


        [TestMethod]
        public async Task DeviceManagerStartAllPcsEmptyTest()
        {
            IDeviceManager manager = CreateDeviceManagerWithDomains();

            await manager.StartAllPcsAsync();

            Assert.HasCount(1, manager.PowerConversionSystems);
            _pcsMock!.Verify(x => x.StartAsync(), Times.Once);
        }


        [TestMethod]
        public async Task DeviceManagerStopAllPcsEmptyTest()
        {
            IDeviceManager manager = CreateDeviceManagerWithDomains();

            await manager.StopAllPcsAsync();

            Assert.HasCount(1, manager.PowerConversionSystems);
            _pcsMock!.Verify(x => x.StopAsync(), Times.Once);
        }


        [TestMethod]
        public async Task DeviceManagerStandbyAllPcsEmptyTest()
        {
            IDeviceManager manager = CreateDeviceManagerWithDomains();

            await manager.StandbyAllPcsAsync();

            Assert.HasCount(1, manager.PowerConversionSystems);
            _pcsMock!.Verify(x => x.StandbyAsync(), Times.Once);
        }


        [TestMethod]
        public async Task DeviceManagerStartPcsMissingDeviceTest()
        {
            IDeviceManager manager = CreateDeviceManagerWithDomains();

            await manager.StartPcsAsync(99);

            Assert.HasCount(1, manager.PowerConversionSystems);
            IEnumerable<FakeLogRecord> logs = _logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.HasCount(1, logs);
            Assert.IsNotNull(logs.FirstOrDefault(l => l.Message.Contains("cannot find", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public async Task DeviceManagerStartPcsTest()
        {
            IDeviceManager manager = CreateDeviceManagerWithDomains();

            await manager.StartPcsAsync(0);

            Assert.HasCount(1, manager.PowerConversionSystems);
            IEnumerable<FakeLogRecord> logs = _logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.HasCount(0, logs);
            _pcsMock!.Verify(x => x.StartAsync(), Times.Once);
        }


        [TestMethod]
        public async Task DeviceManagerStopPcsMissingDeviceTest()
        {
            IDeviceManager manager = CreateDeviceManagerWithDomains();

            await manager.StopPcsAsync(99);

            Assert.HasCount(1, manager.PowerConversionSystems);
            IEnumerable<FakeLogRecord> logs = _logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.HasCount(1, logs);
            Assert.IsNotNull(logs.FirstOrDefault(l => l.Message.Contains("cannot find", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public async Task DeviceManagerStopPcsTest()
        {
            IDeviceManager manager = CreateDeviceManagerWithDomains();

            await manager.StopPcsAsync(0);

            Assert.HasCount(1, manager.PowerConversionSystems);
            IEnumerable<FakeLogRecord> logs = _logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.HasCount(0, logs);
            _pcsMock!.Verify(x => x.StopAsync(), Times.Once);
        }


        [TestMethod]
        public async Task DeviceManagerStandbyPcsMissingDeviceTest()
        {
            IDeviceManager manager = CreateDeviceManagerWithDomains();

            await manager.StandbyPcsAsync(99);

            Assert.HasCount(1, manager.PowerConversionSystems);
            IEnumerable<FakeLogRecord> logs = _logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.HasCount(1, logs);
            Assert.IsNotNull(logs.FirstOrDefault(l => l.Message.Contains("cannot find", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public async Task DeviceManagerStandbyPcsTest()
        {
            IDeviceManager manager = CreateDeviceManagerWithDomains();

            await manager.StandbyPcsAsync(0);

            Assert.HasCount(1, manager.PowerConversionSystems);
            IEnumerable<FakeLogRecord> logs = _logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.HasCount(0, logs);
            _pcsMock!.Verify(x => x.StandbyAsync(), Times.Once);
        }


        [TestMethod]
        public async Task DeviceManagerConnectBatteryBankMissingDeviceTest()
        {
            IDeviceManager manager = CreateDeviceManagerWithDomains();

            await manager.ConnectBatteryBankAsync(99);

            Assert.HasCount(1, manager.BatteryBanks);
            IEnumerable<FakeLogRecord> logs = _logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.HasCount(1, logs);
            Assert.IsNotNull(logs.FirstOrDefault(l => l.Message.Contains("cannot find", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public async Task DeviceManagerConnectBatteryBankTest()
        {
            IDeviceManager manager = CreateDeviceManagerWithDomains();

            await manager.ConnectBatteryBankAsync(0);

            Assert.HasCount(1, manager.BatteryBanks);
            IEnumerable<FakeLogRecord> logs = _logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.HasCount(0, logs);
            _bbMock!.Verify(x => x.ConnectAsync(), Times.Once);
        }


        [TestMethod]
        public async Task DeviceManagerDisconnectBatteryBankMissingDeviceTest()
        {
            IDeviceManager manager = CreateDeviceManagerWithDomains();

            await manager.DisconnectBatteryBankAsync(99);

            Assert.HasCount(1, manager.BatteryBanks);
            IEnumerable<FakeLogRecord> logs = _logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.HasCount(1, logs);
            Assert.IsNotNull(logs.FirstOrDefault(l => l.Message.Contains("cannot find", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public async Task DeviceManagerDisconnectBatteryBankTest()
        {
            IDeviceManager manager = CreateDeviceManagerWithDomains();

            await manager.DisconnectBatteryBankAsync(0);

            Assert.HasCount(1, manager.BatteryBanks);
            IEnumerable<FakeLogRecord> logs = _logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.HasCount(0, logs);
            _bbMock!.Verify(x => x.DisconnectAsync(), Times.Once);
        }


        [TestMethod]
        public void DeviceManagerPutIntoMaintenanceMissingUnitTest()
        {
            IDeviceManager manager = CreateDeviceManagerWithDomains();

            manager.PutIntoMaintenance("Missing unit");

            IEnumerable<FakeLogRecord> logs = _logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.HasCount(1, logs);
            Assert.IsNotNull(logs.FirstOrDefault(l => l.Message.Contains("cannot find", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public async Task DeviceManagerSetPcsPowerTargetMissingDeviceTest()
        {
            IDeviceManager manager = CreateDeviceManagerWithDomains();

            await manager.SetPcsPowerTarget(99, 100, 20);

            Assert.HasCount(1, manager.PowerConversionSystems);
            IEnumerable<FakeLogRecord> logs = _logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.HasCount(1, logs);
            Assert.IsNotNull(logs.FirstOrDefault(l => l.Message.Contains("cannot find", StringComparison.OrdinalIgnoreCase)));
        }


        [TestMethod]
        public async Task DeviceManagerSetPcsPowerTargetTest()
        {
            IDeviceManager manager = CreateDeviceManagerWithDomains();

            await manager.SetPcsPowerTarget(0, 100, 20);

            Assert.HasCount(1, manager.PowerConversionSystems);
            IEnumerable<FakeLogRecord> logs = _logger.Collector.GetSnapshot().Where(l => l.Level == LogLevel.Error);
            Assert.HasCount(0, logs);
            _pcsMock!.Verify(x => x.SetActivePowerTargetAsync(100), Times.Once);
            _pcsMock!.Verify(x => x.SetReactivePowerTargetAsync(20), Times.Once);
        }


        private IDeviceManager CreateDeviceManagerWithDomains()
        {

            Mock<IDerRepository> repositoryMock = new Mock<IDerRepository>();
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();
            DeviceManagerTestClass deviceManager = new DeviceManagerTestClass(_logger, repositoryMock.Object, servicesMock.Object,
                _publisherFactory.Object, _deviceFactoryModbus.Object, _deviceFactoryC37.Object);
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
            Mock<PowerConversionSystemConfig> pcsConfig = new Mock<PowerConversionSystemConfig>();
            pcsConfig.SetupGet(x => x.Name).Returns("PowerConversionSystemConfig");
            Mock<BatteryBankConfig> bbConfig = new Mock<BatteryBankConfig>();
            bbConfig.SetupGet(x => x.Name).Returns("BatteryBankConfig");
            _pcsMock = new Mock<PowerConversionSystemBase>(NullLogger.Instance, pcsConfig.Object, unitMock.Object, metricsPublisherMock.Object, datafaceMock.Object);
            _bbMock = new Mock<BatteryBankBase>(NullLogger.Instance, bbConfig.Object, unitMock.Object, metricsPublisherMock.Object, datafaceMock.Object);
            // Add devices
            deviceManager.AddPcs(0, _pcsMock.Object);
            deviceManager.AddBb(0, _bbMock.Object);

            return deviceManager;
        }


        class DeviceManagerTestClass : DeviceManager
        {
            public DeviceManagerTestClass(ILogger<DeviceManager> logger, IDerRepository repository, IServiceProvider services,
                IMetricsPublisherFactory publisherFactory, IModbusDeviceFactory deviceFactoryModbus, IC37DeviceFactory deviceFactoryC37)
                : base(logger, repository, services, publisherFactory, deviceFactoryModbus, deviceFactoryC37)
            {
            }

            public void AddPcs(int deviceId, PowerConversionSystemBase pcs)
            {
                _powerConversionSystems.Add(deviceId, pcs);
            }

            public void AddBb(int deviceId, BatteryBankBase bb)
            {
                _batteryBanks.Add(deviceId, bb);
            }
        }
    }
}

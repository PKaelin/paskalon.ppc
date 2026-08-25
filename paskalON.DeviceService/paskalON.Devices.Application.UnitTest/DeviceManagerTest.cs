// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Infrastructure.Storage;

namespace paskalON.Devices.Application.UnitTest
{
    [TestClass]
    public sealed class DeviceManagerTest
    {
        [TestMethod]
        public void DeviceManagerConstructorNullLoggerTest()
        {
            Mock<IDerRepository> repositoryMock = new Mock<IDerRepository>();
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new DeviceManager(null!, repositoryMock.Object, servicesMock.Object));
        }


        [TestMethod]
        public void DeviceManagerConstructorNullRepositoryTest()
        {
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new DeviceManager(NullLogger.Instance, null!, servicesMock.Object));
        }


        [TestMethod]
        public void DeviceManagerConstructorNullServicesTest()
        {
            Mock<IDerRepository> repositoryMock = new Mock<IDerRepository>();

            Assert.ThrowsExactly<ArgumentNullException>(() => new DeviceManager(NullLogger.Instance, repositoryMock.Object, null!));
        }


        [TestMethod]
        public void DeviceManagerConstructorTest()
        {
            Mock<IDerRepository> repositoryMock = new Mock<IDerRepository>();
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();

            DeviceManager manager = new DeviceManager(NullLogger.Instance, repositoryMock.Object, servicesMock.Object);

            Assert.IsNotNull(manager.Der);
            Assert.AreEqual("Uninitialized DER", manager.Der.Name);
            Assert.IsNotNull(manager.PowerConversionSystems);
            Assert.IsNotNull(manager.BatteryBanks);
            Assert.IsNotNull(manager.Solars);
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
            DeviceManager manager = new DeviceManager(NullLogger.Instance, repositoryMock.Object, servicesMock.Object);

            await manager.LoadDer();

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
            Assert.HasCount(0, manager.Solars);
        }


        [TestMethod]
        public async Task DeviceManagerLoadDerRepositoryThrowsExceptionTest()
        {
            InvalidOperationException exception = new InvalidOperationException("Test exception");
            Mock<IDerRepository> repositoryMock = new Mock<IDerRepository>();
            repositoryMock.Setup(repository => repository.GetDer(true)).ThrowsAsync(exception);
            Mock<IServiceProvider> servicesMock = new Mock<IServiceProvider>();
            DeviceManager manager = new DeviceManager(NullLogger.Instance, repositoryMock.Object, servicesMock.Object);

            InvalidOperationException result = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await manager.LoadDer());

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
            DeviceManager manager = new DeviceManager(NullLogger.Instance, repositoryMock.Object, servicesMock.Object);

            await manager.LoadDer();

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
            DeviceManager manager = new DeviceManager(NullLogger.Instance, repositoryMock.Object, servicesMock.Object);
            Der placeholderDer = manager.Der;

            await manager.LoadDer();

            Assert.AreNotSame(placeholderDer, manager.Der);
            Assert.AreEqual("Loaded DER", manager.Der.Name);
            Assert.AreSame(manager.Der, manager.Der);
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
            DeviceManager manager = new DeviceManager(NullLogger.Instance, repositoryMock.Object, servicesMock.Object);

            DerUnitConfig unsupportedConfig = new UnsupportedDerUnitConfig
            {
                ChangedBy = "Test",
                Name = "Unsupported",
                DerCircuitConfig = circuitConfig
            };
            circuitConfig.DerUnitConfigs.Add(unsupportedConfig);

            InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await manager.LoadDer());

            StringAssert.Contains(exception.Message, nameof(UnsupportedDerUnitConfig));
            Assert.AreEqual("Uninitialized DER", manager.Der.Name);
        }


        private sealed class UnsupportedDerUnitConfig : DerUnitConfig
        {
        }

    }
}

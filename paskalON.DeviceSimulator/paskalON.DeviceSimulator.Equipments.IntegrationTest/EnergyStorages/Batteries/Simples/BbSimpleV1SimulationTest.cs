// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.DeviceSimulator.Application.Factories;
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.Modbus;
using paskalON.Telemetry;
using System.Net.Sockets;

namespace paskalON.DeviceSimulator.Equipments.IntegrationTest.EnergyStorages.Batteries.Simples
{
    [TestClass]
    public sealed class BbSimpleV1SimulationTest
    {
        private Mock<IMetricsPublisher> _publisher = new Mock<IMetricsPublisher>();
        private Mock<DerBatteryStorageUnit>? _unit;
        private BatteryBankConfig? _bbConfig;
        private ModbusConfig? _bbModbusConfig;

        [TestInitialize]
        public void TestInitialize()
        {
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
            // Unit
            Mock<DerBatteryStorageUnitConfig> unitConfig = new Mock<DerBatteryStorageUnitConfig>();
            unitConfig.SetupGet(x => x.Name).Returns("DerBatteryStorageUnitConfig");
            _unit = new Mock<DerBatteryStorageUnit>(NullLogger.Instance, unitConfig.Object, circuit.Object);
            Mock<PowerConversionSystemDeviceConfig> pcsDeviceConfig = new Mock<PowerConversionSystemDeviceConfig>();
            pcsDeviceConfig.SetupGet(x => x.Name).Returns("PowerConversionSystemDeviceConfig");

            BatteryBankDeviceConfig bbDeviceConfig = new BatteryBankDeviceConfig
            {
                ChangedBy = "Test",
                Name = "BatteryBankDeviceConfig",
                ClassName = "Test",
                AbsoluteMinimumStateOfCharge = 0,
                AbsoluteMaximumStateOfCharge = 100,
                UsableMinimumStateOfCharge = 10,
                UsableMaximumStateOfCharge = 90,
                PreferredMinimumStateOfCharge = 20,
                PreferredMaximumStateOfCharge = 80,
                NameplateCapacity = 5000000
            };

            ModbusConnectionConfig modbusConnection = new ModbusConnectionConfig
            {
                ChangedBy = "Test",
                Name = "ModbusConnectionConfig",
            };

            ModbusConfig pcsModbusConfig = new ModbusConfig
            {
                ChangedBy = "Test",
                Name = "ModbusConfig",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartPcs,
                AddressFamily = AddressFamily.InterNetwork,
                UnitId = 1,
                ModbusConnectionConfig = modbusConnection
            };

            // Device
            PowerConversionSystemConfig pcsConfig = new PowerConversionSystemConfig
            {
                ChangedBy = "Test",
                IsActive = true,
                DeviceId = 1,
                Name = "PowerConversionSystemConfig",
                PowerConversionSystemDeviceConfig = pcsDeviceConfig.Object,
                ModbusConfig = pcsModbusConfig,
                DerUnitConfig = unitConfig.Object,
            };

            _bbModbusConfig = new ModbusConfig
            {
                ChangedBy = "Test",
                Name = "ModbusConfig",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartBms,
                AddressFamily = AddressFamily.InterNetwork,
                UnitId = 1,
                ModbusConnectionConfig = modbusConnection
            };

            _bbConfig = new BatteryBankConfig
            {
                IsActive = true,
                ChangedBy = "Test",
                Name = "BMS 1",
                DeviceId = 0,
                InitiallyConnected = true,
                ModbusConfig = _bbModbusConfig,
                BatteryBankDeviceConfig = bbDeviceConfig,
                DerUnitConfig = unitConfig.Object,
            };
        }


        [TestMethod]
        public void BbSimpleV1SimulationNullStoreTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(_bbModbusConfig!);
            BbSimpleV1Proxy device = new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!, _unit!.Object, _publisher.Object, dataface, client);

            Assert.ThrowsExactly<ArgumentNullException>(() => new BbSimpleV1Simulation(null!, device));
        }


        [TestMethod]
        public void BbSimpleV1SimulationNullDeviceTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(_bbModbusConfig!);

            Assert.ThrowsExactly<ArgumentNullException>(() => new BbSimpleV1Simulation(((MemoryModbusClient)client).Store, null!));
        }


        [TestMethod]
        public void BbSimpleV1SimulationConstructorTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(_bbModbusConfig!);
            BbSimpleV1Proxy device = new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!, _unit!.Object, _publisher.Object, dataface, client);

            BbSimpleV1Simulation simulation = new BbSimpleV1Simulation(((MemoryModbusClient)client).Store, device);

            Assert.IsNotNull(simulation);
            Assert.AreEqual(device.Name, simulation.Name);
            Assert.AreEqual(87.5, simulation.UsableStateOfCharge);
            Assert.AreEqual(4000000, simulation.UsableCapacity);
        }


        [TestMethod]
        public async Task BbSimpleV1SimulationDeviceAllocatedPowerNullTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(_bbModbusConfig!);
            BbSimpleV1Proxy device = new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!, _unit!.Object, _publisher.Object, dataface, client);
            BbSimpleV1Simulation simulation = new BbSimpleV1Simulation(((MemoryModbusClient)client).Store, device);

            await simulation.TickAsync(TimeSpan.FromSeconds(1), CancellationToken.None);

            Assert.AreEqual(87.5, simulation.UsableStateOfCharge);
            Assert.AreEqual(4000000, simulation.UsableCapacity);
        }


        [TestMethod]
        public async Task BbSimpleV1SimulationDeviceAllocatedPowerSameAsCapacityFor30MinTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(_bbModbusConfig!);
            BbSimpleV1Proxy device = new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!, _unit!.Object, _publisher.Object, dataface, client);
            BbSimpleV1Simulation simulation = new BbSimpleV1Simulation(((MemoryModbusClient)client).Store, device);
            device.AllocatedActivePowerValue = 4000000;

            await simulation.TickAsync(TimeSpan.FromMinutes(30), CancellationToken.None);

            Assert.AreEqual(37.5, Math.Round(simulation.UsableStateOfCharge, 2));
            Assert.AreEqual(4000000, simulation.UsableCapacity);
        }


        [TestMethod]
        public async Task BbSimpleV1SimulationDeviceAllocatedPowerSameAsCapacityFor30MinThenHalfFor10MinTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(_bbModbusConfig!);
            BbSimpleV1Proxy device = new BbSimpleV1Proxy(NullLogger.Instance, _bbConfig!, _unit!.Object, _publisher.Object, dataface, client);
            BbSimpleV1Simulation simulation = new BbSimpleV1Simulation(((MemoryModbusClient)client).Store, device);
            device.AllocatedActivePowerValue = 4000000;
            await simulation.TickAsync(TimeSpan.FromMinutes(30), CancellationToken.None);
            device.AllocatedActivePowerValue = 2000000;
            await simulation.TickAsync(TimeSpan.FromMinutes(10), CancellationToken.None);

            Assert.AreEqual(29.17, Math.Round(simulation.UsableStateOfCharge, 2));
            Assert.AreEqual(4000000, simulation.UsableCapacity);
        }
    }
}

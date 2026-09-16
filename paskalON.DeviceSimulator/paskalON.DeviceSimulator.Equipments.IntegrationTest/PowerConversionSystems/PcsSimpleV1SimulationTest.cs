// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.DeviceSimulator.Application.Factories;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems.Simples;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.Modbus;
using paskalON.Telemetry;
using System.Net.Sockets;

namespace paskalON.DeviceSimulator.Equipments.IntegrationTest.PowerConversionSystems
{
    [TestClass]
    public sealed class PcsSimpleV1SimulationTest
    {
        private Mock<IMetricsPublisher> _publisher = new Mock<IMetricsPublisher>();
        private Mock<DerBatteryStorageUnit>? _unit;
        private PowerConversionSystemConfig? _pcsConfig;
        private ModbusConfig? _pcsModbusConfig;


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
            Mock<PowerConversionSystemDeviceConfig> deviceConfig = new Mock<PowerConversionSystemDeviceConfig>();
            deviceConfig.SetupGet(x => x.Name).Returns("PowerConversionSystemDeviceConfig");

            ModbusConnectionConfig modbusConnection = new ModbusConnectionConfig
            {
                ChangedBy = "Test",
                Name = "ModbusConnectionConfig",
            };

            _pcsModbusConfig = new ModbusConfig
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
            _pcsConfig = new PowerConversionSystemConfig
            {
                ChangedBy = "Test",
                IsActive = true,
                DeviceId = 1,
                Name = "PowerConversionSystemConfig",
                PowerConversionSystemDeviceConfig = deviceConfig.Object,
                ModbusConfig = _pcsModbusConfig,
                DerUnitConfig = unitConfig.Object,
            };
        }


        [TestMethod]
        public void PcsSimpleV1SimulationNullStoreTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(_pcsModbusConfig!);
            PcsSimpleV1Proxy device = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, _publisher.Object, dataface, client);

            Assert.ThrowsExactly<ArgumentNullException>(() => new PcsSimpleV1Simulation(null!, device));
        }


        [TestMethod]
        public void PcsSimpleV1SimulationNullDeviceTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(_pcsModbusConfig!);

            Assert.ThrowsExactly<ArgumentNullException>(() => new PcsSimpleV1Simulation(((MemoryModbusClient)client).Store, null!));
        }


        [TestMethod]
        public void PcsSimpleV1SimulationConstructorTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(_pcsModbusConfig!);
            PcsSimpleV1Proxy device = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, _publisher.Object, dataface, client);

            PcsSimpleV1Simulation simulation = new PcsSimpleV1Simulation(((MemoryModbusClient)client).Store, device);

            Assert.IsNotNull(simulation);
            Assert.AreEqual(device.Name, simulation.Name);
        }


        [TestMethod]
        public async Task PcsSimpleV1SimulationWritesTargetAndReadsSimulatedOutputTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(_pcsModbusConfig!);
            PcsSimpleV1Proxy device = new PcsSimpleV1Proxy(NullLogger.Instance, _pcsConfig!, _unit!.Object, _publisher.Object, dataface, client);
            PcsSimpleV1Simulation simulation = new PcsSimpleV1Simulation(((MemoryModbusClient)client).Store, device);

            await client.WriteSingleRegisterAsync((ushort)PcsSimpleV1Description.Register.QReference, 121, ModbusDataType.MbInt16);
            await simulation.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);
            ushort[]? output = await client.ReadHoldingRegistersAsync((ushort)PcsSimpleV1Description.Register.Q, (ushort)PcsSimpleV1Description.Register.Q);

            Assert.IsNotNull(dataface);
            Assert.IsNotNull(output);
            Assert.IsNotNull(client);
            Assert.IsNotNull(((MemoryModbusClient)client).Store);
            Assert.AreEqual((ushort)121, output[0]);
        }

    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Moq;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.DeviceSimulator.Application.Factories;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.Modbus;
using paskalON.Telemetry;
using System.Net.Sockets;

namespace paskalON.DeviceSimulator.Application.IntegrationTest.Factories
{
    [TestClass]
    public sealed class SimulationModbusDeviceFactoryTest
    {
        [TestMethod]
        public async Task SimulationModbusDeviceFactoryNullStoreTest()
        {
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            Assert.ThrowsExactly<ArgumentNullException>(() => factory.Create(null!));
        }


        [TestMethod]
        public async Task SimulationModbusDeviceFactoryWritesTargetAndReadsSimulatedOutputTest()
        {
            ModbusConfig modbusConfig = new ModbusConfig
            {
                ChangedBy = "Test",
                Name = "ModbusConfig",
                Address = Constants.Ip4Localhost,
                Port = Constants.PortStartPcs,
                AddressFamily = AddressFamily.InterNetwork,
                UnitId = 1,
                ModbusConnectionConfig = new ModbusConnectionConfig { Name = "Simulation", ChangedBy = "IntegrationTest" }
            };


            Mock<IMetricsPublisher> publisher = new Mock<IMetricsPublisher>();
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(modbusConfig!);

            Assert.IsNotNull(dataface);
            Assert.AreEqual(modbusConfig.Name, dataface.Name);
            Assert.IsNotNull(client);
            Assert.AreEqual(modbusConfig.Address, client.ServerAddress);
            Assert.AreEqual(modbusConfig.Port, client.ServerPort);
            Assert.AreEqual(modbusConfig.UnitId, client.UnitId);
            // MemoryModbusClient does not communicate hence connected
            Assert.AreEqual(ModbusClientState.Connected, client.State);
        }
    }
}

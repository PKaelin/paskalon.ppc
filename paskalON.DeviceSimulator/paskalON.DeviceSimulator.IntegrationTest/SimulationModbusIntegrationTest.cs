// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.Devices.Equipments.PowerConversionSystems.PowerElectronics;
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.DeviceSimulator.Application.Factories;
using paskalON.DeviceSimulator.Application.Simulations;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems.PowerElectronics;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems.Simples;
using paskalON.Protocols.Modbus;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.IntegrationTest
{
    [TestClass]
    public sealed class SimulationModbusIntegrationTest
    {
        [TestMethod]
        public async Task SimulationModbusDeviceFactoryWritesTargetAndReadsSimulatedOutputTest()
        {
            ModbusConfig config = new ModbusConfig
            {
                Name = "PcsSimpleV1",
                ChangedBy = "IntegrationTest",
                Address = "127.0.0.1",
                Port = 5502,
                UnitId = 1,
                AddressFamily = AddressFamily.InterNetwork,
                ModbusConnectionConfig = new ModbusConnectionConfig { Name = "Simulation", ChangedBy = "IntegrationTest" }
            };
            SimulationStoreRegistry stores = new SimulationStoreRegistry();
            SimulationModbusDeviceFactory factory = new SimulationModbusDeviceFactory(stores);
            (IModbusDataface dataface, IModbusClient client) = factory.Create(config);
            PcsSimpleV1Simulation simulation = new PcsSimpleV1Simulation(
                ((MemoryModbusClient)client).Store);

            await client.WriteSingleRegisterAsync(
                (ushort)PcsSimpleV1Description.Register.QReference,
                120,
                ModbusDataType.MbInt16);
            await simulation.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);
            ushort[]? output = await client.ReadHoldingRegistersAsync(
                (ushort)PcsSimpleV1Description.Register.Q,
                (ushort)PcsSimpleV1Description.Register.Q);

            Assert.IsNotNull(dataface);
            Assert.IsNotNull(output);
            Assert.AreEqual((ushort)114, output[0]);
        }


        [TestMethod]
        public async Task PcsPcskV4SimulationWritesCapabilityAndOutputRegistersTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore();
            PcsPcskV4Simulation simulation = new PcsPcskV4Simulation(store);
            store.HoldingRegisters.WritePoints((ushort)PcsPcskV4Description.Register.QReference, new ushort[] { 200 });

            await simulation.TickAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);

            Assert.AreEqual((ushort)190, store.HoldingRegisters.ReadPoints(
                (ushort)PcsPcskV4Description.Register.Q, 1)[0]);
            Assert.AreEqual((ushort)60000, store.HoldingRegisters.ReadPoints(
                (ushort)PcsPcskV4Description.Register.QCapability, 1)[0]);
        }
    }
}
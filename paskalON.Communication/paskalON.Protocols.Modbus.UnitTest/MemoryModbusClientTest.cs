// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.Protocols.Modbus.UnitTest
{
    [TestClass]
    public sealed class MemoryModbusClientTest
    {
        [TestMethod]
        public void MemoryModbusClientRejectsNullStoreTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new MemoryModbusClient(null!));
        }


        [TestMethod]
        public async Task MemoryModbusClientReadsAndWritesCoilDiscretesTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(4, 4, 8, 8);
            MemoryModbusClient client = new MemoryModbusClient(store, 2);
            store.CoilDiscretes.WritePoints(1, new[] { true, false });

            bool[]? coils = await client.ReadCoilsAsync(1, 2);

            CollectionAssert.AreEqual(new[] { true, false }, coils);
            Assert.AreEqual((byte)2, client.UnitId);
            Assert.AreEqual(ModbusClientState.Connected, client.State);
        }


        [TestMethod]
        public async Task MemoryModbusClientReadsAndWritesCoilInputsTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(4, 4, 8, 8);
            MemoryModbusClient client = new MemoryModbusClient(store, 2);
            store.CoilInputs.WritePoints(1, new[] { false, true });

            bool[]? inputs = await client.ReadDiscreteInputsAsync(1, 2);

            CollectionAssert.AreEqual(new[] { false, true }, inputs);
            Assert.AreEqual((byte)2, client.UnitId);
            Assert.AreEqual(ModbusClientState.Connected, client.State);
        }


        [TestMethod]
        public async Task MemoryModbusClientReadsAndWritesInputRegisterTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(4, 4, 8, 8);
            MemoryModbusClient client = new MemoryModbusClient(store, 2);
            store.InputRegisters.WritePoints(1, new ushort[] { 30, 40 });

            ushort[]? inputRegisters = await client.ReadInputRegistersAsync(1, 2);

            CollectionAssert.AreEqual(new ushort[] { 30, 40 }, inputRegisters);
            Assert.AreEqual((byte)2, client.UnitId);
            Assert.AreEqual(ModbusClientState.Connected, client.State);
        }



        [TestMethod]
        public async Task MemoryModbusClientReadsAndWritesHoldingRegisterTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(4, 4, 8, 8);
            MemoryModbusClient client = new MemoryModbusClient(store, 2);

            await client.WriteSingleRegisterAsync(3, (ushort)123, ModbusDataType.MbUint16);
            await client.WriteMultipleRegistersAsync(4, new ushort[] { 456, 789 }, ModbusDataType.MbUint16);

            CollectionAssert.AreEqual(new ushort[] { 123, 456, 789 }, store.HoldingRegisters.ReadPoints(3, 3));
            Assert.AreEqual((byte)2, client.UnitId);
            Assert.AreEqual(ModbusClientState.Connected, client.State);
        }


        [TestMethod]
        public async Task MemoryModbusClientConvertsDoubleWritesTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(holdingRegisterCount: 4);
            MemoryModbusClient client = new MemoryModbusClient(store);

            await client.WriteSingleRegisterAsync(1, 12.5, ModbusDataType.MbInt16);

            ushort[] registers = store.HoldingRegisters.ReadPoints(1, 2);
            Assert.AreEqual((ushort)12, registers[0]);
            Assert.AreEqual((ushort)0, registers[1]);
        }
    }
}
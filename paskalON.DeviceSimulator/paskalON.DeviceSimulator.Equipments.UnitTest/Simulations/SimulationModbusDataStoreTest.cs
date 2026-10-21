// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Equipments.UnitTest.Simulations
{
    [TestClass]
    public sealed class SimulationModbusDataStoreTest
    {
        [TestMethod]
        public void SimulationModbusDataStoreNullDeviceThrowsTest()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new SimulationModbusDataStore(null!));
        }


        [TestMethod]
        public void TryTakeWriteRecordsClientWriteOnceTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            store.HoldingRegisters.WritePoints(41000, new ushort[] { 3 });

            bool first = store.TryTakeWrite(41000);
            bool second = store.TryTakeWrite(41000);

            Assert.IsTrue(first);
            Assert.IsFalse(second);
            Assert.AreEqual((ushort)3, store.Device.HoldingRegisters.ReadPoints(41000, 1)[0]);
        }


        [TestMethod]
        public void TryTakeWriteRecordsSameValueWrittenAgainTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            store.HoldingRegisters.WritePoints(41000, new ushort[] { 3 });
            store.TryTakeWrite(41000);
            store.HoldingRegisters.WritePoints(41000, new ushort[] { 3 });

            bool written = store.TryTakeWrite(41000);

            Assert.IsTrue(written);
        }


        [TestMethod]
        public void TryTakeWriteIgnoresDeviceWriteTest()
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            store.Device.HoldingRegisters.WritePoints(44000, new ushort[] { 2 });

            bool written = store.TryTakeWrite(44000);

            Assert.IsFalse(written);
            Assert.AreEqual((ushort)2, store.HoldingRegisters.ReadPoints(44000, 1)[0]);
        }


        [TestMethod]
        [DataRow(41001, DisplayName = "First register")]
        [DataRow(41002, DisplayName = "Second register")]
        [DataRow(41003, DisplayName = "Third register")]
        public void TryTakeWriteRecordsEveryRegisterOfMultipleWriteTest(int address)
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());
            store.HoldingRegisters.WritePoints(41001, new ushort[] { 10, 20, 30 });

            bool written = store.TryTakeWrite(address);

            Assert.IsTrue(written);
        }


        [TestMethod]
        [DataRow(-1)]
        [DataRow(65536)]
        public void TryTakeWriteInvalidAddressThrowsTest(int address)
        {
            SimulationModbusDataStore store = new SimulationModbusDataStore(new ModbusDataMemoryStore());

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.TryTakeWrite(address));
        }
    }
}

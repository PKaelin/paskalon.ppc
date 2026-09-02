// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.Protocols.Modbus.UnitTest.Stores
{
    [TestClass]
    public sealed class ModbusDataMemoryStoreTest
    {
        [TestMethod]
        [DataRow(-1, 2, 2, 2, DisplayName = "NegativeCoilCount")]
        [DataRow(65536, 2, 2, 2, DisplayName = "CoilCountTooLarge")]
        [DataRow(2, -1, 2, 2, DisplayName = "NegativeDiscreteInputCount")]
        [DataRow(2, 65536, 2, 2, DisplayName = "DiscreteInputCountTooLarge")]
        [DataRow(2, 2, -1, 2, DisplayName = "NegativeHoldingRegisterCount")]
        [DataRow(2, 2, 65536, 2, DisplayName = "HoldingRegisterCountTooLarge")]
        [DataRow(2, 2, 2, -1, DisplayName = "NegativeInputRegisterCount")]
        [DataRow(2, 2, 2, 65536, DisplayName = "InputRegisterCountTooLarge")]
        public void ModbusDataMemoryStoreConstructorRejectsInvalidCountsTest(int coilCount, int discreteInputCount, int holdingRegisterCount, int inputRegisterCount)
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ModbusDataMemoryStore(coilCount, discreteInputCount, holdingRegisterCount, inputRegisterCount));
        }


        [TestMethod]
        public void ModbusDataMemoryStoreConstructorCreatesEmptyAreasTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(3, 4, 5, 6);

            CollectionAssert.AreEqual(new bool[] { false, false, false }, store.ReadCoils(0, 2));
            CollectionAssert.AreEqual(new bool[] { false, false, false, false }, store.ReadDiscreteInputs(0, 3));
            CollectionAssert.AreEqual(new ushort[] { 0, 0, 0, 0, 0 }, store.ReadHoldingRegisters(0, 4));
            CollectionAssert.AreEqual(new ushort[] { 0, 0, 0, 0, 0, 0 }, store.ReadInputRegisters(0, 5));
        }


        [TestMethod]
        public void ModbusDataMemoryStoreWritesAndReadsCoilsTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(coilCount: 8);
            bool[] values = new[] { true, false, true, true };

            store.WriteCoils(2, values);

            CollectionAssert.AreEqual(new[] { false, false, true, false, true, true, false, false }, store.ReadCoils(0, 7));
            CollectionAssert.AreEqual(values, store.ReadCoils(2, 5));
        }


        [TestMethod]
        public void ModbusDataMemoryStoreWritesAndReadsDiscreteInputsTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(discreteInputCount: 8);
            bool[] values = new[] { false, true, true, false };

            store.WriteDiscreteInputs(2, values);

            CollectionAssert.AreEqual(new[] { false, false, false, true, true, false, false, false }, store.ReadDiscreteInputs(0, 7));
            CollectionAssert.AreEqual(values, store.ReadDiscreteInputs(2, 5));
        }


        [TestMethod]
        public void ModbusDataMemoryStoreWritesAndReadsHoldingRegistersTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(holdingRegisterCount: 8);
            ushort[] values = new ushort[] { 1, 32768, 65535, 42 };

            store.WriteHoldingRegisters(2, values);

            CollectionAssert.AreEqual(new ushort[] { 0, 0, 1, 32768, 65535, 42, 0, 0 }, store.ReadHoldingRegisters(0, 7));
            CollectionAssert.AreEqual(values, store.ReadHoldingRegisters(2, 5));
        }


        [TestMethod]
        public void ModbusDataMemoryStoreWritesAndReadsInputRegistersTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(inputRegisterCount: 8);
            ushort[] values = new ushort[] { 65535, 0, 12345, 54321 };

            store.WriteInputRegisters(2, values);

            CollectionAssert.AreEqual(new ushort[] { 0, 0, 65535, 0, 12345, 54321, 0, 0 }, store.ReadInputRegisters(0, 7));
            CollectionAssert.AreEqual(values, store.ReadInputRegisters(2, 5));
        }


        [TestMethod]
        public void ModbusDataMemoryStoreReadReturnsCopyTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(coilCount: 3, holdingRegisterCount: 3);
            store.WriteCoils(0, new[] { true, false, true });
            store.WriteHoldingRegisters(0, new ushort[] { 10, 20, 30 });

            bool[] coils = store.ReadCoils(0, 2);
            ushort[] registers = store.ReadHoldingRegisters(0, 2);
            coils[0] = false;
            registers[0] = 999;

            CollectionAssert.AreEqual(new[] { true, false, true }, store.ReadCoils(0, 2));
            CollectionAssert.AreEqual(new ushort[] { 10, 20, 30 }, store.ReadHoldingRegisters(0, 2));
        }


        [TestMethod]
        public void ModbusDataMemoryStoreWritesCopyInputValuesTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(coilCount: 2, holdingRegisterCount: 2);
            bool[] coils = new[] { true, false };
            ushort[] registers = new ushort[] { 100, 200 };

            store.WriteCoils(0, coils);
            store.WriteHoldingRegisters(0, registers);
            coils[0] = false;
            registers[0] = 999;

            CollectionAssert.AreEqual(new[] { true, false }, store.ReadCoils(0, 1));
            CollectionAssert.AreEqual(new ushort[] { 100, 200 }, store.ReadHoldingRegisters(0, 1));
        }


        [TestMethod]
        public void ModbusDataMemoryStoreOverwritesOnlyRequestedRangeTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(holdingRegisterCount: 6);
            store.WriteHoldingRegisters(0, new ushort[] { 10, 20, 30, 40, 50, 60 });
            store.WriteHoldingRegisters(2, new ushort[] { 300, 400 });

            CollectionAssert.AreEqual(new ushort[] { 10, 20, 300, 400, 50, 60 }, store.ReadHoldingRegisters(0, 5));
        }


        [TestMethod]
        public void ModbusDataMemoryStoreSupportsLastAddressTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(coilCount: 65535, discreteInputCount: 65535, holdingRegisterCount: 65535, inputRegisterCount: 65535);
            store.WriteCoils(65534, new[] { true });
            store.WriteDiscreteInputs(65534, new[] { true });
            store.WriteHoldingRegisters(65534, new ushort[] { 1234 });
            store.WriteInputRegisters(65534, new ushort[] { 5678 });

            CollectionAssert.AreEqual(new[] { true }, store.ReadCoils(65534, 65534));
            CollectionAssert.AreEqual(new[] { true }, store.ReadDiscreteInputs(65534, 65534));
            CollectionAssert.AreEqual(new ushort[] { 1234 }, store.ReadHoldingRegisters(65534, 65534));
            CollectionAssert.AreEqual(new ushort[] { 5678 }, store.ReadInputRegisters(65534, 65534));
        }


        [TestMethod]
        public void ModbusDataMemoryStoreReadRejectsDescendingRangesTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(2, 2, 2, 2);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.ReadCoils(1, 0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.ReadDiscreteInputs(1, 0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.ReadHoldingRegisters(1, 0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.ReadInputRegisters(1, 0));
        }


        [TestMethod]
        public void ModbusDataMemoryStoreReadRejectsRangesOutsideCapacityTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(2, 2, 2, 2);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.ReadCoils(1, 2));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.ReadDiscreteInputs(1, 2));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.ReadHoldingRegisters(1, 2));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.ReadInputRegisters(1, 2));
        }


        [TestMethod]
        public void ModbusDataMemoryStoreWriteRejectsEmptyValuesTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(2, 2, 2, 2);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.WriteCoils(0, Array.Empty<bool>()));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.WriteDiscreteInputs(0, Array.Empty<bool>()));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.WriteHoldingRegisters(0, Array.Empty<ushort>()));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.WriteInputRegisters(0, Array.Empty<ushort>()));
        }


        [TestMethod]
        public void ModbusDataMemoryStoreWriteRejectsValuesOutsideCapacityTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(2, 2, 2, 2);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.WriteCoils(1, new[] { true, false }));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.WriteDiscreteInputs(1, new[] { true, false }));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.WriteHoldingRegisters(1, new ushort[] { 1, 2 }));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.WriteInputRegisters(1, new ushort[] { 1, 2 }));
        }


        [TestMethod]
        public void ModbusDataMemoryStoreZeroCapacityRejectsReadsAndWritesTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(0, 0, 0, 0);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.ReadCoils(0, 0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.ReadDiscreteInputs(0, 0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.ReadHoldingRegisters(0, 0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.ReadInputRegisters(0, 0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.WriteCoils(0, new[] { true }));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.WriteDiscreteInputs(0, new[] { true }));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.WriteHoldingRegisters(0, new ushort[] { 1 }));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.WriteInputRegisters(0, new ushort[] { 1 }));
        }
    }
}

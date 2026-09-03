// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using NModbus;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.Protocols.Modbus.UnitTest.Stores
{
    [TestClass]
    public sealed class ModbusDataMemoryStoreTest
    {
        [TestMethod]
        [DataRow(-1, 2, 2, 2)]
        [DataRow(65536, 2, 2, 2)]
        [DataRow(2, -1, 2, 2)]
        [DataRow(2, 65536, 2, 2)]
        [DataRow(2, 2, -1, 2)]
        [DataRow(2, 2, 65536, 2)]
        [DataRow(2, 2, 2, -1)]
        [DataRow(2, 2, 2, 65536)]
        public void ModbusDataMemoryStoreConstructorRejectsInvalidCountsTest(int coilCount, int discreteInputCount, int holdingRegisterCount, int inputRegisterCount)
        {
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ModbusDataMemoryStore(coilCount, discreteInputCount, holdingRegisterCount, inputRegisterCount));
        }


        [TestMethod]
        public void ModbusDataMemoryStoreConstructorInitializesPointSourcesTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(3, 4, 5, 6);

            Assert.IsInstanceOfType<ISlaveDataStore>(store);
            Assert.IsNotNull(store.CoilDiscretes);
            Assert.IsNotNull(store.CoilInputs);
            Assert.IsNotNull(store.HoldingRegisters);
            Assert.IsNotNull(store.InputRegisters);
            CollectionAssert.AreEqual(new bool[] { false, false, false }, store.CoilDiscretes.ReadPoints(0, 3));
            CollectionAssert.AreEqual(new bool[] { false, false, false, false }, store.CoilInputs.ReadPoints(0, 4));
            CollectionAssert.AreEqual(new ushort[] { 0, 0, 0, 0, 0 }, store.HoldingRegisters.ReadPoints(0, 5));
            CollectionAssert.AreEqual(new ushort[] { 0, 0, 0, 0, 0, 0 }, store.InputRegisters.ReadPoints(0, 6));
        }


        [TestMethod]
        public void ModbusDataMemoryStorePointSourcesReadAndWriteActualValuesTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(8, 8, 8, 8);

            store.CoilDiscretes.WritePoints(1, new[] { true, false, true });
            store.CoilInputs.WritePoints(1, new[] { false, true, false });
            store.HoldingRegisters.WritePoints(1, new ushort[] { 1234, 32768, 65535 });
            store.InputRegisters.WritePoints(1, new ushort[] { 42, 54321, 0 });

            CollectionAssert.AreEqual(new[] { true, false, true }, store.CoilDiscretes.ReadPoints(1, 3));
            CollectionAssert.AreEqual(new[] { false, true, false }, store.CoilInputs.ReadPoints(1, 3));
            CollectionAssert.AreEqual(new ushort[] { 1234, 32768, 65535 }, store.HoldingRegisters.ReadPoints(1, 3));
            CollectionAssert.AreEqual(new ushort[] { 42, 54321, 0 }, store.InputRegisters.ReadPoints(1, 3));
        }


        [TestMethod]
        public void ModbusDataMemoryStorePointSourcesRemainIndependentTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(4, 4, 4, 4);

            store.CoilDiscretes.WritePoints(0, new[] { true });
            store.CoilInputs.WritePoints(0, new[] { false });
            store.HoldingRegisters.WritePoints(0, new ushort[] { 100 });
            store.InputRegisters.WritePoints(0, new ushort[] { 200 });

            Assert.IsTrue(store.CoilDiscretes.ReadPoints(0, 1)[0]);
            Assert.IsFalse(store.CoilInputs.ReadPoints(0, 1)[0]);
            Assert.AreEqual((ushort)100, store.HoldingRegisters.ReadPoints(0, 1)[0]);
            Assert.AreEqual((ushort)200, store.InputRegisters.ReadPoints(0, 1)[0]);
        }


        [TestMethod]
        public void ModbusDataMemoryStorePointSourcesReturnCopiesTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(2, 2, 2, 2);
            store.CoilDiscretes.WritePoints(0, new[] { true, false });
            store.HoldingRegisters.WritePoints(0, new ushort[] { 123, 456 });

            bool[] coils = store.CoilDiscretes.ReadPoints(0, 2);
            ushort[] registers = store.HoldingRegisters.ReadPoints(0, 2);
            coils[0] = false;
            registers[0] = 999;

            CollectionAssert.AreEqual(new[] { true, false }, store.CoilDiscretes.ReadPoints(0, 2));
            CollectionAssert.AreEqual(new ushort[] { 123, 456 }, store.HoldingRegisters.ReadPoints(0, 2));
        }


        [TestMethod]
        public void ModbusDataMemoryStorePointSourcesCopyInputValuesTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(2, 2, 2, 2);
            bool[] coils = new[] { true, false };
            ushort[] registers = new ushort[] { 100, 200 };

            store.CoilDiscretes.WritePoints(0, coils);
            store.HoldingRegisters.WritePoints(0, registers);
            coils[0] = false;
            registers[0] = 999;

            CollectionAssert.AreEqual(new[] { true, false }, store.CoilDiscretes.ReadPoints(0, 2));
            CollectionAssert.AreEqual(new ushort[] { 100, 200 }, store.HoldingRegisters.ReadPoints(0, 2));
        }


        [TestMethod]
        public void ModbusDataMemoryStorePointSourcesOverwriteRequestedRangeTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(holdingRegisterCount: 6);
            store.HoldingRegisters.WritePoints(0, new ushort[] { 10, 20, 30, 40, 50, 60 });

            store.HoldingRegisters.WritePoints(2, new ushort[] { 300, 400 });

            CollectionAssert.AreEqual(new ushort[] { 10, 20, 300, 400, 50, 60 }, store.HoldingRegisters.ReadPoints(0, 6));
        }


        [TestMethod]
        public void ModbusDataMemoryStorePointSourcesSupportLastAddressTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(65535, 65535, 65535, 65535);

            store.CoilDiscretes.WritePoints(65534, new[] { true });
            store.CoilInputs.WritePoints(65534, new[] { true });
            store.HoldingRegisters.WritePoints(65534, new ushort[] { 1234 });
            store.InputRegisters.WritePoints(65534, new ushort[] { 5678 });

            CollectionAssert.AreEqual(new[] { true }, store.CoilDiscretes.ReadPoints(65534, 1));
            CollectionAssert.AreEqual(new[] { true }, store.CoilInputs.ReadPoints(65534, 1));
            CollectionAssert.AreEqual(new ushort[] { 1234 }, store.HoldingRegisters.ReadPoints(65534, 1));
            CollectionAssert.AreEqual(new ushort[] { 5678 }, store.InputRegisters.ReadPoints(65534, 1));
        }


        [TestMethod]
        public void ModbusDataMemoryStorePointSourcesRejectInvalidRangesTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(2, 2, 2, 2);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.CoilDiscretes.ReadPoints(0, 0));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.CoilInputs.ReadPoints(1, 2));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.HoldingRegisters.ReadPoints(2, 1));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.InputRegisters.WritePoints(1, new ushort[] { 1, 2 }));
        }


        [TestMethod]
        public void ModbusDataMemoryStorePointSourcesRejectNullWritesTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(2, 2, 2, 2);

            Assert.ThrowsExactly<ArgumentNullException>(() => store.CoilDiscretes.WritePoints(0, null!));
            Assert.ThrowsExactly<ArgumentNullException>(() => store.CoilInputs.WritePoints(0, null!));
            Assert.ThrowsExactly<ArgumentNullException>(() => store.HoldingRegisters.WritePoints(0, null!));
            Assert.ThrowsExactly<ArgumentNullException>(() => store.InputRegisters.WritePoints(0, null!));
        }


        [TestMethod]
        public void ModbusDataMemoryStorePointSourcesRejectWritesOutsideCapacityTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(2, 2, 2, 2);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.CoilDiscretes.WritePoints(1, new[] { true, false }));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.CoilInputs.WritePoints(1, new[] { true, false }));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.HoldingRegisters.WritePoints(1, new ushort[] { 1, 2 }));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.InputRegisters.WritePoints(1, new ushort[] { 1, 2 }));
        }


        [TestMethod]
        public void ModbusDataMemoryStoreZeroCapacityRejectsPointOperationsTest()
        {
            ModbusDataMemoryStore store = new ModbusDataMemoryStore(0, 0, 0, 0);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.CoilDiscretes.ReadPoints(0, 1));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.CoilInputs.ReadPoints(0, 1));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.HoldingRegisters.ReadPoints(0, 1));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.InputRegisters.ReadPoints(0, 1));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.CoilDiscretes.WritePoints(0, new[] { true }));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.CoilInputs.WritePoints(0, new[] { true }));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.HoldingRegisters.WritePoints(0, new ushort[] { 1 }));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => store.InputRegisters.WritePoints(0, new ushort[] { 1 }));
        }
    }
}

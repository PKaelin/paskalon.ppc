// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;

namespace paskalON.Dataface.UnitTest.Modbus
{
    [TestClass]
    public class ModbusPollingRangeEntryTest
    {
        [TestMethod]
        public void ConstructorSetsPropertiesTest()
        {
            ushort from = 30001;
            ushort to = 30020;

            ModbusPollingRangeEntry entry = new ModbusPollingRangeEntry(from, to, ModbusRegistryType.InputRegister, 5);

            Assert.AreEqual(from, entry.From);
            Assert.AreEqual(to, entry.To);
            Assert.AreEqual(ModbusRegistryType.InputRegister, entry.RegistryType);
            Assert.AreEqual(5, entry.Interval);
        }


        [TestMethod]
        [DataRow((ushort)0, (ushort)0)]
        [DataRow((ushort)40001, (ushort)40001)]
        [DataRow((ushort)65535, (ushort)65535)]
        [DataRow((ushort)0, (ushort)65535)]
        public void ConstructorWithValidRangeBoundariesTest(ushort from, ushort to)
        {
            ModbusRegistryType registryType = ModbusRegistryType.HoldingRegister;

            ModbusPollingRangeEntry entry = new ModbusPollingRangeEntry(from, to, registryType, 1);

            Assert.AreEqual(from, entry.From);
            Assert.AreEqual(to, entry.To);
        }


        [TestMethod]
        [DataRow((ushort)101, (ushort)100)]
        [DataRow((ushort)65535, (ushort)0)]
        public void ConstructorWithFromGreaterThanToThrowsTest(ushort from, ushort to)
        {
            Action act = () => _ = new ModbusPollingRangeEntry(from, to, ModbusRegistryType.HoldingRegister, 1);

            ArgumentOutOfRangeException exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);

            Assert.AreEqual("from", exception.ParamName);
        }


        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void ConstructorWithIntervalZeroOrNegativeThrowsTest(int interval)
        {
            Action act = () => _ = new ModbusPollingRangeEntry(1, 100, ModbusRegistryType.Coil, interval);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
        }


        [TestMethod]
        [DataRow(1)]
        [DataRow(int.MaxValue)]
        public void ConstructorWithIntervalBoundariesTest(int interval)
        {
            ModbusRegistryType registryType = ModbusRegistryType.DiscreteInput;

            ModbusPollingRangeEntry entry = new ModbusPollingRangeEntry(1, 100, registryType, interval);

            Assert.AreEqual(interval, entry.Interval);
        }
    }
}

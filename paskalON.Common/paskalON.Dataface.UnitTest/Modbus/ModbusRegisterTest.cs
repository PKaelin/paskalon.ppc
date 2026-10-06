// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.C37s;
using paskalON.Dataface.Modbus;
using paskalON.Dataface.UnitTest.TestDoubles;

namespace paskalON.Dataface.UnitTest.Modbus
{
    [TestClass]
    public class ModbusRegisterTest
    {
        [TestMethod]
        public void ConstructorSetsNameAndEmptyListsTest()
        {
            string name = "Battery Bank 1";

            ModbusRegister register = new ModbusRegister(name);

            Assert.AreEqual(name, register.Name);
            Assert.IsEmpty(register.Registers);
            Assert.IsEmpty(register.PollingRanges);
        }


        [TestMethod]
        public void ConstructorWithNameNullThrowsTest()
        {
            Action act = () => _ = new ModbusRegister(null!);

            ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(act);

            Assert.AreEqual("name", exception.ParamName);
        }


        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void ConstructorWithNameEmptyOrWhiteSpaceThrowsTest(string name)
        {
            Action act = () => _ = new ModbusRegister(name);

            ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(act);

            Assert.AreEqual("name", exception.ParamName);
        }


        [TestMethod]
        public void RegisterAddsEntryWithAllValuesTest()
        {
            TestBattery battery = new TestBattery();
            ModbusRegister register = new ModbusRegister("Battery Bank 1");

            register.Register<TestBattery, float?>(battery, "ActivePower", (x, v) => x.ActivePower = v, 30002, ModbusScale.Downscale10,
                ModbusDataType.MbInt32Be, 3);

            Assert.HasCount(1, register.Registers);
            IModbusRegisterEntry entry = register.Registers[0];
            Assert.AreSame(battery, entry.Instance);
            Assert.AreEqual("ActivePower", entry.Name);
            Assert.AreEqual(30002, entry.Register);
            Assert.AreEqual(ModbusScale.Downscale10, entry.Scale);
            Assert.AreEqual(ModbusDataType.MbInt32Be, entry.DataType);
            Assert.AreEqual(3, entry.Offset);
        }


        [TestMethod]
        public void RegisterWithDefaultOffsetTest()
        {
            TestBattery battery = new TestBattery();
            ModbusRegister register = new ModbusRegister("Battery Bank 1");

            register.Register<TestBattery, double>(battery, "StateOfCharge", (x, v) => x.StateOfCharge = v, 30001, ModbusScale.Downscale10,
                ModbusDataType.MbUint16);

            Assert.AreEqual(0, register.Registers[0].Offset);
        }


        [TestMethod]
        public void RegisterWithInstanceNullThrowsTest()
        {
            ModbusRegister register = new ModbusRegister("Battery Bank 1");

            Action act = () => register.Register<TestBattery, double>(null!, "StateOfCharge", (x, v) => x.StateOfCharge = v, 30001,
                ModbusScale.Downscale10, ModbusDataType.MbUint16);

            ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(act);
            Assert.AreEqual("instance", exception.ParamName);
            Assert.IsEmpty(register.Registers);
        }


        [TestMethod]
        public void RegisterWithNameNullThrowsTest()
        {
            TestBattery battery = new TestBattery();
            ModbusRegister register = new ModbusRegister("Battery Bank 1");

            Action act = () => register.Register<TestBattery, double>(battery, null!, (x, v) => x.StateOfCharge = v, 30001,
                ModbusScale.Downscale10, ModbusDataType.MbUint16);

            ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(act);
            Assert.AreEqual("name", exception.ParamName);
            Assert.IsEmpty(register.Registers);
        }


        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void RegisterWithNameEmptyOrWhiteSpaceThrowsTest(string name)
        {
            TestBattery battery = new TestBattery();
            ModbusRegister register = new ModbusRegister("Battery Bank 1");

            Action act = () => register.Register<TestBattery, double>(battery, name, (x, v) => x.StateOfCharge = v, 30001,
                ModbusScale.Downscale10, ModbusDataType.MbUint16);

            ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(act);
            Assert.AreEqual("name", exception.ParamName);
            Assert.IsEmpty(register.Registers);
        }


        [TestMethod]
        public void RegisterWithSetterNullThrowsTest()
        {
            TestBattery battery = new TestBattery();
            ModbusRegister register = new ModbusRegister("Battery Bank 1");

            Action act = () => register.Register<TestBattery, double>(battery, "StateOfCharge", null!, 30001, ModbusScale.Downscale10,
                ModbusDataType.MbUint16);

            ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(act);
            Assert.AreEqual("setter", exception.ParamName);
            Assert.IsEmpty(register.Registers);
        }


        [TestMethod]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void RegisterWithNegativeOffsetThrowsTest(int offset)
        {
            TestBattery battery = new TestBattery();
            ModbusRegister register = new ModbusRegister("Battery Bank 1");

            Action act = () => register.Register<TestBattery, double>(battery, "StateOfCharge", (x, v) => x.StateOfCharge = v, 30001,
                ModbusScale.Downscale10, ModbusDataType.MbUint16, offset);

            ArgumentOutOfRangeException exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
            Assert.AreEqual("offset", exception.ParamName);
            Assert.IsEmpty(register.Registers);
        }


        [TestMethod]
        public void RegisterWithZeroScaleThrowsTest()
        {
            TestBattery battery = new TestBattery();
            ModbusRegister register = new ModbusRegister("Battery Bank 1");

            Action act = () => register.Register<TestBattery, double>(battery, "StateOfCharge", (x, v) => x.StateOfCharge = v, 30001,
                0, ModbusDataType.MbUint16);

            ArgumentOutOfRangeException exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
            Assert.AreEqual("scale", exception.ParamName);
            Assert.IsEmpty(register.Registers);
        }


        [TestMethod]
        [DataRow("StateOfCharge")]
        [DataRow("STATEOFCHARGE")]
        [DataRow("stateofcharge")]
        public void RegisterWithDuplicateNameThrowsTest(string duplicateName)
        {
            TestBattery battery = new TestBattery();
            ModbusRegister register = new ModbusRegister("Battery Bank 1");
            register.Register<TestBattery, double>(battery, "StateOfCharge", (x, v) => x.StateOfCharge = v, 30001, ModbusScale.Downscale10,
                ModbusDataType.MbUint16);

            Action act = () => register.Register<TestBattery, int>(battery, duplicateName, (x, v) => x.CycleCount = v, 30010,
                ModbusScale.NoScale, ModbusDataType.MbInt32Be);

            Assert.ThrowsExactly<ArgumentException>(act);
            Assert.HasCount(1, register.Registers);
            Assert.AreEqual(30001, register.Registers[0].Register);
        }


        [TestMethod]
        public void RegisterWithDuplicateRegisterNumberThrowsTest()
        {
            TestBattery battery = new TestBattery();
            ModbusRegister register = new ModbusRegister("Battery Bank 1");
            register.Register<TestBattery, double>(battery, "StateOfCharge", (x, v) => x.StateOfCharge = v, 30001, ModbusScale.Downscale10,
                ModbusDataType.MbUint16);

            Action act = () => register.Register<TestBattery, int>(battery, "CycleCount", (x, v) => x.CycleCount = v, 30001,
                ModbusScale.NoScale, ModbusDataType.MbInt32Be);

            Assert.ThrowsExactly<ArgumentException>(act);
            Assert.HasCount(1, register.Registers);
            Assert.AreEqual("StateOfCharge", register.Registers[0].Name);
        }


        [TestMethod]
        public void RegisterMultipleEntriesKeepsRegistrationOrderTest()
        {
            TestBattery battery = new TestBattery();
            ModbusRegister register = new ModbusRegister("Battery Bank 1");

            register.Register<TestBattery, double>(battery, "StateOfCharge", (x, v) => x.StateOfCharge = v, 30001, ModbusScale.Downscale10,
                ModbusDataType.MbUint16);
            register.Register<TestBattery, float?>(battery, "ActivePower", (x, v) => x.ActivePower = v, 30002, ModbusScale.NoScale,
                ModbusDataType.MbFloatBe);
            register.Register<TestBattery, short>(battery, "Temperature", (x, v) => x.Temperature = v, 30005, ModbusScale.Downscale10,
                ModbusDataType.MbInt16);

            Assert.HasCount(3, register.Registers);
            Assert.AreEqual("StateOfCharge", register.Registers[0].Name);
            Assert.AreEqual("ActivePower", register.Registers[1].Name);
            Assert.AreEqual("Temperature", register.Registers[2].Name);
        }


        [TestMethod]
        public void RegisterEntryUpdatesRegisteredInstanceTest()
        {
            TestBattery battery = new TestBattery();
            ModbusRegister register = new ModbusRegister("Battery Bank 1");
            register.Register<TestBattery, double>(battery, "StateOfCharge", (x, v) => x.StateOfCharge = v, 30001, ModbusScale.Downscale10,
                ModbusDataType.MbUint16);

            register.Registers[0].Update(85.5d);

            Assert.AreEqual(85.5, battery.StateOfCharge);
        }


        [TestMethod]
        public void RegistersIsReadOnlyTest()
        {
            TestBattery battery = new TestBattery();
            ModbusRegister register = new ModbusRegister("Battery Bank 1");
            register.Register<TestBattery, double>(battery, "StateOfCharge", (x, v) => x.StateOfCharge = v, 30001, ModbusScale.Downscale10,
                ModbusDataType.MbUint16);
            ICollection<IModbusRegisterEntry> registers = (ICollection<IModbusRegisterEntry>)register.Registers;

            Action act = () => registers.Clear();

            Assert.ThrowsExactly<NotSupportedException>(act);
            Assert.IsTrue(registers.IsReadOnly);
            Assert.HasCount(1, register.Registers);
        }


        [TestMethod]
        public void RegisterRangeAddsPollingRangeTest()
        {
            ModbusRegister register = new ModbusRegister("Battery Bank 1");

            register.RegisterRange(30001, 30020, ModbusRegistryType.InputRegister, 3);

            Assert.HasCount(1, register.PollingRanges);
            ModbusPollingRangeEntry entry = register.PollingRanges[0];
            Assert.AreEqual((ushort)30001, entry.From);
            Assert.AreEqual((ushort)30020, entry.To);
            Assert.AreEqual(ModbusRegistryType.InputRegister, entry.RegistryType);
            Assert.AreEqual(3, entry.Interval);
        }


        [TestMethod]
        [DataRow((ushort)40001, (ushort)40001)]
        [DataRow((ushort)0, (ushort)0)]
        [DataRow((ushort)0, (ushort)65535)]
        [DataRow((ushort)65535, (ushort)65535)]
        public void RegisterRangeWithBoundaryAddressesTest(ushort from, ushort to)
        {
            ModbusRegister register = new ModbusRegister("Battery Bank 1");

            register.RegisterRange(from, to, ModbusRegistryType.HoldingRegister, 1);

            Assert.HasCount(1, register.PollingRanges);
            Assert.AreEqual(from, register.PollingRanges[0].From);
            Assert.AreEqual(to, register.PollingRanges[0].To);
        }


        [TestMethod]
        [DataRow((ushort)100, (ushort)1)]
        [DataRow((ushort)40002, (ushort)40001)]
        public void RegisterRangeWithFromGreaterThanToThrowsTest(ushort from, ushort to)
        {
            ModbusRegister register = new ModbusRegister("Battery Bank 1");

            Action act = () => register.RegisterRange(from, to, ModbusRegistryType.HoldingRegister, 1);

            ArgumentOutOfRangeException exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
            Assert.AreEqual("to", exception.ParamName);
            Assert.IsEmpty(register.PollingRanges);
        }


        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void RegisterRangeWithIntervalZeroOrNegativeThrowsTest(int interval)
        {
            ModbusRegister register = new ModbusRegister("Battery Bank 1");

            Action act = () => register.RegisterRange(1, 100, ModbusRegistryType.HoldingRegister, interval);

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
            Assert.IsEmpty(register.PollingRanges);
        }


        [TestMethod]
        public void RegisterRangeWithDuplicateFromThrowsTest()
        {
            ModbusRegister register = new ModbusRegister("Battery Bank 1");
            register.RegisterRange(40001, 40010, ModbusRegistryType.HoldingRegister, 1);

            Action act = () => register.RegisterRange(40001, 40050, ModbusRegistryType.HoldingRegister, 5);

            Assert.ThrowsExactly<ArgumentException>(act);
            Assert.HasCount(1, register.PollingRanges);
            Assert.AreEqual((ushort)40010, register.PollingRanges[0].To);
        }


        [TestMethod]
        public void RegisterRangeMultipleRangesKeepsRegistrationOrderTest()
        {
            ModbusRegister register = new ModbusRegister("Battery Bank 1");

            register.RegisterRange(30001, 30020, ModbusRegistryType.InputRegister, 1);
            register.RegisterRange(40001, 40010, ModbusRegistryType.HoldingRegister, 10);
            register.RegisterRange(10001, 10008, ModbusRegistryType.DiscreteInput, 5);

            Assert.HasCount(3, register.PollingRanges);
            Assert.AreEqual(ModbusRegistryType.InputRegister, register.PollingRanges[0].RegistryType);
            Assert.AreEqual(ModbusRegistryType.HoldingRegister, register.PollingRanges[1].RegistryType);
            Assert.AreEqual(ModbusRegistryType.DiscreteInput, register.PollingRanges[2].RegistryType);
        }


        [TestMethod]
        public void RegisterComWithActionNullThrowsTest()
        {
            ModbusRegister register = new ModbusRegister("Battery Bank 1");

            Action act = () => register.Register<TestBattery, IModbusRegister>(null!);

            ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(act);
            Assert.AreEqual("com", exception.ParamName);
        }


        [TestMethod]
        public void RegisterComWithUnsupportedTypeThrowsTest()
        {
            TestBattery battery = new TestBattery();
            ModbusRegister register = new ModbusRegister("Battery Bank 1");
            bool isInvoked = false;

            Action act = () => register.Register<TestBattery, IC37Register>(r =>
            {
                isInvoked = true;
                r.Register<TestBattery, float>(battery, "Frequency", C37SignalType.Frequency, (x, v) => x.Frequency = v);
            });

            Assert.ThrowsExactly<ArgumentException>(act);
            Assert.IsFalse(isInvoked);
            Assert.IsEmpty(register.Registers);
        }


        [TestMethod]
        public void RegisterComInvokesActionWithItselfTest()
        {
            ModbusRegister register = new ModbusRegister("Battery Bank 1");
            IModbusRegister? received = null;

            register.Register<TestBattery, IModbusRegister>(r => received = r);

            Assert.AreSame(register, received);
        }


        [TestMethod]
        public void RegisterComRegistersEntryAndRangeTest()
        {
            TestBattery battery = new TestBattery();
            ModbusRegister register = new ModbusRegister("Battery Bank 1");

            register.Register<TestBattery, IModbusRegister>(r =>
                r.Register<TestBattery, double>(battery, "StateOfCharge", (x, v) => x.StateOfCharge = v, 30001, ModbusScale.Downscale10,
                    ModbusDataType.MbUint16));
            register.Register<TestBattery, IModbusRegister>(r => r.RegisterRange(30001, 30020, ModbusRegistryType.InputRegister, 1));

            Assert.HasCount(1, register.Registers);
            Assert.AreEqual("StateOfCharge", register.Registers[0].Name);
            Assert.AreEqual(30001, register.Registers[0].Register);
            Assert.HasCount(1, register.PollingRanges);
            Assert.AreEqual((ushort)30001, register.PollingRanges[0].From);
        }
    }
}

using paskalON.Dataface.C37s;
using paskalON.Dataface.Modbus;

namespace paskalON.Dataface.UnitTest.Modbus
{
    [TestClass]
    public class ModbusRegisterTest
    {
        // Test field used to register action.
        private int _myValue = 0;


        [TestMethod]
        public void ModbusRegisterWithInstanceNullTest()
        {
            ModbusRegister register = new ModbusRegister("Test");
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => register.Register<ModbusRegisterTest, int>(null, "Test", (x, v) => x._myValue = v, 1000, ModbusScale.NoScale, ModbusDataType.MbInt16));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void ModbusRegisterWithNameNullTest()
        {
            ModbusRegister register = new ModbusRegister("Test");
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => register.Register<ModbusRegisterTest, int>(this, null, (x, v) => x._myValue = v, 1000, ModbusScale.NoScale, ModbusDataType.MbInt16));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void ModbusRegisterWithNameEmptyTest()
        {
            ModbusRegister register = new ModbusRegister("Test");
            Assert.ThrowsExactly<ArgumentException>(() => register.Register<ModbusRegisterTest, int>(this, string.Empty, (x, v) => x._myValue = v, 1000, ModbusScale.NoScale, ModbusDataType.MbInt16));
        }


        [TestMethod]
        public void ModbusRegisterNameTwiceTest()
        {
            ModbusRegister register = new ModbusRegister("Test");
            string name = "Test";
            register.Register<ModbusRegisterTest, int>(this, name, (x, v) => x._myValue = v, 1000, ModbusScale.NoScale, ModbusDataType.MbInt16);
            Assert.ThrowsExactly<ArgumentException>(() => register.Register<ModbusRegisterTest, int>(this, name, (x, v) => x._myValue = v, 2000, ModbusScale.NoScale, ModbusDataType.MbInt16));
        }


        [TestMethod]
        public void ModbusRegisterRegisterTwiceTest()
        {
            ModbusRegister register = new ModbusRegister("Test");
            int registerNumber = 1000;
            register.Register<ModbusRegisterTest, int>(this, "Test1", (x, v) => x._myValue = v, registerNumber, ModbusScale.NoScale, ModbusDataType.MbInt16);
            Assert.ThrowsExactly<ArgumentException>(() => register.Register<ModbusRegisterTest, int>(this, "Test2", (x, v) => x._myValue = v, registerNumber, ModbusScale.NoScale, ModbusDataType.MbInt16));
        }


        [TestMethod]
        public void ModbusRegisterWithSetterNullTest()
        {
            ModbusRegister register = new ModbusRegister("Test");
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => register.Register<ModbusRegisterTest, int>(this, "Test", null, 1000, ModbusScale.NoScale, ModbusDataType.MbInt16));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void ModbusRegisterOffsetSmallerThan0Test()
        {
            ModbusRegister register = new ModbusRegister("Test");
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => register.Register<ModbusRegisterTest, int>(this, "Test", (x, v) => x._myValue = v, 1, ModbusScale.NoScale, ModbusDataType.MbInt16, -1));
        }


        [TestMethod]
        public void ModbusRegisterAddedToRegistersTest()
        {
            ModbusRegister register = new ModbusRegister("Test");
            string name = "Test";
            register.Register<ModbusRegisterTest, int>(this, name, (x, v) => x._myValue = v, 1000, ModbusScale.NoScale, ModbusDataType.MbInt16);

            Assert.IsNotNull(register.Registers);
            Assert.HasCount(1, register.Registers);
            Assert.IsNotNull(register.Registers.FirstOrDefault(r => r.Name == name));
        }


        [TestMethod]
        public void ModbusRegisterAddedToRegistersCorrectlyTest()
        {
            ModbusRegister register = new ModbusRegister("Test");
            string name = "Test";
            int registerNumber = 1000;
            register.Register<ModbusRegisterTest, int>(this, name, (x, v) => x._myValue = v, registerNumber, ModbusScale.NoScale, ModbusDataType.MbInt16, 3);
            IModbusRegisterEntry? entry = register.Registers.FirstOrDefault(r => r.Name == name);

            Assert.IsNotNull(entry);
            Assert.AreEqual(this, entry.Instance);
            Assert.AreEqual(registerNumber, entry.Register);
            Assert.AreEqual(ModbusScale.NoScale, entry.Scale);
            Assert.AreEqual(ModbusDataType.MbInt16, entry.DataType);
            Assert.AreEqual(3, entry.Offset);
        }


        [TestMethod]
        public void ModbusRegisterRangeFromBiggerThanToTest()
        {
            ModbusRegister register = new ModbusRegister("Test");
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => register.RegisterRange(100, 1, ModbusRegistryType.HoldingRegister, 1));
        }


        [TestMethod]
        public void ModbusRegisterRangeIntervalSmallerThan0Test()
        {
            ModbusRegister register = new ModbusRegister("Test");
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => register.RegisterRange(1, 100, ModbusRegistryType.HoldingRegister, -1));
        }


        [TestMethod]
        public void ModbusRegisterRangeFromAlreadyRegisteredTest()
        {
            ModbusRegister register = new ModbusRegister("Test");
            ushort fromRegister = 1;
            register.RegisterRange(fromRegister, 100, ModbusRegistryType.HoldingRegister, 1);
            Assert.ThrowsExactly<ArgumentException>(() => register.RegisterRange(fromRegister, 100, ModbusRegistryType.HoldingRegister, 1));
        }


        [TestMethod]
        public void ModbusRegisterRangeAddedTest()
        {
            ModbusRegister register = new ModbusRegister("Test");
            register.RegisterRange(1, 100, ModbusRegistryType.HoldingRegister, 3);

            Assert.IsNotNull(register.PollingRanges);
            Assert.HasCount(1, register.PollingRanges);
            ModbusPollingRangeEntry? entry = register.PollingRanges.FirstOrDefault(r => r.From == 1 && r.To == 100);
            Assert.IsNotNull(entry);
            Assert.AreEqual(ModbusRegistryType.HoldingRegister, entry.RegistryType);
            Assert.AreEqual(3, entry.Interval);
        }


        [TestMethod]
        public void RegisterComActionNullTest()
        {
            ModbusRegister register = new ModbusRegister("Test");
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => register.Register<ModbusRegisterTest, IModbusRegister>(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }


        [TestMethod]
        public void RegisterComActionTypeDifferentTest()
        {
            ModbusRegister register = new ModbusRegister("Test");
            Assert.ThrowsExactly<ArgumentException>(() => register.Register<ModbusRegisterTest, IC37Register>
            (r => r.Register<ModbusRegisterTest, int>(this, "T", C37SignalType.Analog, (x, v) => x._myValue = (int)v)));
        }


        [TestMethod]
        public void RegisterComActionTest()
        {
            ModbusRegister register = new ModbusRegister("Test");
            string name = "Test";
            register.Register<ModbusRegisterTest, IModbusRegister>(r =>
                r.Register<ModbusRegisterTest, int>(this, name, (x, v) => x._myValue = v, 1, 1, ModbusDataType.MbInt16));

            Assert.IsNotNull(register);
            Assert.HasCount(1, register.Registers);
            Assert.IsNotNull(register.Registers.FirstOrDefault(r => r.Name == name));
        }
    }
}

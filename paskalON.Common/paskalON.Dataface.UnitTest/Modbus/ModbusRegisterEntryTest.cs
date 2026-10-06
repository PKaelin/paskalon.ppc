// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using paskalON.Dataface.UnitTest.TestDoubles;

namespace paskalON.Dataface.UnitTest.Modbus
{
    [TestClass]
    public class ModbusRegisterEntryTest
    {
        [TestMethod]
        public void ConstructorSetsPropertiesTest()
        {
            TestBattery battery = new TestBattery();

            ModbusRegisterEntry<TestBattery, double> entry = new ModbusRegisterEntry<TestBattery, double>(battery, "StateOfCharge",
                (x, v) => x.StateOfCharge = v, 30001, ModbusScale.Downscale10, ModbusDataType.MbUint16, 2);

            Assert.AreSame(battery, entry.Instance);
            Assert.AreEqual("StateOfCharge", entry.Name);
            Assert.AreEqual(30001, entry.Register);
            Assert.AreEqual(ModbusScale.Downscale10, entry.Scale);
            Assert.AreEqual(ModbusDataType.MbUint16, entry.DataType);
            Assert.AreEqual(2, entry.Offset);
        }


        [TestMethod]
        [DataRow((ushort)855, 855d)]
        [DataRow(855, 855d)]
        [DataRow(85.5f, 85.5d)]
        [DataRow(85.5d, 85.5d)]
        [DataRow(0d, 0d)]
        public void UpdateConvertsValueToDoublePropertyTest(object value, double expected)
        {
            TestBattery battery = new TestBattery();
            ModbusRegisterEntry<TestBattery, double> entry = new ModbusRegisterEntry<TestBattery, double>(battery, "StateOfCharge",
                (x, v) => x.StateOfCharge = v, 30001, ModbusScale.Downscale10, ModbusDataType.MbUint16, 0);

            entry.Update(value);

            Assert.AreEqual(expected, battery.StateOfCharge);
        }


        [TestMethod]
        [DataRow(1234.4d, 1234)]
        [DataRow(1234.6d, 1235)]
        [DataRow(-1234.6d, -1235)]
        public void UpdateRoundsDoubleToIntegerPropertyTest(double value, int expected)
        {
            TestBattery battery = new TestBattery();
            ModbusRegisterEntry<TestBattery, int> entry = new ModbusRegisterEntry<TestBattery, int>(battery, "CycleCount",
                (x, v) => x.CycleCount = v, 30010, ModbusScale.NoScale, ModbusDataType.MbInt32Be, 0);

            entry.Update(value);

            Assert.AreEqual(expected, battery.CycleCount);
        }


        [TestMethod]
        [DataRow(32767, (short)32767)]
        [DataRow(-32768, (short)-32768)]
        public void UpdateShortPropertyWithinBoundariesTest(int value, short expected)
        {
            TestBattery battery = new TestBattery();
            ModbusRegisterEntry<TestBattery, short> entry = new ModbusRegisterEntry<TestBattery, short>(battery, "Temperature",
                (x, v) => x.Temperature = v, 30005, ModbusScale.Downscale10, ModbusDataType.MbInt16, 0);

            entry.Update(value);

            Assert.AreEqual(expected, battery.Temperature);
        }


        [TestMethod]
        [DataRow(32768)]
        [DataRow(-32769)]
        public void UpdateShortPropertyOutOfRangeThrowsTest(int value)
        {
            TestBattery battery = new TestBattery { Temperature = 215 };
            ModbusRegisterEntry<TestBattery, short> entry = new ModbusRegisterEntry<TestBattery, short>(battery, "Temperature",
                (x, v) => x.Temperature = v, 30005, ModbusScale.Downscale10, ModbusDataType.MbInt16, 0);

            Action act = () => entry.Update(value);

            ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(act);
            Assert.IsInstanceOfType<OverflowException>(exception.InnerException);
        }


        [TestMethod]
        [DataRow(0, (ushort)0)]
        [DataRow(65535, (ushort)65535)]
        public void UpdateUshortPropertyWithinBoundariesTest(int value, ushort expected)
        {
            TestBattery battery = new TestBattery();
            ModbusRegisterEntry<TestBattery, ushort> entry = new ModbusRegisterEntry<TestBattery, ushort>(battery, "StatusWord",
                (x, v) => x.StatusWord = v, 30006, ModbusScale.NoScale, ModbusDataType.MbUint16, 0);

            entry.Update(value);

            Assert.AreEqual(expected, battery.StatusWord);
        }


        [TestMethod]
        [DataRow(-1)]
        [DataRow(65536)]
        public void UpdateUshortPropertyOutOfRangeThrowsTest(int value)
        {
            TestBattery battery = new TestBattery { StatusWord = 0x0101 };
            ModbusRegisterEntry<TestBattery, ushort> entry = new ModbusRegisterEntry<TestBattery, ushort>(battery, "StatusWord",
                (x, v) => x.StatusWord = v, 30006, ModbusScale.NoScale, ModbusDataType.MbUint16, 0);

            Action act = () => entry.Update(value);

            ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(act);
            Assert.IsInstanceOfType<OverflowException>(exception.InnerException);
        }


        [TestMethod]
        [DataRow(true, true)]
        [DataRow(false, false)]
        [DataRow(1, true)]
        [DataRow(0, false)]
        public void UpdateBoolPropertyTest(object value, bool expected)
        {
            TestBattery battery = new TestBattery { IsContactorClosed = expected is false };
            ModbusRegisterEntry<TestBattery, bool> entry = new ModbusRegisterEntry<TestBattery, bool>(battery, "IsContactorClosed",
                (x, v) => x.IsContactorClosed = v, 10001, ModbusScale.NoScale, ModbusDataType.MbBool, 0);

            entry.Update(value);

            Assert.AreEqual(expected, battery.IsContactorClosed);
        }


        [TestMethod]
        public void UpdateNullablePropertyWithValueTest()
        {
            TestBattery battery = new TestBattery();
            ModbusRegisterEntry<TestBattery, float?> entry = new ModbusRegisterEntry<TestBattery, float?>(battery, "ActivePower",
                (x, v) => x.ActivePower = v, 30002, ModbusScale.NoScale, ModbusDataType.MbFloatBe, 0);

            entry.Update(-2500.5d);

            Assert.AreEqual(-2500.5f, battery.ActivePower);
        }


        [TestMethod]
        public void UpdateNullablePropertyWithNullSetsNullTest()
        {
            TestBattery battery = new TestBattery { ActivePower = 1500f };
            ModbusRegisterEntry<TestBattery, float?> entry = new ModbusRegisterEntry<TestBattery, float?>(battery, "ActivePower",
                (x, v) => x.ActivePower = v, 30002, ModbusScale.NoScale, ModbusDataType.MbFloatBe, 0);

            entry.Update(null!);

            Assert.IsNull(battery.ActivePower);
        }


        [TestMethod]
        public void UpdateNonNullablePropertyWithNullThrowsTest()
        {
            TestBattery battery = new TestBattery { StateOfCharge = 42.5 };
            ModbusRegisterEntry<TestBattery, double> entry = new ModbusRegisterEntry<TestBattery, double>(battery, "StateOfCharge",
                (x, v) => x.StateOfCharge = v, 30001, ModbusScale.Downscale10, ModbusDataType.MbUint16, 0);

            Action act = () => entry.Update(null!);

            ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(act);
            Assert.AreEqual("value", exception.ParamName);
        }


        [TestMethod]
        public void UpdateWithNonConvertibleValueThrowsTest()
        {
            TestBattery battery = new TestBattery { StateOfCharge = 42.5 };
            ModbusRegisterEntry<TestBattery, double> entry = new ModbusRegisterEntry<TestBattery, double>(battery, "StateOfCharge",
                (x, v) => x.StateOfCharge = v, 30001, ModbusScale.Downscale10, ModbusDataType.MbUint16, 0);

            Action act = () => entry.Update("not a number");

            ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(act);
            Assert.IsInstanceOfType<FormatException>(exception.InnerException);
        }


        [TestMethod]
        public void UpdateWithInstanceOfDifferentTypeThrowsTest()
        {
            TestPowerMeter powerMeter = new TestPowerMeter { ActivePower = 3200 };
            ModbusRegisterEntry<TestBattery, double> entry = new ModbusRegisterEntry<TestBattery, double>(powerMeter, "StateOfCharge",
                (x, v) => x.StateOfCharge = v, 30001, ModbusScale.Downscale10, ModbusDataType.MbUint16, 0);

            Action act = () => entry.Update(85.5d);

            ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(act);
            Assert.AreEqual(nameof(IModbusRegisterEntry.Instance), exception.ParamName);
        }


        [TestMethod]
        public void UpdateSetterExceptionIsWrappedTest()
        {
            TestBattery battery = new TestBattery();
            InvalidOperationException setterException = new InvalidOperationException("State of charge must be between 0 and 100 percent.");
            ModbusRegisterEntry<TestBattery, double> entry = new ModbusRegisterEntry<TestBattery, double>(battery, "StateOfCharge",
                (x, v) => throw setterException, 30001, ModbusScale.Downscale10, ModbusDataType.MbUint16, 0);

            Action act = () => entry.Update(120d);

            ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(act);
            Assert.AreSame(setterException, exception.InnerException);
        }


        [TestMethod]
        public void UpdateMultipleTimesKeepsLastValueTest()
        {
            TestBattery battery = new TestBattery();
            ModbusRegisterEntry<TestBattery, double> entry = new ModbusRegisterEntry<TestBattery, double>(battery, "StateOfCharge",
                (x, v) => x.StateOfCharge = v, 30001, ModbusScale.Downscale10, ModbusDataType.MbUint16, 0);

            entry.Update(10d);
            entry.Update(55.5d);
            entry.Update(99.9d);

            Assert.AreEqual(99.9, battery.StateOfCharge);
        }
    }
}

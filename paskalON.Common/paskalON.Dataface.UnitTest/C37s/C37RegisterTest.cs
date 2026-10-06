// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.C37s;
using paskalON.Dataface.Modbus;
using paskalON.Dataface.UnitTest.TestDoubles;

namespace paskalON.Dataface.UnitTest.C37s
{
    [TestClass]
    public class C37RegisterTest
    {
        [TestMethod]
        public void ConstructorSetsNameAndEmptyRegistersTest()
        {
            string name = "System Power Meter";

            C37Register register = new C37Register(name);

            Assert.AreEqual(name, register.Name);
            Assert.IsEmpty(register.Registers);
        }


        [TestMethod]
        public void ConstructorWithNameNullThrowsTest()
        {
            Action act = () => _ = new C37Register(null!);

            ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(act);

            Assert.AreEqual("name", exception.ParamName);
        }


        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void ConstructorWithNameEmptyOrWhiteSpaceThrowsTest(string name)
        {
            Action act = () => _ = new C37Register(name);

            ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(act);

            Assert.AreEqual("name", exception.ParamName);
        }


        [TestMethod]
        public void RegisterAddsEntryWithAllValuesTest()
        {
            TestBattery battery = new TestBattery();
            C37Register register = new C37Register("System Power Meter");

            register.Register<TestBattery, C37PhasorValue>(battery, "VoltagePhasor", C37SignalType.Phasor, (x, v) => x.VoltagePhasor = v);

            Assert.HasCount(1, register.Registers);
            IC37RegisterEntry entry = register.Registers[0];
            Assert.AreSame(battery, entry.Instance);
            Assert.AreEqual("VoltagePhasor", entry.Name);
            Assert.AreEqual(C37SignalType.Phasor, entry.SignalType);
        }


        [TestMethod]
        public void RegisterWithInstanceNullThrowsTest()
        {
            C37Register register = new C37Register("System Power Meter");

            Action act = () => register.Register<TestBattery, float>(null!, "Frequency", C37SignalType.Frequency, (x, v) => x.Frequency = v);

            ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(act);
            Assert.AreEqual("instance", exception.ParamName);
            Assert.IsEmpty(register.Registers);
        }


        [TestMethod]
        public void RegisterWithNameNullThrowsTest()
        {
            TestBattery battery = new TestBattery();
            C37Register register = new C37Register("System Power Meter");

            Action act = () => register.Register<TestBattery, float>(battery, null!, C37SignalType.Frequency, (x, v) => x.Frequency = v);

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
            C37Register register = new C37Register("System Power Meter");

            Action act = () => register.Register<TestBattery, float>(battery, name, C37SignalType.Frequency, (x, v) => x.Frequency = v);

            ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(act);
            Assert.AreEqual("name", exception.ParamName);
            Assert.IsEmpty(register.Registers);
        }


        [TestMethod]
        public void RegisterWithSetterNullThrowsTest()
        {
            TestBattery battery = new TestBattery();
            C37Register register = new C37Register("System Power Meter");

            Action act = () => register.Register<TestBattery, float>(battery, "Frequency", C37SignalType.Frequency, null!);

            ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(act);
            Assert.AreEqual("setter", exception.ParamName);
            Assert.IsEmpty(register.Registers);
        }


        [TestMethod]
        [DataRow("Frequency")]
        [DataRow("FREQUENCY")]
        [DataRow("frequency")]
        public void RegisterWithDuplicateNameThrowsTest(string duplicateName)
        {
            TestBattery battery = new TestBattery();
            C37Register register = new C37Register("System Power Meter");
            register.Register<TestBattery, float>(battery, "Frequency", C37SignalType.Frequency, (x, v) => x.Frequency = v);

            Action act = () => register.Register<TestBattery, ushort>(battery, duplicateName, C37SignalType.StatusFlag, (x, v) => x.StatusWord = v);

            Assert.ThrowsExactly<ArgumentException>(act);
            Assert.HasCount(1, register.Registers);
            Assert.AreEqual(C37SignalType.Frequency, register.Registers[0].SignalType);
        }


        [TestMethod]
        public void RegisterMultipleEntriesWithSameSignalTypeKeepsRegistrationOrderTest()
        {
            TestBattery battery = new TestBattery();
            C37Register register = new C37Register("System Power Meter");

            register.Register<TestBattery, C37PhasorValue>(battery, "VoltagePhasor", C37SignalType.Phasor, (x, v) => x.VoltagePhasor = v);
            register.Register<TestBattery, C37PhasorValue?>(battery, "CurrentPhasor", C37SignalType.Phasor, (x, v) => x.CurrentPhasor = v);
            register.Register<TestBattery, float>(battery, "Frequency", C37SignalType.Frequency, (x, v) => x.Frequency = v);

            Assert.HasCount(3, register.Registers);
            Assert.AreEqual("VoltagePhasor", register.Registers[0].Name);
            Assert.AreEqual("CurrentPhasor", register.Registers[1].Name);
            Assert.AreEqual("Frequency", register.Registers[2].Name);
        }


        [TestMethod]
        public void RegisterEntryUpdatesRegisteredInstanceTest()
        {
            TestBattery battery = new TestBattery();
            C37Register register = new C37Register("System Power Meter");
            register.Register<TestBattery, float>(battery, "Frequency", C37SignalType.Frequency, (x, v) => x.Frequency = v);

            register.Registers[0].Update(49.98f);

            Assert.AreEqual(49.98f, battery.Frequency);
        }


        [TestMethod]
        public void RegistersIsReadOnlyTest()
        {
            TestBattery battery = new TestBattery();
            C37Register register = new C37Register("System Power Meter");
            register.Register<TestBattery, float>(battery, "Frequency", C37SignalType.Frequency, (x, v) => x.Frequency = v);
            ICollection<IC37RegisterEntry> registers = (ICollection<IC37RegisterEntry>)register.Registers;

            Action act = () => registers.Clear();

            Assert.ThrowsExactly<NotSupportedException>(act);
            Assert.IsTrue(registers.IsReadOnly);
            Assert.HasCount(1, register.Registers);
        }


        [TestMethod]
        public void RegisterComWithActionNullThrowsTest()
        {
            C37Register register = new C37Register("System Power Meter");

            Action act = () => register.Register<TestBattery, IC37Register>(null!);

            ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(act);
            Assert.AreEqual("com", exception.ParamName);
        }


        [TestMethod]
        public void RegisterComWithUnsupportedTypeThrowsTest()
        {
            TestBattery battery = new TestBattery();
            C37Register register = new C37Register("System Power Meter");
            bool isInvoked = false;

            Action act = () => register.Register<TestBattery, IModbusRegister>(r =>
            {
                isInvoked = true;
                r.Register<TestBattery, double>(battery, "StateOfCharge", (x, v) => x.StateOfCharge = v, 30001, ModbusScale.Downscale10,
                    ModbusDataType.MbUint16);
            });

            Assert.ThrowsExactly<ArgumentException>(act);
            Assert.IsFalse(isInvoked);
            Assert.IsEmpty(register.Registers);
        }


        [TestMethod]
        public void RegisterComInvokesActionWithItselfTest()
        {
            C37Register register = new C37Register("System Power Meter");
            IC37Register? received = null;

            register.Register<TestBattery, IC37Register>(r => received = r);

            Assert.AreSame(register, received);
        }


        [TestMethod]
        public void RegisterComRegistersEntryTest()
        {
            TestBattery battery = new TestBattery();
            C37Register register = new C37Register("System Power Meter");

            register.Register<TestBattery, IC37Register>(r =>
                r.Register<TestBattery, float>(battery, "Frequency", C37SignalType.Frequency, (x, v) => x.Frequency = v));

            Assert.HasCount(1, register.Registers);
            Assert.AreEqual("Frequency", register.Registers[0].Name);
            Assert.AreEqual(C37SignalType.Frequency, register.Registers[0].SignalType);
        }
    }
}

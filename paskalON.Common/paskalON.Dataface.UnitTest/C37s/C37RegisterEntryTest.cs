// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.C37s;
using paskalON.Dataface.UnitTest.TestDoubles;

namespace paskalON.Dataface.UnitTest.C37s
{
    [TestClass]
    public class C37RegisterEntryTest
    {
        [TestMethod]
        public void ConstructorSetsPropertiesTest()
        {
            TestBattery battery = new TestBattery();

            C37RegisterEntry<TestBattery, float> entry = new C37RegisterEntry<TestBattery, float>(battery, "Frequency",
                C37SignalType.Frequency, (x, v) => x.Frequency = v);

            Assert.AreSame(battery, entry.Instance);
            Assert.AreEqual("Frequency", entry.Name);
            Assert.AreEqual(C37SignalType.Frequency, entry.SignalType);
        }


        [TestMethod]
        [DataRow(50.01f, 50.01f)]
        [DataRow(50.01d, 50.01f)]
        [DataRow(60, 60f)]
        public void UpdateConvertsValueToFloatPropertyTest(object value, float expected)
        {
            TestBattery battery = new TestBattery();
            C37RegisterEntry<TestBattery, float> entry = new C37RegisterEntry<TestBattery, float>(battery, "Frequency",
                C37SignalType.Frequency, (x, v) => x.Frequency = v);

            entry.Update(value);

            Assert.AreEqual(expected, battery.Frequency);
        }


        [TestMethod]
        public void UpdatePhasorPropertyTest()
        {
            TestBattery battery = new TestBattery();
            C37PhasorValue phasor = new C37PhasorValue(230.4f, -0.5236f);
            C37RegisterEntry<TestBattery, C37PhasorValue> entry = new C37RegisterEntry<TestBattery, C37PhasorValue>(battery, "VoltagePhasor",
                C37SignalType.Phasor, (x, v) => x.VoltagePhasor = v);

            entry.Update(phasor);

            Assert.AreEqual(230.4f, battery.VoltagePhasor.Magnitude);
            Assert.AreEqual(-0.5236f, battery.VoltagePhasor.Angle);
        }


        [TestMethod]
        public void UpdateNullablePhasorPropertyWithValueTest()
        {
            TestBattery battery = new TestBattery();
            C37PhasorValue phasor = new C37PhasorValue(12.75f, 1.0472f);
            C37RegisterEntry<TestBattery, C37PhasorValue?> entry = new C37RegisterEntry<TestBattery, C37PhasorValue?>(battery, "CurrentPhasor",
                C37SignalType.Phasor, (x, v) => x.CurrentPhasor = v);

            entry.Update(phasor);

            Assert.IsNotNull(battery.CurrentPhasor);
            Assert.AreEqual(12.75f, battery.CurrentPhasor.Value.Magnitude);
            Assert.AreEqual(1.0472f, battery.CurrentPhasor.Value.Angle);
        }


        [TestMethod]
        public void UpdateNullablePhasorPropertyWithNullSetsNullTest()
        {
            TestBattery battery = new TestBattery { CurrentPhasor = new C37PhasorValue(12.75f, 1.0472f) };
            C37RegisterEntry<TestBattery, C37PhasorValue?> entry = new C37RegisterEntry<TestBattery, C37PhasorValue?>(battery, "CurrentPhasor",
                C37SignalType.Phasor, (x, v) => x.CurrentPhasor = v);

            entry.Update(null!);

            Assert.IsNull(battery.CurrentPhasor);
        }


        [TestMethod]
        public void UpdatePhasorPropertyWithScalarValueThrowsTest()
        {
            C37PhasorValue initialPhasor = new C37PhasorValue(230.4f, -0.5236f);
            TestBattery battery = new TestBattery { VoltagePhasor = initialPhasor };
            C37RegisterEntry<TestBattery, C37PhasorValue> entry = new C37RegisterEntry<TestBattery, C37PhasorValue>(battery, "VoltagePhasor",
                C37SignalType.Phasor, (x, v) => x.VoltagePhasor = v);

            Action act = () => entry.Update(230.4f);

            ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(act);
            Assert.IsInstanceOfType<InvalidCastException>(exception.InnerException);
            Assert.AreEqual(initialPhasor, battery.VoltagePhasor);
        }


        [TestMethod]
        public void UpdateNonNullablePropertyWithNullThrowsTest()
        {
            TestBattery battery = new TestBattery { Frequency = 49.98f };
            C37RegisterEntry<TestBattery, float> entry = new C37RegisterEntry<TestBattery, float>(battery, "Frequency",
                C37SignalType.Frequency, (x, v) => x.Frequency = v);

            Action act = () => entry.Update(null!);

            ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(act);
            Assert.AreEqual("value", exception.ParamName);
        }


        [TestMethod]
        [DataRow(-1)]
        [DataRow(65536)]
        public void UpdateUshortPropertyOutOfRangeThrowsTest(int value)
        {
            TestBattery battery = new TestBattery { StatusWord = 0x8000 };
            C37RegisterEntry<TestBattery, ushort> entry = new C37RegisterEntry<TestBattery, ushort>(battery, "StatusWord",
                C37SignalType.StatusFlag, (x, v) => x.StatusWord = v);

            Action act = () => entry.Update(value);

            ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(act);
            Assert.IsInstanceOfType<OverflowException>(exception.InnerException);
        }


        [TestMethod]
        public void UpdateWithInstanceOfDifferentTypeThrowsTest()
        {
            TestPowerMeter powerMeter = new TestPowerMeter { ActivePower = 3200 };
            C37RegisterEntry<TestBattery, float> entry = new C37RegisterEntry<TestBattery, float>(powerMeter, "Frequency",
                C37SignalType.Frequency, (x, v) => x.Frequency = v);

            Action act = () => entry.Update(50.01f);

            ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(act);
            Assert.AreEqual(nameof(IC37RegisterEntry.Instance), exception.ParamName);
        }


        [TestMethod]
        public void UpdateSetterExceptionIsWrappedTest()
        {
            TestBattery battery = new TestBattery();
            InvalidOperationException setterException = new InvalidOperationException("Frequency is outside of the allowed grid band.");
            C37RegisterEntry<TestBattery, float> entry = new C37RegisterEntry<TestBattery, float>(battery, "Frequency",
                C37SignalType.Frequency, (x, v) => throw setterException);

            Action act = () => entry.Update(65.5f);

            ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(act);
            Assert.AreSame(setterException, exception.InnerException);
        }
    }
}

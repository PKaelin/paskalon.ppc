using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.PhysicalUnits.UnitTest.Electricals.Powers
{
    [TestClass]
    public class VoltageTest
    {
        [TestMethod]
        public void VoltageConstructorTest()
        {
            Voltage voltage = new Voltage(12.345678901);
            Assert.AreEqual(12.34568, voltage.VoltsPrecision);
            Assert.AreEqual(5, voltage.Precision);
        }


        [TestMethod]
        public void VoltageKiloWattsTest()
        {
            Voltage voltage = new Voltage(12345.678);
            Assert.AreEqual(12.34568, voltage.KiloVoltsPrecision);
        }


        [TestMethod]
        public void VoltageMegaWattsTest()
        {
            Voltage voltage = new Voltage(12345678.901);
            Assert.AreEqual(12.34568, voltage.MegaVoltsPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void VoltageEqualsTest(double voltage1, double voltage2, bool expected)
        {
            Voltage v1 = new Voltage(voltage1);
            Voltage v2 = new Voltage(voltage2);
            Assert.AreEqual(expected, v1.Equals(v2));
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void VoltageEqualsObjectTest(double voltage1, double voltage2, bool expected)
        {
            object v1 = new Voltage(voltage1);
            object v2 = new Voltage(voltage2);
            Assert.AreEqual(expected, v1.Equals(v2));
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void VoltageEqualsOperatorTest(double voltage1, double voltage2, bool expected)
        {
            Voltage v1 = new Voltage(voltage1);
            Voltage v2 = new Voltage(voltage2);
            Assert.AreEqual(expected, v1 == v2);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, false)]
        [DataRow(12.34567, 12.34567111, false)]
        [DataRow(12.34567, 12.34567890, true)]
        [DataRow(12.34567, 12.34544, true)]
        public void VoltageNotEqualsOperatorTest(double voltage1, double voltage2, bool expected)
        {
            Voltage v1 = new Voltage(voltage1);
            Voltage v2 = new Voltage(voltage2);
            Assert.AreEqual(expected, v1 != v2);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 24.69134)]
        [DataRow(12.34567, 12.34567111, 24.69134)]
        [DataRow(12.34567, 12.34567890, 24.69135)]
        [DataRow(12.34567, 12.34544, 24.69111)]
        public void VoltageAddOperatorTest(double voltage1, double voltage2, double expected)
        {
            Voltage v1 = new Voltage(voltage1);
            Voltage v2 = new Voltage(voltage2);
            Voltage power = v1 + v2;
            Assert.AreEqual(expected, power.VoltsPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 0)]
        [DataRow(12.34567, 12.34567111, 0)]
        [DataRow(12.34567, 12.34567890, -0.00001)]
        [DataRow(12.34567, 12.34544, 0.00023)]
        public void VoltageMinusOperatorTest(double voltage1, double voltage2, double expected)
        {
            Voltage v1 = new Voltage(voltage1);
            Voltage v2 = new Voltage(voltage2);
            Voltage power = v1 - v2;
            Assert.AreEqual(expected, power.VoltsPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 152.41557)]
        [DataRow(12.34567, 12.34567111, 152.41558)]
        [DataRow(12.34567, 12.34567890, 152.41568)]
        [DataRow(12.34567, 12.34544, 152.41273)]
        public void VoltageMultiplyOperatorTest(double voltage1, double voltage2, double expected)
        {
            Voltage v1 = new Voltage(voltage1);
            Voltage v2 = new Voltage(voltage2);
            Voltage power = v1 * v2;
            Assert.AreEqual(expected, power.VoltsPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 2, 6.17284)]
        [DataRow(12.34567, 3, 4.11522)]
        [DataRow(12.34567, 2.5, 4.93827)]
        [DataRow(12.34567, 12, 1.02881)]
        [DataRow(12.34567, 0, double.NaN)]
        public void VoltageDivisionOperatorTest(double voltage1, double voltage2, double expected)
        {
            Voltage v1 = new Voltage(voltage1);
            Voltage v2 = new Voltage(voltage2);
            Voltage power = v1 / v2;
            Assert.AreEqual(expected, power.VoltsPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 2, false)]
        [DataRow(12.34567, 12.34566, false)]
        [DataRow(12.34567, 2.5, false)]
        [DataRow(12.34567, 12.5, true)]
        [DataRow(12.34567, 12.999999, true)]
        public void VoltageSmallerOperatorTest(double voltage1, double voltage2, bool expected)
        {
            Voltage v1 = new Voltage(voltage1);
            Voltage v2 = new Voltage(voltage2);
            Assert.AreEqual(expected, v1 < v2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, false)]
        [DataRow(12.34567, 12.34566, false)]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 2.5, false)]
        [DataRow(12.34567, 12.5, true)]
        [DataRow(12.34567, 12.999999, true)]
        public void VoltageSmallerEqualOperatorTest(double voltage1, double voltage2, bool expected)
        {
            Voltage v1 = new Voltage(voltage1);
            Voltage v2 = new Voltage(voltage2);
            Assert.AreEqual(expected, v1 <= v2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, true)]
        [DataRow(12.34567, 12.34566, true)]
        [DataRow(12.34567, 2.5, true)]
        [DataRow(12.34567, 12.5, false)]
        [DataRow(12.34567, 12.999999, false)]
        public void VoltageGreaterOperatorTest(double voltage1, double voltage2, bool expected)
        {
            Voltage v1 = new Voltage(voltage1);
            Voltage v2 = new Voltage(voltage2);
            Assert.AreEqual(expected, v1 > v2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, true)]
        [DataRow(12.34567, 12.34566, true)]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 2.5, true)]
        [DataRow(12.34567, 12.5, false)]
        [DataRow(12.34567, 12.999999, false)]
        public void VoltageGreaterEqualOperatorTest(double voltage1, double voltage2, bool expected)
        {
            Voltage v1 = new Voltage(voltage1);
            Voltage v2 = new Voltage(voltage2);
            Assert.AreEqual(expected, v1 >= v2);
        }


        [TestMethod]
        [DataRow(double.NaN, true)]
        [DataRow(12.34567, false)]
        [DataRow(double.MinValue, false)]
        [DataRow(double.MaxValue, false)]
        public void VoltageIsNaNOperatorTest(double voltage1, bool expected)
        {
            Voltage v1 = new Voltage(voltage1);
            Assert.AreEqual(expected, v1.IsNaN());
        }

    }
}

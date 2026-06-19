using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.PhysicalUnits.UnitTest.Electricals.Powers
{
    [TestClass]
    public class ActivePowerTest
    {
        [TestMethod]
        public void ActivePowerConstructorTest()
        {
            ActivePower activePower = new ActivePower(12.345678901);
            Assert.AreEqual(12.34568, activePower.WattsPrecision);
            Assert.AreEqual(5, activePower.Precision);
        }


        [TestMethod]
        public void ActivePowerKiloWattsTest()
        {
            ActivePower activePower = new ActivePower(12345.678);
            Assert.AreEqual(12.34568, activePower.KiloWattsPrecision);
        }


        [TestMethod]
        public void ActivePowerMegaWattsTest()
        {
            ActivePower activePower = new ActivePower(12345678.901);
            Assert.AreEqual(12.34568, activePower.MegaWattsPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void ActivePowerEqualsTest(double activePower1, double activePower2, bool expected)
        {
            ActivePower ap1 = new ActivePower(activePower1);
            ActivePower ap2 = new ActivePower(activePower2);
            Assert.AreEqual(expected, ap1.Equals(ap2));
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void ActivePowerEqualsObjectTest(double activePower1, double activePower2, bool expected)
        {
            object ap1 = new ActivePower(activePower1);
            object ap2 = new ActivePower(activePower2);
            Assert.AreEqual(expected, ap1.Equals(ap2));
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void ActivePowerEqualsOperatorTest(double activePower1, double activePower2, bool expected)
        {
            ActivePower ap1 = new ActivePower(activePower1);
            ActivePower ap2 = new ActivePower(activePower2);
            Assert.AreEqual(expected, ap1 == ap2);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, false)]
        [DataRow(12.34567, 12.34567111, false)]
        [DataRow(12.34567, 12.34567890, true)]
        [DataRow(12.34567, 12.34544, true)]
        public void ActivePowerNotEqualsOperatorTest(double activePower1, double activePower2, bool expected)
        {
            ActivePower ap1 = new ActivePower(activePower1);
            ActivePower ap2 = new ActivePower(activePower2);
            Assert.AreEqual(expected, ap1 != ap2);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 24.69134)]
        [DataRow(12.34567, 12.34567111, 24.69134)]
        [DataRow(12.34567, 12.34567890, 24.69135)]
        [DataRow(12.34567, 12.34544, 24.69111)]
        public void ActivePowerAddOperatorTest(double activePower1, double activePower2, double expected)
        {
            ActivePower ap1 = new ActivePower(activePower1);
            ActivePower ap2 = new ActivePower(activePower2);
            ActivePower power = ap1 + ap2;
            Assert.AreEqual(expected, power.WattsPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 0)]
        [DataRow(12.34567, 12.34567111, 0)]
        [DataRow(12.34567, 12.34567890, -0.00001)]
        [DataRow(12.34567, 12.34544, 0.00023)]
        public void ActivePowerMinusOperatorTest(double activePower1, double activePower2, double expected)
        {
            ActivePower ap1 = new ActivePower(activePower1);
            ActivePower ap2 = new ActivePower(activePower2);
            ActivePower power = ap1 - ap2;
            Assert.AreEqual(expected, power.WattsPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 152.41557)]
        [DataRow(12.34567, 12.34567111, 152.41558)]
        [DataRow(12.34567, 12.34567890, 152.41568)]
        [DataRow(12.34567, 12.34544, 152.41273)]
        public void ActivePowerMultiplyOperatorTest(double activePower1, double activePower2, double expected)
        {
            ActivePower ap1 = new ActivePower(activePower1);
            ActivePower ap2 = new ActivePower(activePower2);
            ActivePower power = ap1 * ap2;
            Assert.AreEqual(expected, power.WattsPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 2, 6.17284)]
        [DataRow(12.34567, 3, 4.11522)]
        [DataRow(12.34567, 2.5, 4.93827)]
        [DataRow(12.34567, 12, 1.02881)]
        [DataRow(12.34567, 0, double.NaN)]
        public void ActivePowerDevisionOperatorTest(double activePower1, double activePower2, double expected)
        {
            ActivePower ap1 = new ActivePower(activePower1);
            ActivePower ap2 = new ActivePower(activePower2);
            ActivePower power = ap1 / ap2;
            Assert.AreEqual(expected, power.WattsPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 2, false)]
        [DataRow(12.34567, 12.34566, false)]
        [DataRow(12.34567, 2.5, false)]
        [DataRow(12.34567, 12.5, true)]
        [DataRow(12.34567, 12.999999, true)]
        public void ActivePowerSmallerOperatorTest(double activePower1, double activePower2, bool expected)
        {
            ActivePower ap1 = new ActivePower(activePower1);
            ActivePower ap2 = new ActivePower(activePower2);
            Assert.AreEqual(expected, ap1 < ap2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, false)]
        [DataRow(12.34567, 12.34566, false)]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 2.5, false)]
        [DataRow(12.34567, 12.5, true)]
        [DataRow(12.34567, 12.999999, true)]
        public void ActivePowerSmallerEqualOperatorTest(double activePower1, double activePower2, bool expected)
        {
            ActivePower ap1 = new ActivePower(activePower1);
            ActivePower ap2 = new ActivePower(activePower2);
            Assert.AreEqual(expected, ap1 <= ap2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, true)]
        [DataRow(12.34567, 12.34566, true)]
        [DataRow(12.34567, 2.5, true)]
        [DataRow(12.34567, 12.5, false)]
        [DataRow(12.34567, 12.999999, false)]
        public void ActivePowerGreaterOperatorTest(double activePower1, double activePower2, bool expected)
        {
            ActivePower ap1 = new ActivePower(activePower1);
            ActivePower ap2 = new ActivePower(activePower2);
            Assert.AreEqual(expected, ap1 > ap2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, true)]
        [DataRow(12.34567, 12.34566, true)]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 2.5, true)]
        [DataRow(12.34567, 12.5, false)]
        [DataRow(12.34567, 12.999999, false)]
        public void ActivePowerGreaterEqualOperatorTest(double activePower1, double activePower2, bool expected)
        {
            ActivePower ap1 = new ActivePower(activePower1);
            ActivePower ap2 = new ActivePower(activePower2);
            Assert.AreEqual(expected, ap1 >= ap2);
        }


        [TestMethod]
        [DataRow(double.NaN, true)]
        [DataRow(12.34567, false)]
        [DataRow(double.MinValue, false)]
        [DataRow(double.MaxValue, false)]
        public void ActivePowerIsNaNOperatorTest(double activePower1, bool expected)
        {
            ActivePower ap1 = new ActivePower(activePower1);
            Assert.AreEqual(expected, ap1.IsNaN());
        }

    }
}

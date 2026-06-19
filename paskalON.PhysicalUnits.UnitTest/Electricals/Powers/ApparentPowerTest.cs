using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.PhysicalUnits.UnitTest.Electricals.Powers
{
    [TestClass]
    public class ApparentPowerTest
    {
        [TestMethod]
        public void ApparentPowerConstructorTest()
        {
            ApparentPower apparentPower = new ApparentPower(12.345678901);
            Assert.AreEqual(12.34568, apparentPower.VoltAmperesPrecision);
            Assert.AreEqual(5, apparentPower.Precision);
        }



        [TestMethod]
        [DataRow(12, 1, 12.04159)]
        [DataRow(50, 10, 50.9902)]
        [DataRow(10, 50, 50.9902)]
        [DataRow(65.4321, 12.3456, 66.58659)]
        public void ApparentPowerConstructorActiveReactiveTest(double activePower, double reactivePower, double expected)
        {
            ApparentPower apparentPower = new ApparentPower(new ActivePower(activePower), new ReactivePower(reactivePower));
            Assert.AreEqual(expected, apparentPower.VoltAmperesPrecision);
            Assert.AreEqual(5, apparentPower.Precision);
        }


        [TestMethod]
        public void ApparentPowerKiloVoltAmperesApparentTest()
        {
            ApparentPower apparentPower = new ApparentPower(12345.678);
            Assert.AreEqual(12.34568, apparentPower.KiloVoltAmperesPrecision);
        }


        [TestMethod]
        public void ApparentPowerMegaVoltAmperesApparentTest()
        {
            ApparentPower apparentPower = new ApparentPower(12345678.901);
            Assert.AreEqual(12.34568, apparentPower.MegaVoltAmperesPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void ApparentPowerEqualsTest(double apparentPower1, double apparentPower2, bool expected)
        {
            ApparentPower ap1 = new ApparentPower(apparentPower1);
            ApparentPower ap2 = new ApparentPower(apparentPower2);
            Assert.AreEqual(expected, ap1.Equals(ap2));
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void ApparentPowerEqualsObjectTest(double apparentPower1, double apparentPower2, bool expected)
        {
            object ap1 = new ApparentPower(apparentPower1);
            object ap2 = new ApparentPower(apparentPower2);
            Assert.AreEqual(expected, ap1.Equals(ap2));
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void ApparentPowerEqualsOperatorTest(double apparentPower1, double apparentPower2, bool expected)
        {
            ApparentPower ap1 = new ApparentPower(apparentPower1);
            ApparentPower ap2 = new ApparentPower(apparentPower2);
            Assert.AreEqual(expected, ap1 == ap2);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, false)]
        [DataRow(12.34567, 12.34567111, false)]
        [DataRow(12.34567, 12.34567890, true)]
        [DataRow(12.34567, 12.34544, true)]
        public void ApparentPowerNotEqualsOperatorTest(double apparentPower1, double apparentPower2, bool expected)
        {
            ApparentPower ap1 = new ApparentPower(apparentPower1);
            ApparentPower ap2 = new ApparentPower(apparentPower2);
            Assert.AreEqual(expected, ap1 != ap2);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 24.69134)]
        [DataRow(12.34567, 12.34567111, 24.69134)]
        [DataRow(12.34567, 12.34567890, 24.69135)]
        [DataRow(12.34567, 12.34544, 24.69111)]
        public void ApparentPowerAddOperatorTest(double apparentPower1, double apparentPower2, double expected)
        {
            ApparentPower ap1 = new ApparentPower(apparentPower1);
            ApparentPower ap2 = new ApparentPower(apparentPower2);
            ApparentPower power = ap1 + ap2;
            Assert.AreEqual(expected, power.VoltAmperesPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 0)]
        [DataRow(12.34567, 12.34567111, 0)]
        [DataRow(12.34567, 12.34567890, -0.00001)]
        [DataRow(12.34567, 12.34544, 0.00023)]
        public void ApparentPowerMinusOperatorTest(double apparentPower1, double apparentPower2, double expected)
        {
            ApparentPower ap1 = new ApparentPower(apparentPower1);
            ApparentPower ap2 = new ApparentPower(apparentPower2);
            ApparentPower power = ap1 - ap2;
            Assert.AreEqual(expected, power.VoltAmperesPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 152.41557)]
        [DataRow(12.34567, 12.34567111, 152.41558)]
        [DataRow(12.34567, 12.34567890, 152.41568)]
        [DataRow(12.34567, 12.34544, 152.41273)]
        public void ApparentPowerMultiplyOperatorTest(double apparentPower1, double apparentPower2, double expected)
        {
            ApparentPower ap1 = new ApparentPower(apparentPower1);
            ApparentPower ap2 = new ApparentPower(apparentPower2);
            ApparentPower power = ap1 * ap2;
            Assert.AreEqual(expected, power.VoltAmperesPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 2, 6.17284)]
        [DataRow(12.34567, 3, 4.11522)]
        [DataRow(12.34567, 2.5, 4.93827)]
        [DataRow(12.34567, 12, 1.02881)]
        [DataRow(12.34567, 0, double.NaN)]
        public void ApparentPowerDevisionOperatorTest(double apparentPower1, double apparentPower2, double expected)
        {
            ApparentPower ap1 = new ApparentPower(apparentPower1);
            ApparentPower ap2 = new ApparentPower(apparentPower2);
            ApparentPower power = ap1 / ap2;
            Assert.AreEqual(expected, power.VoltAmperesPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 2, false)]
        [DataRow(12.34567, 12.34566, false)]
        [DataRow(12.34567, 2.5, false)]
        [DataRow(12.34567, 12.5, true)]
        [DataRow(12.34567, 12.999999, true)]
        public void ApparentPowerSmallerOperatorTest(double apparentPower1, double apparentPower2, bool expected)
        {
            ApparentPower ap1 = new ApparentPower(apparentPower1);
            ApparentPower ap2 = new ApparentPower(apparentPower2);
            Assert.AreEqual(expected, ap1 < ap2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, false)]
        [DataRow(12.34567, 12.34566, false)]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 2.5, false)]
        [DataRow(12.34567, 12.5, true)]
        [DataRow(12.34567, 12.999999, true)]
        public void ApparentPowerSmallerEqualOperatorTest(double apparentPower1, double apparentPower2, bool expected)
        {
            ApparentPower ap1 = new ApparentPower(apparentPower1);
            ApparentPower ap2 = new ApparentPower(apparentPower2);
            Assert.AreEqual(expected, ap1 <= ap2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, true)]
        [DataRow(12.34567, 12.34566, true)]
        [DataRow(12.34567, 2.5, true)]
        [DataRow(12.34567, 12.5, false)]
        [DataRow(12.34567, 12.999999, false)]
        public void ApparentPowerGreaterOperatorTest(double apparentPower1, double apparentPower2, bool expected)
        {
            ApparentPower ap1 = new ApparentPower(apparentPower1);
            ApparentPower ap2 = new ApparentPower(apparentPower2);
            Assert.AreEqual(expected, ap1 > ap2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, true)]
        [DataRow(12.34567, 12.34566, true)]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 2.5, true)]
        [DataRow(12.34567, 12.5, false)]
        [DataRow(12.34567, 12.999999, false)]
        public void ApparentPowerGreaterEqualOperatorTest(double apparentPower1, double apparentPower2, bool expected)
        {
            ApparentPower ap1 = new ApparentPower(apparentPower1);
            ApparentPower ap2 = new ApparentPower(apparentPower2);
            Assert.AreEqual(expected, ap1 >= ap2);
        }


        [TestMethod]
        [DataRow(double.NaN, true)]
        [DataRow(12.34567, false)]
        [DataRow(double.MinValue, false)]
        [DataRow(double.MaxValue, false)]
        public void ApparentPowerIsNaNOperatorTest(double apparentPower1, bool expected)
        {
            ApparentPower ap1 = new ApparentPower(apparentPower1);
            Assert.AreEqual(expected, ap1.IsNaN());
        }

    }
}

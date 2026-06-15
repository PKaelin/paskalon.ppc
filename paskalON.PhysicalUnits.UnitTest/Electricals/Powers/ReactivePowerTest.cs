using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.PhysicalUnits.UnitTest.Electricals.Powers
{
    [TestClass]
    public class ReactivePowerTest
    {
        [TestMethod]
        public void ReactivePowerConstructorTest()
        {
            ReactivePower reactivePower = new ReactivePower(12.345678901);
            Assert.AreEqual(12.34568, reactivePower.VoltAmperesReactivePercision);
            Assert.AreEqual(5, reactivePower.Precision);
        }


        [TestMethod]
        public void ReactivePowerKiloVoltAmperesReactiveTest()
        {
            ReactivePower reactivePower = new ReactivePower(12345.678);
            Assert.AreEqual(12.34568, reactivePower.KiloVoltAmperesReactivePercision);
        }


        [TestMethod]
        public void ReactivePowerMegaVoltAmperesReactiveTest()
        {
            ReactivePower reactivePower = new ReactivePower(12345678.901);
            Assert.AreEqual(12.34568, reactivePower.MegaVoltAmperesReactivePercision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void ReactivePowerEqualsTest(double reactivePower1, double reactivePower2, bool expected)
        {
            ReactivePower rp1 = new ReactivePower(reactivePower1);
            ReactivePower rp2 = new ReactivePower(reactivePower2);
            Assert.AreEqual(expected, rp1.Equals(rp2));
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void ReactivePowerEqualsObjectTest(double reactivePower1, double reactivePower2, bool expected)
        {
            object rp1 = new ReactivePower(reactivePower1);
            object rp2 = new ReactivePower(reactivePower2);
            Assert.AreEqual(expected, rp1.Equals(rp2));
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void ReactivePowerEqualsOperatorTest(double reactivePower1, double reactivePower2, bool expected)
        {
            ReactivePower rp1 = new ReactivePower(reactivePower1);
            ReactivePower rp2 = new ReactivePower(reactivePower2);
            Assert.AreEqual(expected, rp1 == rp2);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, false)]
        [DataRow(12.34567, 12.34567111, false)]
        [DataRow(12.34567, 12.34567890, true)]
        [DataRow(12.34567, 12.34544, true)]
        public void ReactivePowerNotEqualsOperatorTest(double reactivePower1, double reactivePower2, bool expected)
        {
            ReactivePower rp1 = new ReactivePower(reactivePower1);
            ReactivePower rp2 = new ReactivePower(reactivePower2);
            Assert.AreEqual(expected, rp1 != rp2);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 24.69134)]
        [DataRow(12.34567, 12.34567111, 24.69134)]
        [DataRow(12.34567, 12.34567890, 24.69135)]
        [DataRow(12.34567, 12.34544, 24.69111)]
        public void ReactivePowerAddOperatorTest(double reactivePower1, double reactivePower2, double expected)
        {
            ReactivePower rp1 = new ReactivePower(reactivePower1);
            ReactivePower rp2 = new ReactivePower(reactivePower2);
            ReactivePower power = rp1 + rp2;
            Assert.AreEqual(expected, power.VoltAmperesReactivePercision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 0)]
        [DataRow(12.34567, 12.34567111, 0)]
        [DataRow(12.34567, 12.34567890, -0.00001)]
        [DataRow(12.34567, 12.34544, 0.00023)]
        public void ReactivePowerMinusOperatorTest(double reactivePower1, double reactivePower2, double expected)
        {
            ReactivePower rp1 = new ReactivePower(reactivePower1);
            ReactivePower rp2 = new ReactivePower(reactivePower2);
            ReactivePower power = rp1 - rp2;
            Assert.AreEqual(expected, power.VoltAmperesReactivePercision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 152.41557)]
        [DataRow(12.34567, 12.34567111, 152.41558)]
        [DataRow(12.34567, 12.34567890, 152.41568)]
        [DataRow(12.34567, 12.34544, 152.41273)]
        public void ReactivePowerMultiplyOperatorTest(double reactivePower1, double reactivePower2, double expected)
        {
            ReactivePower rp1 = new ReactivePower(reactivePower1);
            ReactivePower rp2 = new ReactivePower(reactivePower2);
            ReactivePower power = rp1 * rp2;
            Assert.AreEqual(expected, power.VoltAmperesReactivePercision);
        }


        [TestMethod]
        [DataRow(12.34567, 2, 6.17284)]
        [DataRow(12.34567, 3, 4.11522)]
        [DataRow(12.34567, 2.5, 4.93827)]
        [DataRow(12.34567, 12, 1.02881)]
        [DataRow(12.34567, 0, double.NaN)]
        public void ReactivePowerDevisionOperatorTest(double reactivePower1, double reactivePower2, double expected)
        {
            ReactivePower rp1 = new ReactivePower(reactivePower1);
            ReactivePower rp2 = new ReactivePower(reactivePower2);
            ReactivePower power = rp1 / rp2;
            Assert.AreEqual(expected, power.VoltAmperesReactivePercision);
        }


        [TestMethod]
        [DataRow(12.34567, 2, false)]
        [DataRow(12.34567, 12.34566, false)]
        [DataRow(12.34567, 2.5, false)]
        [DataRow(12.34567, 12.5, true)]
        [DataRow(12.34567, 12.999999, true)]
        public void ReactivePowerSmallerOperatorTest(double reactivePower1, double reactivePower2, bool expected)
        {
            ReactivePower rp1 = new ReactivePower(reactivePower1);
            ReactivePower rp2 = new ReactivePower(reactivePower2);
            Assert.AreEqual(expected, rp1 < rp2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, false)]
        [DataRow(12.34567, 12.34566, false)]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 2.5, false)]
        [DataRow(12.34567, 12.5, true)]
        [DataRow(12.34567, 12.999999, true)]
        public void ReactivePowerSmallerEqualOperatorTest(double reactivePower1, double reactivePower2, bool expected)
        {
            ReactivePower rp1 = new ReactivePower(reactivePower1);
            ReactivePower rp2 = new ReactivePower(reactivePower2);
            Assert.AreEqual(expected, rp1 <= rp2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, true)]
        [DataRow(12.34567, 12.34566, true)]
        [DataRow(12.34567, 2.5, true)]
        [DataRow(12.34567, 12.5, false)]
        [DataRow(12.34567, 12.999999, false)]
        public void ReactivePowerGreaterOperatorTest(double reactivePower1, double reactivePower2, bool expected)
        {
            ReactivePower rp1 = new ReactivePower(reactivePower1);
            ReactivePower rp2 = new ReactivePower(reactivePower2);
            Assert.AreEqual(expected, rp1 > rp2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, true)]
        [DataRow(12.34567, 12.34566, true)]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 2.5, true)]
        [DataRow(12.34567, 12.5, false)]
        [DataRow(12.34567, 12.999999, false)]
        public void ReactivePowerGreaterEqualOperatorTest(double reactivePower1, double reactivePower2, bool expected)
        {
            ReactivePower rp1 = new ReactivePower(reactivePower1);
            ReactivePower rp2 = new ReactivePower(reactivePower2);
            Assert.AreEqual(expected, rp1 >= rp2);
        }


        [TestMethod]
        [DataRow(double.NaN, true)]
        [DataRow(12.34567, false)]
        [DataRow(double.MinValue, false)]
        [DataRow(double.MaxValue, false)]
        public void ReactivePowerIsNaNOperatorTest(double reactivePower1, bool expected)
        {
            ReactivePower rp1 = new ReactivePower(reactivePower1);
            Assert.AreEqual(expected, rp1.IsNaN());
        }

    }
}

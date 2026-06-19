using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.PhysicalUnits.UnitTest.Electricals.Powers
{
    [TestClass]
    public class CurrentScalarTest
    {
        [TestMethod]
        public void CurrentScalarConstructorTest()
        {
            CurrentScalar current = new CurrentScalar(12.345678901);
            Assert.AreEqual(12.34568, current.AmperesPrecision);
            Assert.AreEqual(5, current.Precision);
        }


        [TestMethod]
        public void CurrentScalarKiloWattsTest()
        {
            CurrentScalar current = new CurrentScalar(12345.678);
            Assert.AreEqual(12.34568, current.KiloAmperesPrecision);
        }


        [TestMethod]
        public void CurrentScalarMegaWattsTest()
        {
            CurrentScalar current = new CurrentScalar(12345678.901);
            Assert.AreEqual(12.34568, current.MegaAmperesPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void CurrentScalarEqualsTest(double current1, double current2, bool expected)
        {
            CurrentScalar c1 = new CurrentScalar(current1);
            CurrentScalar c2 = new CurrentScalar(current2);
            Assert.AreEqual(expected, c1.Equals(c2));
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void CurrentScalarEqualsObjectTest(double current1, double current2, bool expected)
        {
            object c1 = new CurrentScalar(current1);
            object c2 = new CurrentScalar(current2);
            Assert.AreEqual(expected, c1.Equals(c2));
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void CurrentScalarEqualsOperatorTest(double current1, double current2, bool expected)
        {
            CurrentScalar c1 = new CurrentScalar(current1);
            CurrentScalar c2 = new CurrentScalar(current2);
            Assert.AreEqual(expected, c1 == c2);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, false)]
        [DataRow(12.34567, 12.34567111, false)]
        [DataRow(12.34567, 12.34567890, true)]
        [DataRow(12.34567, 12.34544, true)]
        public void CurrentScalarNotEqualsOperatorTest(double current1, double current2, bool expected)
        {
            CurrentScalar c1 = new CurrentScalar(current1);
            CurrentScalar c2 = new CurrentScalar(current2);
            Assert.AreEqual(expected, c1 != c2);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 24.69134)]
        [DataRow(12.34567, 12.34567111, 24.69134)]
        [DataRow(12.34567, 12.34567890, 24.69135)]
        [DataRow(12.34567, 12.34544, 24.69111)]
        public void CurrentScalarAddOperatorTest(double current1, double current2, double expected)
        {
            CurrentScalar c1 = new CurrentScalar(current1);
            CurrentScalar c2 = new CurrentScalar(current2);
            CurrentScalar power = c1 + c2;
            Assert.AreEqual(expected, power.AmperesPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 0)]
        [DataRow(12.34567, 12.34567111, 0)]
        [DataRow(12.34567, 12.34567890, -0.00001)]
        [DataRow(12.34567, 12.34544, 0.00023)]
        public void CurrentScalarMinusOperatorTest(double current1, double current2, double expected)
        {
            CurrentScalar c1 = new CurrentScalar(current1);
            CurrentScalar c2 = new CurrentScalar(current2);
            CurrentScalar power = c1 - c2;
            Assert.AreEqual(expected, power.AmperesPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 152.41557)]
        [DataRow(12.34567, 12.34567111, 152.41558)]
        [DataRow(12.34567, 12.34567890, 152.41568)]
        [DataRow(12.34567, 12.34544, 152.41273)]
        public void CurrentScalarMultiplyOperatorTest(double current1, double current2, double expected)
        {
            CurrentScalar c1 = new CurrentScalar(current1);
            CurrentScalar c2 = new CurrentScalar(current2);
            CurrentScalar power = c1 * c2;
            Assert.AreEqual(expected, power.AmperesPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 2, 6.17284)]
        [DataRow(12.34567, 3, 4.11522)]
        [DataRow(12.34567, 2.5, 4.93827)]
        [DataRow(12.34567, 12, 1.02881)]
        [DataRow(12.34567, 0, double.NaN)]
        public void CurrentScalarDivisionOperatorTest(double current1, double current2, double expected)
        {
            CurrentScalar c1 = new CurrentScalar(current1);
            CurrentScalar c2 = new CurrentScalar(current2);
            CurrentScalar power = c1 / c2;
            Assert.AreEqual(expected, power.AmperesPrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 2, false)]
        [DataRow(12.34567, 12.34566, false)]
        [DataRow(12.34567, 2.5, false)]
        [DataRow(12.34567, 12.5, true)]
        [DataRow(12.34567, 12.999999, true)]
        public void CurrentScalarSmallerOperatorTest(double current1, double current2, bool expected)
        {
            CurrentScalar c1 = new CurrentScalar(current1);
            CurrentScalar c2 = new CurrentScalar(current2);
            Assert.AreEqual(expected, c1 < c2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, false)]
        [DataRow(12.34567, 12.34566, false)]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 2.5, false)]
        [DataRow(12.34567, 12.5, true)]
        [DataRow(12.34567, 12.999999, true)]
        public void CurrentScalarSmallerEqualOperatorTest(double current1, double current2, bool expected)
        {
            CurrentScalar c1 = new CurrentScalar(current1);
            CurrentScalar c2 = new CurrentScalar(current2);
            Assert.AreEqual(expected, c1 <= c2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, true)]
        [DataRow(12.34567, 12.34566, true)]
        [DataRow(12.34567, 2.5, true)]
        [DataRow(12.34567, 12.5, false)]
        [DataRow(12.34567, 12.999999, false)]
        public void CurrentScalarGreaterOperatorTest(double current1, double current2, bool expected)
        {
            CurrentScalar c1 = new CurrentScalar(current1);
            CurrentScalar c2 = new CurrentScalar(current2);
            Assert.AreEqual(expected, c1 > c2);
        }


        [TestMethod]
        [DataRow(12.34567, 2, true)]
        [DataRow(12.34567, 12.34566, true)]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 2.5, true)]
        [DataRow(12.34567, 12.5, false)]
        [DataRow(12.34567, 12.999999, false)]
        public void CurrentScalarGreaterEqualOperatorTest(double current1, double current2, bool expected)
        {
            CurrentScalar c1 = new CurrentScalar(current1);
            CurrentScalar c2 = new CurrentScalar(current2);
            Assert.AreEqual(expected, c1 >= c2);
        }


        [TestMethod]
        [DataRow(double.NaN, true)]
        [DataRow(12.34567, false)]
        [DataRow(double.MinValue, false)]
        [DataRow(double.MaxValue, false)]
        public void CurrentScalarIsNaNOperatorTest(double current1, bool expected)
        {
            CurrentScalar c1 = new CurrentScalar(current1);
            Assert.AreEqual(expected, c1.IsNaN());
        }

    }
}

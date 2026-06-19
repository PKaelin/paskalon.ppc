using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.PhysicalUnits.UnitTest.Electricals.Powers
{
    [TestClass]
    public class ComplexPowerTest
    {
        [TestMethod]
        public void ComplexPowerConstructorTest()
        {
            ActivePower activePower = new ActivePower(5);
            ReactivePower reactivePower = new ReactivePower(3);
            ComplexPower complexPower = new ComplexPower(activePower, reactivePower);

            Assert.AreEqual(5, complexPower.ActivePower.Watts);
            Assert.AreEqual(3, complexPower.ReactivePower.VoltAmperesReactive);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(15.34567, 15.34567, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void ComplexPowerEqualsTest(double power1, double power2, bool expected)
        {
            ActivePower activePower1 = new ActivePower(power1);
            ReactivePower reactivePower1 = new ReactivePower(power1 + 2);
            ActivePower activePower2 = new ActivePower(power2);
            ReactivePower reactivePower2 = new ReactivePower(power2 + 2);
            ComplexPower cp1 = new ComplexPower(activePower1, reactivePower1);
            ComplexPower cp2 = new ComplexPower(activePower2, reactivePower2);

            Assert.AreEqual(expected, cp1.Equals(cp2));
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void ComplexPowerEqualsObjectTest(double power1, double power2, bool expected)
        {
            ActivePower activePower1 = new ActivePower(power1);
            ReactivePower reactivePower1 = new ReactivePower(power1 + 2);
            ActivePower activePower2 = new ActivePower(power2);
            ReactivePower reactivePower2 = new ReactivePower(power2 + 2);
            ComplexPower cp1 = new ComplexPower(activePower1, reactivePower1);
            ComplexPower cp2 = new ComplexPower(activePower2, reactivePower2);

            Assert.AreEqual(expected, cp1.Equals(cp2));
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, true)]
        [DataRow(12.34567, 12.34567111, true)]
        [DataRow(12.34567, 12.34567890, false)]
        [DataRow(12.34567, 12.34544, false)]
        public void ComplexPowerEqualsOperatorTest(double power1, double power2, bool expected)
        {
            // Compare has a default precision
            ActivePower activePower1 = new ActivePower(power1);
            ReactivePower reactivePower1 = new ReactivePower(power1 + 2);
            ActivePower activePower2 = new ActivePower(power2);
            ReactivePower reactivePower2 = new ReactivePower(power2 + 2);
            ComplexPower cp1 = new ComplexPower(activePower1, reactivePower1);
            ComplexPower cp2 = new ComplexPower(activePower2, reactivePower2);

            Assert.AreEqual(expected, cp1 == cp2);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, false)]
        [DataRow(14.345, 14.345, false)]
        [DataRow(12.34567, 12.34567111, false)]
        [DataRow(12.34567, 12.34567890, true)]
        [DataRow(12.34567, 12.34544, true)]
        public void ComplexPowerNotEqualsOperatorTest(double power1, double power2, bool expected)
        {
            ActivePower activePower1 = new ActivePower(power1);
            ReactivePower reactivePower1 = new ReactivePower(power1 + 2);
            ActivePower activePower2 = new ActivePower(power2);
            ReactivePower reactivePower2 = new ReactivePower(power2 + 2);
            ComplexPower cp1 = new ComplexPower(activePower1, reactivePower1);
            ComplexPower cp2 = new ComplexPower(activePower2, reactivePower2);

            Assert.AreEqual(expected, cp1 != cp2);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 24.69134)]
        [DataRow(12.34567, 12.34567111, 24.69134)]
        [DataRow(12.34567, 12.34567890, 24.69135)]
        [DataRow(12.34567, 12.34544, 24.69111)]
        public void ComplexPowerAddOperatorTest(double power1, double power2, double expected)
        {
            ActivePower activePower1 = new ActivePower(power1);
            ReactivePower reactivePower1 = new ReactivePower(power1 + 2);
            ActivePower activePower2 = new ActivePower(power2);
            ReactivePower reactivePower2 = new ReactivePower(power2 + 2);
            ComplexPower cp1 = new ComplexPower(activePower1, reactivePower1);
            ComplexPower cp2 = new ComplexPower(activePower2, reactivePower2);

            ComplexPower power = cp1 + cp2;
            Assert.AreEqual(expected, power.ActivePower.WattsPrecision);
            Assert.AreEqual(expected + 4, power.ReactivePower.VoltAmperesReactivePrecision);
        }


        [TestMethod]
        [DataRow(12.34567, 12.34567, 0)]
        [DataRow(12.34567, 12.34567111, 0)]
        [DataRow(12.34567, 12.34567890, -0.00001)]
        [DataRow(12.34567, 12.34544, 0.00023)]
        public void ComplexPowerMinusOperatorTest(double power1, double power2, double expected)
        {
            ActivePower activePower1 = new ActivePower(power1);
            ReactivePower reactivePower1 = new ReactivePower(power1 + 2);
            ActivePower activePower2 = new ActivePower(power2);
            ReactivePower reactivePower2 = new ReactivePower(power2 + 2);
            ComplexPower cp1 = new ComplexPower(activePower1, reactivePower1);
            ComplexPower cp2 = new ComplexPower(activePower2, reactivePower2);

            ComplexPower power = cp1 - cp2;
            Assert.AreEqual(expected, power.ActivePower.WattsPrecision);
            Assert.AreEqual(expected, power.ReactivePower.VoltAmperesReactivePrecision);

        }

    }
}

using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.PhysicalUnits.UnitTest.Electricals.Powers
{
    [TestClass]
    public class IecPowerFactorTest
    {
        /// Uses IEC power factor sign convention:
        /// P+ Q+ = PF+
        /// P+ Q- = PF+
        /// P- Q+ = PF-
        /// P- Q- = PF-.
        [TestMethod]
        [DataRow(100, 50, 1)]
        [DataRow(100, -50, 1)]
        [DataRow(-100, 50, -1)]
        [DataRow(-100, -50, -1)]
        public void PowerFactorIecSignTest(double activePower, double reactivePower, int expectedSign)
        {
            int sign = IecPowerFactor.IecSign(activePower, reactivePower);
            Assert.AreEqual(expectedSign, sign);
        }


        [TestMethod]
        [DataRow(5, 10, 0.44721)]
        [DataRow(5, -2, 0.92848)]
        [DataRow(-10, 4, -0.92848)]
        [DataRow(-20, -2, -0.99504)]
        public void PowerFactorIecCalculateTest(double activePower, double reactivePower, double expectedPowerFactor)
        {
            double powerFactor = IecPowerFactor.Calculate(activePower, reactivePower).PowerFactor;
            Assert.AreEqual(expectedPowerFactor, Math.Round(powerFactor, 5));
        }
    }
}

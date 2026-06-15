using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.PhysicalUnits.UnitTest.Electricals.Powers
{
    [TestClass]
    public class IeeePowerFactorTest
    {
        /// Uses IEEE power factor sign convention:
        /// P+ Q+ = PF- (lag)
        /// P+ Q- = PF+ (lead)
        /// P- Q+ = PF+ (lead)
        /// P- Q- = PF- (lag)
        [TestMethod]
        [DataRow(100, 50, -1)]
        [DataRow(100, -50, 1)]
        [DataRow(-100, 50, 1)]
        [DataRow(-100, -50, -1)]
        public void PowerFactorIeeeSignTest(double activePower, double reactivePower, int expectedSign)
        {
            int sign = IeeePowerFactor.IeeeSign(activePower, reactivePower);
            Assert.AreEqual(expectedSign, sign);
        }


        [TestMethod]
        [DataRow(5, 10, -0.44721)]
        [DataRow(5, -2, 0.92848)]
        [DataRow(-10, 4, 0.92848)]
        [DataRow(-20, -2, -0.99504)]
        public void PowerFactorIeeeCalculateTest(double activePower, double reactivePower, double expectedPowerFactor)
        {
            double powerFactor = IeeePowerFactor.Calculate(activePower, reactivePower).PowerFactor;
            Assert.AreEqual(expectedPowerFactor, Math.Round(powerFactor, 5));
        }
    }
}

using paskalON.Maths.Calculuses.Exponents;

namespace paskalON.Maths.UnitTest.Calculuses.Exponents
{
    [TestClass]
    public class ExponentialFunctionTest
    {

        [TestMethod]
        public void FactorNegativeTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() => new ExponentialFunction(520, -5));
        }


        [TestMethod]
        public void Factor0Test()
        {
            ExponentialFunction ef = new ExponentialFunction(520, 0);

            Assert.AreEqual(0, ef.CalculateOutputPrecision(0));
            Assert.AreEqual(0, ef.CalculateOutputPrecision(1));
            Assert.AreEqual(0, ef.CalculateOutputPrecision(3));
        }


        [TestMethod]
        public void GrowthByDoubleTest()
        {
            ExponentialFunction ef = new ExponentialFunction(520, 2);

            Assert.AreEqual(520, ef.CalculateOutputPrecision(0));
            Assert.AreEqual(1040, ef.CalculateOutputPrecision(1));
            Assert.AreEqual(2080, ef.CalculateOutputPrecision(2));
            Assert.AreEqual(4160, ef.CalculateOutputPrecision(3));
            Assert.AreEqual(8320, ef.CalculateOutputPrecision(4));
        }


        [TestMethod]
        public void GrowthByDoubleAddOffestTest()
        {
            ExponentialFunction ef = new ExponentialFunction(520, 2, 10);

            Assert.AreEqual(530, ef.CalculateOutputPrecision(0));
            Assert.AreEqual(1050, ef.CalculateOutputPrecision(1));
            Assert.AreEqual(2090, ef.CalculateOutputPrecision(2));
            Assert.AreEqual(4170, ef.CalculateOutputPrecision(3));
            Assert.AreEqual(8330, ef.CalculateOutputPrecision(4));
        }


        [TestMethod]
        public void GrowthByDoubleInitialNegativeTest()
        {
            ExponentialFunction ef = new ExponentialFunction(-520, 2);

            Assert.AreEqual(-520, ef.CalculateOutputPrecision(0));
            Assert.AreEqual(-1040, ef.CalculateOutputPrecision(1));
            Assert.AreEqual(-2080, ef.CalculateOutputPrecision(2));
            Assert.AreEqual(-4160, ef.CalculateOutputPrecision(3));
            Assert.AreEqual(-8320, ef.CalculateOutputPrecision(4));
        }



        [TestMethod]
        public void DecayByMultiplierTest()
        {
            ExponentialFunction ef = new ExponentialFunction(9000, 0.925, 0);
            // Value decrease is 7.5% each X
            Assert.AreEqual(9000, ef.CalculateOutputPrecision(0, 2));
            Assert.AreEqual(8325, ef.CalculateOutputPrecision(1, 2));
            Assert.AreEqual(7700.63, ef.CalculateOutputPrecision(2, 2));
            Assert.AreEqual(7123.08, ef.CalculateOutputPrecision(3, 2));
            Assert.AreEqual(6588.85, ef.CalculateOutputPrecision(4, 2));
        }


        [TestMethod]
        public void DecayByMultiplierInitialNegativeTest()
        {
            ExponentialFunction ef = new ExponentialFunction(-9000, 0.925, 0);
            // Value decrease is 7.5% each X
            Assert.AreEqual(-9000, ef.CalculateOutputPrecision(0, 2));
            Assert.AreEqual(-8325, ef.CalculateOutputPrecision(1, 2));
            Assert.AreEqual(-7700.63, ef.CalculateOutputPrecision(2, 2));
            Assert.AreEqual(-7123.08, ef.CalculateOutputPrecision(3, 2));
            Assert.AreEqual(-6588.85, ef.CalculateOutputPrecision(4, 2));
        }


        [TestMethod]
        public void GrowthByDoubleNoiseAddedTest()
        {
            ExponentialFunction ef = new ExponentialFunction(520, 2, 0, -1, 1);

            double value = ef.CalculateOutputPrecision(0);
            Assert.IsTrue(value > 519 && value < 521);

            value = ef.CalculateOutputPrecision(1);
            Assert.IsTrue(value > 1039 && value < 1041);

            value = ef.CalculateOutputPrecision(2);
            Assert.IsTrue(value > 2079 && value < 2081);

            value = ef.CalculateOutputPrecision(3);
            Assert.IsTrue(value > 4159 && value < 4161);

            value = ef.CalculateOutputPrecision(4);
            Assert.IsTrue(value > 8319 && value < 8321);
        }
    }
}

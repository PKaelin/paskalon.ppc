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

            Assert.AreEqual(0, ef.CalculateOutput(0));
            Assert.AreEqual(0, ef.CalculateOutput(1));
            Assert.AreEqual(0, ef.CalculateOutput(3));
        }


        [TestMethod]
        public void GrowthByDoubleTest()
        {
            ExponentialFunction ef = new ExponentialFunction(520, 2);

            Assert.AreEqual(520, ef.CalculateOutput(0));
            Assert.AreEqual(1040, ef.CalculateOutput(1));
            Assert.AreEqual(2080, ef.CalculateOutput(2));
            Assert.AreEqual(4160, ef.CalculateOutput(3));
            Assert.AreEqual(8320, ef.CalculateOutput(4));
        }


        [TestMethod]
        public void GrowthByDoubleAddOffestTest()
        {
            ExponentialFunction ef = new ExponentialFunction(520, 2, 10);

            Assert.AreEqual(530, ef.CalculateOutput(0));
            Assert.AreEqual(1050, ef.CalculateOutput(1));
            Assert.AreEqual(2090, ef.CalculateOutput(2));
            Assert.AreEqual(4170, ef.CalculateOutput(3));
            Assert.AreEqual(8330, ef.CalculateOutput(4));
        }


        [TestMethod]
        public void GrowthByDoubleInitialNegativeTest()
        {
            ExponentialFunction ef = new ExponentialFunction(-520, 2);

            Assert.AreEqual(-520, ef.CalculateOutput(0));
            Assert.AreEqual(-1040, ef.CalculateOutput(1));
            Assert.AreEqual(-2080, ef.CalculateOutput(2));
            Assert.AreEqual(-4160, ef.CalculateOutput(3));
            Assert.AreEqual(-8320, ef.CalculateOutput(4));
        }



        [TestMethod]
        public void DecayByMultiplierTest()
        {
            ExponentialFunction ef = new ExponentialFunction(9000, 0.925, 0, 2);
            // Value decrease is 7.5% each X
            Assert.AreEqual(9000, ef.CalculateOutput(0));
            Assert.AreEqual(8325, ef.CalculateOutput(1));
            Assert.AreEqual(7700.63, ef.CalculateOutput(2));
            Assert.AreEqual(7123.08, ef.CalculateOutput(3));
            Assert.AreEqual(6588.85, ef.CalculateOutput(4));
        }


        [TestMethod]
        public void DecayByMultiplierInitialNegativeTest()
        {
            ExponentialFunction ef = new ExponentialFunction(-9000, 0.925, 0, 2);
            // Value decrease is 7.5% each X
            Assert.AreEqual(-9000, ef.CalculateOutput(0));
            Assert.AreEqual(-8325, ef.CalculateOutput(1));
            Assert.AreEqual(-7700.63, ef.CalculateOutput(2));
            Assert.AreEqual(-7123.08, ef.CalculateOutput(3));
            Assert.AreEqual(-6588.85, ef.CalculateOutput(4));
        }


        [TestMethod]
        public void GrowthByDoubleNoiseAddedTest()
        {
            ExponentialFunction ef = new ExponentialFunction(520, 2, 0, -1, 1, 2);

            double value = ef.CalculateOutput(0);
            Assert.IsTrue(value > 519 && value < 521);

            value = ef.CalculateOutput(1);
            Assert.IsTrue(value > 1039 && value < 1041);

            value = ef.CalculateOutput(2);
            Assert.IsTrue(value > 2079 && value < 2081);

            value = ef.CalculateOutput(3);
            Assert.IsTrue(value > 4159 && value < 4161);

            value = ef.CalculateOutput(4);
            Assert.IsTrue(value > 8319 && value < 8321);
        }
    }
}

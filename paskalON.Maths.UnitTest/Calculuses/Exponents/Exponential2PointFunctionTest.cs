using paskalON.Maths.Calculuses.Exponents;

namespace paskalON.Maths.UnitTest.Calculuses.Exponents
{
    [TestClass]
    public class Exponential2PointFunctionTest
    {

        [TestMethod]
        public void ToBiggerTest()
        {
            Exponential2PointFunction ef = new Exponential2PointFunction((2, 4), (5, 5));
            Assert.AreEqual(4, ef.CalculateOutputPrecision(2));
            Assert.AreEqual(5, ef.CalculateOutputPrecision(5));

            ef = new Exponential2PointFunction((0, 5), (10, 500));
            Assert.AreEqual(5, ef.CalculateOutputPrecision(0));
            Assert.AreEqual(500, ef.CalculateOutputPrecision(10));

            ef = new Exponential2PointFunction((0, 0.001), (100, 1000));
            Assert.AreEqual(0.001, ef.CalculateOutputPrecision(0));
            Assert.AreEqual(1000, ef.CalculateOutputPrecision(100));
            Assert.AreEqual(1000000, ef.CalculateOutputPrecision(150));
        }


        [TestMethod]
        public void ToSmallerTest()
        {
            Exponential2PointFunction ef = new Exponential2PointFunction((0, 10), (10, 1));

            Assert.AreEqual(10, ef.CalculateOutputPrecision(0));
            double value = ef.CalculateOutputPrecision(3);
            Assert.IsTrue(value < 10 && value > 1);
            value = ef.CalculateOutputPrecision(8);
            Assert.IsTrue(value < 10 && value > 1);
            Assert.AreEqual(1, ef.CalculateOutputPrecision(10));
        }


        [TestMethod]
        public void Y1AndY2EqualTest()
        {
            Exponential2PointFunction ef = new Exponential2PointFunction((0, 10), (10, 10));
            Assert.AreEqual(10, ef.CalculateOutputPrecision(0));
            Assert.AreEqual(10, ef.CalculateOutputPrecision(5));
            Assert.AreEqual(10, ef.CalculateOutputPrecision(10));
        }


        [TestMethod]
        public void X1AndX2EqualTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() => new Exponential2PointFunction((10, 10), (10, 20)));
        }


        [TestMethod]
        public void ToAlmost0Test()
        {
            Exponential2PointFunction ef = new Exponential2PointFunction((20, 100), (30, 0.000001));

            Assert.IsGreaterThan(100, ef.CalculateOutputPrecision(0));
            Assert.AreEqual(100, ef.CalculateOutputPrecision(20));
            double value = ef.CalculateOutputPrecision(21);
            Assert.IsTrue(value > 0.001 && value < 100);
            value = ef.CalculateOutputPrecision(25);
            Assert.IsTrue(value > 0.001 && value < 100);
            Assert.AreEqual(0, ef.CalculateOutputPrecision(30));
        }


        [TestMethod]
        public void To0Test()
        {
            Assert.ThrowsExactly<ArgumentException>(() => new Exponential2PointFunction((20, 100), (30, 0)));
        }


        [TestMethod]
        public void Exponential2PointTest()
        {
            // a = 3.4471, b = 1.0772 => f(x) = 3.4471 * 1.0772^x
            Exponential2PointFunction ef = new Exponential2PointFunction((2, 4), (5, 5));
            Assert.AreEqual(4, ef.CalculateOutputPrecision(2));         // 4
            Assert.AreEqual(4.642, ef.CalculateOutputPrecision(4, 3));     // 4.64158883361278		
            Assert.AreEqual(5.386, ef.CalculateOutputPrecision(6, 3));     // 5.3860867250797106
            Assert.AreEqual(6.733, ef.CalculateOutputPrecision(9, 3));     // 6.7326084063496383

            // a = 0.5, b = 1.3493 => f(x) = 0.5 * 1.3493^x
            ef = new Exponential2PointFunction((10, 10), (20, 200));
            Assert.AreEqual(10d, ef.CalculateOutputPrecision(10, 2));
            Assert.AreEqual(18.21, ef.CalculateOutputPrecision(12, 2));
            Assert.AreEqual(44.72, ef.CalculateOutputPrecision(15, 2));
        }


        [TestMethod]
        public void Exponential2PointOffsetTest()
        {
            // a = 3.4471, b = 1.0772 => f(x) = 3.4471 * 1.0772^x
            Exponential2PointFunction ef = new Exponential2PointFunction((2, 4), (5, 5), 10);
            Assert.AreEqual(14, ef.CalculateOutputPrecision(2));         // 4
            Assert.AreEqual(14.642, ef.CalculateOutputPrecision(4));     // 4.64158883361278		
            Assert.AreEqual(15.386, ef.CalculateOutputPrecision(6));     // 5.3860867250797106
            Assert.AreEqual(16.733, ef.CalculateOutputPrecision(9));     // 6.7326084063496383
        }


        [TestMethod]
        public void PositiveToNegativeTest()
        {
            Assert.ThrowsExactly<ArgumentException>(() => new Exponential2PointFunction((10, 10), (20, -10), 10));
        }



        [TestMethod]
        public void Point1BiggerThanPoint2Test()
        {
            Exponential2PointFunction ef = new Exponential2PointFunction((5, 5), (2, 4));
            Assert.AreEqual(4, ef.CalculateOutputPrecision(2));         // 4
            Assert.AreEqual(4.642, ef.CalculateOutputPrecision(4));     // 4.64158883361278		
            Assert.AreEqual(5.386, ef.CalculateOutputPrecision(6));     // 5.3860867250797106
            Assert.AreEqual(6.733, ef.CalculateOutputPrecision(9));     // 6.7326084063496383
        }


        [TestMethod]
        public void NegativeXinputTest()
        {
            Exponential2PointFunction ef = new Exponential2PointFunction((5, 5), (2, 4));
            Assert.AreEqual(2.971, ef.CalculateOutputPrecision(-2));    // 2.9707163943
            Assert.AreEqual(0.904, ef.CalculateOutputPrecision(-18));   // 0.90389852377
            Assert.AreEqual(0.002, ef.CalculateOutputPrecision(-100));  // 0.002031512077
        }


        [TestMethod]
        public void Exponential2PointNoiseAddedTest()
        {
            // a = 0.5, b = 1.3493 => f(x) = 0.5 * 1.3493^x
            Exponential2PointFunction ef = new Exponential2PointFunction((10, 10), (20, 200), 0, -1, 1);
            double value = ef.CalculateOutputPrecision(10);
            Assert.IsTrue(value > 9 && value < 11);

            for (int i = 0; i < 10; i++)
            {
                value = ef.CalculateOutputPrecision(12);
                Assert.IsTrue(value > 17.21d && value < 19.21d);
            }

            for (int i = 0; i < 10; i++)
            {
                value = ef.CalculateOutputPrecision(15);
                Assert.IsTrue(value > 43.72d && value < 45.72d);
            }
        }

    }
}
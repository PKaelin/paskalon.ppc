using paskalON.Maths.Calculuses.Exponents;

namespace paskalON.Maths.IntegrationTest.Calculuses.Exponents
{
    [TestClass]
    public class Exponential2PointFunctionTest
    {

        [TestMethod]
        public void ToBiggerTest()
        {
            Exponential2PointFunction ef = new Exponential2PointFunction((2, 4), (5, 5));
            Assert.AreEqual(4, ef.CalculateOutput(2));
            Assert.AreEqual(5, ef.CalculateOutput(5));

            ef = new Exponential2PointFunction((0, 5), (10, 500));
            Assert.AreEqual(5, ef.CalculateOutput(0));
            Assert.AreEqual(500, ef.CalculateOutput(10));

            ef = new Exponential2PointFunction((0, 0.001), (100, 1000));
            Assert.AreEqual(0.001, ef.CalculateOutput(0));
            Assert.AreEqual(1000, ef.CalculateOutput(100));
            Assert.AreEqual(1000000, ef.CalculateOutput(150));
        }


        [TestMethod]
        public void ToSmallerTest()
        {
            Exponential2PointFunction ef = new Exponential2PointFunction((0, 10), (10, 1));

            Assert.AreEqual(10, ef.CalculateOutput(0));
            double value = ef.CalculateOutput(3);
            Assert.IsTrue(value < 10 && value > 1);
            value = ef.CalculateOutput(8);
            Assert.IsTrue(value < 10 && value > 1);
            Assert.AreEqual(1, ef.CalculateOutput(10));
        }


        [TestMethod]
        public void Y1AndY2EqualTest()
        {
            Exponential2PointFunction ef = new Exponential2PointFunction((0, 10), (10, 10));
            Assert.AreEqual(10, ef.CalculateOutput(0));
            Assert.AreEqual(10, ef.CalculateOutput(5));
            Assert.AreEqual(10, ef.CalculateOutput(10));
        }


        [TestMethod]
        public void X1AndX2EqualTest()
        {
            Exponential2PointFunction ef = new Exponential2PointFunction((10, 10), (10, 20));
            Assert.ThrowsExactly<ArgumentException>(() => ef.CalculateOutput(0));
        }


        [TestMethod]
        public void ToAlmost0Test()
        {
            Exponential2PointFunction ef = new Exponential2PointFunction((20, 100), (30, 0.000001));

            Assert.IsGreaterThan(100, ef.CalculateOutput(0));
            Assert.AreEqual(100, ef.CalculateOutput(20));
            double value = ef.CalculateOutput(21);
            Assert.IsTrue(value > 0.001 && value < 100);
            value = ef.CalculateOutput(25);
            Assert.IsTrue(value > 0.001 && value < 100);
            Assert.AreEqual(0, ef.CalculateOutput(30));
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
            Assert.AreEqual(4, ef.CalculateOutput(2));         // 4
            Assert.AreEqual(4.642, ef.CalculateOutput(4));     // 4.64158883361278		
            Assert.AreEqual(5.386, ef.CalculateOutput(6));     // 5.3860867250797106
            Assert.AreEqual(6.733, ef.CalculateOutput(9));     // 6.7326084063496383

            // a = 0.5, b = 1.3493 => f(x) = 0.5 * 1.3493^x
            ef = new Exponential2PointFunction((10, 10), (20, 200), 0, 2);
            Assert.AreEqual(10d, ef.CalculateOutput(10));
            Assert.AreEqual(18.21d, ef.CalculateOutput(12));
            Assert.AreEqual(44.72d, ef.CalculateOutput(15));
        }


        [TestMethod]
        public void Exponential2PointOffsetTest()
        {
            // a = 3.4471, b = 1.0772 => f(x) = 3.4471 * 1.0772^x
            Exponential2PointFunction ef = new Exponential2PointFunction((2, 4), (5, 5), 10);
            Assert.AreEqual(14, ef.CalculateOutput(2));         // 4
            Assert.AreEqual(14.642, ef.CalculateOutput(4));     // 4.64158883361278		
            Assert.AreEqual(15.386, ef.CalculateOutput(6));     // 5.3860867250797106
            Assert.AreEqual(16.733, ef.CalculateOutput(9));     // 6.7326084063496383
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
            Assert.AreEqual(4, ef.CalculateOutput(2));         // 4
            Assert.AreEqual(4.642, ef.CalculateOutput(4));     // 4.64158883361278		
            Assert.AreEqual(5.386, ef.CalculateOutput(6));     // 5.3860867250797106
            Assert.AreEqual(6.733, ef.CalculateOutput(9));     // 6.7326084063496383
        }


        [TestMethod]
        public void NegativeXinputTest()
        {
            Exponential2PointFunction ef = new Exponential2PointFunction((5, 5), (2, 4));
            Assert.AreEqual(2.971, ef.CalculateOutput(-2));    // 2.9707163943
            Assert.AreEqual(0.904, ef.CalculateOutput(-18));   // 0.90389852377
            Assert.AreEqual(0.002, ef.CalculateOutput(-100));  // 0.002031512077
        }


        [TestMethod]
        public void Exponential2PointNoiseAddedTest()
        {
            // a = 0.5, b = 1.3493 => f(x) = 0.5 * 1.3493^x
            Exponential2PointFunction ef = new Exponential2PointFunction((10, 10), (20, 200), 0, -1, 1, 2);
            double value = ef.CalculateOutput(10);
            Assert.IsTrue(value > 9 && value < 11);

            for (int i = 0; i < 10; i++)
            {
                value = ef.CalculateOutput(12);
                Assert.IsTrue(value > 17.21d && value < 19.21d);
            }

            for (int i = 0; i < 10; i++)
            {
                value = ef.CalculateOutput(15);
                Assert.IsTrue(value > 43.72d && value < 45.72d);
            }
        }

    }
}
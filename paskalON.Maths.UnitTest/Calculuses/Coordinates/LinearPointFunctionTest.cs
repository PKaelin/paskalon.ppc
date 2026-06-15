using paskalON.Maths.Calculuses.Coordinates;

namespace paskalON.Maths.UnitTest.Calculuses.Coordinates
{
    [TestClass]
    public class LinearPointFunctionTest
    {

        [TestMethod]
        public void NullConstructorTest()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            LinearPointFunction lf = new LinearPointFunction(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentException>(() => lf.CalculateOutput(10));
        }


        [TestMethod]
        public void OnlyOnePointDefinitionTest()
        {
            List<LinearPoint> points = new List<LinearPoint> { new LinearPoint(10, 100) };
            LinearPointFunction lf = new LinearPointFunction(points);
            Assert.AreEqual(0, lf.CalculateOutput(5));
            Assert.AreEqual(100, lf.CalculateOutput(15));
        }


        [TestMethod]
        public void XinputSamllerThanSmallestConfiguredTest()
        {
            List<LinearPoint> points = new List<LinearPoint> { new LinearPoint(10, 100), new LinearPoint(20, 200) };
            LinearPointFunction lf = new LinearPointFunction(points);
            Assert.AreEqual(0, lf.CalculateOutput(5));
        }


        [TestMethod]
        public void XinputBiggerThanBiggestConfiguredTest()
        {
            List<LinearPoint> points = new List<LinearPoint> { new LinearPoint(10, 100), new LinearPoint(20, 200) };
            LinearPointFunction lf = new LinearPointFunction(points);
            Assert.AreEqual(200, lf.CalculateOutput(800));
        }


        [TestMethod]
        public void XinputBiggerThanBiggestConfiguredUnsortedInputTest()
        {
            List<LinearPoint> points = new List<LinearPoint> { new LinearPoint(20, 200), new LinearPoint(10, 100) };
            LinearPointFunction lf = new LinearPointFunction(points);
            Assert.AreEqual(200, lf.CalculateOutput(800));
        }


        [TestMethod]
        public void XinputEqualToXdefenitionTest()
        {
            List<LinearPoint> points = new List<LinearPoint> { new LinearPoint(10, 100), new LinearPoint(20, 200), new LinearPoint(30, 300) };
            LinearPointFunction lf = new LinearPointFunction(points);
            Assert.AreEqual(100, lf.CalculateOutput(10));
            Assert.AreEqual(200, lf.CalculateOutput(20));
            Assert.AreEqual(300, lf.CalculateOutput(30));
        }


        [TestMethod]
        public void LinearInputWholePositiveNumbersTest()
        {
            List<LinearPoint> points = new List<LinearPoint> { new LinearPoint(10, 100), new LinearPoint(20, 200), new LinearPoint(30, 300), new LinearPoint(40, 400) };
            LinearPointFunction lf = new LinearPointFunction(points);
            Assert.AreEqual(110, lf.CalculateOutput(11));
            Assert.AreEqual(140, lf.CalculateOutput(14));
            Assert.AreEqual(210, lf.CalculateOutput(21));
            Assert.AreEqual(240, lf.CalculateOutput(24));
            Assert.AreEqual(310, lf.CalculateOutput(31));
            Assert.AreEqual(340, lf.CalculateOutput(34));
            Assert.AreEqual(400, lf.CalculateOutput(41));
        }


        [TestMethod]
        public void LinearInputWholePositiveNumbersOffsetTest()
        {
            List<LinearPoint> points = new List<LinearPoint> { new LinearPoint(10, 100), new LinearPoint(20, 200), new LinearPoint(30, 300), new LinearPoint(40, 400) };
            LinearPointFunction lf = new LinearPointFunction(points, 10);
            Assert.AreEqual(120, lf.CalculateOutput(11));
            Assert.AreEqual(150, lf.CalculateOutput(14));
            Assert.AreEqual(220, lf.CalculateOutput(21));
            Assert.AreEqual(250, lf.CalculateOutput(24));
            Assert.AreEqual(320, lf.CalculateOutput(31));
            Assert.AreEqual(350, lf.CalculateOutput(34));
            Assert.AreEqual(410, lf.CalculateOutput(41));
        }


        [TestMethod]
        public void LinearInputWholePositiveNegativeNumbersTest()
        {
            List<LinearPoint> points = new List<LinearPoint> { new LinearPoint(10, 100), new LinearPoint(20, -200), new LinearPoint(40, 400), new LinearPoint(60, -600) };
            LinearPointFunction lf = new LinearPointFunction(points);
            Assert.AreEqual(70, lf.CalculateOutput(11));       // 100 + (11 - 10) * ((-200 - 100) / (20 - 10)) = 70
            Assert.AreEqual(-20, lf.CalculateOutput(14));      // 100 + (14 - 10) * ((-200 - 100) / (20 - 10)) = -20
            Assert.AreEqual(-170, lf.CalculateOutput(21));     // -200 + (21 - 20) * ((400 - -200) / (40 - 20)) = -170
            Assert.AreEqual(-80, lf.CalculateOutput(24));      // -200 + (24 - 20) * ((400 - -200) / (40 - 20)) = -80
            Assert.AreEqual(130, lf.CalculateOutput(31));      // -200 + (31 - 20) * ((400 - -200) / (40 - 20)) = 130
            Assert.AreEqual(220, lf.CalculateOutput(34));      // -200 + (34 - 20) * ((400 - -200) / (40 - 20)) = 220
            Assert.AreEqual(350, lf.CalculateOutput(41));      // 400 + (41 - 40) * ((-600 - 400) / (60 - 40)) = 350
        }


        [TestMethod]
        public void LinearInputFloatingThroughZeroTest()
        {
            List<LinearPoint> points = new List<LinearPoint> { new LinearPoint(-10, -10), new LinearPoint(10, 10) };
            LinearPointFunction lf = new LinearPointFunction(points);
            Assert.AreEqual(-10d, lf.CalculateOutput(-10));
            Assert.AreEqual(0d, lf.CalculateOutput(0));
            Assert.AreEqual(10d, lf.CalculateOutput(10));
        }


        [TestMethod]
        public void LinearInputFloatingPositiveNumbersTest()
        {
            List<LinearPoint> points = new List<LinearPoint> { new LinearPoint(10, 10.25), new LinearPoint(20, 20.75) };
            LinearPointFunction lf = new LinearPointFunction(points);
            Assert.AreEqual(11.3d, lf.CalculateOutput(11));    // 10.25 + (11 - 10) * ((20.75 - 10.25) / (20 - 10)) = 11.3
            Assert.AreEqual(14.45d, lf.CalculateOutput(14));   // 10.25 + (14 - 10) * ((20.75 - 10.25) / (20 - 10)) = 14.45
        }


        [TestMethod]
        public void NegativeXinputNumberTest()
        {
            List<LinearPoint> points = new List<LinearPoint> { new LinearPoint(-10, 100), new LinearPoint(10, 200) };
            LinearPointFunction lf = new LinearPointFunction(points);
            Assert.AreEqual(150, lf.CalculateOutput(0));     // 100 + (0 - -10) * ((200 - 100) / (10 - -10)) = 150
        }


        [TestMethod]
        public void DigitsRoundTest()
        {
            List<LinearPoint> points = new List<LinearPoint> { new LinearPoint(10, 10.25), new LinearPoint(20, 20.75) };
            LinearPointFunction lf = new LinearPointFunction(points, 0, 2);
            Assert.AreEqual(11.04d, lf.CalculateOutput(10.75));    // 10.25 + (10.75 - 10) * ((20.75 - 10.25) / (20 - 10)) = 11.3
        }


        [TestMethod]
        public void TwoSameXTest()
        {
            List<LinearPoint> points = new List<LinearPoint> { new LinearPoint(10, 10), new LinearPoint(10, 20) };
            LinearPointFunction lf = new LinearPointFunction(points, 0, 2);
            Assert.AreEqual(0, lf.CalculateOutput(8));
            Assert.AreEqual(10, lf.CalculateOutput(10));
            Assert.AreEqual(20, lf.CalculateOutput(12));
        }


        [TestMethod]
        public void To0Test()
        {
            List<LinearPoint> points = new List<LinearPoint> { new LinearPoint(10, 100), new LinearPoint(20, 0) };
            LinearPointFunction lf = new LinearPointFunction(points);
            Assert.AreEqual(100, lf.CalculateOutput(10));
            Assert.AreEqual(0, lf.CalculateOutput(20));
            Assert.AreEqual(20, lf.CalculateOutput(18));
        }


        [TestMethod]
        public void NoiseAddedTest()
        {
            List<LinearPoint> points = new List<LinearPoint> { new LinearPoint(10, 100), new LinearPoint(20, 200), new LinearPoint(30, 300) };
            LinearPointFunction lf = new LinearPointFunction(points, 0, -1, 1);

            double value = lf.CalculateOutput(11);
            Assert.IsTrue(value >= 109 && value <= 111);

            value = lf.CalculateOutput(14);
            Assert.IsTrue(value >= 139 && value <= 141);

            value = lf.CalculateOutput(21);
            Assert.IsTrue(value >= 209 && value <= 211);

            value = lf.CalculateOutput(24);
            Assert.IsTrue(value >= 239 && value <= 241);

            value = lf.CalculateOutput(31);
            Assert.IsTrue(value >= 299 && value <= 301);

            value = lf.CalculateOutput(33);
            Assert.IsTrue(value >= 299 && value <= 301);
        }
    }
}
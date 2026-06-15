using paskalON.Maths.Calculuses.Curves;

namespace paskalON.Maths.UnitTest.Calculuses.Curves
{
    [TestClass]
    public class PiecewiseFunctionTest
    {

        [TestMethod]
        public void EmptyListTest()
        {
            List<PiecewisePoint> points = new List<PiecewisePoint>();
            PiecewiseFunction pf = new PiecewiseFunction(points);
            Assert.ThrowsExactly<ArgumentException>(() => pf.CalculateOutput(0));
        }


        [TestMethod]
        public void LinearBelowFirstDataPointTest()
        {
            List<PiecewisePoint> points = new List<PiecewisePoint>();
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 10, 10));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 20, 40));
            PiecewiseFunction pf = new PiecewiseFunction(points);
            Assert.AreEqual(0d, pf.CalculateOutput(-10));
            Assert.AreEqual(0d, pf.CalculateOutput(-5));
            Assert.AreEqual(0, pf.CalculateOutput(0));
        }


        [TestMethod]
        public void LinearEqualsFirstDataPointTest()
        {
            List<PiecewisePoint> points = new List<PiecewisePoint>();
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 10, 10));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 20, 40));
            PiecewiseFunction pf = new PiecewiseFunction(points);
            Assert.AreEqual(10, pf.CalculateOutput(10));
        }


        [TestMethod]
        public void LinearAboveLastDataPointTest()
        {
            List<PiecewisePoint> points = new List<PiecewisePoint>();
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 10, 10));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 20, 40));
            PiecewiseFunction pf = new PiecewiseFunction(points);
            Assert.AreEqual(40d, pf.CalculateOutput(50));
            Assert.AreEqual(40d, pf.CalculateOutput(55));
            Assert.AreEqual(40d, pf.CalculateOutput(60));
        }


        [TestMethod]
        public void LinearPointsOnlyTest()
        {
            List<PiecewisePoint> points = new List<PiecewisePoint>();
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 0, 0));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 10, 10));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 20, 40));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 40, 80));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 50, 0));
            PiecewiseFunction pf = new PiecewiseFunction(points);

            Assert.AreEqual(0d, pf.CalculateOutput(0));
            Assert.AreEqual(5d, pf.CalculateOutput(5));
            Assert.AreEqual(25d, pf.CalculateOutput(15));
            Assert.AreEqual(60d, pf.CalculateOutput(30));
            Assert.AreEqual(40d, pf.CalculateOutput(45));
            Assert.AreEqual(0d, pf.CalculateOutput(50));
            Assert.AreEqual(0d, pf.CalculateOutput(51));
        }


        [TestMethod]
        public void ExponentialPointsOnlyTest()
        {
            List<PiecewisePoint> points = new List<PiecewisePoint>();
            points.Add(new PiecewisePoint(PiecewiseFunctionType.Exponential2PointFunction, 0, 1));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.Exponential2PointFunction, 10, 100));
            List<double> results = new List<double>();

            PiecewiseFunction pf = new PiecewiseFunction(points);

            for (int x = 0; x < 10; x++)
            {
                double output = pf.CalculateOutput(x);
                results.Add(output);
            }

            Assert.AreEqual(results.Count, results.Where((item, index) => (index == 0) || (index > 0 ? results[index - 1] <= item : false)).Count());
        }

    }
}

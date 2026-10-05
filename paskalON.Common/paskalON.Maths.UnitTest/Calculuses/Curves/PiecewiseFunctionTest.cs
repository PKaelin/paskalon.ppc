// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
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
            Assert.ThrowsExactly<ArgumentException>(() => new PiecewiseFunction(points));
        }


        [TestMethod]
        public void LinearBelowFirstDataPointTest()
        {
            List<PiecewisePoint> points = new List<PiecewisePoint>();
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 10, 10));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 20, 40));
            PiecewiseFunction pf = new PiecewiseFunction(points);
            Assert.AreEqual(10, pf.CalculateOutputPrecision(-10));
            Assert.AreEqual(10, pf.CalculateOutputPrecision(-5));
            Assert.AreEqual(10, pf.CalculateOutputPrecision(0));
        }


        [TestMethod]
        public void LinearEqualsFirstDataPointTest()
        {
            List<PiecewisePoint> points = new List<PiecewisePoint>();
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 10, 10));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 20, 40));
            PiecewiseFunction pf = new PiecewiseFunction(points);
            Assert.AreEqual(10, pf.CalculateOutputPrecision(10));
        }


        [TestMethod]
        public void LinearAboveLastDataPointTest()
        {
            List<PiecewisePoint> points = new List<PiecewisePoint>();
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 10, 10));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 20, 40));
            PiecewiseFunction pf = new PiecewiseFunction(points);
            Assert.AreEqual(40d, pf.CalculateOutputPrecision(50));
            Assert.AreEqual(40d, pf.CalculateOutputPrecision(55));
            Assert.AreEqual(40d, pf.CalculateOutputPrecision(60));
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

            Assert.AreEqual(0d, pf.CalculateOutputPrecision(0));
            Assert.AreEqual(5d, pf.CalculateOutputPrecision(5));
            Assert.AreEqual(25d, pf.CalculateOutputPrecision(15));
            Assert.AreEqual(60d, pf.CalculateOutputPrecision(30));
            Assert.AreEqual(40d, pf.CalculateOutputPrecision(45));
            Assert.AreEqual(0d, pf.CalculateOutputPrecision(50));
            Assert.AreEqual(0d, pf.CalculateOutputPrecision(51));
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
                double output = pf.CalculateOutputPrecision(x);
                results.Add(output);
            }

            Assert.AreEqual(results.Count, results.Where((item, index) => (index == 0) || (index > 0 ? results[index - 1] <= item : false)).Count());
        }


        [TestMethod]
        public void SingleExponentialPointTest()
        {
            List<PiecewisePoint> points = new List<PiecewisePoint>
            {
                new PiecewisePoint(PiecewiseFunctionType.Exponential2PointFunction, 10, 100)
            };
            PiecewiseFunction pf = new PiecewiseFunction(points, 5);

            Assert.AreEqual(105, pf.CalculateOutputPrecision(0));
            Assert.AreEqual(105, pf.CalculateOutputPrecision(10));
            Assert.AreEqual(105, pf.CalculateOutputPrecision(20));
        }


        [TestMethod]
        public void DuplicateXDefinitionTest()
        {
            List<PiecewisePoint> points = new List<PiecewisePoint>
            {
                new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 10, 100),
                new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 10, 200)
            };

            Assert.ThrowsExactly<ArgumentException>(() => new PiecewiseFunction(points));
        }

    }
}

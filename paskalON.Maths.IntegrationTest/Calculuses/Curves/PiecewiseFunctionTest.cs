using paskalON.Maths.Calculuses.Curves;

namespace paskalON.Maths.IntegrationTest.Calculuses.Curves
{
    [TestClass]
    public class PiecewiseFunctionTest
    {
        [TestMethod]
        public void PiecewiseFunctionLinearToFileTest()
        {
            string file = "PiecewiseFunctionLinearTest.csv";
            List<PiecewisePoint> points = new List<PiecewisePoint>();

            // Multiple linears
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 0, 0));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 10, 10));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 20, 15));
            PiecewiseFunction pf = new PiecewiseFunction(points);

            WriteToFile(file, pf, 0, 150);
            Assert.IsTrue(File.Exists(file));
        }


        [TestMethod]
        public void PiecewiseFunctionExponentialToFileTest()
        {
            string file = "PiecewiseFunctionExponentialTest.csv";
            List<PiecewisePoint> points = new List<PiecewisePoint>();

            // Exponential Growth Curve than linear (Use 1 for accaptable groth)
            points.Add(new PiecewisePoint(PiecewiseFunctionType.Exponential2PointFunction, 0, 1));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 100, 1000));
            PiecewiseFunction pf = new PiecewiseFunction(points);

            WriteToFile(file, pf, 0, 150);
            Assert.IsTrue(File.Exists(file));
        }


        [TestMethod]
        public void PiecewiseFunctionMixLinearExponentialToFileTest()
        {
            string file = "PiecewiseFunctionMixLinearExponentialToFileTest.csv";
            List<PiecewisePoint> points = new List<PiecewisePoint>();
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 10, 10));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 20, 100));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.Exponential2PointFunction, 30, 0));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.Exponential2PointFunction, 40, 400));
            points.Add(new PiecewisePoint(PiecewiseFunctionType.LinearPointFunction, 50, 100));
            PiecewiseFunction pf = new PiecewiseFunction(points);

            WriteToFile(file, pf, 0, 150);
            Assert.IsTrue(File.Exists(file));
        }


        private void WriteToFile(string fileName, PiecewiseFunction picewiseFunction, int xStart, int xEnd)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                for (int x = xStart; x < xEnd; x++)
                {
                    sw.WriteLine($"{x},{picewiseFunction.CalculateOutput(x)}");
                }
            }
        }


    }
}

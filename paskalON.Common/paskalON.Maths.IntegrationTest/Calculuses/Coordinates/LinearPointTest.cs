using paskalON.Maths.Calculuses.Coordinates;

namespace paskalON.Maths.IntegrationTest.Calculuses.Coordinates
{
    [TestClass]
    public class LinearPointTest
    {
        [TestMethod]
        public void LinearPointFunctionOnePointToFileTest()
        {
            string file = "LinearPointFunctionOnePointTest.csv";
            List<LinearPoint> onePoints = new List<LinearPoint> { new LinearPoint(10, 100) };
            LinearPointFunction lf1 = new LinearPointFunction(onePoints);
            WriteToFile(file, lf1, 0, 20);

            Assert.IsTrue(File.Exists(file));
        }


        [TestMethod]
        public void LinearPointFunctionTwoPointToFileTest()
        {
            string file = "LinearPointFunctionTwoPointTest.csv";
            List<LinearPoint> twoPoints = new List<LinearPoint> { new LinearPoint(0, 0), new LinearPoint(10, 10) };
            LinearPointFunction lf2 = new LinearPointFunction(twoPoints);
            WriteToFile(file, lf2, 0, 30);

            Assert.IsTrue(File.Exists(file));
        }


        [TestMethod]
        public void LinearPointFunctionTwoPointNoiseToFileTest()
        {
            string file = "LinearPointFunctionTwoPointNoiseTest.csv";
            List<LinearPoint> twoPointsNoise = new List<LinearPoint> { new LinearPoint(0, 0), new LinearPoint(100, 100) };
            LinearPointFunction lf2n = new LinearPointFunction(twoPointsNoise, 0, -3, 3);
            WriteToFile(file, lf2n, 0, 30);

            Assert.IsTrue(File.Exists(file));
        }


        private void WriteToFile(string fileName, LinearPointFunction linearPointFunction, int xStart, int xEnd)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                for (int x = xStart; x < xEnd; x++)
                {
                    sw.WriteLine($"{x},{linearPointFunction.CalculateOutputPrecision(x)}");
                }
            }
        }
    }
}

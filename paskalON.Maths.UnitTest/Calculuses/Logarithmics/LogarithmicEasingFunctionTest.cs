using paskalON.Maths.IntegrationTest.Calculuses.Logarithmics;

namespace paskalON.Maths.UnitTest.Calculuses.Logarithmics
{
    [TestClass]
    public class LogarithmicEasingFunctionTest
    {
        [TestMethod]
        public void LogarithmicEasingFunctionIncreaseTest()
        {
            LogarithmicEasingFunction lf = new LogarithmicEasingFunction(0, 100, 60, 0, 9, 0, 0, 0);
            List<double> results = new List<double>();

            for (int i = 0; i < 100; i++)
            {
                double output = lf.CalculateOutputPrecision(i);
                results.Add(output);
            }

            // Assert that the values increase
            Assert.AreEqual(results.Count(), results.Where((item, index) => index == 0 || ((index > 0) ? results[index - 1] <= item : false)).Count());
        }


        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(10, 79.588)]
        [DataRow(15, 102.377)]
        [DataRow(40, 169.02)]
        [DataRow(50, 185.884)]
        [DataRow(60, 200)]
        [DataRow(int.MaxValue, 200)]
        public void LogarithmicEasingFunctionValueUpTest(double x, double expectedValue)
        {
            LogarithmicEasingFunction lf = new LogarithmicEasingFunction(0, 200, 60, 0, 9, 0, 0, 0);
            List<double> results = new List<double>();
            double output = lf.CalculateOutputPrecision(x);

            Assert.AreEqual(expectedValue, output);
        }


        [TestMethod]
        [DataRow(0, 200)]
        [DataRow(10, 120.412)]
        [DataRow(15, 97.623)]
        [DataRow(40, 30.98)]
        [DataRow(50, 14.116)]
        [DataRow(60, 0)]
        [DataRow(int.MaxValue, 0)]
        public void LogarithmicEasingFunctionValueDownTest(double x, double expectedValue)
        {
            LogarithmicEasingFunction lf = new LogarithmicEasingFunction(200, 0, 60, 0, 9, 0, 0, 0);
            List<double> results = new List<double>();
            double output = lf.CalculateOutputPrecision(x);

            Assert.AreEqual(expectedValue, output);
        }
    }
}

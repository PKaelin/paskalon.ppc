using paskalON.Maths.Calculuses.Logarithmics;

namespace paskalON.Maths.UnitTest.Calculuses.Logarithmics
{
    [TestClass]
    public class LogarithmicScaledShiftedFunctionTest
    {

        [TestMethod]
        public void LogarithmicScaledShiftedFunctionIncreaseTest()
        {
            LogarithmicScaledShiftedFunction lf = new LogarithmicScaledShiftedFunction(100, 200, 0, 0, 0);
            List<double> results = new List<double>();

            for (int i = 1; i < 100; i++)
            {
                double output = lf.CalculateOutputPrecision(i);
                results.Add(output);
            }

            // Assert that the values increase
            Assert.AreEqual(results.Count(), results.Where((item, index) => index == 0 || ((index > 0) ? results[index - 1] <= item : false)).Count());
        }


        [TestMethod]
        [DataRow(1, 2)]
        [DataRow(1.4, 2.58)]
        [DataRow(2.1, 3.29)]
        [DataRow(3.46, 4.16)]
        [DataRow(7.2, 5.43)]
        [DataRow(8, 5.61)]
        public void LogarithmicFunctionScaledShiftedValueTest(double x, double expectedValue)
        {
            LogarithmicScaledShiftedFunction lf = new LogarithmicScaledShiftedFunction(2, 4, 0, 0, 0);
            List<double> results = new List<double>();
            double output = lf.CalculateOutputPrecision(x, 2);

            Assert.AreEqual(expectedValue, output);
        }


        [TestMethod]
        [DataRow(2, 802)]
        [DataRow(3, 2563)]
        [DataRow(5, 4782)]
        [DataRow(10, 7792)]
        [DataRow(20, 10802)]
        [DataRow(166.27, 20000)]
        public void LogarithmicScaledShiftedFunctionLearnNewWordsTest(double x, double expectedValue)
        {
            int monthsPerYear = 12;
            LogarithmicScaledShiftedFunction lf = new LogarithmicScaledShiftedFunction(-13000, 10000, 0, 0, 0);
            // Formula: words=–13,000 + 10,000 * log(months)
            // Number of words can be related to the child’s age in months
            Assert.AreEqual(expectedValue, lf.CalculateOutputPrecision(x * monthsPerYear), 2);
        }
    }
}

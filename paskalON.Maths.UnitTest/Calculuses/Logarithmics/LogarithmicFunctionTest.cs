using paskalON.Maths.Calculuses.Logarithmics;

namespace paskalON.Maths.UnitTest.Calculuses.Logarithmics
{
    [TestClass]
    public class LogarithmicFunctionTest
    {

        [TestMethod]
        public void LogarithmicFunctionSimple()
        {
            LogarithmicFunction lf = new LogarithmicFunction(100, 200, 0, 0, 0, 0);
            List<double> results = new List<double>();

            for (int i = 1; i < 100; i++)
            {
                double output = lf.CalculateOutputPrecision(i);
                results.Add(output);
            }

            Assert.IsGreaterThan(0, results.Count);
            Assert.AreEqual(100, results[0]);
            // Assert that the values increase
            Assert.AreEqual(results.Count(), results.Where((item, index) => index == 0 || ((index > 0) ? results[index - 1] <= item : false)).Count());
        }


        [TestMethod]
        public void LogarithmicFunctionLearnNewWordsTest()
        {
            int monthsPerYear = 12;
            LogarithmicFunction lf = new LogarithmicFunction(-13000, 10000, 0, 0, 0, 0);
            // Formula: words=–13,000 + 10,000 * log(months)
            // Number of words can be related to the child’s age in months
            Assert.AreEqual(802, lf.CalculateOutputPrecision(2 * monthsPerYear));
            Assert.AreEqual(2563, lf.CalculateOutputPrecision(3 * monthsPerYear));
            Assert.AreEqual(4782, lf.CalculateOutputPrecision(5 * monthsPerYear));
            Assert.AreEqual(7792, lf.CalculateOutputPrecision(10 * monthsPerYear));
            Assert.AreEqual(10802, lf.CalculateOutputPrecision(20 * monthsPerYear));
            Assert.AreEqual(20000, lf.CalculateOutputPrecision(166.27 * monthsPerYear));
        }
    }
}

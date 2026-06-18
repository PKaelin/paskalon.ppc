using paskalON.Maths.Calculuses.Logarithmics;

namespace paskalON.Maths.IntegrationTest.Calculuses.Logarithmics
{
    [TestClass]
    public class LogarithmicScaledShiftedFunctionTest
    {
        [TestMethod]
        public void LogarithmicScaledShiftedFunctionToFileTest()
        {
            LogarithmicScaledShiftedFunction lf = new LogarithmicScaledShiftedFunction(100, 200, 0, 0, 0);

            using (StreamWriter sw = new StreamWriter("LogarithmicScaledShiftedFunctionTest.csv"))
            {
                for (int i = 0; i < 100; i++)
                {
                    double output = lf.CalculateOutputPrecision(i);
                    sw.WriteLine($"{i}, {output}");
                }
            }
        }
    }
}

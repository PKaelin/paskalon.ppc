using paskalON.Maths.Calculuses.Logarithmics;

namespace paskalON.Maths.IntegrationTest.Calculuses.Logarithmics
{
    [TestClass]
    public class LogarithmicFunctionTest
    {
        [TestMethod]
        public void LogarithmicFunctionToFileTest()
        {
            LogarithmicFunction lf = new LogarithmicFunction(100, 200, 0, 0, 0, 0);

            using (StreamWriter sw = new StreamWriter("LogarithmicFunctionTest.csv"))
            {
                for (int i = 0; i < 100; i++)
                {
                    double output = lf.CalculateOutput(i);
                    sw.WriteLine($"{i}, {output}");
                }
            }
        }
    }
}

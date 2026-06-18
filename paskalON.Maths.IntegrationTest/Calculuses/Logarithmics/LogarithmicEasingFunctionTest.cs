namespace paskalON.Maths.IntegrationTest.Calculuses.Logarithmics
{
    [TestClass]
    public class LogarithmicEasingFunctionTest
    {
        [TestMethod]
        public void LogarithmicEasingFunctionUpToFileTest()
        {
            LogarithmicEasingFunction lf = new LogarithmicEasingFunction(0, 200, 60, 0, 9, 0, 0, 0);

            using (StreamWriter sw = new StreamWriter("LogarithmicEasingFunctionUpTest.csv"))
            {
                for (int i = 0; i < 100; i++)
                {
                    double output = lf.CalculateOutputPrecision(i);
                    sw.WriteLine($"{i}, {output}");
                }
            }
        }


        [TestMethod]
        public void LogarithmicEasingFunctionDownToFileTest()
        {
            LogarithmicEasingFunction lf = new LogarithmicEasingFunction(200, 0, 60, 0, 9, 0, 0, 0);

            using (StreamWriter sw = new StreamWriter("LogarithmicEasingFunctionDownTest.csv"))
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

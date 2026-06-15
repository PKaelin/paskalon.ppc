using paskalON.Maths.Calculuses.Exponents;

namespace paskalON.Maths.IntegrationTest.Calculuses.Exponents
{
    [TestClass]
    public class ExponentialFunctionTest
    {
        [TestMethod]
        public void ExponentialFunctionToFileTest()
        {
            ExponentialFunction ef = new ExponentialFunction(520, 2);

            using (StreamWriter sw = new StreamWriter("ExponentialFunctionTest.csv"))
            {
                for (int x = 0; x < 15; x++)
                {
                    sw.WriteLine($"{x},{ef.CalculateOutput(x)}");
                }
            }
        }
    }
}

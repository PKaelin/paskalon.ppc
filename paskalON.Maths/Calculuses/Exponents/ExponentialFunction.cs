using paskalON.Maths.Randoms;

namespace paskalON.Maths.Calculuses.Exponents
{
    /// <summary>
    /// An exponential function represents the relationship between an input and output, where we use repeated multiplication 
    /// on an initial value to get the output for any given input. 
    /// Exponential functions can grow or decay very quickly.
    /// </summary>
    public class ExponentialFunction
    {
        /// <summary>
        /// Used for random noise
        /// </summary>
        private Random _random = new Random();


        /// <summary>
        /// Coefficient, initial value, starting point, constant
        /// </summary>
        public double A { get; init; }


        /// <summary>
        /// Growth or decay factor
        /// </summary>
        public double B { get; init; }


        /// <summary>
        /// Adds an offset to f(x)
        /// </summary>
        public double Offset { get; set; } = 0;


        /// <summary>
        /// Precision to round the output to
        /// </summary>
        public int Precision { get; set; }


        /// <summary>
        /// Minimum of noise range that gets applied
        /// </summary>
        public double NoiseMin { get; set; } = 0;


        /// <summary>
        /// Miaximum of noise range that gets applied
        /// </summary>
        public double NoiseMax { get; set; } = 0;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="initialValue">Initial value (a) used in the calculation</param>
        /// <param name="factor">Factor (b) used in the calculation</param>
        /// <param name="offset">Offset to f(x)</param>
        /// <param name="digits">Digits to round the output to</param>
        public ExponentialFunction(double initialValue, double factor, double offset = 0, int digits = 3)
            : this(initialValue, factor, offset, 0, 0, digits)
        {
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="initialValue">Initial value (a) used in the calculation</param>
        /// <param name="factor">Factor (b) used in the calculation</param>
        /// <param name="offset">Offset to f(x)</param>
        /// <param name="noiseMin">Minimum of noise range that gets applied</param>
        /// <param name="noiseMax">Maximum of noise range that gets applied</param>
        /// <param name="digits">Digits to round the output to</param>
        public ExponentialFunction(double initialValue, double factor, double offset, double noiseMin, double noiseMax, int digits = 3)
        {
            if (factor < 0)
            {
                throw new ArgumentException("B must be bigger than 0. b>1=growth. 0<b<1=decay.");
            }

            A = initialValue;
            B = factor;
            Precision = digits;
            NoiseMin = noiseMin;
            NoiseMax = noiseMax;
            Offset = offset;
        }


        /// <summary>
        /// Calculates the output from the piecewise exponential curve
        /// </summary>
        /// <param name="x">X input</param>
        /// <returns>"f(x) Output (Y) from the calculated curve</returns>
        /// <remarks>
        /// Few possible basic formulas for exponential function:
        /// f(x) = b^x
        /// f(x) = ab^x (this methods implementation)
        /// f(x) = ab^kx
        /// f(x) = a(1 + r)^x
        /// 
        /// x: x-axis, time, is a variable
        /// a: coefficient, initial value, starting point, constant
        /// b: growth or decay factor. E.g. b=2 results in double (multiplying) when x is 1,2,3,4
        /// Exponential growth: If b > 1 then starting amount (a) is multiplied by a number greater than 1 and will increase as x increases
        /// In exponential growth, a quantity slowly increases in the beginning and then it increases rapidly.
        /// Exponential decay: If 0 < b < 1 then starting amount (a) is multiplied by a number between 0 and 1 and will decrease as x increases         
        /// In exponential decay, a quantity decreases very rapidly in the beginning, and then it decreases slowly.
        /// Analyse Y points in a curve to figure out the grow factor
        /// </remarks>
        public double CalculateOutput(double x)
        {
            if (B == 0)
            {
                return 0;
            }

            // Compute f(x) given X via exponential interpolation with a initial value and a factor            
            return Math.Round(A * Math.Pow(B, x) + Offset + _random.NextDoubleInRange(NoiseMin, NoiseMax), Precision);
        }
    }
}

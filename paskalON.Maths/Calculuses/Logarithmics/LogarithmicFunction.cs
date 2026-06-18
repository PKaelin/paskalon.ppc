using paskalON.Maths.Randoms;

namespace paskalON.Maths.Calculuses.Logarithmics
{
    /// <summary>
    /// Class representing a logarithmic function, which is defined by an initial value (A) and a growth or decay factor (B).
    /// </summary>
    public class LogarithmicFunction : ICalculateOutputFunction
    {
        /// <summary>
        /// Used for random noise.
        /// </summary>
        private Random _random = new Random();


        /// <summary>
        /// Coefficient, initial value, starting point, constant.
        /// </summary>
        public double A { get; init; }


        /// <summary>
        /// Growth or decay factor.
        /// </summary>
        public double B { get; init; }


        /// <summary>
        /// Adds an offset to f(x).
        /// </summary>
        public double Offset { get; set; } = 0;


        /// <summary>
        /// Precision to round the output to.
        /// </summary>
        public int Precision { get; set; }


        /// <summary>
        /// Minimum of noise range that gets applied.
        /// </summary>
        public double NoiseMin { get; set; } = 0;


        /// <summary>
        /// Maximum of noise range that gets applied.
        /// </summary>
        public double NoiseMax { get; set; } = 0;


        /// <summary>
        /// Constructor of <see cref="LogarithmicFunction"/>.
        /// </summary>
        /// <param name="initialValue">Initial value (a) used in the calculation.</param>
        /// <param name="factor">Factor (b) used in the calculation.</param>
        /// <param name="offset">Offset to f(x).</param>
        /// <param name="precision">Precision to round the output to.</param>
        public LogarithmicFunction(double initialValue, double factor, double offset = 0, int precision = 3) : this(initialValue, factor, offset, 0, 0, precision)
        {
        }


        /// <summary>
        /// Constructor of <see cref="LogarithmicFunction"/>
        /// </summary>
        /// <param name="initialValue">Initial value (a) used in the calculation.</param>
        /// <param name="factor">Factor (b) used in the calculation.</param>
        /// <param name="offset">Offset to f(x).</param>
        /// <param name="noiseMin">Minimum of noise range that gets applied.</param>
        /// <param name="noiseMax">Maximum of noise range that gets applied.</param>
        /// <param name="precision">Precision to round the output to.</param>
        public LogarithmicFunction(double initialValue, double factor, double offset, double noiseMin, double noiseMax, int precision = 3)
        {
            A = initialValue;
            B = factor;
            Precision = precision;
            NoiseMin = noiseMin;
            NoiseMax = noiseMax;
            Offset = offset;
        }


        /// <summary>
        /// Calculates the output from the logarithmic function.
        /// </summary>
        /// <param name="x">X input.</param>
        /// <returns>"f(x) Output (Y) from the calculated curve.</returns>
        /// <remarks>
        /// Formula:
        /// f(x) = A + B * log(t)
        /// </remarks>
        public double CalculateOutput(double x)
        {
            if (x == 0)
            {
                return A;
            }

            // Compute f(x) given X via logarithmic with a initial value and a factor
            if (NoiseMin == 0 && NoiseMax == 0)
            {
                return A + (B * Math.Log10(x)) + Offset;
            }

            return A + (B * Math.Log10(x)) + Offset + _random.NextDoubleInRange(NoiseMin, NoiseMax);
        }


        /// <summary>
        /// Calculates the output from the logarithmic function.
        /// </summary>
        /// <param name="x">X input.</param>
        /// <returns>"f(x) Output (Y) from the calculated curve.</returns>
        /// <remarks>
        /// Formula:
        /// f(x) = A + B * log(t)
        /// </remarks>
        public double CalculateOutputPrecision(double x)
        {
            return Math.Round(CalculateOutput(x), Precision);
        }


        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>String representation of this instance.</returns>
        public override string ToString()
        {
            return $"{nameof(LogarithmicFunction)} Offset: {Offset} Precision: {Precision} A: {A} B: {B}";
        }
    }
}

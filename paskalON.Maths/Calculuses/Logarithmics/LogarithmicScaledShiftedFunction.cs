using paskalON.Maths.Randoms;

namespace paskalON.Maths.Calculuses.Logarithmics
{
    /// <summary>
    /// Class representing a scaled and shifted base-10 logarithmic function,
    /// which is defined by an initial value (A) and a growth or decay factor (B).
    /// </summary>
    /// <remarks>
    /// While its rate of growth slows down significantly as A gets larger,
    /// it never reaches a horizontal asymptote and will continue to grow without bound.
    /// </remarks>
    public class LogarithmicScaledShiftedFunction : ICalculateOutputFunction
    {
        /// <summary>
        /// Used for random noise.
        /// </summary>
        private Random _random = new Random();


        /// <summary>
        /// Initial value, starting point.
        /// </summary>
        public double A { get; init; }


        /// <summary>
        /// Growth or decay factor.
        /// </summary>
        public double B { get; init; }


        /// <summary>
        /// Adds an offset to f(x).
        /// </summary>
        public double Offset { get; init; } = 0;


        /// <summary>
        /// Minimum of noise range that gets applied.
        /// </summary>
        public double NoiseMin { get; init; } = 0;


        /// <summary>
        /// Maximum of noise range that gets applied.
        /// </summary>
        public double NoiseMax { get; init; } = 0;


        /// <summary>
        /// Constructor of <see cref="LogarithmicScaledShiftedFunction"/>.
        /// </summary>
        /// <param name="initialValue">Initial value (a) used in the calculation.</param>
        /// <param name="factor">Factor (b) used in the calculation.</param>
        /// <param name="offset">Offset to f(x).</param>
        public LogarithmicScaledShiftedFunction(double initialValue, double factor, double offset = 0) : this(initialValue, factor, offset, 0, 0)
        {
        }


        /// <summary>
        /// Constructor of <see cref="LogarithmicScaledShiftedFunction"/>
        /// </summary>
        /// <param name="initialValue">Initial value (a) used in the calculation.</param>
        /// <param name="factor">Factor (b) used in the calculation.</param>
        /// <param name="offset">Offset to f(x).</param>
        /// <param name="noiseMin">Minimum of noise range that gets applied.</param>
        /// <param name="noiseMax">Maximum of noise range that gets applied.</param>
        public LogarithmicScaledShiftedFunction(double initialValue, double factor, double offset, double noiseMin, double noiseMax)
        {
            A = initialValue;
            B = factor;
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
        /// <param name="precision">Precision of the returned value.</param>
        /// <returns>"f(x) Output (Y) from the calculated curve.</returns>
        public double CalculateOutputPrecision(double x, int precision = 3)
        {
            return Math.Round(CalculateOutput(x), precision);
        }


        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>String representation of this instance.</returns>
        public override string ToString()
        {
            return $"{nameof(LogarithmicScaledShiftedFunction)} Offset: {Offset} A: {A} B: {B}";
        }
    }
}

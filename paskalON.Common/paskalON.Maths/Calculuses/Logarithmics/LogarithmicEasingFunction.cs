using paskalON.Maths.Calculuses;
using paskalON.Maths.Randoms;

namespace paskalON.Maths.IntegrationTest.Calculuses.Logarithmics
{
    /// <summary>
    /// Class representing a normalized logarithmic function,
    /// which is defined by an initial value (Y0), a target (Yt), a tuning parameter (K) and a time to approach (T).
    /// </summary>
    /// <remarks>
    /// Reaches target exactly. Fast initially, slower near target. Single tuning parameter k.
    /// </remarks>
    public class LogarithmicEasingFunction : ICalculateOutputFunction
    {
        /// <summary>
        /// Used for random noise.
        /// </summary>
        private Random _random = new Random();


        /// <summary>
        /// Initial X.
        /// </summary>
        /// <example>
        /// When dealing with coordinates use:
        /// InitialX = Number, Period = Number.
        /// When dealing with time use:
        /// InitialX = Ticks, Period = Ticks.
        /// </example>
        public long InitialX { get; init; }


        /// <summary>
        /// Initial value, starting point.
        /// </summary>
        public double InitialValue { get; init; }


        /// <summary>
        /// Target value.
        /// </summary>
        public double TargetValue { get; init; }


        /// <summary>
        /// Period how long till target value should be reached.
        /// </summary>
        public long Period { get; init; }


        /// <summary>
        /// Tuning value (controls the shape).
        /// </summary>
        /// <example>
        /// 1 = almost linear
        /// 5 = moderate logarithmic curve
        /// 9 = strong logarithmic curve (default)
        /// </example>
        public int TuningValue { get; init; } = 9;


        /// <summary>
        /// Used for easing.
        /// </summary>
        public double Normalization { get => Math.Log10(1d + TuningValue); }


        /// <summary>
        /// Adds an offset.
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
        /// Constructor of <see cref="LogarithmicEasingFunction"/>
        /// </summary>
        /// <param name="initialValue">Initial value used in the calculation.</param>
        /// <param name="targetValue">Target value used in the calculation.</param>
        /// <param name="period">Period used in the calculation.</param>
        /// <param name="initialX">Initial X value used in the calculation.</param>
        /// <param name="offset">Offset to f(x).</param>
        public LogarithmicEasingFunction(double initialValue, double targetValue, long period, long initialX = 0, int tuningValue = 9, double offset = 0) :
            this(initialValue, targetValue, period, initialX, tuningValue, offset, 0, 0)
        {
        }


        /// <summary>
        /// Constructor of <see cref="LogarithmicEasingFunction"/>
        /// </summary>
        /// <param name="initialValue">Initial value used in the calculation.</param>
        /// <param name="targetValue">Target value used in the calculation.</param>
        /// <param name="period">Period used in the calculation.</param>
        /// <param name="initialX">Initial X value used in the calculation.</param>
        /// <param name="offset">Offset to f(x).</param>
        /// <param name="noiseMin">Minimum of noise range that gets applied.</param>
        /// <param name="noiseMax">Maximum of noise range that gets applied.</param>
        public LogarithmicEasingFunction(double initialValue, double targetValue, long period, long initialX, int tuningValue, double offset, double noiseMin, double noiseMax)
        {
            InitialX = initialX;
            InitialValue = initialValue;
            TargetValue = targetValue;
            TuningValue = tuningValue;
            Period = period;
            NoiseMin = noiseMin;
            NoiseMax = noiseMax;
            Offset = offset;
        }


        /// <summary>
        /// Calculates the output from the logarithmic function.
        /// </summary>
        /// <param name="x">X input.</param>
        /// <returns>"Output (Y) from the calculated curve.</returns>
        public double CalculateOutput(double x)
        {
            // Clamp elapsed to [0,1] using elapsed and period
            double elapsed = Math.Clamp(x - InitialX, 0d, Period);
            double progress = elapsed / Period;
            // Logarithmic easing
            double factor = Math.Log10(1.0 + TuningValue * progress) / Normalization;

            if (NoiseMin == 0 && NoiseMax == 0)
            {
                return (InitialValue + (TargetValue - InitialValue) * factor) + Offset;
            }

            return (InitialValue + (TargetValue - InitialValue) * factor + _random.NextDoubleInRange(NoiseMin, NoiseMax)) + Offset;
        }


        /// <summary>
        /// Calculates the output from the logarithmic function.
        /// </summary>
        /// <param name="x">X input.</param>
        /// <param name="precision">Precision of the returned value.</param>
        /// <returns>"Output (Y) from the calculated curve.</returns>
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
            return $"{nameof(LogarithmicEasingFunction)} Offset: {Offset} InitialValue: {InitialValue} TargetValue: {TargetValue} TuningValue: {TuningValue}";
        }
    }
}

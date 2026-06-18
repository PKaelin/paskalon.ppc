using paskalON.Maths.Randoms;


namespace paskalON.Maths.Calculuses.Coordinates
{
    /// <summary>
    /// Linear function from real numbers to real numbers is a function whose graph is a non-vertical line in the plane.
    /// The characteristic property of linear functions is that when the input variable is changed, 
    /// the change in the output is proportional to the change in the input.
    /// </summary>
    /// <remarks>
    ///       /
    /// [Y]  /
    ///     / 
    ///      [X]
    /// </remarks>
    public class LinearPointFunction : ICalculateOutputFunction
    {
        /// <summary>
        /// Used for random noise.
        /// </summary>
        private Random _random = new Random();

        /// <summary>
        /// Definition of linear points X and Y.
        /// </summary>
        public List<LinearPoint> LinearPoints { get; init; } = new List<LinearPoint>();


        /// <summary>
        /// Adds an offset to f(x).
        /// </summary>
        public double Offset { get; set; } = 0;


        /// <summary>
        /// Minimum of noise range that gets applied.
        /// </summary>
        public double NoiseMin { get; set; } = 0;


        /// <summary>
        /// Maximum of noise range that gets applied.
        /// </summary>
        public double NoiseMax { get; set; } = 0;


        /// <summary>
        /// Constructor of <see cref="LinearPointFunction"/>.
        /// </summary>
        /// <param name="linearPoints">Definition of linear points.</param>
        /// <param name="offset">Offset to f(x).</param>
        public LinearPointFunction(List<LinearPoint> linearPoints, double offset = 0) : this(linearPoints, offset, 0, 0)
        {
        }


        /// <summary>
        /// Constructor of <see cref="LinearPointFunction"/>.
        /// </summary>
        /// <param name="linearPoints">Definition of linear points.</param>
        /// <param name="offset">Offset to f(x).</param>        
        /// <param name="noiseMin">Minimum of noise range that gets applied.</param>
        /// <param name="noiseMax">Maximum of noise range that gets applied.</param>
        public LinearPointFunction(List<LinearPoint> linearPoints, double offset, double noiseMin, double noiseMax)
        {
            ArgumentNullException.ThrowIfNull(linearPoints);

            LinearPoints = linearPoints;
            // Ascend x points
            LinearPoints.Sort((l1, l2) => l1.X.CompareTo(l2.X));
            Offset = offset;
            NoiseMin = noiseMin;
            NoiseMax = noiseMax;
        }


        /// <summary>
        /// Calculates the output from the piecewise linear curve
        /// </summary>
        /// <param name="x">X input</param>
        /// <returns>"Y absolute output from the calculated curve</returns>
        /// <remarks>
        /// Linear equation from two points (x1, y1) and (x2, y2) in the slope-Intercept form
        /// Formula: y = ax + b
        /// Slope: a = (y2-y1) / (x2-x1)
        /// Intercept: b = y1 for absolute and b = y1 - a * x1 for relative
        /// </remarks>
        public double CalculateOutput(double x)
        {
            if (LinearPoints.Any())
            {
                for (int i = 0; i < LinearPoints.Count - 1; i++)
                {
                    LinearPoint point1 = LinearPoints[i];
                    LinearPoint point2 = LinearPoints[i + 1];

                    if (x < point1.X)
                    {
                        return 0;
                    }
                    else if (x == point1.X)
                    {
                        if (NoiseMin == 0 && NoiseMax == 0)
                        {
                            return point1.Y + Offset;
                        }

                        return point1.Y + Offset + _random.NextDoubleInRange(NoiseMin, NoiseMax);
                    }
                    else if (x == point2.X)
                    {
                        if (NoiseMin == 0 && NoiseMax == 0)
                        {
                            return point2.Y + Offset;
                        }

                        return point2.Y + Offset + _random.NextDoubleInRange(NoiseMin, NoiseMax);
                    }
                    else if (x < point2.X)
                    {
                        // Compute Y given X via linear interpolation in the slope-Intercept form                        
                        if (NoiseMin == 0 && NoiseMax == 0)
                        {
                            return (point2.Y - point1.Y) / (point2.X - point1.X) * (x - point1.X) + point1.Y + Offset;
                        }

                        return (point2.Y - point1.Y) / (point2.X - point1.X) * (x - point1.X) + point1.Y + Offset + _random.NextDoubleInRange(NoiseMin, NoiseMax);
                    }
                }

                if (LinearPoints.Count == 1)
                {
                    if (x < LinearPoints.First().X)
                    {
                        return Offset;
                    }

                    return LinearPoints.First().Y + Offset;
                }

                // If X is bigger than last X return the max Y
                return LinearPoints.Last().Y + Offset;
            }


            return Offset;
        }


        /// <summary>
        /// Calculates the output from the piecewise linear curve
        /// </summary>
        /// <param name="x">X input</param>
        /// <param name="precision">Precision of the returned value.</param>
        /// <returns>"Y absolute output from the calculated curve</returns>

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
            return $"{nameof(LinearPointFunction)} Offset: {Offset} Points: {string.Join(',', LinearPoints.Select(p => p.ToString()))}";
        }
    }
}

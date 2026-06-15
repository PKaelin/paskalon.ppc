using paskalON.Maths.Calculuses.Curves;
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
    public class LinearPointFunction : IPiecewiseFunction
    {
        /// <summary>
        /// Used for random noise
        /// </summary>
        private Random _random = new Random();

        /// <summary>
        /// Definition of linear points X and Y
        /// </summary>
        public List<LinearPoint> LinearPoints { get; init; } = new List<LinearPoint>();


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
        /// Maximum of noise range that gets applied
        /// </summary>
        public double NoiseMax { get; set; } = 0;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="linearPoints">Definition of linear points</param>
        /// <param name="offset">Offset to f(x)</param>
        /// <param name="digits">Digits to round the output to</param>
        public LinearPointFunction(List<LinearPoint> linearPoints, double offset = 0, int digits = 3) : this(linearPoints, offset, 0, 0, digits)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="linearPoints">Definition of linear points</param>
        /// <param name="offset">Offset to f(x)</param>        
        /// <param name="noiseMin">Minimum of noise range that gets applied</param>
        /// <param name="noiseMax">Maximum of noise range that gets applied</param>
        /// <param name="digits">Digits to round the output to</param>
        public LinearPointFunction(List<LinearPoint> linearPoints, double offset, double noiseMin, double noiseMax, int digits = 3)
        {
            ArgumentNullException.ThrowIfNull(linearPoints);

            LinearPoints = linearPoints;
            // Ascend x points
            LinearPoints.Sort((l1, l2) => l1.X.CompareTo(l2.X));
            Offset = offset;
            NoiseMin = noiseMin;
            NoiseMax = noiseMax;
            Precision = digits;
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
                        return Math.Round(point1.Y + Offset + _random.NextDoubleInRange(NoiseMin, NoiseMax), Precision);
                    }
                    else if (x == point2.X)
                    {
                        return Math.Round(point2.Y + Offset + _random.NextDoubleInRange(NoiseMin, NoiseMax), Precision);
                    }
                    else if (x < point2.X)
                    {
                        // Compute Y given X via linear interpolation in the slope-Intercept form                        
                        return Math.Round((point2.Y - point1.Y) / (point2.X - point1.X) * (x - point1.X) + point1.Y + Offset + _random.NextDoubleInRange(NoiseMin, NoiseMax), Precision);
                    }
                }

                if (LinearPoints.Count == 1)
                {
                    if (x < LinearPoints.First().X)
                    {
                        return Offset;
                    }

                    return Math.Round(LinearPoints.First().Y + Offset, Precision);
                }

                // If X is bigger than last X return the max Y
                return Math.Round(LinearPoints.Last().Y + Offset + _random.NextDoubleInRange(NoiseMin, NoiseMax));
            }


            return Offset;
        }
    }
}

namespace paskalON.Maths.Calculuses.Curves
{
    /// <summary>
    /// Class representing a point of definition for a piecewise function, including the type of function to use, 
    /// the X and Y coordinates, and optional parameters for noise and rounding.
    /// </summary>
    public class PiecewisePoint
    {
        /// <summary>
        /// Type of the piecewise function
        /// </summary>
        public PiecewiseFunctionType Type { get; set; }

        /// <summary>
        /// X point of definition
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y point of definition
        /// </summary>
        public double Y { get; set; }


        /// <summary>
        /// Precision to round the output to
        /// </summary>
        public int Precision { get; set; } = 3;

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
        public PiecewisePoint() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type of the function to use</param>
        /// <param name="x">X point definition</param>
        /// <param name="y">Y point definition</param>
        public PiecewisePoint(PiecewiseFunctionType type, double x, double y)
        {
            Type = type;
            X = x;
            Y = y;
        }
    }
}

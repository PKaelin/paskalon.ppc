namespace paskalON.Maths.Calculuses.Coordinates
{
    /// <summary>
    /// Class representing a point of definition for a linear function, including the X and Y coordinates.
    /// </summary>
    public class LinearPoint
    {
        /// <summary>
        /// X point of a linear definition
        /// </summary>
        public double X { get; set; }


        /// <summary>
        /// Y point of a linear definition
        /// </summary>
        public double Y { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">X point definition</param>
        /// <param name="y">Y point definition</param>
        public LinearPoint(double x, double y)
        {
            X = x;
            Y = y;
        }


        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>String representation of this instance.</returns>
        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}

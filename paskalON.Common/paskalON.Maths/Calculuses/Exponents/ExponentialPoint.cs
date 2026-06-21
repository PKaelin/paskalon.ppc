namespace paskalON.Maths.Calculuses.Exponents
{
    /// <summary>
    /// Class representing a point of definition for an exponential function, including the X and Y coordinates,
    /// as well as the parameters A, B, C, D, and K of the exponential curve.
    /// </summary>
    public class ExponentialPoint
    {
        /// <summary>
        /// X point of definition for the exponential function.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y point of definition for the exponential function.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// A Value of the exponential curve, which determines the vertical stretch or compression of the curve.
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// B Value of the exponential curve, which determines the horizontal stretch or compression of the curve.
        /// </summary>
        public double B { get; set; }

        /// <summary>
        /// C Value of the exponential curve, which determines the vertical shift of the curve.
        /// </summary>
        public double C { get; set; }

        /// <summary>
        /// D Value of the exponential curve, which determines the horizontal shift of the curve.
        /// </summary>
        public double D { get; set; }

        /// <summary>
        /// K Value of the exponential curve, which determines the growth rate of the curve.
        /// </summary>
        public double K { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X Value of the exponential point.</param>
        /// <param name="y">Y Value of the exponential point.</param>
        /// <param name="aValue">A Value of the exponential curve, which determines the vertical stretch or compression of the curve.</param>
        /// <param name="bvalue">B Value of the exponential curve, which determines the horizontal stretch or compression of the curve.</param>
        /// <param name="cvalue">C Value of the exponential curve, which determines the vertical shift of the curve.</param>
        /// <param name="dvalue">D Value of the exponential curve, which determines the horizontal shift of the curve.</param>
        /// <param name="kvalue">K Value of the exponential curve, which determines the growth rate of the curve.</param>
        public ExponentialPoint(double x, double y, double aValue, double bvalue, double cvalue, double dvalue, double kvalue)
        {
            X = x;
            Y = y;
            A = aValue;
            B = bvalue;
            C = cvalue;
            D = dvalue;
            K = kvalue;
        }
    }
}

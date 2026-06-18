namespace paskalON.Maths.Calculuses
{
    /// <summary>
    /// Interface for piecewise functions.
    /// </summary>
    public interface ICalculateOutputFunction
    {
        /// <summary>
        /// Calculates the output of the piecewise function for a given input x.
        /// </summary>
        /// <param name="x">The input value for which to calculate the output.</param>
        /// <returns>The output value of the piecewise function.</returns>
        double CalculateOutput(double x);


        /// <summary>
        /// Calculates the output to a precision of the piecewise function for a given input x.
        /// </summary>
        /// <param name="x">The input value for which to calculate the output.</param>
        /// <param name="precision">Precision of the returned value.</param>
        /// <returns>The output value of the piecewise function.</returns>
        double CalculateOutputPrecision(double x, int precision = 3);
    }
}

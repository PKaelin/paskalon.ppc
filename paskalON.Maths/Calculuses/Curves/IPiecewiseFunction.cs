namespace paskalON.Maths.Calculuses.Curves
{
    /// <summary>
    /// Interface for piecewise functions.
    /// </summary>
    public interface IPiecewiseFunction
    {
        /// <summary>
        /// Calculates the output of the piecewise function for a given input x.
        /// </summary>
        /// <param name="x">The input value for which to calculate the output.</param>
        /// <returns>The output value of the piecewise function.</returns>
        double CalculateOutput(double x);
    }
}

namespace paskalON.OperatingModes.Domain.Curves
{
    /// <summary>
    /// A Volt-VAR (reactive power) curve in a power plant controller automatically increase/decrease reactive power output when grid voltage rises too low/high. 
    /// </summary>
    /// <remarks>
    /// When grid voltage rises above a certain threshold, the system will absorb (decrease output) reactive power to pull the voltage down. 
    /// If voltage is too low, it will inject (increase output) reactive power to boost the voltage.    
    /// </remarks>
    /// <example>
    /// Example of a Volt-VAR Curve connected to a 480 Volt AC grid.
    /// Point 1 (Over-Voltage Absorption) X-axis: 528 V (10% over nominal voltage) Y-axis: -100.0% (Maximum Absorption)
    /// Point 2 (Deadband - Upper Limit) X-axis: 504 V (5% over nominal voltage) Y-axis: 0.0% (No Action)
    /// Point 3 (Deadband - Lower Limit) X-axis: 456 V (5% under nominal voltage) Y-axis: 0.0% (No Action)
    /// Point 4 (Under-Voltage Injection) X-axis: 432 V (10% under nominal voltage) Y-axis: 100.0% (Maximum Injection)
    /// </example>
    public class VoltVarCurveConfig : CurveBaseConfig
    {
        /// <summary>
        /// X-Points represent the measured system or grid voltage at the point of common coupling.
        /// </summary>
        public override CurveUnit XUnit { get; init; } = CurveUnit.Voltage;


        /// <summary>
        /// Y-Points represent the maximum allowed reactive power output.
        /// </summary>
        /// <remarks>
        /// Percent of the power plant's maximum rated power (%) or reactive power (VAR/kVAR/MVAR).
        /// Positive + values represent VAR injection (to raise voltage), while negative - values represent VAR absorption (to lower voltage).
        /// </remarks>
        public override CurveUnit YUnit { get; init; } = CurveUnit.Percentage;


        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>String representation of this instance.</returns>
        public override string ToString()
        {
            return $"{nameof(VoltVarCurveConfig)} Name: {Name} XUnit: {XUnit} YUnit: {YUnit} Points: {Points.Select(p => p.ToString())}";
        }
    }
}

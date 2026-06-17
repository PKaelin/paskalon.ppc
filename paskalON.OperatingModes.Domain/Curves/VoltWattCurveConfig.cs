namespace paskalON.OperatingModes.Domain.Curves
{
    /// <summary>
    /// A Volt-Watt (active power) curve in a power plant controller automatically curtails (reduces) active power output when grid voltage rises too high. 
    /// </summary>
    /// <remarks>
    /// When grid voltage rises above a certain threshold, the system actively curtails or decreases active power to stop the voltage from climbing further. 
    /// By removing active power generation from the circuit, it relieves the strain on the grid.
    /// </remarks>
    /// <example>
    /// Example of a Volt-Watt Curve connected to a 480 Volt AC grid.
    /// Point 1 (Normal Operation Start) X-axis: 456 V -> Y-axis: 100% Power.
    /// Point 2 (Start of Curtailment) X-axis: 504 V -> Y-axis: 100% Power (This creates a "deadband" between 456V and 504V where the plant runs at full blast).
    /// Point 3 (Maximum Curtailment) X-axis: 528 V -> Y-axis: 20% Power (The plant smoothly ramps down between Point 2 and Point 3).
    /// Point 4 (Emergency Shut-off) X-axis: 532 V -> Y-axis: 0% Power (If voltage keeps rising past Point 3, the plant quickly drops to zero).
    /// </example>
    public class VoltWattCurveConfig : CurveBaseConfig
    {
        /// <summary>
        /// X-Points represent the measured system or grid voltage at the point of common coupling.
        /// </summary>
        public override CurveUnit XUnit { get; init; } = CurveUnit.Voltage;


        /// <summary>
        /// Y-Points represent the maximum allowed active power output.
        /// </summary>
        /// <remarks>
        /// Percent of the power plant's maximum rated power (%) or active power (W/kW/MW).
        /// </remarks>
        public override CurveUnit YUnit { get; init; } = CurveUnit.Percentage;


        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>String representation of this instance.</returns>
        public override string ToString()
        {
            return $"{nameof(VoltWattCurveConfig)} Name: {Name} XUnit: {XUnit} YUnit: {YUnit} Points: {Points.Select(p => p.ToString())}";
        }
    }
}

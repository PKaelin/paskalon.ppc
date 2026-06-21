namespace paskalON.OperatingModes.Domain.Curves
{
    /// <summary>
    /// A Frequency Watt curve in a power plant controller automatically increase/decrease active power output when grid frequency is too low/high. 
    /// </summary>
    /// <remarks>
    /// When the grid frequency changes because of an imbalance between power supply and demand, the PPC changes the active power output to help fix it.
    /// Deadband: Range around the normal center (like 60 Hz). Inside this zone, the plant does not change its power. This stops the plant from constantly reacting to tiny ripples.
    /// Droop (Slope): The steepness of the curve outside of the deadband. It decides exactly how many megawatts (MW) the plant will add or cut for every fraction of a hertz the grid changes.
    /// Curtailment (Over-frequency): When the grid frequency gets too high, it means there is too much power on the grid. The plant must quickly lower its power output.
    /// </remarks>
    /// <example>
    /// Point 1 (Under-Frequency Limit) X-axis: 59.50 Hz -> Y-axis: 120.0% (limited by the systems nameplate)
    /// Point 2 (Deadband - Lower Limit) X-axis: 59.95 Hz -> Y-axis: 100.0%
    /// Point 3 (Deadband - Upper Limit) X-axis: 60.05 Hz -> Y-axis: 100.0%
    /// Point 4 (Over-Frequency Curtailment) X-axis: 60.50 Hz -> Y-axis: 40.0%
    /// </example>
    public class FrequencyWattCurveConfig : CurveBaseConfig
    {
        /// <summary>
        /// X-Points represent the measured system or grid frequency at the point of common coupling.
        /// </summary>
        public override CurveUnit XUnit { get; init; } = CurveUnit.Frequency;


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
            return $"{nameof(FrequencyWattCurveConfig)} Name: {Name} XUnit: {XUnit} YUnit: {YUnit} Points: {Points.Select(p => p.ToString())}";
        }
    }
}

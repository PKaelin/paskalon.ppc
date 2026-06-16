namespace paskalON.OperatingModes.Domain.Ramps
{
    /// <summary>
    /// Configuration for Rate-Based (Speed) ramp based on percentage.
    /// </summary>
    /// <remarks>
    /// Ramp rate is the speed of that transition and how much the value changes per unit of time.
    /// </remarks>
    public class RampRatePercentageConfig : RampBaseConfig
    {
        /// <summary>
        /// Ramp up rate percentage in seconds between current setpoint and target setpoint.
        /// </summary>
        /// <example>
        /// Current: 0, Target: 60, RampUpRatePercentPerSecond: 50 = Every second a new target 0s=0, 1s=30, 2s=45, 3s=52.5, 4s=56.25, etc. 
        /// </example>
        public double RampUpRatePercentPerSecond { get; set; }


        /// <summary>
        /// Ramp down rate percentage in seconds between current setpoint and target setpoint.
        /// </summary>
        /// <example>
        /// Current: 0, Target: 60, RampUpRatePercentPerSecond: 50 = Every second a new target 0s=60, 1s=30, 2s=15, 3s=7.5, 4s=3.75, 5s=1.875, etc. 
        /// </example>
        public double RampDownRatePercentPerSecond { get; set; }


        /// <summary>
        /// Ramp rate percision for rounding ramp up and ramp down rate.
        /// </summary>
        /// <example>
        /// Ramp rates in percent are very flat at the end and it is therefore recommended to cut the ramp off at some point.
        /// Current: 0, Target: 60, RampUpRatePercentPerSecond: 50 = After 5s=1.875, 10s=0.014648 hence a precision of 1 would reach target after 6s.
        /// </example>
        public int RampRatePercentPercision { get; set; } = 1;
    }
}

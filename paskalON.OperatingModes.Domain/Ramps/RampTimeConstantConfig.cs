namespace paskalON.OperatingModes.Domain.Ramps
{
    /// <summary>
    /// Configuration for Rate-Based (Speed) ramp in a logarithmic ramp.
    /// </summary>
    public class RampTimeConstantConfig : RampBaseConfig
    {
        /// <summary>
        /// Ramp up time in seconds between current setpoint and target setpoint.
        /// </summary>
        /// <example>
        /// Current: 0, Target: 60, RampUpTimeConstantSeconds: 30 = Ramp reaches target of 60 in 30 seconds using logarithmic ramp.
        /// </example>
        public int RampUpTimeConstantSeconds { get; set; }


        /// <summary>
        /// Ramp down time in seconds between current setpoint and target setpoint.
        /// </summary>
        /// <example>
        /// Current: 60, Target: 0, RampDownTimeConstantSeconds: 30 = Ramp reaches target of 0 in 30 seconds using logarithmic ramp.
        /// </example>
        public int RampDownTimeConstantSeconds { get; set; }


        /// <summary>
        /// Ramp rate percision for rounding ramp up and ramp down constant.
        /// </summary>
        /// <example>
        /// Ramp rates in logarithmic ramps are very flat at the end and it is therefore recommended to cut the ramp off at some point.
        /// </example>
        public int RampTimeConstantPercision { get; set; } = 1;

    }
}

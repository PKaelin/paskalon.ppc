namespace paskalON.OperatingModes.Domain.Ramps
{
    /// <summary>
    /// Configuration for Time-Based (Duration) ramp.
    /// </summary>
    public class RampTimeConfig : RampBaseConfig
    {

        /// <summary>
        /// Ramp up time in seconds between current setpoint and target setpoint.
        /// </summary>
        /// <example>
        /// Current: 0, Target: 60, RampUpTimeSeconds: 30 = Ramp reaches target of 60 in 30 seconds.
        /// </example>
        public int RampUpTimeSeconds { get; set; }


        /// <summary>
        /// Ramp down time in seconds between current setpoint and target setpoint.
        /// </summary>
        /// <example>
        /// Current: 60, Target: 0, RampDownTimeSeconds: 30 = Ramp reaches target of 0 in 30 seconds.
        /// </example>
        public int RampDownTimeSeconds { get; set; }

    }
}

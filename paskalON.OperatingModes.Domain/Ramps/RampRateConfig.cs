namespace paskalON.OperatingModes.Domain.Ramps
{
    /// <summary>
    /// Configuration for Rate-Based (Speed) ramp based on value.
    /// </summary>
    /// <remarks>
    /// Ramp rate is the speed of that transition and how much the value changes per unit of time.
    /// </remarks>
    public class RampRateConfig : RampBaseConfig
    {
        /// <summary>
        /// Ramp up rate percentage in seconds between current setpoint and target setpoint.
        /// </summary>
        /// <example>
        /// Current: 0, Target: 60, RampUpRatePerSecond: 10 = Every second a new target 0s=0, 1s=10, 2s=20, 3s=30, 4s=40, etc. 
        /// </example>
        public double RampUpRatePerSecond
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, 0); field = value; }
        }

        /// <summary>
        /// Ramp down rate percentage in seconds between current setpoint and target setpoint.
        /// </summary>
        /// <example>
        /// Current: 0, Target: 60, RampDownRatePerSecond: 10 = Every second a new target 0s=60, 1s=50, 2s=40, 3s=30, 4s=20, etc. 
        /// </example>
        public double RampDownRatePerSecond
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, 0); field = value; }
        }
    }
}

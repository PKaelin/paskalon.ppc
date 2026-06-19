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
        /// RampUpRatePercentPerSecond: 50 = 50%
        /// Current: 0, Target: 60, RampUpRatePercentPerSecond: 50 = Every second a new target 0s=0, 1s=30, 2s=45, 3s=67.5 -> set to 60.
        /// </example>
        public double RampUpRatePercentPerSecond
        {
            get { return field; }
            set
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 100);
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);
                field = value;
            }
        }


        /// <summary>
        /// Ramp down rate percentage in seconds between current setpoint and target setpoint.
        /// </summary>
        /// <example>
        /// RampDownRatePercentPerSecond: 50 = 50%
        /// Current: 0, Target: 60, RampUpRatePercentPerSecond: 50 = Every second a new target 0s=60, 1s=30, 2s=15, 3s=7.5, 4s=3.75, 5s=1.875, etc. 
        /// </example>
        public double RampDownRatePercentPerSecond
        {
            get { return field; }
            set
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 100);
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);
                field = value;
            }
        }


        /// <summary>
        /// Ramp rates in percent go very flat at the end.
        /// The precisions is used to cut off ramping at some point.
        /// </summary>
        public int RampRatePrecision
        {
            get { return field; }
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);
                field = value;
            }
        }
    }
}
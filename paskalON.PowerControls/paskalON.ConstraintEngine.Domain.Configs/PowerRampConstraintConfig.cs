// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ConstraintEngine.Domain.Configs
{
    public class PowerRampConstraintConfig : ConstraintBaseConfig
    {
        /// <summary>
        /// Maximum ramp rate allowed by the constraint in active power per second.
        /// </summary>
        public double MaximumActivePowerWattRampRatePerSecond
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                field = value;
            }
        }


        /// <summary>
        /// Maximum ramp up rate allowed by the constraint in reactive power per second.
        /// </summary>
        public double MaximumReactivePowerVarsRampRatePerSecond
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                field = value;
            }
        }


        /// <summary>
        /// Maximum time, in milliseconds, since the last applied target that is used to calculate the allowed ramp step.
        /// </summary>
        /// <remarks>
        /// The allowed ramp step is the ramp rate multiplied with the time since the last applied target. Without this maximum a long
        /// gap between two setpoints, e.g. while the operating mode holds its setpoints, would allow a step of any size.
        /// Must be longer than the setpoint republish interval of the operating mode.
        /// </remarks>
        public int MaximumRampIntervalMilliseconds
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
                field = value;
            }
        } = 2000;
    }
}

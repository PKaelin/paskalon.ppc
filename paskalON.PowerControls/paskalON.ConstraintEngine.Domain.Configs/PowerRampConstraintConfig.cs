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
        public double MaximumActivePowerKiloWattRampRatePerSecond
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
        public double MaximumReactivePowerKiloVarsRampRatePerSecond
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                field = value;
            }
        }
    }
}

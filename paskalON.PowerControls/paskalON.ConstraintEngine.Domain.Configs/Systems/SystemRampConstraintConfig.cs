// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.ConstraintEngine.Domain.Configs.Systems
{
    /// <summary>
    /// Configuration for system ramp constraints.
    /// </summary>
    public class SystemRampConstraintConfig : ConstraintBaseConfig
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

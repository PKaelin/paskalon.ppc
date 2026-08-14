// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.ConstraintEngine.Domain.Configs.Ders
{
    /// <summary>
    /// Configuration for DER unit ramp constraints.
    /// </summary>
    public class DerUnitRampConstraintConfig : ConstraintBaseConfig
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
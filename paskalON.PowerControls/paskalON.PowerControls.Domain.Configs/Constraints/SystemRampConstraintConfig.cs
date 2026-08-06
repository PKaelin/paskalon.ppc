// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.PowerControls.Domain.Configs.Constraints
{
    /// <summary>
    /// Configuration for system ramp constraints.
    /// </summary>
    public class SystemRampConstraintConfig : ConstraintBaseConfig
    {
        /// <summary>
        /// Maximum ramp up rate allowed by the constraint in active power per second.
        /// </summary>
        public ActivePower MaxActivePowerRampUpRatePerSecond { get; set; }


        /// <summary>
        /// Maximum ramp down rate allowed by the constraint in active power per second.
        /// </summary>
        public ActivePower MaxActivePowerRampDownRatePerSecond { get; set; }


        /// <summary>
        /// Maximum ramp up rate allowed by the constraint in reactive power per second.
        /// </summary>
        public ReactivePower MaxReactivePowerRampUpRatePerSecond { get; set; }


        /// <summary>
        /// Maximum ramp down rate allowed by the constraint in reactive power per second.
        /// </summary>
        public ReactivePower MaxReactivePowerRampDownRatePerSecond { get; set; }

    }
}

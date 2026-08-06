// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.PowerControls.Domain.Configs.Constraints
{
    /// <summary>
    /// Configuration for power constraints.
    /// </summary>
    public class PowerConstraintConfig : ConstraintBaseConfig
    {
        /// <summary>
        /// Maximum active power allowed by the constraint.
        /// </summary>
        public ActivePower? MaxActivePower { get; set; }


        /// <summary>
        /// Minimum active power allowed by the constraint.
        /// </summary>
        public ActivePower? MinActivePower { get; set; }


        /// <summary>
        /// Maximum reactive power allowed by the constraint.
        /// </summary>
        public ReactivePower? MaxReactivePower { get; set; }


        /// <summary>
        /// Minimum reactive power allowed by the constraint.
        /// </summary>
        public ReactivePower? MinReactivePower { get; set; }

    }
}

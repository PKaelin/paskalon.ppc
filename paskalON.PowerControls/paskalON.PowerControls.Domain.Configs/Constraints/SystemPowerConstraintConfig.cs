// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.PowerControls.Domain.Configs.Constraints
{
    /// <summary>
    /// Configuration for system power constraints.
    /// </summary>
    public class SystemPowerConstraintConfig : PowerConstraintConfig
    {
        /// <summary>
        /// Indicates whether the system should derate per unit stopped.
        /// </summary>
        public bool DeratePerUnitStopped { get; set; } = true;


        /// <summary>
        /// Indicates whether the system should derate per unit in maintenance.
        /// </summary>
        public bool DeratePerUnitInMaintenance { get; set; }

    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.PowerControls.Domain.Configs.Constraints
{
    /// <summary>
    /// Configuration for DER unit power constraints.
    /// </summary>
    public class DerUnitPowerConstraintConfig : PowerConstraintConfig
    {
        /// <summary>
        /// Corresponding DER unit name for which this power constraint is defined.
        /// </summary>
        public required string DerUnitName { get; set; }
    }
}

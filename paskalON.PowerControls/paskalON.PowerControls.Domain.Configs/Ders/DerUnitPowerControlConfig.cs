// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.ConstraintEngine.Domain.Configs.Ders;

namespace paskalON.PowerControls.Domain.Configs.Ders
{
    public class DerUnitPowerControlConfig : PowerControlBaseConfig
    {
        /// <summary>
        /// Corresponding DER unit name for which this power constraint is defined.
        /// </summary>
        public required string DerUnitName { get; set; }


        /// <summary>
        /// If priority distribution strategy is used then this priority is used.
        /// </summary>
        public int? Priority { get; set; }


        /// <summary>
        /// If weighted distribution strategy is used then this weight is used.
        /// </summary>
        public double? Weight { get; set; }


        /// <summary>
        /// Relationship to DerUnitPowerConstraintConfig Id
        /// </summary>
        public int DerUnitPowerConstraintConfigId { get; set; }


        /// <summary>
        /// Relationship to DerUnitPowerConstraintConfig Id
        /// </summary>
        public required DerUnitPowerConstraintConfig DerUnitPowerConstraintConfig { get; set; }
    }
}

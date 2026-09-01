// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.ConstraintEngine.Domain.Configs;
using paskalON.PowerControls.Domain.Configs.Strategies;

namespace paskalON.PowerControls.Domain.Configs.Ders
{
    public class DerUnitPowerEnergyStorageControlConfig : PowerControlBaseConfig
    {
        /// <summary>
        /// Corresponding DER unit name for which this power constraint is defined.
        /// </summary>
        public required string DerUnitName { get; set; }


        /// <summary>
        /// Distribution strategy type.
        /// </summary>
        public required DistributionStrategyType DistributionStrategyType { get; set; }


        /// <summary>
        /// Collection of constraints.
        /// </summary>
        public ICollection<ConstraintBaseConfig> Constraints { get; set; } = [];
    }
}

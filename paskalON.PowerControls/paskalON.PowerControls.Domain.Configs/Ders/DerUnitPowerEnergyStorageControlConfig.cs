// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.ConstraintEngine.Domain.Configs;
using paskalON.PowerControls.Domain.Configs.Strategies;

namespace paskalON.PowerControls.Domain.Configs.Ders
{
    public class DerUnitPowerEnergyStorageControlConfig : PowerControlBaseConfig
    {
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

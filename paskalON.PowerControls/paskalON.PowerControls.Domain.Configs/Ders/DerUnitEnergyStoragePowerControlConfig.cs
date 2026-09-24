// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PowerControls.Domain.Configs.Strategies;

namespace paskalON.PowerControls.Domain.Configs.Ders
{
    public class DerUnitEnergyStoragePowerControlConfig : PowerControlBaseConfig
    {
        /// <summary>
        /// Corresponding DER unit name for which this power control configuration is defined.
        /// </summary>
        public required string DerUnitName { get; set; }


        /// <summary>
        /// Distribution strategy type.
        /// </summary>
        public required DistributionStrategyType DistributionStrategyType { get; set; }
    }
}

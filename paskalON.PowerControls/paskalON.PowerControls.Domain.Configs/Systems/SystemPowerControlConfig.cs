// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.ConstraintEngine.Domain.Configs;

namespace paskalON.PowerControls.Domain.Configs.Systems
{
    public class SystemPowerControlConfig : PowerControlBaseConfig
    {
        /// <summary>
        /// Collection of constraints.
        /// </summary>
        public ICollection<ConstraintBaseConfig> Constraints { get; set; } = [];
    }
}

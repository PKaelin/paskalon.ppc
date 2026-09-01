// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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

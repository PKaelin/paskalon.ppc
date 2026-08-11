// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.ConstraintEngine.Domain.Configs.Systems;

namespace paskalON.PowerControls.Domain.Configs.Systems
{
    public class SystemPowerControlConfig : PowerControlBaseConfig
    {
        /// <summary>
        /// Relationship to SystemPowerConstraintConfig Id
        /// </summary>
        public int SystemPowerConstraintConfigId { get; set; }


        /// <summary>
        /// Relationship to SystemPowerConstraintConfig Id
        /// </summary>
        public required SystemPowerConstraintConfig SystemPowerConstraintConfig { get; set; }
    }
}

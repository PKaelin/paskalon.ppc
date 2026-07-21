// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.OperatingModes.Domain.Configs
{
    /// <summary>
    /// Operating mode custom configuration.
    /// </summary>
    /// <remarks>
    /// A Domain configuration might have a required attribute that is need but should not be included in the standard.
    /// Hence custom configuration domains shall extend the standard domain:  [Domain] 1----* [DomainCustom]
    /// Type safety is sacrificed for the sake of cleaner structure.
    /// </remarks>
    public class OperatingModeCustomConfig : ConfigurationBase
    {
        /// <summary>
        /// Parent relationship to OperatingModeBaseConfig Id.
        /// </summary>
        public int OperatingModeBaseConfigId { get; set; }


        /// <summary>
        /// Parent relationship to OperatingModeBaseConfig Id.
        /// </summary>
        public required OperatingModeBaseConfig OperatingModeConfig { get; set; } = null!;
    }
}

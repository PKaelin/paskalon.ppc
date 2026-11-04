// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.Devices.Domain.Configs.PowerConversionSystems
{
    /// <summary>
    /// Power Conversion SystemDevice Custom Configuration
    /// </summary>
    /// <remarks>
    /// A Domain configuration might have a required attribute that is need but should not be included in the standard.
    /// Hence custom configuration domains shall extend the standard domain:  [Domain] 1----* [DomainCustom]
    /// Type safety is sacrificed for the sake of cleaner structure.
    /// </remarks>
    public class PowerConversionSystemDeviceCustomConfig : ConfigurationBase
    {
        /// <summary>
        /// Parent relationship to PowerConversionSystemDeviceConfig Id.
        /// </summary>
        public int PowerConversionSystemDeviceConfigId { get; set; }

        /// <summary>
        /// Parent relationship to PowerConversionSystemDeviceConfig.
        /// </summary>
        public PowerConversionSystemDeviceConfig PowerConversionSystemDeviceConfig { get; set; } = null!;
    }
}

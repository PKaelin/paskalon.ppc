// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.Devices.Domain.Configs.EnergyStorages.Batteries
{
    /// <summary>
    /// Battery Bank Device Custom Configuration
    /// </summary>
    /// <remarks>
    /// A Domain configuration might have a required attribute that is need but should not be included in the standard.
    /// Hence custom configuration domains shall extend the standard domain:  [Domain] 1----* [DomainCustom]
    /// Type safety is sacrificed for the sake of cleaner structure.
    /// </remarks>
    public class BatteryBankDeviceCustomConfig : ConfigurationBase
    {
        /// <summary>
        /// Parent relationship to BatteryBankDeviceConfig Id.
        /// </summary>
        public int BatteryBankDeviceConfigId { get; set; }

        /// <summary>
        /// Parent relationship to BatteryBankDeviceConfig.
        /// </summary>
        public BatteryBankDeviceConfig BatteryBankDeviceConfig { get; set; } = null!;
    }
}

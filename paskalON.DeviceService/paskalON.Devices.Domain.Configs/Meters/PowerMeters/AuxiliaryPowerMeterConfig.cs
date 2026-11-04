// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Domain.Configs.Meters.PowerMeters
{
    public class AuxiliaryPowerMeterConfig : PowerMeterBaseConfig
    {
        /// <summary>
        /// Parent relationship to DerConfig Id.
        /// </summary>
        public int DerConfigId { get; set; }


        /// <summary>
        /// Parent relationship to DerConfig.
        /// </summary>
        public required DerConfig DerConfig { get; set; }
    }
}

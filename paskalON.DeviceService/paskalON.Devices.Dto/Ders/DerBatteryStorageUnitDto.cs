// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.EnergyStorages.Batteries;
using paskalON.Devices.Dto.PowerConversionSystems;

namespace paskalON.Devices.Dto.Ders
{
    /// <summary>
    /// Data Transfer Object for DER battery storage unit.
    /// </summary>
    public record DerBatteryStorageUnitDto : DerUnitDto
    {
        /// <summary>
        /// Power conversion system for this battery storage unit.
        /// </summary>
        public required PcsDto PowerConversionSystem { get; init; }


        /// <summary>
        /// One or many battery banks for this battery storage unit.
        /// </summary>
        public List<BbDto> BatteryBanks { get; init; } = new List<BbDto>();


        /// <summary>
        /// Include operations sent to parent or PCS in the BatteryStorageUnits.
        /// Default this to true; almost all BatteryStorageUnits will want to behave this way.
        /// </summary>
        public bool IncludeBatteryInOperations { get; init; }
    }
}

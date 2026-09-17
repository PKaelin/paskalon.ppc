// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.DeviceSimulator.Dto.EnergyStorages.Batteries;
using paskalON.DeviceSimulator.Dto.PowerConversionSystems;

namespace paskalON.DeviceSimulator.Dto.Ders
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
    }
}

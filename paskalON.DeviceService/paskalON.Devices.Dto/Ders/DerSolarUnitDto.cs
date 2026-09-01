// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.EnergyResources.Solars;
using paskalON.Devices.Dto.PowerConversionSystems;

namespace paskalON.Devices.Dto.Ders
{
    /// <summary>
    /// Data Transfer Object for DER solar unit.
    /// </summary>
    public record DerSolarUnitDto : DerUnitDto
    {
        /// <summary>
        /// Power conversion system for this battery storage unit.
        /// </summary>
        public required PcsDto PowerConversionSystem { get; init; }


        /// <summary>
        /// List of one or multiple solar panels.
        /// </summary>
        public List<PvDto> SolarPanels { get; init; } = new List<PvDto>();


        /// <summary>
        /// Number of solar panels.
        /// </summary>
        public int NumberOfPanels { get; init; }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Text.Json.Serialization;

namespace paskalON.Devices.Dto.Ders
{
    /// <summary>
    /// Data Transfer Object for DER unit.
    /// </summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "unitType")]
    [JsonDerivedType(typeof(DerBatteryStorageUnitDto), "battery")]
    [JsonDerivedType(typeof(DerSolarUnitDto), "solar")]
    public abstract record DerUnitDto
    {
        /// <summary>
        /// Name of the Distributed Energy Resource (DER) Data Transfer Object (DTO.
        /// </summary>
        public required string Name { get; set; }


        /// <summary>
        /// Flag whether this unit is in maintenance mode.
        /// </summary>
        /// <remarks>
        /// Single devices dont get set into maintenance mode. The unit does.
        /// </remarks>
        public bool IsInMaintenanceMode { get; init; }
    }
}

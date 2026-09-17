// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Dto.Ders
{
    /// <summary>
    /// Data Transfer Object for DER unit.
    /// </summary>
    public record DerUnitDto
    {
        /// <summary>
        /// Name of the Distributed Energy Resource (DER) Data Transfer Object (DTO.
        /// </summary>
        public required string Name { get; init; }


        /// <summary>
        /// Flag whether this unit is in maintenance mode.
        /// </summary>
        /// <remarks>
        /// Single devices dont get set into maintenance mode. The unit does.
        /// </remarks>
        public bool IsInMaintenanceMode { get; init; }
    }
}

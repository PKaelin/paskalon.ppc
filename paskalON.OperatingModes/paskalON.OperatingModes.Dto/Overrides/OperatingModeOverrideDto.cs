// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Dto.Overrides
{
    /// <summary>
    /// Data Transfer Object (DTO) representing an override operating mode.
    /// </summary>
    public record OperatingModeOverrideDto
    {
        /// <summary>
        /// Gets the name of the override operating mode.
        /// </summary>
        public required string OperatingModeName { get; init; }


        /// <summary>
        /// Gets the name of the DER unit the override operating mode targets.
        /// </summary>
        /// <remarks>
        /// Null for a plant-wide override operating mode, e.g. a shutdown.
        /// </remarks>
        public string? TargetDerUnit { get; init; }
    }
}

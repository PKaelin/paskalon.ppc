// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Dto.Stacks
{
    /// <summary>
    /// Data Transfer Object (DTO) representing an operating mode stack item.
    /// </summary>
    public record OperatingModeStackDto
    {
        /// <summary>
        /// Gets or sets the name of the operating mode.
        /// </summary>
        public required string OperatingModeName { get; init; }
    }
}

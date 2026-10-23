// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Dto.Stacks
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a limiting operating mode stack item.
    /// </summary>
    public record OperatingModeLimitingStackDto : OperatingModeStackDto
    {
        /// <summary>
        /// The limit of the operating mode.
        /// </summary>
        public double Limit { get; set; }
    }
}

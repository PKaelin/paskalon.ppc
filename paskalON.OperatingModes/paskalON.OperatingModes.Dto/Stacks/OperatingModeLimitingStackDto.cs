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
        /// The maximum limit of the operating mode.
        /// </summary>
        public double MaximumLimit { get; set; }


        /// <summary>
        /// The minimum limit of the operating mode or null if the operating mode has no minimum limit.
        /// </summary>
        public double? MinimumLimit { get; set; }
    }
}

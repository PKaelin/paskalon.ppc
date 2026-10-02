// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Dto.Stacks.ClosedModes.VoltageReactives
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a power factor mode.
    /// </summary>
    public record PowerFactorModeStackDto
    {
        /// <summary>
        /// Gets the power factor.
        /// </summary>
        public required double PowerFactor { get; init; }
    }
}

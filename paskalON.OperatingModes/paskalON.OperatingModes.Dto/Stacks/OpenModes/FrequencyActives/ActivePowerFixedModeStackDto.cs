// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Dto.Stacks.OpenModes.FrequencyActives
{
    /// <summary>
    /// Data Transfer Object (DTO) representing an active power fixed mode.
    /// </summary>
    public record ActivePowerFixedModeStackDto : OperatingModeStackDto
    {
        /// <summary>
        /// Gets the setpoint active power.
        /// </summary>
        public required ActivePower SetpointActivePower { get; init; }
    }
}

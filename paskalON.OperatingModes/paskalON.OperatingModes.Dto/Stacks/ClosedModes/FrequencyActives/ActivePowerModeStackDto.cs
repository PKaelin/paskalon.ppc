// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Dto.Stacks.ClosedModes.FrequencyActives
{
    /// <summary>
    /// Data Transfer Object (DTO) representing an active power mode.
    /// </summary>
    public record ActivePowerModeStackDto : OperatingModeStackDto
    {
        /// <summary>
        /// Gets the setpoint active power.
        /// </summary>
        public required ActivePower SetpointActivePower { get; init; }
    }
}

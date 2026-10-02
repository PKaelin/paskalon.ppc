// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Dto.Stacks.ClosedModes.VoltageReactives
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a reactive power mode.
    /// </summary>
    public record ReactivePowerModeStackDto : OperatingModeStackDto
    {
        /// <summary>
        /// Gets the setpoint reactive power.
        /// </summary>
        public required ReactivePower SetpointReactivePower { get; init; }
    }
}

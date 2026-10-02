// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Dto.Stacks.OpenModes.VoltageReactives
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a reactive power fixed mode.
    /// </summary>
    public record ReactivePowerFixedModeStackDto : OperatingModeStackDto
    {
        /// <summary>
        /// Gets the setpoint reactive power.
        /// </summary>
        public required ReactivePower SetpointReactivePower { get; init; }
    }
}

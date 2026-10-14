// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.Systems
{
    /// <summary>
    /// Forced setpoints of an override mode for the power control service.
    /// </summary>
    /// <param name="OperatingModeName">The name of the override mode.</param>
    /// <param name="DerUnitName">The overridden DER unit or null if the override applies to the whole plant.</param>
    /// <param name="ActivePower">The forced active power or null if the override mode does not control active power.</param>
    /// <param name="ReactivePower">The forced reactive power or null if the override mode does not control reactive power.</param>
    public sealed record UnitOverrideSetpoint(string OperatingModeName, string? DerUnitName, ActivePower? ActivePower, ReactivePower? ReactivePower);
}

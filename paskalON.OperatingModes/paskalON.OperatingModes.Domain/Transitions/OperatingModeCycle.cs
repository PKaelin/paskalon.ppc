// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Abstractions;

namespace paskalON.OperatingModes.Domain.Transitions
{
    /// <summary>
    /// Consistent snapshot of the committed operating modes and the changes since the last calculation cycle.
    /// </summary>
    /// <typeparam name="TMode">The operating mode type.</typeparam>
    /// <param name="OperatingModes">The committed operating modes in calculation order.</param>
    /// <param name="Changes">The committed changes since the last calculation cycle in commit order.</param>
    public sealed record OperatingModeCycle<TMode>(IReadOnlyList<TMode> OperatingModes, IReadOnlyList<OperatingModeChange<TMode>> Changes) where TMode : class, IOperatingMode;
}

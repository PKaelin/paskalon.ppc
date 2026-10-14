// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Abstractions;

namespace paskalON.OperatingModes.Domain.Transitions
{
    /// <summary>
    /// Committed change of the operating mode stack or the override slot that is applied by the next calculation cycle.
    /// </summary>
    /// <typeparam name="TMode">The operating mode type.</typeparam>
    /// <param name="Kind">The kind of change.</param>
    /// <param name="OperatingMode">The added, removed or replacing operating mode.</param>
    /// <param name="Predecessor">The replaced operating mode for a replacement otherwise null.</param>
    public sealed record OperatingModeChange<TMode>(OperatingModeChangeKind Kind, TMode OperatingMode, TMode? Predecessor = null) where TMode : class, IOperatingMode;
}

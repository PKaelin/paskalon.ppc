// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Abstractions;

namespace paskalON.OperatingModes.Domain.Stacks
{
    /// <summary>
    /// Operating mode of the stack with its priority.
    /// </summary>
    /// <param name="Priority">The priority of the operating mode in the stack.</param>
    /// <param name="OperatingMode">The operating mode.</param>
    public sealed record OperatingModeStackEntry(int Priority, IStackableMode OperatingMode);
}

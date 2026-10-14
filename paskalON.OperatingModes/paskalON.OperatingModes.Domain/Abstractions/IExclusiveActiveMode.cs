// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Abstractions
{
    /// <summary>
    /// Classification: Exclusive Active
    /// Position: First or second in the stack (mutually ordered with Exclusive Reactive)
    /// Cardinality: 0..1 per active power type
    /// Description: The only active power mode that can be active at a time.
    /// </summary>
    /// <remarks>
    /// Reads <see cref="OperatingModeSignals.SetpointActivePower"/> and writes <see cref="OperatingModeSignals.BaseActivePower"/>. No base, additive or limiting active mode can be combined with it.
    /// </remarks>
    public interface IExclusiveActiveMode : IStackableMode
    {
    }
}

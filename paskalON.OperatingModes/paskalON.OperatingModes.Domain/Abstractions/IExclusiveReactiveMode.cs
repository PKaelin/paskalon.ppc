// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Abstractions
{
    /// <summary>
    /// Classification: Exclusive Reactive
    /// Position: First or second in the stack (mutually ordered with Exclusive Active)
    /// Cardinality: 0..1 per reactive power type
    /// Description: The only reactive power mode that can be active at a time.
    /// </summary>
    /// <remarks>
    /// Reads <see cref="OperatingModeSignals.SetpointReactivePower"/> and writes <see cref="OperatingModeSignals.BaseReactivePower"/>. No base, additive or limiting reactive mode can be combined with it.
    /// </remarks>
    public interface IExclusiveReactiveMode : IStackableMode
    {
    }
}

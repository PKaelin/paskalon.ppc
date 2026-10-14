// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Abstractions
{
    /// <summary>
    /// Classification: Base Active 
    /// Position: First or second in the stack (mutually ordered with Base Reactive) 
    /// Cardinality: Exactly one per active power type
    /// Description: The primary active power mode that determines the target.
    /// </summary>
    /// <remarks>
    /// Reads <see cref="OperatingModeSignals.SetpointActivePower"/> and writes <see cref="OperatingModeSignals.BaseActivePower"/>.
    /// </remarks>
    public interface IBaseActiveMode : IStackableMode
    {
    }
}

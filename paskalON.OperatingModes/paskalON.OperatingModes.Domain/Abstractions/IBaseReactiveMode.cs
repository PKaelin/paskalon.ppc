// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Abstractions
{
    /// <summary>
    /// Classification: Base Reactive 
    /// Position: First or second in the stack (mutually ordered with Base Active) 
    /// Cardinality: Exactly one per reactive power type
    /// Description: The primary reactive power mode that determines the target.
    /// </summary>
    /// <remarks>
    /// Reads <see cref="OperatingModeSignals.SetpointReactivePower"/> and writes <see cref="OperatingModeSignals.BaseReactivePower"/>.
    /// </remarks>
    public interface IBaseReactiveMode : IStackableMode
    {
    }
}

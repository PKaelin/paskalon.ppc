// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Abstractions
{
    /// <summary>
    /// Classification: Additive Reactive 
    /// Position: After the base must have a Base Reactive before this mode 
    /// Cardinality: 0..n
    /// Description: Contributes a correction or offset that can be combined with another mode's output.
    /// </summary>
    /// <remarks>
    /// Adds its correction to <see cref="OperatingModeSignals.AdditiveReactivePowerDelta"/> (+=).
    /// </remarks>
    public interface IAdditiveReactiveMode : IStackableMode
    {
    }
}

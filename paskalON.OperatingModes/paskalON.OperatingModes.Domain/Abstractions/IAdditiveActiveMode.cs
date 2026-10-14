// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Abstractions
{
    /// <summary>
    /// Classification: Additive Active 
    /// Position: After the base must have a Base Active before this mode 
    /// Cardinality: 0..n
    /// Description: Contributes a correction or offset that can be combined with another mode's output.
    /// </summary>
    /// <remarks>
    /// Adds its correction to <see cref="OperatingModeSignals.AdditiveActivePowerDelta"/> (+=).
    /// </remarks>
    public interface IAdditiveActiveMode : IStackableMode
    {
    }
}

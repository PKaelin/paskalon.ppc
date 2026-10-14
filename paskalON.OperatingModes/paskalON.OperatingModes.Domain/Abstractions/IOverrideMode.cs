// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Abstractions
{
    /// <summary>
    /// Classification: Override
    /// Position: Before the stack
    /// Cardinality: 0..1 per unit
    /// Description: Force P or Q targets and remove a unit from normal dispatch.
    /// </summary>
    /// <remarks>
    /// Override modes are not part of the operating mode stack. They are held in a separate override slot and are calculated before the stack.
    /// They do not write into the <see cref="OperatingModeSignals"/>, the system operating mode control reads their targets directly.
    /// </remarks>
    public interface IOverrideMode : IOperatingMode
    {
        /// <summary>
        /// Gets the name of the DER unit that is overridden.
        /// </summary>
        /// <remarks>
        /// Null means the override applies to the whole plant.
        /// </remarks>
        string? TargetDerUnit { get => null; }
    }
}

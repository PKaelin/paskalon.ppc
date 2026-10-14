// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Abstractions
{
    /// <summary>
    /// Marker for operating modes that can be selected by a user onto the operating mode stack.
    /// </summary>
    /// <remarks>
    /// All exclusive, base, additive and limiting classification interfaces derive from this interface.
    /// Override modes are not stackable, they are held in a separate override slot.
    /// </remarks>
    public interface IStackableMode : IOperatingMode
    {
    }
}

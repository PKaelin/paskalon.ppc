// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Abstractions
{
    /// <summary>
    /// Operating modes implementing this interface are exclusive and
    /// cannot simultaneously be enabled with other operating modes.
    /// </summary>
    public interface IExclusiveMode : IOperatingMode
    {
    }
}

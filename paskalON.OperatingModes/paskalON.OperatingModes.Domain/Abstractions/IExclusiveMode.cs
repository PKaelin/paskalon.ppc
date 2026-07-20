// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
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

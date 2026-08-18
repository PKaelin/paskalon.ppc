// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto
{
    /// <summary>
    /// Interface for devices.
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// Unique device id.
        /// </summary>
        int DeviceId { get; }
    }
}

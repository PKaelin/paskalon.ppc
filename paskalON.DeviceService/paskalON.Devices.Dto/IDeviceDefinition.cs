// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto
{
    /// <summary>
    /// Interface for device definitions.
    /// </summary>
    public interface IDeviceDefinition : IDevice
    {
        /// <summary>
        /// Name of the device.
        /// </summary>
        string Name { get; }
    }
}

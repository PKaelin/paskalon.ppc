// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Application.Publishers
{
    /// <summary>
    /// Device publisher interface definition.
    /// </summary>
    public interface IDevicePublisher
    {
        /// <summary>
        /// Publishes the DTO parts depending on their interval.
        /// </summary>
        Task Publish(int currentInterval);
    }
}

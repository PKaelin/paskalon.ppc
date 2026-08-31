// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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

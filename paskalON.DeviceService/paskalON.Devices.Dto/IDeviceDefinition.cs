// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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


        /// <summary>
        /// Target address of the device.
        /// </summary>
        string TargetAddress { get; }


        /// <summary>
        /// Target port of the device.
        /// </summary>
        int TargetPort { get; }
    }
}

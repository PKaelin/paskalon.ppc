// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Client.Registers
{
    /// <summary>
    /// Device register interface definition.
    /// Device register holds device Ids and their devices to update the definition, core and detail properties of a device.
    /// </summary>
    /// <typeparam name="TDevice">The device type.</typeparam>
    /// <typeparam name="TDefinition">The definition type of the device.</typeparam>
    /// <typeparam name="TCore">The core type of the device.</typeparam>
    /// <typeparam name="TDetail">The detail type of the device.</typeparam>
    public interface IDeviceRegister<TDevice, TDefinition, TCore, TDetail>
    {
        /// <summary>
        /// Adds a device to the register.
        /// </summary>
        /// <param name="device">The device instance.</param>
        void Add(TDevice device);


        /// <summary>
        /// Tries to get a device according to its device Id.
        /// </summary>
        /// <param name="deviceId">The device Id.</param>
        /// <param name="device">The device that was registered with the device Id.</param>
        /// <returns></returns>
        bool TryGet(int deviceId, out TDevice? device);


        /// <summary>
        /// Update the definition property of <see cref="DeviceBase"/> with the new definition message.
        /// </summary>
        /// <param name="message">The definition message.</param>
        void UpdateDefinition(TDefinition message);


        /// <summary>
        /// Update the core property of <see cref="DeviceBase"/> with the new core message.
        /// </summary>
        /// <param name="message">The core message.</param>
        void UpdateCore(TCore message);


        /// <summary>
        /// Update the detail property of <see cref="DeviceBase"/> with the new detail message.
        /// </summary>
        /// <param name="message">The detail message.</param>
        void UpdateDetail(TDetail message);
    }
}

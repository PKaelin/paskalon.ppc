// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.GenericModbusDevices
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Generic Modbus device interface for the instances that communicate with the device.
    /// </summary>
    /// <typeparam name="T">The type of the generic Modbus device.</typeparam>
    public interface IGenericModbusDevice : IDevice
    {
        /// <summary>
        /// Connects the generic Modbus device and starts communicating once in state connected.
        /// </summary>
        void Connect();


        /// <summary>
        /// Disconnects the generic Modbus device after it stops communicating.
        /// </summary>
        void Disconnect();


        /// <summary>
        /// Tries to reset all latched alarms.
        /// </summary>
        void ResetLatchedAlarms();
    }
}

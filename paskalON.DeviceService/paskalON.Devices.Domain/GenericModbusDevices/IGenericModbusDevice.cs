// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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
        Task ConnectAsync();


        /// <summary>
        /// Disconnects the generic Modbus device after it stops communicating.
        /// </summary>
        Task DisconnectAsync();


        /// <summary>
        /// Tries to reset all latched alarms.
        /// </summary>
        Task ResetLatchedAlarmsAsync();
    }
}

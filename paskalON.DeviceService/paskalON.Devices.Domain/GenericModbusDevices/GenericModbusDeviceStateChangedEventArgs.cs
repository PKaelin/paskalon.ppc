// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.GenericModbusDevices
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Event argument class for generic Modbus device state changed events.
    /// </summary>
    public class GenericModbusDeviceStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Generic Modbus device state.
        /// </summary>
        public GenericModbusDeviceState State { get; private set; }


        /// <summary>
        /// Constructor of <see cref="GenericModbusDeviceStateChangedEventArgs"/>.
        /// </summary>
        /// <param name="state">The generic Modbus device state.</param>
        public GenericModbusDeviceStateChangedEventArgs(GenericModbusDeviceState state)
        {
            State = state;
        }
    }
}

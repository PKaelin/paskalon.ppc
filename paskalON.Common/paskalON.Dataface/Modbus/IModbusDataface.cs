// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.Modbus
{
    /// <summary>
    /// IModbusDataface is the specific dataface for Modbus registrations and communications.
    /// </summary>    
    public interface IModbusDataface : IDataface
    {
        /// <summary>
        /// List of IModbusRegisterEntry registrations.
        /// </summary>
        List<IModbusRegisterEntry> Registers { get; }


        /// <summary>
        /// List of Modbus polling ranges.
        /// </summary>
        List<ModbusPollingRangeEntry> PollingRanges { get; }
    }
}

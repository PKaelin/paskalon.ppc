// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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

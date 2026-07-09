// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.Modbus
{
    /// <summary>
    /// Interface for Modbus data face.
    /// </summary>
    public interface IModbusDataface
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

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;

namespace paskalON.Protocols.Modbus.Converters
{
    /// <summary>
    /// Interface definition for Modbus data conversions.
    /// </summary>
    public interface IModbusDataConverter
    {
        /// <summary>
        /// Converts raw data value into a bool value. 
        /// </summary>
        /// <param name="rawData">List of bool values</param>
        /// <param name="register">The Modbus register entry.</param>
        /// <param name="startAddress">The start address of the first raw data value.</param>
        /// <returns>The data value.</returns>
        bool ConvertRawData(bool[] rawData, IModbusRegisterEntry register, ushort startAddress);


        /// <summary>
        /// Converts raw data value into a value. 
        /// </summary>
        /// <param name="rawData">List of ushort value.</param>
        /// <param name="register">The Modbus register entry.</param>
        /// <param name="startAddress">The start address of the first raw data value.</param>
        /// <returns>The data value.</returns>
        object? ConvertRawData(ushort[] rawData, IModbusRegisterEntry register, ushort startAddress);
    }
}

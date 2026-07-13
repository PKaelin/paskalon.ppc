// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;

namespace paskalON.Communication.Protocols.Modbus.Converters
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
        object ConvertRawData(ushort[] rawData, IModbusRegisterEntry register, ushort startAddress);



        /// <summary>
        /// Gets the register length of the Modbus data type.
        /// </summary>
        /// <param name="type">The Modbus data type.</param>
        /// <returns>The length of the data type.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws an exception when the data type is not considered.</exception>
        int GetRegisterLength(ModbusDataType type);



        /// <summary>
        /// Gets whether the Modbus data type is big or little endian.
        /// </summary>
        /// <param name="type">The Modbus data type.</param>
        /// <returns>True if Modbus data type is big endian otherwise false.</returns>
        bool IsBigEndian(ModbusDataType type);


        /// <summary>
        /// Converts the register values to a bite array.
        /// </summary>
        /// <param name="registers">Array of register values that make up the whole data value.</param>
        /// <param name="type">The Modbus data type.</param>
        /// <returns>Byte array containing the whole data value.</returns>
        byte[] ConvertToByteArray(ushort[] registers, ModbusDataType type);


        /// <summary>
        /// Gets the actual value form an array of bytes.
        /// </summary>
        /// <param name="bytes">Byte array containing the whole data value.</param>
        /// <param name="type">The Modbus data type.</param>
        /// <returns>The actual value.</returns>
        object ConvertBytesToType(byte[] bytes, ModbusDataType type);


        /// <summary>
        /// Apply scale if scale is defined.
        /// </summary>
        /// <param name="value">The value to scale.</param>
        /// <param name="scale">The scale that gets applied.</param>
        /// <returns>Scaled value.</returns>
        object ApplyScale(object value, double scale);
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Dataface.Modbus
{
    /// <summary>
    /// Interface for the Modbus register entry.
    /// </summary>
    public interface IModbusRegisterEntry
    {
        /// <summary>
        /// Instance the entry is member of.
        /// </summary>
        object Instance { get; }


        /// <summary>
        /// Name of the entry.
        /// </summary>
        string Name { get; }


        /// <summary>
        /// Modbus register number.
        /// </summary>
        int Register { get; }


        /// <summary>
        /// Scale that is applied to the register value.
        /// </summary>
        double Scale { get; }


        /// <summary>
        /// The register data type.
        /// </summary>
        ModbusDataType DataType { get; }


        /// <summary>
        /// The offset applied to the register entry.
        /// </summary>
        int Offset { get; }


        /// <summary>
        /// Update action.
        /// </summary>
        /// <param name="value">Value to be update the entry with.</param>
        void Update(object value);
    }
}

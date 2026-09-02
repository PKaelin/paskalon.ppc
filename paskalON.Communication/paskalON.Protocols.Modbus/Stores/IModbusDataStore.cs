// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.Modbus.Stores
{
    /// <summary>
    /// Modbus data store interface definition.
    /// </summary>
    public interface IModbusDataStore
    {
        /// <summary>
        /// Reads coils from the Modbus data store.
        /// </summary>
        /// <param name="startAddress">The start address of the read.</param>
        /// <param name="endAddress">The end address of the read.</param>
        bool[] ReadCoils(ushort startAddress, ushort endAddress);


        /// <summary>
        /// Reads discrete inputs from the Modbus data store.
        /// </summary>
        /// <param name="startAddress">The start address of the read.</param>
        /// <param name="endAddress">The end address of the read.</param>
        bool[] ReadDiscreteInputs(ushort startAddress, ushort endAddress);


        /// <summary>
        /// Reads holding registers from the Modbus data store.
        /// </summary>
        /// <param name="startAddress">The start address of the read.</param>
        /// <param name="endAddress">The end address of the read.</param>
        ushort[] ReadHoldingRegisters(ushort startAddress, ushort endAddress);


        /// <summary>
        /// Reads input registers from the Modbus data store.
        /// </summary>
        /// <param name="startAddress">The start address of the read.</param>
        /// <param name="endAddress">The end address of the read.</param>
        ushort[] ReadInputRegisters(ushort startAddress, ushort endAddress);


        /// <summary>
        /// Writes a coil values to the Modbus data store.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The values to write to the register(s).</param>
        void WriteCoils(ushort startAddress, bool[] values);


        /// <summary>
        /// Writes a discrete input values to the Modbus data store.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The values to write to the register(s).</param>
        void WriteDiscreteInputs(ushort startAddress, bool[] values);


        /// <summary>
        /// Writes a holding register values to the Modbus data store.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The values to write to the register(s).</param>
        void WriteHoldingRegisters(ushort startAddress, ushort[] values);


        /// <summary>
        /// Writes a input register values to the Modbus data store.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The values to write to the register(s).</param>
        void WriteInputRegisters(ushort startAddress, ushort[] values);
    }
}
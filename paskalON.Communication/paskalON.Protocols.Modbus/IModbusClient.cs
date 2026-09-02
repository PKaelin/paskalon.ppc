// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using paskalON.Protocols.Modbus.Converters;

namespace paskalON.Protocols.Modbus
{
    /// <summary>
    /// Modbus client interface definition.
    /// </summary>
    public interface IModbusClient : IModbusDataConverter
    {
        /// <summary>
        /// Event triggered when a communication error occurs.
        /// </summary>
        event EventHandler<EventArgs> OnCommunicationError;


        /// <summary>
        /// Current state of the Modbus client.
        /// </summary>
        ModbusClientState State { get; }


        /// <summary>
        /// Server address the client connects to.
        /// </summary>
        string ServerAddress { get; }


        /// <summary>
        /// Server port the client connects to.
        /// </summary>
        int ServerPort { get; }

        /// <summary>
        /// Unit ID of the Modbus.
        /// </summary>
        byte UnitId { get; }


        /// <summary>
        /// Connects the Modbus client to the server.
        /// </summary>
        Task ConnectAsync(CancellationToken cancellationToken = default);


        /// <summary>
        /// Disconnects the Modbus client from the server.
        /// </summary>
        Task DisconnectAsync(CancellationToken cancellationToken = default);


        /// <summary>
        /// Reads coils from the Modbus.
        /// </summary>
        /// <param name="startAddress">The start address of the read.</param>
        /// <param name="endAddress">The end address of the read.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        Task<bool[]> ReadCoilsAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default);


        /// <summary>
        /// Reads discrete inputs from the Modbus.
        /// </summary>
        /// <param name="startAddress">The start address of the read.</param>
        /// <param name="endAddress">The end address of the read.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        Task<bool[]> ReadDiscreteInputsAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default);


        /// <summary>
        /// Reads holding registers from the Modbus.
        /// </summary>
        /// <param name="startAddress">The start address of the read.</param>
        /// <param name="endAddress">The end address of the read.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        Task<ushort[]> ReadHoldingRegistersAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default);


        /// <summary>
        /// Reads input registers from the Modbus.
        /// </summary>
        /// <param name="startAddress">The start address of the read.</param>
        /// <param name="endAddress">The end address of the read.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        Task<ushort[]> ReadInputRegistersAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default);


        /// <summary>
        /// Writes a double to single register to the Modbus.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to write to the register.</param>
        /// <param name="type">The data type of the register.</param>
        /// <param name="priority">The priority of the write operation.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        Task WriteSingleRegisterAsync(ushort address, double value, ModbusDataType type, short priority = 3,
            double scale = 1, CancellationToken cancellationToken = default);


        /// <summary>
        /// Writes a ushort to single register to the Modbus.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to write to the register.</param>
        /// <param name="type">The data type of the register.</param>
        /// <param name="priority">The priority of the write operation.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        Task WriteSingleRegisterAsync(ushort address, ushort value, ModbusDataType type, short priority = 3,
            double scale = 1, CancellationToken cancellationToken = default);


        /// <summary>
        /// Writes a bool to single register to the Modbus.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to write to the register.</param>
        /// <param name="type">The data type of the register.</param>
        /// <param name="priority">The priority of the write operation.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        Task WriteSingleRegisterAsync(ushort address, bool value, ModbusDataType type, short priority = 3,
            double scale = 1, CancellationToken cancellationToken = default);


        /// <summary>
        /// Writes multiple ushort to a register start address to the Modbus.
        /// </summary>
        /// <param name="address">The address of the register to write.</param>
        /// <param name="value">The value to write to the register.</param>
        /// <param name="type">The data type of the register.</param>
        /// <param name="priority">The priority of the write operation.</param>
        /// <param name="cancellationToken">Cancelation token.</param>
        Task WriteMultipleRegistersAsync(ushort address, ushort[] values, ModbusDataType type, short priority = 3, CancellationToken cancellationToken = default);
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using paskalON.Protocols.Modbus.Converters;

namespace paskalON.Protocols.Modbus
{
    public interface IModbusClient : IModbusDataConverter
    {
        event EventHandler<EventArgs> OnCommunicationError;
        ModbusClientState State { get; }
        string ServerAddress { get; }
        int ServerPort { get; }

        // Connection Management
        Task ConnectAsync(CancellationToken cancellationToken = default);
        Task DisconnectAsync(CancellationToken cancellationToken = default);


        Task<bool[]> ReadCoilsAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default);
        Task<bool[]> ReadDiscreteInputsAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default);
        Task<ushort[]> ReadHoldingRegistersAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default);
        Task<ushort[]> ReadInputRegistersAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default);


        Task WriteSingleRegisterAsync(ushort address, double value, ModbusDataType type, short priority = 3, CancellationToken cancellationToken = default);
        Task WriteSingleRegisterAsync(ushort address, ushort value, ModbusDataType type, short priority = 3, CancellationToken cancellationToken = default);
        Task WriteSingleRegisterAsync(ushort address, bool value, ModbusDataType type, short priority = 3, CancellationToken cancellationToken = default);
        Task WriteMultipleRegistersAsync(ushort address, ushort[] values, ModbusDataType type, short priority = 3, CancellationToken cancellationToken = default);
    }
}

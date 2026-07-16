// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using paskalON.Protocols.Modbus.Converters;

namespace paskalON.Protocols.Modbus
{
    public interface IModbusClient : IModbusDataConverter
    {
        public ModbusClientState State { get; }
        public string ServerAddress { get; }
        public int ServerPort { get; }

        // Connection Management
        Task ConnectAsync(CancellationToken cancellationToken = default);
        Task DisconnectAsync(CancellationToken cancellationToken = default);


        Task<bool[]> ReadCoilsAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default);
        Task<bool[]> ReadDiscreteInputsAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default);
        Task<ushort[]> ReadHoldingRegistersAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default);
        Task<ushort[]> ReadInputRegistersAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default);


        Task WriteSingleRegisterAsync(ushort address, double value, ModbusDataType type, CancellationToken cancellationToken = default);
        Task WriteSingleRegisterAsync(ushort address, ushort value, ModbusDataType type, CancellationToken cancellationToken = default);
        Task WriteSingleRegisterAsync(ushort address, bool value, ModbusDataType type, CancellationToken cancellationToken = default);
        Task WriteMultipleRegistersAsync(ushort address, ushort[] values, ModbusDataType type, CancellationToken cancellationToken = default);
    }
}

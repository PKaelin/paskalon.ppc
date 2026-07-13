// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Communication.Protocols.Modbus.Converters;
using paskalON.Dataface.Modbus;

namespace paskalON.Communication.Protocols.Modbus
{
    public interface IModbusClient : IModbusDataConverter
    {
        public ModbusState State { get; }

        // Connection Management
        Task ConnectAsync(CancellationToken cancellationToken = default);
        Task DisconnectAsync();


        ValueTask<bool[]> ReadCoilsAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default);
        ValueTask<bool[]> ReadDiscreteInputsAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default);
        ValueTask<ushort[]> ReadHoldingRegistersAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default);
        ValueTask<ushort[]> ReadInputRegistersAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default);


        Task WriteSingleRegisterAsync(ushort address, double value, ModbusDataType type, CancellationToken cancellationToken = default);
        Task WriteSingleRegisterAsync(ushort address, ushort value, ModbusDataType type, CancellationToken cancellationToken = default);
        Task WriteSingleRegisterAsync(ushort address, bool value, ModbusDataType type, CancellationToken cancellationToken = default);
        Task WriteMultipleRegistersAsync(ushort address, ushort[] values, ModbusDataType type, CancellationToken cancellationToken = default);
    }
}

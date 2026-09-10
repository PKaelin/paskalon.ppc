// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using NModbus;
using paskalON.Dataface.Modbus;
using paskalON.Protocols.Modbus.Converters;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.Protocols.Modbus
{
    /// <summary>
    /// Modbus client implementation backed by an in-memory data store.
    /// </summary>
    public sealed class MemoryModbusClient : IModbusClient
    {
        /// <summary>
        /// Modbus data converter.
        /// </summary>
        private readonly ModbusDataConverter _converter = new ModbusDataConverter();


        /// <inheritdoc/>
        public event EventHandler<EventArgs>? OnCommunicationError;


        /// <inheritdoc/>
        public ModbusClientState State { get; } = ModbusClientState.Connected;


        /// <inheritdoc/>
        public string ServerAddress { get => "MemoryClient"; }


        /// <inheritdoc/>
        public int ServerPort { get => 0; }


        /// <inheritdoc/>
        public byte UnitId { get; init; }


        /// <summary>
        /// Backing data store.
        /// </summary>
        public IModbusDataStore Store { get; init; }


        /// <summary>
        /// Constructor of <see cref="MemoryModbusClient"/>.
        /// </summary>
        /// <param name="store">The Modbus data store.</param>
        /// <param name="unitId">Teh unit id.</param>
        public MemoryModbusClient(IModbusDataStore store, byte unitId = 1)
        {
            ArgumentNullException.ThrowIfNull(store);

            Store = store;
            UnitId = unitId;
            // Dummy call
            RaiseCommunicationError();
        }


        /// <inheritdoc/>
        public Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            // Mark the client as connected.
            return Task.CompletedTask;
        }


        /// <inheritdoc/>
        public Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            // Never really disconnected.
            return Task.CompletedTask;
        }


        /// <inheritdoc/>
        public bool ConvertRawData(bool[] rawData, IModbusRegisterEntry register, ushort startAddress)
        {
            return _converter.ConvertRawData(rawData, register, startAddress);
        }


        /// <inheritdoc/>
        public object? ConvertRawData(ushort[] rawData, IModbusRegisterEntry register, ushort startAddress)
        {
            return _converter.ConvertRawData(rawData, register, startAddress);
        }


        /// <inheritdoc/>
        public Task<bool[]?> ReadCoilsAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<bool[]?>(ReadPoints(Store.CoilDiscretes, startAddress, endAddress));
        }

        /// <inheritdoc/>
        public Task<bool[]?> ReadDiscreteInputsAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<bool[]?>(ReadPoints(Store.CoilInputs, startAddress, endAddress));
        }


        /// <inheritdoc/>
        public Task<ushort[]?> ReadHoldingRegistersAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default)
        {
            ushort count = checked((ushort)(endAddress - startAddress + 1));

            return Task.FromResult<ushort[]?>(Store.HoldingRegisters.ReadPoints(startAddress, count));
        }


        /// <inheritdoc/>
        public Task<ushort[]?> ReadInputRegistersAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<ushort[]?>(ReadPoints(Store.InputRegisters, startAddress, endAddress));
        }


        /// <inheritdoc/>
        public Task WriteMultipleRegistersAsync(ushort address, ushort[] values, ModbusDataType type, short priority = 3, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(values);

            Store.HoldingRegisters.WritePoints(address, values);

            return Task.CompletedTask;
        }


        /// <inheritdoc/>
        public Task WriteSingleRegisterAsync(ushort address, double value, ModbusDataType type, short priority = 3, double scale = 1, CancellationToken cancellationToken = default)
        {
            ushort[] registers = _converter.RegisterArrayFromValue(value, type, scale);
            Store.HoldingRegisters.WritePoints(address, registers);

            return Task.CompletedTask;
        }


        /// <inheritdoc/>
        public Task WriteSingleRegisterAsync(ushort address, ushort value, ModbusDataType type, short priority = 3, double scale = 1, CancellationToken cancellationToken = default)
        {
            ushort[] registers = _converter.RegisterArrayFromValue(value, type, scale);
            Store.HoldingRegisters.WritePoints(address, registers);

            return Task.CompletedTask;
        }


        /// <inheritdoc/>
        public Task WriteSingleRegisterAsync(ushort address, bool value, ModbusDataType type, short priority = 3, double scale = 1, CancellationToken cancellationToken = default)
        {
            ushort[] registers = _converter.RegisterArrayFromValue(Convert.ToDouble(value), type, scale);
            Store.HoldingRegisters.WritePoints(address, registers);

            return Task.CompletedTask;
        }


        /// <inheritdoc/>
        private T[] ReadPoints<T>(IPointSource<T> source, ushort startAddress, ushort endAddress)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(startAddress, endAddress);

            ushort count = checked((ushort)(endAddress - startAddress + 1));

            return source.ReadPoints(startAddress, count);
        }


        /// <summary>
        /// Raise communication error.
        /// </summary>
        private void RaiseCommunicationError()
        {
            if (UnitId < 0)
            {
                OnCommunicationError?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

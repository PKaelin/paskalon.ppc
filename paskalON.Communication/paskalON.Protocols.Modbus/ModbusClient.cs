// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface.Modbus;

namespace paskalON.Protocols.Modbus
{
    public class ModbusClient : IModbusClient
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<ModbusClient> _logger;

        public ModbusClientState State { get; set; } = ModbusClientState.Disconnected;

        public string ServerAddress { get; init; }

        public int ServerPort { get; init; }


        public event EventHandler<EventArgs>? OnCommunicationError;


        public ModbusClient(ILogger<ModbusClient> logger, string serverAddress, int serverPort)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(serverAddress);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(serverPort);

            _logger = logger;
            ServerAddress = serverAddress;
            ServerPort = serverPort;
        }

        public Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            try
            {

            }
            catch (Exception ex)
            {
                OnCommunicationError?.Invoke(this, new EventArgs());
                _logger.LogError("Connect error occurred. Device: {ServerAddress}:{ServerPort}. {Error}:", ServerAddress, ServerPort, ex.Message);
            }
            return Task.CompletedTask;
        }

        public bool ConvertRawData(bool[] rawData, IModbusRegisterEntry register, ushort startAddress)
        {
            return true;
        }

        public object? ConvertRawData(ushort[] rawData, IModbusRegisterEntry register, ushort startAddress)
        {
            return Task.CompletedTask;
        }

        public Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<bool[]> ReadCoilsAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Array.Empty<bool>());
        }

        public Task<bool[]> ReadDiscreteInputsAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Array.Empty<bool>());
        }

        public Task<ushort[]> ReadHoldingRegistersAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Array.Empty<ushort>());
        }

        public Task<ushort[]> ReadInputRegistersAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Array.Empty<ushort>());
        }

        public Task WriteMultipleRegistersAsync(ushort address, ushort[] values, ModbusDataType type, short priority = 3, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task WriteSingleRegisterAsync(ushort address, double value, ModbusDataType type, short priority = 3, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task WriteSingleRegisterAsync(ushort address, ushort value, ModbusDataType type, short priority = 3, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task WriteSingleRegisterAsync(ushort address, bool value, ModbusDataType type, short priority = 3, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}

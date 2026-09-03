// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using NModbus;
using paskalON.Dataface.Modbus;
using paskalON.Protocols.Modbus.Converters;
using paskalON.Protocols.Modbus.Stores;
using System.Net;
using System.Net.Sockets;

namespace paskalON.Protocols.Modbus.NModbus
{
    /// <summary>
    /// Modbus server (Modbus Slave) implementation for over Modbus TCP communications.
    /// </summary>
    /// <remarks>
    /// The server owns an in-memory <see cref="IModbusDataStore"/> with the four standard Modbus tables.
    /// </remarks>
    public sealed class NModbusServer : IModbusServer, IAsyncDisposable
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<NModbusServer> _logger;

        /// <summary>
        /// Interface definition for Modbus data conversions.
        /// </summary>
        private readonly IModbusDataConverter _converter;


        /// <summary>
        /// Modbus factory for creating Modbus server and network instances.
        /// </summary>
        private readonly IModbusFactory _factory = new ModbusFactory();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        private readonly IModbusDataStore _dataStore;

        /// <summary>
        /// TCP listener for this Modbus server.
        /// </summary>
        private TcpListener? _listener;


        /// <summary>
        /// Modbus TCP slave network.
        /// </summary>
        private IModbusTcpSlaveNetwork? _network;


        /// <summary>
        /// Listening task for the Modbus server, which runs the listen loop asynchronously.
        /// </summary>
        private Task? _listenTask;


        /// <summary>
        /// Cancellation token source for the listen loop, allowing it to be cancelled when stopping the server.
        /// </summary>
        private CancellationTokenSource? _listenCts;


        /// <summary>
        /// Modbus server state.
        /// </summary>
        private volatile ModbusServerState _state = ModbusServerState.Stopped;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event EventHandler<EventArgs>? OnCommunicationError;


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ModbusServerState State { get => _state; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ListenAddress { get; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int ListenPort { get; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public byte UnitId { get; }


        /// <summary>
        /// Constructor of <see cref="NModbusServer"/>.
        /// </summary>
        /// <param name="listenAddress">Local address to bind, e.g. "0.0.0.0" for all interfaces, or "127.0.0.1".</param>
        /// <param name="listenPort">TCP port to listen on (standard Modbus TCP is 502; ports below 1024 need elevated privileges on most OSes).</param>
        /// <param name="unitId">Modbus unit/slave id this server responds as.</param>
        /// <param name="converter">Optional custom data converter; defaults to <see cref="ModbusDataConverter"/>.</param>
        public NModbusServer(ILogger<NModbusServer> logger, IModbusDataStore dataStore, string listenAddress, int listenPort, byte unitId = 1)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(dataStore);
            ArgumentNullException.ThrowIfNullOrEmpty(listenAddress);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(listenPort);

            _logger = logger;
            _dataStore = dataStore;
            ListenAddress = listenAddress;
            ListenPort = listenPort;
            UnitId = unitId;
            _converter = new ModbusDataConverter();
        }


        /// <summary>
        /// Starts the server, binding to the configured address and port and beginning to listen for incoming Modbus TCP connections.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_state == ModbusServerState.Listening)
            {
                return;
            }

            _state = ModbusServerState.BeginListen;

            try
            {
                IPAddress address = string.IsNullOrEmpty(ListenAddress) || ListenAddress == "0.0.0.0" ? IPAddress.Any : IPAddress.Parse(ListenAddress);
                _listener = new TcpListener(address, ListenPort);
                _listener.Start();

                _network = _factory.CreateSlaveNetwork(_listener);
                IModbusSlave slave = _factory.CreateSlave(UnitId, _dataStore);
                _network.AddSlave(slave);

                _listenCts = new CancellationTokenSource();
                CancellationToken loopToken = _listenCts.Token;

                _listenTask = Task.Run(async () =>
                {
                    try
                    {
                        await _network.ListenAsync(loopToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        // expected on StopAsync
                    }
                    catch
                    {
                        _state = ModbusServerState.Faulted;
                        RaiseCommunicationError();
                    }
                }, loopToken);

                _state = ModbusServerState.Listening;
            }
            catch
            {
                _state = ModbusServerState.Faulted;
                RaiseCommunicationError();
                throw;
            }
        }


        /// <summary>
        /// Stops the server, cancelling any active listen loop and closing the TCP listener.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>        
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (_state == ModbusServerState.Stopped) return;

            _state = ModbusServerState.StopListen;
            _listenCts?.Cancel();

            try
            {
                if (_listenTask is not null)
                {
                    await _listenTask.ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // expected exception when canceling
            }

            _listener?.Stop();
            _listener = null;
            _network = null;
            _listenCts = null;
            _listenTask = null;

            _state = ModbusServerState.Stopped;
        }



        /// <summary>
        /// Raises communication error event.
        /// </summary>
        private void RaiseCommunicationError()
        {
            OnCommunicationError?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// Disposes the server, stopping it if necessary and releasing resources.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await StopAsync().ConfigureAwait(false);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool ConvertRawData(bool[] rawData, IModbusRegisterEntry register, ushort startAddress)
        {
            return _converter.ConvertRawData(rawData, register, startAddress);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public object? ConvertRawData(ushort[] rawData, IModbusRegisterEntry register, ushort startAddress)
        {
            return _converter.ConvertRawData(rawData, register, startAddress);
        }
    }
}
// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using NModbus;
using paskalON.Dataface.Modbus;
using paskalON.Protocols.Modbus.Configs;
using paskalON.Protocols.Modbus.Converters;
using paskalON.Protocols.Modbus.Dispatchers;
using System.Net.Sockets;

namespace paskalON.Protocols.Modbus.NModbus
{
    /// <summary>
    /// Modbus client implementation for over Modbus TCP communications.
    /// </summary>
    /// <remarks>
    /// - "priority" has no meaning in the Modbus protocol itself. Because only one request can be
    ///   in flight at a time on a single TCP connection, writes are queued and dispatched in
    ///   priority order by <see cref="PriorityDispatcher"/> (lower value first, default 3).
    /// - WriteSingleRegisterAsync may internally issue a multi-register write (function code 16)
    ///   when the chosen <see cref="ModbusDataType"/> spans more than one register (e.g. Float32).
    /// </remarks>
    public sealed class NModbusClient : IModbusClient, IAsyncDisposable
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<NModbusClient> _logger;


        /// <summary>
        /// Configuration for the client connection.
        /// </summary>
        private readonly ClientConnectionConfig _clientConnection;


        /// <summary>
        /// Modbus data converter helper.
        /// </summary>
        private readonly ModbusDataConverter _converter;


        /// <summary>
        /// Modbus factory that create a Modbus client with a TCP client.
        /// </summary>
        private readonly IModbusFactory _factory = new ModbusFactory();


        /// <summary>
        /// IO lock that limits the number of threads that can access a resource.
        /// </summary>
        private readonly SemaphoreSlim _ioLock = new(1, 1);


        /// <summary>
        /// Queues the messages and sends them according to their priority.
        /// </summary>
        private readonly PriorityDispatcher _dispatcher = new PriorityDispatcher();


        /// <summary>
        /// TCP client for the Modbus client.
        /// </summary>
        private TcpClient? _tcpClient;


        /// <summary>
        /// The Modbus client interface instance.
        /// </summary>
        private IModbusMaster? _master;


        /// <summary>
        /// Modbus client state.
        /// </summary>
        private volatile ModbusClientState _state = ModbusClientState.Disconnected;


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event EventHandler<EventArgs>? OnCommunicationError;


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ModbusClientState State { get; init; } = ModbusClientState.Disconnected;


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string ServerAddress { get => _clientConnection.ServerAddress; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int ServerPort { get => _clientConnection.ServerPort; }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public byte UnitId { get; init; }


        /// <summary>
        /// Constructor of <see cref="NModbusClient"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="serverAddress">Host name or IP address of the Modbus TCP server.</param>
        /// <param name="serverPort">TCP port of the Modbus server (standard Modbus TCP is 502).</param>
        /// <param name="unitId">Modbus unit/slave id to address (ignored by most direct Modbus TCP servers, but required when talking through a gateway).</param>
        public NModbusClient(ILogger<NModbusClient> logger, ClientConnectionConfig clientConnection, byte unitId = 1)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(clientConnection);

            _logger = logger;
            _clientConnection = clientConnection;
            _converter = new ModbusDataConverter();
            UnitId = unitId;
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


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            if (_state == ModbusClientState.Connected)
            {
                return;
            }

            _state = ModbusClientState.Connecting;

            Exception? lastException = null;

            // Number of attempts = initial attempt + retries.
            int maxAttempts = _clientConnection.ConnectRetryCount + 1;

            try
            {
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    TcpClient? tcpClient = null;

                    try
                    {
                        tcpClient = new TcpClient();

                        using CancellationTokenSource timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        timeoutCts.CancelAfter(_clientConnection.ConnectionTimeoutMilliseconds);
                        await tcpClient.ConnectAsync(ServerAddress, ServerPort, timeoutCts.Token).ConfigureAwait(false);
                        _tcpClient = tcpClient;
                        _master = _factory.CreateMaster(tcpClient);
                        // NModbus Modbus-level settings
                        _master.Transport.ReadTimeout = _clientConnection.OperationTimeoutMilliseconds;
                        _master.Transport.WriteTimeout = _clientConnection.OperationTimeoutMilliseconds;
                        _master.Transport.Retries = _clientConnection.SendRetryCount;
                        _master.Transport.WaitToRetryMilliseconds = _clientConnection.SendRetryIntervalMilliseconds;
                        _dispatcher.Start();

                        _state = ModbusClientState.Connected;

                        return;
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested == false)
                    {
                        lastException = new TimeoutException($"Connection attempt {attempt} timed out after {_clientConnection.ConnectionTimeoutMilliseconds} ms.");
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                    }

                    tcpClient?.Dispose();

                    if (attempt < maxAttempts)
                    {
                        await Task.Delay(_clientConnection.ConnectRetryIntervalMilliseconds, cancellationToken).ConfigureAwait(false);
                    }
                }

                throw new InvalidOperationException($"Unable to connect to {ServerAddress}:{ServerPort} after {maxAttempts} attempt(s).", lastException);
            }
            catch
            {
                _state = ModbusClientState.Faulted;
                RaiseCommunicationError();
                throw;
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            if (_state == ModbusClientState.Disconnected)
            {
                return;
            }

            _state = ModbusClientState.Disconnecting;

            await _dispatcher.StopAsync().ConfigureAwait(false);

            _master?.Dispose();
            _master = null;

            _tcpClient?.Close();
            _tcpClient?.Dispose();
            _tcpClient = null;

            _state = ModbusClientState.Disconnected;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task<bool[]> ReadCoilsAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default)
        {
            return _dispatcher.EnqueueAsync(ModbusOperation.Read, startAddress, 3,
                () => ExecuteReadAsync<bool[]>(() => _master!.ReadCoilsAsync(UnitId, startAddress, ToCount(startAddress, endAddress))), cancellationToken);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task<bool[]> ReadDiscreteInputsAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default)
        {
            return _dispatcher.EnqueueAsync(ModbusOperation.Read, startAddress, 3,
                () => ExecuteReadAsync<bool[]>(() => _master!.ReadInputsAsync(UnitId, startAddress, ToCount(startAddress, endAddress))), cancellationToken);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task<ushort[]> ReadHoldingRegistersAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default)
        {
            return _dispatcher.EnqueueAsync(ModbusOperation.Read, startAddress, 3,
                () => ExecuteReadAsync<ushort[]>(() => _master!.ReadHoldingRegistersAsync(UnitId, startAddress, ToCount(startAddress, endAddress))), cancellationToken);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task<ushort[]> ReadInputRegistersAsync(ushort startAddress, ushort endAddress, CancellationToken cancellationToken = default)
        {
            return _dispatcher.EnqueueAsync(ModbusOperation.Read, startAddress, 3,
                () => ExecuteReadAsync<ushort[]>(() => _master!.ReadInputRegistersAsync(UnitId, startAddress, ToCount(startAddress, endAddress))), cancellationToken);
        }


        /// <summary>
        /// Execute the read operation with proper locking and error handling.
        /// </summary>
        /// <typeparam name="T">Type of the return values.</typeparam>
        /// <param name="read">The read operation to execute.</param>
        /// <returns>Task</returns>
        private async Task<T> ExecuteReadAsync<T>(Func<Task<T>> read)
        {
            EnsureConnected();

            await _ioLock.WaitAsync().ConfigureAwait(false);

            try
            {
                return await read().ConfigureAwait(false);
            }
            catch
            {
                RaiseCommunicationError();
                throw;
            }
            finally
            {
                _ioLock.Release();
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task WriteSingleRegisterAsync(ushort address, double value, ModbusDataType type, short priority = 3,
            double scale = 1, CancellationToken cancellationToken = default)
        {
            return _dispatcher.EnqueueAsync(ModbusOperation.Write, address, priority,
                () => ExecuteWriteAsync(UnitId, address, _converter.RegisterArrayFromValue(value, type, scale)), cancellationToken);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task WriteSingleRegisterAsync(ushort address, ushort value, ModbusDataType type, short priority = 3,
            double scale = 1, CancellationToken cancellationToken = default)
        {
            return _dispatcher.EnqueueAsync(ModbusOperation.Write, address, priority,
                () => ExecuteWriteAsync(UnitId, address, _converter.RegisterArrayFromValue(value, type, scale)), cancellationToken);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task WriteSingleRegisterAsync(ushort address, bool value, ModbusDataType type, short priority = 3,
            double scale = 1, CancellationToken cancellationToken = default)
        {
            return _dispatcher.EnqueueAsync(ModbusOperation.Write, address, priority,
                () => ExecuteWriteAsync(UnitId, address, _converter.RegisterArrayFromValue(Convert.ToDouble(value), type, scale)), cancellationToken);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task WriteMultipleRegistersAsync(ushort address, ushort[] values, ModbusDataType type, short priority = 3, CancellationToken cancellationToken = default)
        {
            return _dispatcher.EnqueueAsync(ModbusOperation.Write, address, priority,
                () => ExecuteWriteAsync(UnitId, address, values), cancellationToken);
        }


        /// <summary>
        ///  Execute the write operation..
        /// </summary>
        /// <param name="unitId">The Modbus unit ID.</param>
        /// <param name="address">The Modbus register.</param>
        /// <param name="values">Teh Modbus value.</param>
        /// <returns></returns>
        private async Task ExecuteWriteAsync(byte unitId, ushort address, ushort[] values)
        {
            await ExecuteWriteAsync(values.Length == 1
                ? () => _master!.WriteSingleRegisterAsync(unitId, address, values[0])
                : () => _master!.WriteMultipleRegistersAsync(unitId, address, values)).ConfigureAwait(false);
        }


        /// <summary>
        ///  Execute the write operation with proper locking and error handling.
        /// </summary>
        /// <param name="write">The write operation to execute.</param>
        /// <returns>Task</returns>
        private async Task ExecuteWriteAsync(Func<Task> write)
        {
            EnsureConnected();

            await _ioLock.WaitAsync().ConfigureAwait(false);

            try
            {
                await write().ConfigureAwait(false);
            }
            catch
            {
                RaiseCommunicationError();
                throw;
            }
            finally
            {
                _ioLock.Release();
            }
        }


        /// <summary>
        /// Helper method 
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="endAddress"></param>
        /// <returns></returns>
        private static ushort ToCount(ushort startAddress, ushort endAddress)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(startAddress, endAddress, "EndAddress must be greater than or equal to startAddress.");

            return (ushort)(endAddress - startAddress + 1);
        }


        /// <summary>
        /// Ensures that the client is connected.
        /// </summary>
        private void EnsureConnected()
        {
            if (_state != ModbusClientState.Connected || _master is null)
            {
                throw new InvalidOperationException("The client is not connected. Call ConnectAsync first.");
            }
        }


        /// <summary>
        /// Raise communication error.
        /// </summary>
        private void RaiseCommunicationError()
        {
            OnCommunicationError?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// Disposes the client.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync().ConfigureAwait(false);
            _ioLock.Dispose();
        }
    }
}
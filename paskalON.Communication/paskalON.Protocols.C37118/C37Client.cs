// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface.C37s;
using paskalON.Protocols.C37118.Configs;
using paskalON.Protocols.C37118.Frames;
using System.Buffers.Binary;
using System.Net.Sockets;

namespace paskalON.Protocols.C37118
{
    /// <summary>
    /// C37.118 client for C37.118 protocol communication over TCP/IP.
    /// </summary>
    public sealed class C37Client : IC37Client, IAsyncDisposable
    {
        /// <summary>
        /// ILogger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<C37Client> _logger;

        /// <summary>
        /// Client connection configuration.
        /// </summary>
        private readonly ClientConnectionConfig _clientConnection;

        /// <summary>
        /// TCP client for the C37 client.
        /// </summary>
        private TcpClient? _tcpClient;


        /// <summary>
        /// Network stream for the communication.
        /// </summary>
        private NetworkStream? _stream;


        /// <summary>
        /// Cancellation token for the receiver loop.
        /// </summary>
        private CancellationTokenSource _shutdownReceiverLoop = new CancellationTokenSource();


        /// <summary>
        /// Receiver task.
        /// </summary>
        private Task? _receiveTask;


        /// <inheritdoc/>
        public event EventHandler<EventArgs>? OnCommunicationError;


        /// <inheritdoc/>
        public event EventHandler<C37DataFrameEventArgs>? DataFrameReceived;


        /// <inheritdoc/>
        public event EventHandler<C37ConfigFrameEventArgs>? ConfigFrameReceived;


        /// <inheritdoc/>
        public C37ClientState State { get; private set; } = C37ClientState.Disconnected;


        /// <inheritdoc/>
        public string ServerAddress { get => _clientConnection.ServerAddress; }


        /// <inheritdoc/>
        public int ServerPort { get => _clientConnection.ServerPort; }



        /// <summary>
        /// Constructor of <see cref="C37Client"/>.
        /// </summary>
        public C37Client(ILogger<C37Client> logger, ClientConnectionConfig clientConnection)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(clientConnection);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(clientConnection.ServerPort);
            ArgumentOutOfRangeException.ThrowIfNegative(clientConnection.ConnectionTimeoutMilliseconds);
            ArgumentOutOfRangeException.ThrowIfNegative(clientConnection.ConnectRetryIntervalMilliseconds);

            _logger = logger;
            _clientConnection = clientConnection;
        }


        /// <inheritdoc/>
        public async Task StartStreamingAsync(CancellationToken cancellationToken = default)
        {
            if (State == C37ClientState.Connected)
            {
                return;
            }

            State = C37ClientState.Connecting;
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
                        tcpClient = new TcpClient(_clientConnection.AddressFamily);
                        using CancellationTokenSource timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        timeoutCts.CancelAfter(_clientConnection.ConnectionTimeoutMilliseconds);
                        await tcpClient.ConnectAsync(ServerAddress, ServerPort, timeoutCts.Token).ConfigureAwait(false);
                        _tcpClient = tcpClient;
                        _stream = _tcpClient.GetStream();
                        _shutdownReceiverLoop = new CancellationTokenSource();
                        State = C37ClientState.Connected;
                        _receiveTask = ReceiveFramesAsync(_shutdownReceiverLoop.Token);

                        return;
                    }
                    catch (Exception ex) when (ex is SocketException || (ex is OperationCanceledException && cancellationToken.IsCancellationRequested == false))
                    {
                        string msgAttempt = $"Device connect to {ServerAddress}:{ServerPort} failed. Attempt {attempt} timed out after {_clientConnection.ConnectionTimeoutMilliseconds} ms";
                        _logger.LogError(msgAttempt);
                        lastException = new TimeoutException(msgAttempt);
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

                string msgConnect = $"Device connect to {ServerAddress}:{ServerPort} failed. Unable to connect {maxAttempts} attempt(s)";
                _logger.LogError(msgConnect);
                throw new InvalidOperationException(msgConnect, lastException);
            }
            catch (Exception)
            {
                DisposeConnection();
                State = C37ClientState.Disconnected;
                RaiseCommunicationError();
                throw;
            }
        }


        /// <inheritdoc/>
        public async Task StopStreamingAsync(CancellationToken cancellationToken = default)
        {
            if (State == C37ClientState.Disconnected)
            {
                return;
            }

            State = C37ClientState.Disconnecting;

            _shutdownReceiverLoop.Cancel();

            if (_receiveTask is not null)
            {
                try
                {
                    await _receiveTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Expected when canceled
                }
                catch (IOException)
                {
                    // Possible when canceled
                }
            }

            DisposeConnection();
            State = C37ClientState.Disconnected;
        }



        /// <inheritdoc/>
        public async Task SendCommandAsync(C37CommandType command)
        {
            if (State != C37ClientState.Connected || _stream is null)
            {
                throw new InvalidOperationException($"The C37 client is not connected. Destination: {ServerAddress}:{ServerPort}");
            }

            await _stream!.WriteAsync(C37FrameCodec.CreateCommandFrame(0, (ushort)command), _shutdownReceiverLoop.Token).ConfigureAwait(false);
        }


        /// <inheritdoc/>
        public async Task RequestConfigurationAsync()
        {
            await SendCommandAsync(C37CommandType.SendConfigFrame2);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task</returns>
        private async Task ReceiveFramesAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (cancellationToken.IsCancellationRequested == false)
                {
                    byte[] header = await ReadExactlyAsync(14, cancellationToken).ConfigureAwait(false);
                    ushort frameSize = BinaryPrimitives.ReadUInt16BigEndian(header.AsSpan(2, 2));
                    ArgumentOutOfRangeException.ThrowIfLessThan(frameSize, 16);
                    byte[] frame = new byte[frameSize];
                    header.CopyTo(frame, 0);
                    await ReadExactlyAsync(frame.AsMemory(14, frameSize - 14), cancellationToken).ConfigureAwait(false);
                    ushort frameType = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(frame.AsSpan(0, 2)) & 0x00F0);

                    if (frameType == 0x0000)
                    {
                        DataFrameReceived?.Invoke(this, new C37DataFrameEventArgs(frame));
                    }
                    else if (frameType == 0x0020 || frameType == 0x0030)
                    {
                        ConfigFrameReceived?.Invoke(this, new C37ConfigFrameEventArgs(frame));
                    }
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
            catch (Exception ex)
            {
                _logger.LogError("C37 client receive loop failed. Destination: {ServerAddress}:{ServerPort} {Error}", ServerAddress, ServerPort, ex);
                RaiseCommunicationError();
            }
        }


        /// <summary>
        /// Reads a certain amount of bytes from the stream.
        /// </summary>
        /// <param name="count">The buffer size.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task of bytes.</returns>
        private async Task<byte[]> ReadExactlyAsync(int count, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[count];
            await ReadExactlyAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false);

            return buffer;
        }


        /// <summary>
        /// Reads from the stream.
        /// </summary>
        /// <param name="buffer">Buffer to read into.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task</returns>
        /// <exception cref="EndOfStreamException">Throws an exception when read bytes is 0.</exception>
        private async Task ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken)
        {
            int offset = 0;

            while (offset < buffer.Length)
            {
                int read = await _stream!.ReadAsync(buffer[offset..], cancellationToken).ConfigureAwait(false);

                if (read == 0)
                {
                    throw new EndOfStreamException();
                }

                offset += read;
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
        /// Disposes the connections.
        /// </summary>
        private void DisposeConnection()
        {
            _stream?.Dispose();
            _tcpClient?.Dispose();
            _stream = null;
            _tcpClient = null;
        }


        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await StopStreamingAsync().ConfigureAwait(false);
            _shutdownReceiverLoop.Dispose();
        }
    }
}

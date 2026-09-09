// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Protocols.C37118.Frames;
using paskalON.Protocols.C37118.Simulations;
using System.Net;
using System.Net.Sockets;

namespace paskalON.Protocols.C37118
{
    /// <summary>
    /// Hosts simulated PMU streams over TCP.
    /// </summary>
    public sealed class C37Server : IC37Server, IAsyncDisposable
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<C37Server> _logger;


        /// <summary>
        /// TCP listener for the C37 server.
        /// </summary>
        private readonly TcpListener _listener;

        /// <summary>
        /// List of PMU simulation data.
        /// </summary>
        private readonly IReadOnlyList<IPmuDataSimulation> _simulations;


        /// <summary>
        /// Data rate to send data frames with.
        /// </summary>
        private readonly ushort _dataRate;


        /// <summary>
        /// Shuts down acceptance of client connection requests.
        /// </summary>
        private CancellationTokenSource _shutdownClientConnects = new CancellationTokenSource();


        /// <summary>
        /// Client acceptance task.
        /// </summary>
        private Task? _acceptTask;


        /// <summary>
        /// Client sessions.
        /// </summary>
        private readonly List<Task> _clientSessions = [];


        /// <inheritdoc/>
        public C37ServerState State { get; private set; } = C37ServerState.Idle;


        /// <inheritdoc/>
        public event EventHandler<EventArgs>? OnCommunicationError;


        /// <summary>
        /// Constructor of <see cref="C37Server"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="simulations">List of PMU simulations.</param>
        /// <param name="port">Port of the C37 server.</param>
        /// <param name="dataRate">Data rate in which the server streams the data.</param>
        public C37Server(ILogger<C37Server> logger, IReadOnlyList<IPmuDataSimulation> simulations, int port, ushort dataRate)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(port);
            ArgumentNullException.ThrowIfNull(simulations);
            ArgumentOutOfRangeException.ThrowIfZero(simulations.Count);
            ArgumentOutOfRangeException.ThrowIfZero(dataRate);

            _logger = logger;
            _listener = new TcpListener(IPAddress.Any, port);
            _simulations = simulations;
            _dataRate = dataRate;
        }


        /// <inheritdoc/>
        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (State != C37ServerState.Idle)
            {
                return Task.CompletedTask;
            }

            State = C37ServerState.Starting;

            try
            {
                _listener.Start();
                _shutdownClientConnects = new CancellationTokenSource();
                State = C37ServerState.Started;
                _logger.LogInformation("C37 server started. {Address}", _listener.LocalEndpoint);
                _acceptTask = AcceptClientsAsync(_shutdownClientConnects.Token);
            }
            catch (Exception ex)
            {
                State = C37ServerState.Idle;
                RaiseCommunicationError();
                _logger.LogError("Unexpected C37 server exception occurred {Address}. {Error}", _listener?.LocalEndpoint, ex);
                throw;
            }

            return Task.CompletedTask;
        }


        /// <inheritdoc/>
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (State == C37ServerState.Idle)
            {
                return;
            }

            _shutdownClientConnects.Cancel();
            _listener.Stop();

            if (_acceptTask is not null)
            {
                try
                {
                    await _acceptTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Expected when canceled
                }
                catch (SocketException)
                {
                    // Possible when canceled
                }
            }

            Task[] sessions = _clientSessions.ToArray();

            try
            {
                await Task.WhenAll(sessions).WaitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (_shutdownClientConnects.IsCancellationRequested)
            {
                // Expected when active streaming sessions are canceled during shutdown.
            }

            State = C37ServerState.Idle;
        }


        /// <summary>
        /// Accepts new client connections.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task</returns>
        private async Task AcceptClientsAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (cancellationToken.IsCancellationRequested == false)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
                    Task session = RunSessionAsync(client, cancellationToken);

                    lock (_clientSessions)
                    {
                        _clientSessions.Add(session);
                    }
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Expected when canceled
            }
            catch (SocketException) when (cancellationToken.IsCancellationRequested)
            {
                // Possible when canceled
            }
        }


        /// <summary>
        /// Run a client session and send data simulations via a configured data rate.
        /// </summary>
        /// <param name="client">The TCP local client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task</returns>
        /// <remarks>At this point the client sessions never get cleaned up when a client disconnect.</remarks>
        private async Task RunSessionAsync(TcpClient client, CancellationToken cancellationToken)
        {
            using (client)
            {
                using (NetworkStream stream = client.GetStream())
                {
                    await stream.WriteAsync(C37FrameCodec.CreateConfigurationFrame(_simulations, _dataRate), cancellationToken).ConfigureAwait(false);
                    // DataRate | Math | Interval | Milliseconds
                    //   60     | 1/60 | 0.01666s | 16.67ms
                    //   50     | 1/50 | 0.2s     | 20.00ms
                    //   30     | 1/30 | 0.03333s | 33.33ms
                    //   25     | 1/25 | 0.04s    | 40.00ms
                    //   10     | 1/10 | 0.1s     | 100.00ms
                    //   1      | 1/1  | 1s       | 1s
                    TimeSpan interval = TimeSpan.FromSeconds(1d / _dataRate);
                    State = C37ServerState.Streaming;

                    try
                    {
                        while (cancellationToken.IsCancellationRequested == false)
                        {
                            foreach (IPmuDataSimulation simulation in _simulations)
                            {
                                await stream.WriteAsync(C37FrameCodec.CreateDataFrame(simulation), cancellationToken).ConfigureAwait(false);
                            }

                            await Task.Delay(interval, cancellationToken).ConfigureAwait(false);
                        }
                    }
                    catch (IOException ex) when (ex.InnerException is SocketException socketEx && socketEx.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        _logger.LogInformation("C37 client disconnected/reset the connection. {Server}", _listener?.LocalEndpoint);
                    }
                }
            }
        }


        /// <summary>
        /// Raise communication error.
        /// </summary>
        private void RaiseCommunicationError()
        {
            OnCommunicationError?.Invoke(this, EventArgs.Empty);
        }


        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await StopAsync().ConfigureAwait(false);
            _shutdownClientConnects.Dispose();
        }
    }
}

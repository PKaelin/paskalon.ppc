// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
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
        public C37ServerState State { get; private set; } = C37ServerState.Disconnected;


        /// <summary>
        /// Constructor of <see cref="C37Server"/>.
        /// </summary>
        public C37Server(int port, IReadOnlyList<IPmuDataSimulation> simulations, ushort dataRate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(port);
            ArgumentNullException.ThrowIfNull(simulations);
            ArgumentOutOfRangeException.ThrowIfZero(simulations.Count);
            ArgumentOutOfRangeException.ThrowIfZero(dataRate);

            _listener = new TcpListener(IPAddress.Any, port);
            _simulations = simulations;
            _dataRate = dataRate;
        }


        /// <inheritdoc/>
        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (State != C37ServerState.Disconnected)
            {
                return Task.CompletedTask;
            }

            _listener.Start();
            _shutdownClientConnects = new CancellationTokenSource();
            State = C37ServerState.Connected;
            _acceptTask = AcceptClientsAsync(_shutdownClientConnects.Token);

            return Task.CompletedTask;
        }


        /// <inheritdoc/>
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (State == C37ServerState.Disconnected)
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
            await Task.WhenAll(sessions).WaitAsync(cancellationToken).ConfigureAwait(false);
            State = C37ServerState.Disconnected;
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
                    TimeSpan interval = TimeSpan.FromSeconds(1d / _dataRate);
                    State = C37ServerState.Streaming;

                    while (cancellationToken.IsCancellationRequested == false)
                    {
                        foreach (IPmuDataSimulation simulation in _simulations)
                        {
                            await stream.WriteAsync(C37FrameCodec.CreateDataFrame(simulation), cancellationToken).ConfigureAwait(false);
                        }

                        await Task.Delay(interval, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
        }


        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await StopAsync().ConfigureAwait(false);
            _shutdownClientConnects.Dispose();
        }
    }
}

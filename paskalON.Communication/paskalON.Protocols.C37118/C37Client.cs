// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface.C37s;
using paskalON.Protocols.C37118.Frames;

namespace paskalON.Protocols.C37118
{
    public class C37Client : IC37Client
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<C37Client> _logger;

        public C37ClientState State { get; set; } = C37ClientState.Disconnected;

        public string ServerAddress { get; set; }

        public int ServerPort { get; set; }

        public event EventHandler<EventArgs>? OnCommunicationError;
        public event EventHandler<C37DataFrameEventArgs>? DataFrameReceived;
        public event EventHandler<C37ConfigFrameEventArgs>? ConfigFrameReceived;
        public event EventHandler<Exception>? ConnectionError;


        public C37Client(ILogger<C37Client> logger, string serverAddress, int serverPort)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(serverAddress);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(serverPort);

            _logger = logger;
            ServerAddress = serverAddress;
            ServerPort = serverPort;
        }

        public Task RequestConfigurationAsync()
        {
            OnCommunicationError?.Invoke(this, new EventArgs());
            DataFrameReceived?.Invoke(this, new C37DataFrameEventArgs(null!));
            ConfigFrameReceived?.Invoke(this, new C37ConfigFrameEventArgs(null!));
            ConnectionError?.Invoke(this, null!);

            return Task.CompletedTask;
        }

        public Task SendCommandAsync(C37CommandType command)
        {
            return Task.CompletedTask;
        }

        public Task StartStreamingAsync()
        {
            return Task.CompletedTask;
        }

        public Task StopStreamingAsync()
        {
            return Task.CompletedTask;
        }
    }
}

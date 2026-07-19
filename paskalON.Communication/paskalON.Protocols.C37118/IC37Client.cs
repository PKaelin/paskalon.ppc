// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Dataface.C37s;
using paskalON.Protocols.C37118.Frames;

namespace paskalON.Protocols.C37118
{
    public interface IC37Client
    {
        event EventHandler<EventArgs> OnCommunicationError;
        C37ClientState State { get; }
        string ServerAddress { get; }
        int ServerPort { get; }

        // Connection Management
        Task StartStreamingAsync();
        Task StopStreamingAsync();


        // Protocol Specific Commands
        Task SendCommandAsync(C37CommandType command);
        Task RequestConfigurationAsync();


        event EventHandler<C37DataFrameEventArgs> DataFrameReceived;
        event EventHandler<C37ConfigFrameEventArgs> ConfigFrameReceived;
        event EventHandler<Exception> ConnectionError;

    }
}

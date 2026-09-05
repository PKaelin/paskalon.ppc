// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.C37s;
using paskalON.Protocols.C37118.Frames;

namespace paskalON.Protocols.C37118
{
    /// <summary>
    /// Interface definition of the C37 client.
    /// </summary>
    public interface IC37Client
    {
        /// <summary>
        /// Triggered when a data frame has been received.
        /// </summary>
        event EventHandler<C37DataFrameEventArgs> DataFrameReceived;


        /// <summary>
        /// Triggered when a configuration frame has been received.
        /// </summary>
        event EventHandler<C37ConfigFrameEventArgs> ConfigFrameReceived;


        /// <summary>
        /// Triggered when a communication error occurred.
        /// </summary>
        event EventHandler<EventArgs> OnCommunicationError;


        /// <summary>
        /// Client state.
        /// </summary>
        C37ClientState State { get; }


        /// <summary>
        /// Server address the client connects to.
        /// </summary>
        string ServerAddress { get; }


        /// <summary>
        /// Server port the client connects to.
        /// </summary>
        int ServerPort { get; }


        /// <summary>
        /// Starts streaming.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task</returns>
        Task StartStreamingAsync(CancellationToken cancellationToken = default);


        /// <summary>
        /// Stops streaming.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task</returns>
        Task StopStreamingAsync(CancellationToken cancellationToken = default);


        /// <summary>
        /// Sends a command to the stream.
        /// </summary>
        /// <param name="command">The command to send.</param>
        /// <returns>Task</returns>
        Task SendCommandAsync(C37CommandType command);


        /// <summary>
        /// Requests a configuration from the server.
        /// </summary>
        /// <returns>Task</returns>
        Task RequestConfigurationAsync();
    }
}

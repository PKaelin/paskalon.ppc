// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.C37118
{
    /// <summary>
    /// Definition of the C37 server.
    /// </summary>
    public interface IC37Server
    {
        /// <summary>
        /// Current server state.
        /// </summary>
        C37ServerState State { get; }


        /// <summary>
        /// Starts accepting C37 clients.
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken = default);


        /// <summary>
        /// Stops the server and active client sessions.
        /// </summary>
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Net.Sockets;

namespace paskalON.Protocols.Modbus.Configs
{
    /// <summary>
    /// Client connection configuration.
    /// </summary>
    public record ClientConnectionConfig
    {
        /// <summary>
        /// Server address the client connects to.
        /// </summary>
        public required string ServerAddress { get; init; }


        /// <summary>
        /// Server port the client connects to.
        /// </summary>
        public required int ServerPort { get; init; }


        /// <summary>
        /// Address family to connect with. Default is IP4.
        /// </summary>
        public AddressFamily AddressFamily { get; init; }


        /// <summary>
        /// Wait time for to client to be successfully connected before raising an error.
        /// </summary>
        public required int ConnectionTimeoutMilliseconds { get; init; }


        /// <summary>
        /// Wait time for to client to be successfully disconnected before raising an error.
        /// </summary>
        public required int DisconnectionTimeoutMilliseconds { get; init; }


        /// <summary>
        /// How many times the client tries to reconnect or when negative (-1) then endless retry or 0 connects once.
        /// When this is negative this is equivalent with maintain connection.
        /// </summary>
        public required int ConnectRetryCount { get; init; }


        /// <summary>
        /// How long to wait before retrying to connect in milliseconds.
        /// </summary>
        public required int ConnectRetryIntervalMilliseconds { get; init; }


        /// <summary>
        /// Timeout for sending/reading a response in milliseconds or -1 when no timeout
        /// </summary>
        public required int OperationTimeoutMilliseconds { get; init; }


        /// <summary>
        /// How many times the client tries to send a failed send
        /// </summary>
        public required int SendRetryCount { get; init; }


        /// <summary>
        /// How long to wait before retrying to send in milliseconds.
        /// </summary>
        public required int SendRetryIntervalMilliseconds { get; init; }
    }
}

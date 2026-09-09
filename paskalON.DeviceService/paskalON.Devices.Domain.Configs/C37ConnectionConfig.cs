// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.Devices.Domain.Configs
{
    /// <summary>
    /// Configuration class for a host connections.
    /// </summary>
    public class C37ConnectionConfig : NameBase
    {
        /// <summary>
        /// Wait time for to client to be successfully connected before raising an error.
        /// </summary>
        public int ConnectionTimeoutMilliseconds { get; set; } = 5000;


        /// <summary>
        /// Wait time for to client to be successfully disconnected before raising an error.
        /// </summary>
        public int DisconnectionTimeoutMilliseconds { get; set; } = 10000;


        /// <summary>
        /// How many times the client tries to reconnect or when negative (-1) then endless retry or 0 connects once.
        /// When this is negative this is equivalent with maintain connection.
        /// </summary>
        public int ConnectRetryCount { get; set; } = 3;


        /// <summary>
        /// How long to wait before retrying to connect in milliseconds.
        /// </summary>
        public int ConnectRetryIntervalMilliseconds { get; set; } = 5000;


        /// <summary>
        /// Timeout for sending/reading a response in milliseconds or -1 when no timeout
        /// </summary>
        public int OperationTimeoutMilliseconds { get; set; } = 30000;
    }
}

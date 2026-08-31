// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.Devices.Domain.Configs
{
    /// <summary>
    /// Configuration class for a host poll connections 
    /// </summary>
    public class ModbusConnectionConfig : NameBase
    {
        /// <summary>
        /// Polling interval in milliseconds.
        /// </summary>
        public long PollingIntervalMilliseconds { get; set; }


        /// <summary>
        /// Polling factor for class 1 Modbus endpoints.
        /// </summary>
        /// <remarks>
        /// The factor is multiplied by the PollingIntervalMilliseconds.
        /// </remarks>
        /// <example>
        /// PollingIntervalMilliseconds = 1000, PollingFactorClass1 = 1 means every 1 second class 1 endpoints get polled.
        /// </example>
        public int PollingFactorClass1 { get; set; } = 1;


        /// <summary>
        /// Polling factor for class 2 Modbus endpoints.
        /// </summary>
        /// <remarks>
        /// The factor is multiplied by the PollingIntervalMilliseconds.
        /// </remarks>
        /// <example>
        /// PollingIntervalMilliseconds = 1000, PollingFactorClass2 = 3 means every 3 second class 2 endpoints get polled.
        /// </example>
        public int PollingFactorClass2 { get; set; } = 3;


        /// <summary>
        /// Polling factor for class 3 Modbus endpoints.
        /// </summary>
        /// <remarks>
        /// The factor is multiplied by the PollingIntervalMilliseconds.
        /// </remarks>
        /// <example>
        /// PollingIntervalMilliseconds = 1000, PollingFactorClass2 = 10 means every 10 second class 3 endpoints get polled.
        /// </example>
        public int PollingFactorClass3 { get; set; } = 10;


        /// <summary>
        /// Polling factor for class 4 Modbus endpoints.
        /// </summary>
        /// <remarks>
        /// The factor is multiplied by the PollingIntervalMilliseconds.
        /// </remarks>
        /// <example>
        /// PollingIntervalMilliseconds = 1000, PollingFactorClass2 = 30 means every 30 second class 4 endpoints get polled.
        /// </example>
        public int PollingFactorClass4 { get; set; } = 30;


        /// <summary>
        /// Polling factor for class 5 Modbus endpoints.
        /// </summary>
        /// <remarks>
        /// The factor is multiplied by the PollingIntervalMilliseconds.
        /// </remarks>
        /// <example>
        /// PollingIntervalMilliseconds = 1000, PollingFactorClass2 = 300 means every 300 second class 5 endpoints get polled.
        /// </example>
        public int PollingFactorClass5 { get; set; } = 300;


        /// <summary>
        /// Master HeartBeat interval in milliseconds.
        /// </summary>
        public long MasterHeartBeatIntervalMilliseconds { get; set; }


        /// <summary>
        /// Whether this device should utilize connection pipelining.
        /// </summary>
        /// <remarks>
        /// Pipelining is a non-standard connection feature that allows a master/client to issue multiple requests without waiting for a response.
        /// This is not a ubiquitous feature, though many devices do support it.
        /// Pipelining cannot be enabled after a network master has been initialized.
        /// Many slaves expect requests to be sent in a half-duplex fashion on a given connection.
        /// That is, they expect the master to wait for the response to a request that was sent
        /// before issuing another request.  For performance reasons some slaves allow multiple
        /// requests to be sent on the same connection, each of which will be processed in parallel.
        /// This feature is referred to as "pipelining".  
        /// Set this property to true if your server can handle pipelined requests (multiple requests).
        /// </remarks>
        public bool IsPipeliningEnabled { get; set; }


        /// <summary>
        /// Wait time for to client to be successfully connected before raising an error.
        /// </summary>
        public int ConnectionTimeoutMilliseconds { get; set; } = 30000;


        /// <summary>
        /// Wait time for to client to be successfully disconnected before raising an error.
        /// </summary>
        public int DisconnectionTimeoutMilliseconds { get; set; } = 30000;


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
        /// Timeout for sending a response in milliseconds or -1 when no timeout
        /// </summary>
        public int SendTimeoutMilliseconds { get; set; } = 30000;


        /// <summary>
        /// How many times the client tries to send a failed send
        /// </summary>
        public int SendRetryCount { get; set; } = 2;


        /// <summary>
        /// How long to wait before retrying to send in milliseconds.
        /// </summary>
        public int SendRetryIntervalMilliseconds { get; set; } = 5000;


        /// <summary>
        /// Interval to check whether clients are still alive in seconds or -1 never check.
        /// </summary>
        public int ServerToClientAliveIntervalSeconds { get; set; } = -1;


        /// <summary>
        /// Maximum connections allowed on this server or -1 for unlimited.
        /// </summary>
        public int ServerMaximumConnections { get; set; } = 10;

    }
}

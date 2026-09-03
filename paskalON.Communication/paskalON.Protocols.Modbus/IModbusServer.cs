// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Protocols.Modbus.Converters;

namespace paskalON.Protocols.Modbus
{
    /// <summary>
    /// Modbus server interface definition.
    /// </summary>
    public interface IModbusServer : IModbusDataConverter
    {
        /// <summary>
        /// Raised when a communication error occurs.
        /// </summary>
        event EventHandler<EventArgs> OnCommunicationError;


        /// <summary>
        /// Modbus server state.
        /// </summary>
        ModbusServerState State { get; }

        /// <summary>
        /// Modbus server listen address.
        /// </summary>
        string ListenAddress { get; }


        /// <summary>
        /// Modbus server listen port.
        /// </summary>
        int ListenPort { get; }


        /// <summary>
        /// Modbus server unit ID.
        /// </summary>
        byte UnitId { get; }


        /// <summary>
        /// Starts the Modbus server and begins listening for incoming connections.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task</returns>
        Task StartAsync(CancellationToken cancellationToken = default);


        /// <summary>
        /// Stops the Modbus server and ceases listening for incoming connections.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task</returns>
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}

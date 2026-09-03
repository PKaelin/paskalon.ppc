// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Equipments.Modbus
{
    /// <summary>
    /// Interface for the Modbus polling engine
    /// </summary>
    public interface IModbusPollingEngine
    {
        /// <summary>
        /// Gets the Modbus destination address (IP or hostname + Port).
        /// </summary>
        string ModbusPollingDestination { get; }


        /// <summary>
        /// Connect the Modbus client to the server.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>Task</returns>
        Task ConnectAsync(CancellationToken cancellationToken);


        /// <summary>
        /// Executes a asynchronous poll of the Modbus definition. 
        /// </summary>
        /// <param name="currentInterval">Current interval of the poll.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>Task</returns>
        Task PollAsync(int currentInterval, CancellationToken cancellationToken);
    }
}

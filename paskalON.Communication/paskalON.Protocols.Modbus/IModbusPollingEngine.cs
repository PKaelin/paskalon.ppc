// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Protocols.Modbus
{
    /// <summary>
    /// Interface for the Modbus polling engine
    /// </summary>
    public interface IModbusPollingEngine
    {
        /// <summary>
        /// Executes a asynchronous poll of the Modbus definition. 
        /// </summary>
        /// <param name="currentInterval">Current interval of the poll.</param>
        /// <returns></returns>
        Task PollAsync(int currentInterval);
    }
}

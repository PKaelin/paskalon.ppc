// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Equipments.Modbus
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
        /// <param name="cancellationToken">Cancelation token.</param>
        /// <returns></returns>
        Task PollAsync(int currentInterval, CancellationToken cancellationToken);
    }
}

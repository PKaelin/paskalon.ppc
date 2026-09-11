// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain
{
    /// <summary>
    /// Heartbeat interface definition.
    /// </summary>
    /// <remarks>
    /// Heartbeat is a check or action to ensure the connection between a device controller and
    /// the actual device is still ok.
    /// </remarks>
    public interface IDeviceHeartbeat
    {
        /// <summary>
        /// Method to check or execute action.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task HeartbeatAsync(CancellationToken cancellationToken);
    }
}

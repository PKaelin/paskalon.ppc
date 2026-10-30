// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Luna.Application.Dependencies
{
    /// <summary>
    /// Names of the dependencies of the Luna service registered in the dependency health monitor.
    /// </summary>
    public static class DependencyNames
    {
        /// <summary>
        /// The message broker (Redis), lost while disconnected.
        /// </summary>
        public const string Redis = "Redis";


        /// <summary>
        /// The operating mode service, lost without heartbeats.
        /// </summary>
        public const string OperatingMode = "OperatingMode";


        /// <summary>
        /// The power control service, lost without heartbeats.
        /// </summary>
        public const string PowerControl = "PowerControl";


        /// <summary>
        /// The device service, lost without device core data.
        /// </summary>
        public const string DeviceService = "DeviceService";
    }
}

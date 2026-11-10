// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Application.Dependencies
{
    /// <summary>
    /// Names of the dependencies of the operating mode service registered in the dependency health monitor.
    /// </summary>
    public static class DependencyNames
    {
        /// <summary>
        /// The message broker (Redis), lost while disconnected.
        /// </summary>
        public const string Redis = "Redis";


        /// <summary>
        /// The power control service, lost without heartbeats.
        /// </summary>
        public const string PowerControl = "PowerControl";


        /// <summary>
        /// The device service, lost without device core data.
        /// </summary>
        public const string DeviceService = "DeviceService";


        /// <summary>
        /// The system power meter at the point of interconnection, lost while no system power meter delivers valid data.
        /// </summary>
        public const string SystemPowerMeter = "SystemPowerMeter";
    }
}

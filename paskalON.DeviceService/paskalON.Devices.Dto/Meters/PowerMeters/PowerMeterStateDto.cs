// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.Meters.PowerMeters
{
    /// <summary>
    /// Power meter states published by the device service.
    /// </summary>
    /// <remarks>
    /// Mirrors PowerMeterState (paskalON.Devices.Domain) with the same names and numbers, mapped in the application layer.
    /// </remarks>
    public enum PowerMeterStateDto
    {
        /// <summary>
        /// The current state of the power meter is not determined or is undefined.
        /// </summary>
        Unknown = 0,


        /// <summary>
        /// The power meter is disconnected.
        /// </summary>
        Disconnected = 1,


        /// <summary>
        /// The power meter is establishing a connection.
        /// </summary>
        Connecting = 2,


        /// <summary>
        /// The power meter is connected and fully operational.
        /// </summary>
        Connected = 3,


        /// <summary>
        /// The power meter is disconnecting.
        /// </summary>
        Disconnecting = 4
    }
}

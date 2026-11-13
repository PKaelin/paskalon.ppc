// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Dto.EnergyResources.Solars
{
    /// <summary>
    /// Solar panel states published by the device service.
    /// </summary>
    /// <remarks>
    /// Mirrors SolarPanelState (paskalON.Devices.Domain) with the same names and numbers, mapped in the application layer.
    /// </remarks>
    public enum SolarPanelStateDto
    {
        /// <summary>
        /// The communication with the solar panel is lost.
        /// </summary>
        Disconnected = 0,


        /// <summary>
        /// The solar panel is establishing a connection.
        /// </summary>
        Connecting = 1,


        /// <summary>
        /// The solar panel is connected and fully operational.
        /// </summary>
        Connected = 2,


        /// <summary>
        /// The solar panel is disconnecting.
        /// </summary>
        Disconnecting = 3
    }
}

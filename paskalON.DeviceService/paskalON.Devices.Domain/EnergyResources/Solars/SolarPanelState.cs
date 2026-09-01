// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.EnergyResources.Solars
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Solar panel states.
    /// </summary>
    /// <remarks>
    /// Underlying state have to be mapped to these states.
    /// </remarks>
    public enum SolarPanelState
    {
        /// <summary>
        /// Disconnected" state means the supervisory controller (Master/Client) has lost active communication
        /// with the field device (Slave/Server).
        /// </summary>
        Disconnected = 0,
        /// <summary>
        /// Connecting means that the solar panel is in the process of establishing a connection.
        /// </summary>
        Connecting = 1,
        /// <summary>
        /// Standby means the solar panel is connected and fully operational. 
        /// </summary>
        Connected = 2,
        /// <summary>
        /// Disconnecting means that the solar panel is in the process of disconnecting.
        /// </summary>
        Disconnecting = 3
    }
}

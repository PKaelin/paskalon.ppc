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
    /// Solar panel device interface for the instances that communicate with the device.
    /// </summary>
    /// <typeparam name="T">The type of the solar device.</typeparam>
    public interface ISolarPanel : IDevice
    {
        /// <summary>
        /// Connects the solar panel and starts communicating once in state connected.
        /// </summary>
        Task ConnectAsync();


        /// <summary>
        /// Disconnects the solar panel after it stops communicating.
        /// </summary>
        Task DisconnectAsync();
    }
}

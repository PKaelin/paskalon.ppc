// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
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
    public interface ISolarPanel<T> : IDevice<T>
    {
        /// <summary>
        /// Connects the solar panel and starts communicating once in state connected.
        /// </summary>
        void Connect();


        /// <summary>
        /// Disconnects the solar panel after it stops communicating.
        /// </summary>
        void Disconnect();
    }
}

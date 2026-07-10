// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.EnergyStorages.Batteries
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Battery bank interface for the instances that communicate with the device.
    /// </summary>
    public interface IBatteryBank : IDevice
    {
        /// <summary>
        /// Connects the battery bank and starts communicating once in state connected.
        /// </summary>
        Task ConnectAsync();


        /// <summary>
        /// Disconnects the battery bank after it stops communicating.
        /// </summary>
        Task DisconnectAsync();
    }
}

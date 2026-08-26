// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Dataface;

namespace paskalON.Devices.Domain
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Base interface for all DER devices providing access to metrics publishing and data setting functionalities.
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// Device Id of the device.
        /// </summary>
        int DeviceId { get; }


        /// <summary>
        /// Data setters for total loose coupled interfaces.
        /// </summary>
        IDataface Dataface { get; }


        /// <summary>
        /// Check the health of the device.
        /// </summary>
        /// <returns>
        /// Returns a task as its implemented asynchronously.
        /// </returns>
        Task CheckHealthAsync();
    }
}

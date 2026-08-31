// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Domain.Meters.PowerMeters
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Power meter interface for the instances that communicate with the device.
    /// </summary>
    public interface IPowerMeter : IDevice
    {
        /// <summary>
        /// Connects the power meter and starts communicating once in state connected.
        /// </summary>
        Task ConnectAsync();


        /// <summary>
        /// Disconnects the power meter after it stops communicating.
        /// </summary>
        Task DisconnectAsync();
    }
}

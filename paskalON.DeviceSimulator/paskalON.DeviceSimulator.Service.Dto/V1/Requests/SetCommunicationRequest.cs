// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.DeviceSimulator.Dto.Simulations;

namespace paskalON.DeviceSimulator.Service.Dto.V1.Requests
{
    /// <summary>
    /// Starts or stops answering Modbus requests of a simulated PCS or battery bank (communication loss).
    /// </summary>
    public class SetCommunicationRequest
    {
        /// <summary>
        /// Type of the simulated device.
        /// </summary>
        public SimulationDeviceType DeviceType { get; set; }


        /// <summary>
        /// Device ID of the simulated device (unique within the device type).
        /// </summary>
        public int DeviceId { get; set; }


        /// <summary>
        /// True to answer Modbus requests, false to stop answering them (communication loss).
        /// </summary>
        public bool IsResponding { get; set; }
    }
}

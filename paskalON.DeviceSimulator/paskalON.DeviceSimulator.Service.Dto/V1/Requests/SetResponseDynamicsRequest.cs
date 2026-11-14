// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Service.Dto.V1.Requests
{
    /// <summary>
    /// Sets the dead time and the first-order lag of the active and reactive power response of a simulated PCS.
    /// </summary>
    public class SetResponseDynamicsRequest
    {
        /// <summary>
        /// Device ID of the simulated PCS.
        /// </summary>
        public int DeviceId { get; set; }


        /// <summary>
        /// Time in milliseconds after which the PCS starts to react to a new setpoint (0..10000). 0 means no delay (ideal).
        /// </summary>
        public double DeadTimeMilliseconds { get; set; }


        /// <summary>
        /// Time constant in milliseconds of the first-order lag (0..60000): 63% of a setpoint step after one time constant. 0 means instant (ideal).
        /// </summary>
        public double TimeConstantMilliseconds { get; set; }
    }
}

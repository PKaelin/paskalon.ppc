// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Service.Dto.Pcs
{
    /// <summary>
    /// Slows down the active and reactive power response of a simulated PCS.
    /// </summary>
    public class SlowDownPowerRequest
    {
        /// <summary>
        /// Device ID of the simulated PCS.
        /// </summary>
        public int DeviceId { get; set; }


        /// <summary>
        /// Maximum active power change per second in percent of the nameplate active power. 0 means instant (ideal).
        /// </summary>
        public double SlowActivePowerByPercent { get; set; }


        /// <summary>
        /// Maximum reactive power change per second in percent of the nameplate reactive power. 0 means instant (ideal).
        /// </summary>
        public double SlowReactivePowerByPercent { get; set; }
    }
}

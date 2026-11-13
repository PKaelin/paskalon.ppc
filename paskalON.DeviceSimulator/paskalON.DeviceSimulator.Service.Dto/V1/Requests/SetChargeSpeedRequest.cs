// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Service.Dto.V1.Requests
{
    /// <summary>
    /// Sets how much faster a simulated battery bank charges and discharges than its physical definition.
    /// </summary>
    public class SetChargeSpeedRequest
    {
        /// <summary>
        /// Device ID of the simulated battery bank.
        /// </summary>
        public int DeviceId { get; set; }


        /// <summary>
        /// Charge speed factor from 1 (real time) to 100 (one real second charges or discharges like 100 seconds).
        /// </summary>
        public double ChargeSpeedFactor { get; set; }
    }
}

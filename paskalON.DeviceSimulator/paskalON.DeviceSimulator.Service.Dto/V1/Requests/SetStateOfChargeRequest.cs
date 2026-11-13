// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Service.Dto.V1.Requests
{
    /// <summary>
    /// Sets the state of charge of a simulated battery bank.
    /// </summary>
    public class SetStateOfChargeRequest
    {
        /// <summary>
        /// Device ID of the simulated battery bank.
        /// </summary>
        public int DeviceId { get; set; }


        /// <summary>
        /// Preferred (operator window) state of charge in percent (0..100).
        /// </summary>
        public double PreferredStateOfCharge { get; set; }
    }
}

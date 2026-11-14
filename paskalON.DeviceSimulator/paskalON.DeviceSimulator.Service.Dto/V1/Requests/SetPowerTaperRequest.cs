// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Service.Dto.V1.Requests
{
    /// <summary>
    /// Enables or disables the power taper of a simulated battery bank near empty and full.
    /// </summary>
    public class SetPowerTaperRequest
    {
        /// <summary>
        /// Device ID of the simulated battery bank.
        /// </summary>
        public int DeviceId { get; set; }


        /// <summary>
        /// True to taper the available power below 10% and above 90% usable state of charge, false to cut off hard at 0% and 100%.
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}

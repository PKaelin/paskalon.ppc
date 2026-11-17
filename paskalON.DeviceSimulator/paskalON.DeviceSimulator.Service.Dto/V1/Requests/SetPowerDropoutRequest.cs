// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Service.Dto.V1.Requests
{
    /// <summary>
    /// Switches the random dropout of the active and reactive power updates of a simulated system power meter on or off.
    /// A dropped update keeps the last reported value (telemetry impairment).
    /// </summary>
    public class SetPowerDropoutRequest
    {
        /// <summary>
        /// Device ID of the simulated system power meter.
        /// </summary>
        public int DeviceId { get; set; }


        /// <summary>
        /// True to drop active power updates randomly, false to report every update.
        /// </summary>
        public bool IsActivePowerDropoutEnabled { get; set; }


        /// <summary>
        /// Chance in percent (0..100) that an active power update is dropped.
        /// </summary>
        public double ActivePowerDropoutPercent { get; set; }


        /// <summary>
        /// True to drop reactive power updates randomly, false to report every update.
        /// </summary>
        public bool IsReactivePowerDropoutEnabled { get; set; }


        /// <summary>
        /// Chance in percent (0..100) that a reactive power update is dropped.
        /// </summary>
        public double ReactivePowerDropoutPercent { get; set; }
    }
}

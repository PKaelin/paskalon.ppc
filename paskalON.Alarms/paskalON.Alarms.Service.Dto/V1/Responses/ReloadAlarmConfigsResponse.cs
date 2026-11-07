// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Service.Dto.V1.Responses
{
    /// <summary>
    /// Response class of a reload of the alarm configurations.
    /// </summary>
    public class ReloadAlarmConfigsResponse
    {
        /// <summary>
        /// Gets or sets the number of loaded enabled alarm configurations.
        /// </summary>
        public required int ConfigCount { get; set; }


        /// <summary>
        /// Gets or sets the number of normalized active alarms whose configuration was removed or disabled.
        /// </summary>
        public required int NormalizedAlarmCount { get; set; }
    }
}

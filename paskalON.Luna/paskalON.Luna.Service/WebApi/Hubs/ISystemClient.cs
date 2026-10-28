// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Luna.Dto.Devices;

namespace paskalON.Luna.Service.WebApi.Hubs
{
    /// <summary>
    /// Messages the system hub sends to its clients (see useSystemMeterSummary.ts of paskalON.Luna.Web).
    /// </summary>
    public interface ISystemClient
    {
        /// <summary>
        /// Current summary of the system power meter.
        /// </summary>
        /// <param name="summary">The summary, or null if no system power meter delivers data.</param>
        /// <returns>Task</returns>
        Task SystemMeterSummaryUpdated(SystemMeterSummaryDto? summary);
    }
}

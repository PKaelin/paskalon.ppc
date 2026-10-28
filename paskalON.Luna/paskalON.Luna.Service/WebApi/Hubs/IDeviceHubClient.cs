// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Meters.PowerMeters;
using paskalON.Devices.Dto.PowerConversionSystems;

namespace paskalON.Luna.Service.WebApi.Hubs
{
    /// <summary>
    /// Messages the device hub sends to its clients (see useDeviceSubscription.ts of paskalON.Luna.Web).
    /// </summary>
    public interface IDeviceHubClient
    {
        /// <summary>
        /// Current values of the subscribed system power meter.
        /// </summary>
        /// <param name="deviceId">Device ID of the subscribed system power meter.</param>
        /// <param name="powerMeter">The system power meter, or null if the DER configuration has no system power meter with the device ID.</param>
        /// <returns>Task</returns>
        Task SystemPowerMeterUpdated(int deviceId, PmSystemDto? powerMeter);


        /// <summary>
        /// Current values of the subscribed auxiliary power meter.
        /// </summary>
        /// <param name="deviceId">Device ID of the subscribed auxiliary power meter.</param>
        /// <param name="powerMeter">The auxiliary power meter, or null if the DER configuration has no auxiliary power meter with the device ID.</param>
        /// <returns>Task</returns>
        Task AuxiliaryPowerMeterUpdated(int deviceId, PmAuxiliaryDto? powerMeter);


        /// <summary>
        /// Current values of the subscribed external power meter.
        /// </summary>
        /// <param name="deviceId">Device ID of the subscribed external power meter.</param>
        /// <param name="powerMeter">The external power meter, or null if the DER configuration has no external power meter with the device ID.</param>
        /// <returns>Task</returns>
        Task ExternalPowerMeterUpdated(int deviceId, PmExternalDto? powerMeter);


        /// <summary>
        /// Current values of the subscribed power conversion system (PCS).
        /// </summary>
        /// <param name="deviceId">Device ID of the subscribed PCS.</param>
        /// <param name="pcs">The PCS, or null if no DER unit of the DER configuration has a PCS with the device ID.</param>
        /// <returns>Task</returns>
        Task PcsUpdated(int deviceId, PcsDto? pcs);
    }
}

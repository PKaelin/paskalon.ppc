// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Ders;
using paskalON.Devices.Dto.Meters.PowerMeters;
using paskalON.Devices.Dto.PowerConversionSystems;
using paskalON.Luna.Dto.Devices;

namespace paskalON.Luna.Application
{
    /// <summary>
    /// Luna device manager interface definition.
    /// </summary>
    public interface ILunaDeviceManager
    {
        /// <summary>
        /// Get the distributed energy resource (DER) device DTO.
        /// </summary>
        /// <returns>The DER device DTO.</returns>
        DerDto GetDer();


        /// <summary>
        /// Get the summary of the system power meter including the average state of charge of the battery banks.
        /// </summary>
        /// <returns>
        /// The summary of the first system power meter that is connected and has no communication error,
        /// or null if there is no such system power meter.
        /// </returns>
        SystemMeterSummaryDto? GetSystemMeterSummary();


        /// <summary>
        /// Gets a system power meter of the DER configuration.
        /// </summary>
        /// <param name="deviceId">The device ID of the system power meter.</param>
        /// <returns>The system power meter, or null if the DER configuration has no system power meter with the device ID.</returns>
        PmSystemDto? GetSystemPowerMeter(int deviceId);


        /// <summary>
        /// Gets an auxiliary power meter of the DER configuration.
        /// </summary>
        /// <param name="deviceId">The device ID of the auxiliary power meter.</param>
        /// <returns>The auxiliary power meter, or null if the DER configuration has no auxiliary power meter with the device ID.</returns>
        PmAuxiliaryDto? GetAuxiliaryPowerMeter(int deviceId);


        /// <summary>
        /// Gets an external power meter of the DER configuration.
        /// </summary>
        /// <param name="deviceId">The device ID of the external power meter.</param>
        /// <returns>The external power meter, or null if the DER configuration has no external power meter with the device ID.</returns>
        PmExternalDto? GetExternalPowerMeter(int deviceId);


        /// <summary>
        /// Gets a power conversion system (PCS) of a DER unit of the DER configuration.
        /// </summary>
        /// <param name="deviceId">The device ID of the PCS.</param>
        /// <returns>The PCS, or null if no DER unit of the DER configuration has a PCS with the device ID.</returns>
        PcsDto? GetPcs(int deviceId);
    }
}

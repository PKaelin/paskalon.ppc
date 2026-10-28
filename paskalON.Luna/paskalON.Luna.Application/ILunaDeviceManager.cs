// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Dto.Ders;
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
    }
}

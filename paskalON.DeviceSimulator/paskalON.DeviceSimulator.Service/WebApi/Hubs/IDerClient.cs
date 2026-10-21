// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.DeviceSimulator.Dto;
using paskalON.DeviceSimulator.Dto.Ders;

namespace paskalON.DeviceSimulator.Service.WebApi.Hubs
{
    /// <summary>
    /// Messages the DER hub sends to its clients (see useLiveUpdates.ts of paskalON.DeviceSimulator.Web).
    /// </summary>
    public interface IDerClient
    {
        /// <summary>
        /// Batch of complete DER units including their devices.
        /// </summary>
        /// <param name="units">The updated units.</param>
        /// <returns>Task</returns>
        Task UnitsUpdated(List<DerUnitDto> units);


        /// <summary>
        /// Batch of single devices (e.g. power meters that are not part of a unit).
        /// </summary>
        /// <param name="devices">The updated devices.</param>
        /// <returns>Task</returns>
        Task DevicesUpdated(List<DeviceUpdateDto> devices);
    }
}

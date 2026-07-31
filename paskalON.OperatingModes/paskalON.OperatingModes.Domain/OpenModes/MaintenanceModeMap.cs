// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Ders;

namespace paskalON.OperatingModes.Domain.OpenModes
{
    /// <summary>
    /// Input mapping class for maintenance mode.
    /// </summary>
    public class MaintenanceModeMap : OperatingModeBaseMap
    {
        /// <summary>
        /// DER unit to put into maintenance.
        /// </summary>
        public required Func<DerUnit> DerUnit { get; set; }
    }
}

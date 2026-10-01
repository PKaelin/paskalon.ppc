// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
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
        public required string DerUnitName { get; set; }
    }
}

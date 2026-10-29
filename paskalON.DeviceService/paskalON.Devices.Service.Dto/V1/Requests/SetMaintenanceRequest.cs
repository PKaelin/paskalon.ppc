// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.ComponentModel.DataAnnotations;

namespace paskalON.Devices.Service.Dto.V1.Requests
{
    /// <summary>
    /// Set maintenance DTO request.
    /// </summary>
    public class SetMaintenanceRequest
    {
        /// <summary>
        /// Unit name of the unit to set the maintenance mode for.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string UnitName { get; set; } = default!;


        /// <summary>
        /// True to put the unit into maintenance, false to take it out of maintenance.
        /// </summary>
        public bool IntoMaintenance { get; set; }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.ComponentModel.DataAnnotations;

namespace paskalON.Devices.Service.Dto.V1.Requests
{
    /// <summary>
    /// Put into maintenance DTO request.
    /// </summary>
    public class PutIntoMaintenanceRequest
    {
        /// <summary>
        /// Unit name of the unit to put into maintenance.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string UnitName { get; set; } = default!;
    }
}

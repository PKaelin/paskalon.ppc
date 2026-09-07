// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Service.Dto.V1.Requests
{
    /// <summary>
    /// Standby PCS DTO request.
    /// </summary>
    public class StandbyPcsRequestDto
    {
        /// <summary>
        /// Device ID of the PCS to put into standby.
        /// </summary>
        public int DeviceId { get; set; }
    }
}

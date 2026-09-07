// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Service.Dto.V1.Requests
{
    /// <summary>
    /// Start PCS DTO request.
    /// </summary>
    public class StartPcsRequestDto
    {
        /// <summary>
        /// Device ID of the PCS to start.
        /// </summary>
        public int DeviceId { get; set; }
    }
}

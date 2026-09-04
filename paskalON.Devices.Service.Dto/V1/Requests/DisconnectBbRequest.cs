// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Service.Dto.V1.Requests
{
    /// <summary>
    /// Disconnect BB DTO request.
    /// </summary>
    public class DisconnectBbRequest
    {
        /// <summary>
        /// Device ID of the BB to disconnect.
        /// </summary>
        public int DeviceId { get; set; }
    }
}

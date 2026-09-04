// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Service.Dto.V1.Requests
{
    /// <summary>
    /// Connect BB DTO request.
    /// </summary>
    public class ConnectBbRequest
    {
        /// <summary>
        /// Device ID of the BB to connect.
        /// </summary>
        public int DeviceId { get; set; }
    }
}

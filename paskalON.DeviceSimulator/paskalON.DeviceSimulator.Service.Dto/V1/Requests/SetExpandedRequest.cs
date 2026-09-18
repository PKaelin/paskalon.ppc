// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Service.Dto.V1.Requests
{
    /// <summary>
    /// Set the expanded flags for the publisher to publish data to the clients.
    /// </summary>
    /// <remarks>
    /// For performance reason we dont want to send all the updates to the clients.
    /// Only the ones that are expanded and their data actually viewable.
    /// </remarks>
    public class SetExpandedRequest
    {
        /// <summary>
        /// Name of the DER units that are expanded and need updating.
        /// </summary>
        public List<string> ExpandedUnits { get; set; } = new List<string>();


        /// <summary>
        /// Name of the DER devices that are expanded and need updating.
        /// </summary>
        public List<string> ExpandedDevices { get; set; } = new List<string>();
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Service.Dto.V1.Requests
{
    /// <summary>
    /// Request class for deactivating an override mode.
    /// </summary>
    public class DeactivateOverrideRequest
    {
        /// <summary>
        /// Gets or sets the name of the override mode to deactivate.
        /// </summary>
        public required string OperatingModeName { get; set; }
    }
}

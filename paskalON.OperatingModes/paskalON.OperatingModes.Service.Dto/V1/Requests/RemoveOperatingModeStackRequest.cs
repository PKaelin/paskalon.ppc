// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Service.Dto.V1.Requests
{
    /// <summary>
    /// Request class for removing an operating mode from the stack.
    /// </summary>
    public class RemoveOperatingModeStackRequest
    {
        /// <summary>
        /// Gets or sets the name of the operating mode to remove from the stack.
        /// </summary>
        public required string OperatingModeName { get; set; }
    }
}

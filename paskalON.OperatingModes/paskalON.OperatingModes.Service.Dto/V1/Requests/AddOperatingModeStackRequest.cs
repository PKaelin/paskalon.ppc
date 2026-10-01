// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Service.Dto.V1.Requests
{
    /// <summary>
    /// Request class for adding an operating mode to the stack.
    /// </summary>
    public class AddOperatingModeStackRequest
    {
        /// <summary>
        /// Gets or sets the name of the operating mode to add to the stack.
        /// </summary>
        public required string OperatingModeName { get; set; }


        /// <summary>
        /// Gets or sets the priority of the operating mode in the stack.
        /// </summary>
        public int Priority { get; set; }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Service.Dto.V1.Requests
{
    /// <summary>
    /// Request class for replacing an operating mode of the stack in one atomic change.
    /// </summary>
    public class ReplaceOperatingModeStackRequest
    {
        /// <summary>
        /// Gets or sets the name of the operating mode to replace.
        /// </summary>
        public required string PredecessorName { get; set; }


        /// <summary>
        /// Gets or sets the name of the replacing operating mode. It takes the priority of the replaced operating mode.
        /// </summary>
        public required string OperatingModeName { get; set; }
    }
}

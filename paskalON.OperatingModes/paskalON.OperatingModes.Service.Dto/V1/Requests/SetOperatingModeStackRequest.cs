// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Service.Dto.V1.Requests
{
    /// <summary>
    /// Request class for setting the whole operating mode stack in one atomic change.
    /// </summary>
    public class SetOperatingModeStackRequest
    {
        /// <summary>
        /// Gets or sets the names of the operating modes of the new stack in ascending priority order (first = priority 1).
        /// </summary>
        public required List<string> OperatingModeNames { get; set; }
    }
}

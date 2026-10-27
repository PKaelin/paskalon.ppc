// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Luna.Dto.Systems
{
    /// <summary>
    /// System command response data transfer object (DTO).
    /// </summary>
    public record SystemCommandResponseDto
    {
        /// <summary>
        /// Message describing the outcome of the system command, shown to the operator.
        /// </summary>
        public required string Message { get; init; }
    }
}

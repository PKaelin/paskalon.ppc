// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Text.Json.Serialization;

namespace paskalON.OperatingModes.Dto.Stacks
{
    /// <summary>
    /// Data Transfer Object (DTO) representing an operating mode stack item.
    /// </summary>
    /// <remarks>
    /// Serialized polymorphic with the type discriminator "$type", so a list of this base type keeps the properties of the derived DTOs.
    /// Every derived DTO must be registered with a <see cref="JsonDerivedTypeAttribute"/>.
    /// </remarks>
    [JsonPolymorphic]
    [JsonDerivedType(typeof(OperatingModeStackDto), "stack")]
    [JsonDerivedType(typeof(OperatingModeLimitingStackDto), "limiting")]
    public record OperatingModeStackDto
    {
        /// <summary>
        /// Gets or sets the name of the operating mode.
        /// </summary>
        public required string OperatingModeName { get; init; }


        /// <summary>
        /// Gets the operating mode stack type.
        /// </summary>
        public required OperatingModeStackTypeDto Type { get; init; }


        /// <summary>
        /// Gets the priority of the operating mode in the stack.
        /// </summary>
        /// <remarks>
        /// Only set for active operating modes, null for available operating modes that are not in the stack.
        /// </remarks>
        public int? Priority { get; init; }
    }
}

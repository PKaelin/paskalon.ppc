// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Luna.Dto.Systems
{
    /// <summary>
    /// Availability of a dependency of the Luna service data transfer object (DTO).
    /// </summary>
    public record DependencyStatusDto
    {
        /// <summary>
        /// Name of the dependency, e.g. PowerControl.
        /// </summary>
        public required string Name { get; init; }


        /// <summary>
        /// Value indicating whether the dependency is available.
        /// </summary>
        public bool IsAvailable { get; init; }
    }
}

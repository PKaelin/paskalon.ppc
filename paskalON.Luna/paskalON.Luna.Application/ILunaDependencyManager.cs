// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Luna.Dto.Systems;

namespace paskalON.Luna.Application
{
    /// <summary>
    /// Defines a manager that provides the availability of the dependencies of the Luna service.
    /// </summary>
    public interface ILunaDependencyManager
    {
        /// <summary>
        /// Gets the current availability of all dependencies.
        /// </summary>
        /// <returns>The availability of each dependency.</returns>
        IReadOnlyList<DependencyStatusDto> GetDependencyStatus();
    }
}

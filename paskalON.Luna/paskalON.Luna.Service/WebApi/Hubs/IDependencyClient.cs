// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Luna.Dto.Systems;

namespace paskalON.Luna.Service.WebApi.Hubs
{
    /// <summary>
    /// Messages the dependency hub sends to its clients (see useDependencyStatus.ts of paskalON.Luna.Web).
    /// </summary>
    public interface IDependencyClient
    {
        /// <summary>
        /// Current availability of the dependencies of the Luna service.
        /// </summary>
        /// <param name="statuses">The availability of each dependency.</param>
        /// <returns>Task</returns>
        Task DependencyStatusUpdated(IReadOnlyList<DependencyStatusDto> statuses);
    }
}

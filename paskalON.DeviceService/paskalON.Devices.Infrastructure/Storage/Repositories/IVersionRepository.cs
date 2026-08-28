// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// Version repository interface definition.
    /// </summary>
    public interface IVersionRepository
    {
        /// <summary>
        /// Gets the latest database version.
        /// </summary>
        /// <returns>The latest database version.</returns>
        Task<string> GetDatabaseVersionAsync();
    }
}

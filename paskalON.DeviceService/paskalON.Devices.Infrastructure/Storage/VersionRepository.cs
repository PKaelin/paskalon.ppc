// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using paskalON.Devices.Infrastructure.Storage.Repositories;
using paskalON.Domains;

namespace paskalON.Devices.Infrastructure.Storage
{
    /// <summary>
    /// Version repository for getting version information.
    /// </summary>
    public class VersionRepository : RepositoryBase<DeviceServiceContext>
    {
        /// <summary>
        /// Constructor of <see cref="VersionRepository"/>.
        /// </summary>
        /// <param name="logger">The logger interface for application logging and diagnostics.</param>
        /// <param name="context">The device service database context.</param>
        public VersionRepository(ILogger logger, DeviceServiceContext context)
            : base(logger, context)
        {
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task<string> GetDatabaseVersionAsync()
        {
            History? history = null;

            try
            {
                history = await Context.Histories.OrderBy(o => o.MigrationId).LastOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting database version. Most likely there hasn't been a migration executed yet. {ex.Message}");
            }

            return history?.MigrationId ?? "No version history found";
        }
    }
}

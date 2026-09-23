// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using paskalON.Domains;
using paskalON.Infrastructure.Repositories;

namespace paskalON.PowerControls.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// Version repository for getting version information.
    /// </summary>
    public class VersionRepository : RepositoryBase<PowerControlContext>, IVersionRepository
    {
        /// <summary>
        /// Constructor of <see cref="VersionRepository"/>.
        /// </summary>
        /// <param name="logger">The logger interface for application logging and diagnostics.</param>
        /// <param name="context">The power control database context.</param>
        public VersionRepository(ILogger<VersionRepository> logger, PowerControlContext context) : base(logger, context)
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

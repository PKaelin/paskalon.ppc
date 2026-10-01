// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using paskalON.Infrastructure.Repositories;
using paskalON.OperatingModes.Domain.Configs;

namespace paskalON.OperatingModes.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// Operating mode repository for managing operating mode data in the database.
    /// </summary>
    public class OperatingModeRepository : RepositoryBase<OperatingModeContext>, IOperatingModeRepository
    {
        /// <summary>
        /// Constructor of <see cref="OperatingModeRepository"/>.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="context">The operating mode context.</param>
        public OperatingModeRepository(ILogger<OperatingModeRepository> logger, OperatingModeContext context) : base(logger, context)
        {
        }


        public async Task<List<OperatingModeBaseConfig>> GetAllOperatingModes(bool isActive = true)
        {
            return await Context.OperatingModeBaseConfigs
                .Where(om => om.IsActive == isActive)
                .Include(om => om.RampConfig)
                .Include(om => om.CurveConfig)
                .ToListAsync();
        }
    }
}

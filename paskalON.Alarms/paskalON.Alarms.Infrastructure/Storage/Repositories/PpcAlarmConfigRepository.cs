// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Infrastructure.Repositories;

namespace paskalON.Alarms.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// PPC alarm configuration repository for reading the alarm configurations from the database.
    /// </summary>
    public class PpcAlarmConfigRepository : RepositoryBase<AlarmContext>, IPpcAlarmConfigRepository
    {
        /// <summary>
        /// Constructor of <see cref="PpcAlarmConfigRepository"/>.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="context">The alarm context.</param>
        public PpcAlarmConfigRepository(ILogger<PpcAlarmConfigRepository> logger, AlarmContext context) : base(logger, context)
        {
        }


        /// <inheritdoc/>
        /// <remarks>
        /// The configurations are tracked, so that raised alarms referencing them can be added in the same context.
        /// </remarks>
        public async Task<List<PpcAlarmConfig>> GetEnabledAsync(PpcAlarmType ppcAlarmType, CancellationToken cancellationToken = default)
        {
            return await Context.PpcAlarmConfigs
                .Where(config => config.IsEnabled && config.PpcAlarmType == ppcAlarmType)
                .Include(config => config.AlarmActions)
                .ThenInclude(action => action.AlarmActionParameters)
                .OrderBy(config => config.Id)
                .ToListAsync(cancellationToken);
        }


        /// <inheritdoc/>
        public async Task<List<PpcAlarmConfig>> GetAllEnabledAsync(CancellationToken cancellationToken = default)
        {
            return await Context.PpcAlarmConfigs
                .Where(config => config.IsEnabled)
                .Include(config => config.AlarmActions)
                .ThenInclude(action => action.AlarmActionParameters)
                .OrderBy(config => config.Id)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}

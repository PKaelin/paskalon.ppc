// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using paskalON.Alarms.Domain;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Infrastructure.Repositories;

namespace paskalON.Alarms.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// PPC alarm repository for persisting the alarms in the database.
    /// </summary>
    public class PpcAlarmRepository : RepositoryBase<AlarmContext>, IPpcAlarmRepository
    {
        /// <summary>
        /// Constructor of <see cref="PpcAlarmRepository"/>.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="context">The alarm context.</param>
        public PpcAlarmRepository(ILogger<PpcAlarmRepository> logger, AlarmContext context) : base(logger, context)
        {
        }


        /// <inheritdoc/>
        public async Task<List<PpcAlarm>> GetActiveAsync(PpcAlarmKey key, CancellationToken cancellationToken = default)
        {
            return await Context.PpcAlarms
                .Where(alarm => alarm.AlarmSequenceState != AlarmSequenceState.Normal)
                .Where(alarm => alarm.DeviceId == key.DeviceId && alarm.PpcAlarmConfig.PpcAlarmType == key.PpcAlarmType)
                .Include(alarm => alarm.PpcAlarmConfig)
                .ThenInclude(config => config.AlarmActions)
                .ThenInclude(action => action.AlarmActionParameters)
                .OrderBy(alarm => alarm.Id)
                .ToListAsync(cancellationToken);
        }


        /// <inheritdoc/>
        /// <remarks>
        /// The configuration of an alarm already exists, a detached configuration is attached unchanged so that it is not inserted again.
        /// </remarks>
        public void Add(IEnumerable<PpcAlarm> alarms)
        {
            ArgumentNullException.ThrowIfNull(alarms);

            foreach (PpcAlarm alarm in alarms)
            {
                // The configuration might be cached (detached) while the context already tracks the same configuration
                // (e.g. loaded with an active alarm), a second instance with the same key must not be attached.
                PpcAlarmConfig? trackedConfig = Context.PpcAlarmConfigs.Local.FirstOrDefault(config => config.Id == alarm.PpcAlarmConfig.Id);

                if (trackedConfig is null)
                {
                    Context.PpcAlarmConfigs.Attach(alarm.PpcAlarmConfig);
                }
                else if (ReferenceEquals(trackedConfig, alarm.PpcAlarmConfig) is false)
                {
                    alarm.PpcAlarmConfig = trackedConfig;
                }

                Context.PpcAlarms.Add(alarm);
            }
        }


        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await Context.SaveChangesAsync(cancellationToken);
        }
    }
}

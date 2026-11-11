// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using paskalON.Alarms.Domain;
using paskalON.Alarms.Domain.Configs;
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
        public async Task<List<PpcAlarm>> GetAllActiveAsync(CancellationToken cancellationToken = default)
        {
            return await QueryActive().ToListAsync(cancellationToken);
        }


        /// <inheritdoc/>
        public async Task<List<PpcAlarm>> GetActiveAsync(PpcAlarmKey key, CancellationToken cancellationToken = default)
        {
            return await QueryActive()
                .Where(alarm => alarm.DeviceId == key.DeviceId && alarm.PpcAlarmConfig.PpcAlarmType == key.PpcAlarmType)
                .ToListAsync(cancellationToken);
        }


        /// <inheritdoc/>
        /// <remarks>
        /// The actions of the configurations are not loaded, they are not needed to list the alarms.
        /// </remarks>
        public async Task<List<PpcAlarm>> GetAsync(PpcAlarmFilter filter, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(filter);

            DateTimeOffset createdFrom = filter.CreatedFrom.ToUniversalTime();
            IQueryable<PpcAlarm> query = Context.PpcAlarms
                .AsNoTrackingWithIdentityResolution()
                .Include(alarm => alarm.PpcAlarmConfig)
                .Where(alarm => alarm.CreatedOn >= createdFrom);

            if (filter.SeverityLevel.HasValue)
            {
                AlarmSeverity severityLevel = filter.SeverityLevel.Value;
                query = query.Where(alarm => alarm.PpcAlarmConfig.SeverityLevel == severityLevel);
            }

            if (filter.AlarmSequenceState.HasValue)
            {
                AlarmSequenceState sequenceState = filter.AlarmSequenceState.Value;
                query = query.Where(alarm => alarm.AlarmSequenceState == sequenceState);
            }

            if (string.IsNullOrWhiteSpace(filter.DeviceName) is false)
            {
                string deviceName = filter.DeviceName.Trim().ToLowerInvariant();
                query = query.Where(alarm => alarm.DeviceName != null && alarm.DeviceName.ToLower().Contains(deviceName));
            }

            return await query
                .OrderByDescending(alarm => alarm.CreatedOn)
                .ThenByDescending(alarm => alarm.Id)
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
                AttachConfig(alarm);
                Context.PpcAlarms.Add(alarm);
            }
        }


        /// <inheritdoc/>
        /// <remarks>
        /// The alarm is attached and marked as modified, its configuration is attached unchanged so that it is not written.
        /// </remarks>
        public void Update(IEnumerable<PpcAlarm> alarms)
        {
            ArgumentNullException.ThrowIfNull(alarms);

            foreach (PpcAlarm alarm in alarms)
            {
                if (Context.Entry(alarm).State != EntityState.Detached)
                {
                    continue;
                }

                AttachConfig(alarm);
                Context.PpcAlarms.Attach(alarm);
                Context.Entry(alarm).State = EntityState.Modified;
            }
        }


        /// <inheritdoc/>
        public async Task<List<PpcAlarm>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(ids);

            List<int> distinctIds = ids.Distinct().ToList();

            return await Context.PpcAlarms
                .AsNoTrackingWithIdentityResolution()
                .Where(alarm => distinctIds.Contains(alarm.Id))
                .Include(alarm => alarm.PpcAlarmConfig)
                .ThenInclude(config => config.AlarmActions)
                .ThenInclude(action => action.AlarmActionParameters)
                .OrderBy(alarm => alarm.Id)
                .ToListAsync(cancellationToken);
        }


        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await Context.SaveChangesAsync(cancellationToken);
        }


        /// <summary>
        /// Query of the active alarms including their configuration, not tracked.
        /// </summary>
        /// <remarks>
        /// With identity resolution the alarms of the same configuration share one configuration instance.
        /// </remarks>
        private IQueryable<PpcAlarm> QueryActive()
        {
            return Context.PpcAlarms
                .AsNoTrackingWithIdentityResolution()
                .Where(alarm => alarm.AlarmSequenceState != AlarmSequenceState.Normal)
                .Include(alarm => alarm.PpcAlarmConfig)
                .ThenInclude(config => config.AlarmActions)
                .ThenInclude(action => action.AlarmActionParameters)
                .OrderBy(alarm => alarm.Id);
        }


        /// <summary>
        /// Attaches the configuration of an alarm unchanged, or uses the configuration the context already tracks.
        /// </summary>
        /// <remarks>
        /// The configuration might be cached (detached) while the context already tracks the same configuration
        /// (e.g. of another alarm), a second instance with the same key must not be attached.
        /// Only the configuration itself is attached, not its graph: the actions are shared by several configurations
        /// and other instances of the same action (e.g. of a configuration loaded from the database) must not collide.
        /// The actions are only read, they are never saved with the alarms.
        /// </remarks>
        private void AttachConfig(PpcAlarm alarm)
        {
            PpcAlarmConfig? trackedConfig = Context.PpcAlarmConfigs.Local.FirstOrDefault(config => config.Id == alarm.PpcAlarmConfig.Id);

            if (trackedConfig is null)
            {
                Context.Entry(alarm.PpcAlarmConfig).State = EntityState.Unchanged;
            }
            else if (ReferenceEquals(trackedConfig, alarm.PpcAlarmConfig) is false)
            {
                alarm.PpcAlarmConfig = trackedConfig;
            }
        }
    }
}

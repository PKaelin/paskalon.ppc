// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using paskalON.Alarms.Domain.Configs;
using paskalON.Alarms.Domain.Configs.Actions;
using paskalON.Alarms.Domain.Configs.Ppcs;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Infrastructure.Storage.Outboxes;
using paskalON.Domains;

namespace paskalON.Alarms.Infrastructure.Storage
{
    /// <summary>
    /// Alarm service database context interface definition.
    /// </summary>
    public interface IAlarmContext
    {
        /// <summary>
        /// General key/value configurations of the microservice.
        /// </summary>
        DbSet<Configuration> Configurations { get; set; }


        /// <summary>
        /// Database migration history.
        /// </summary>
        DbSet<History> Histories { get; set; }


        /// <summary>
        /// System configuration of the microservice.
        /// </summary>
        DbSet<SystemConfig> SystemConfigs { get; set; }


        /// <summary>
        /// PPC alarm configurations.
        /// </summary>
        DbSet<PpcAlarmConfig> PpcAlarmConfigs { get; set; }


        /// <summary>
        /// Alarm action configurations.
        /// </summary>
        DbSet<AlarmActionConfig> AlarmActionConfigs { get; set; }


        /// <summary>
        /// Alarm action parameter configurations.
        /// </summary>
        DbSet<AlarmActionParameterConfig> AlarmActionParameterConfigs { get; set; }


        /// <summary>
        /// PPC alarms.
        /// </summary>
        DbSet<PpcAlarm> PpcAlarms { get; set; }


        /// <summary>
        /// Alarm action messages of the transactional outbox.
        /// </summary>
        DbSet<AlarmActionOutboxMessage> AlarmActionOutboxMessages { get; set; }


        /// <summary>
        /// Save changes.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task result contains the number of state entries written to the underlying database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

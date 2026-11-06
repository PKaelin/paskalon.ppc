// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Alarms.Infrastructure.Storage.Outboxes;
using paskalON.Infrastructure.Repositories;

namespace paskalON.Alarms.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// Alarm action outbox repository for persisting the alarm action messages in the database.
    /// </summary>
    public class AlarmActionOutboxRepository : RepositoryBase<AlarmContext>, IAlarmActionOutboxRepository
    {
        /// <summary>
        /// Constructor of <see cref="AlarmActionOutboxRepository"/>.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="context">The alarm context.</param>
        public AlarmActionOutboxRepository(ILogger<AlarmActionOutboxRepository> logger, AlarmContext context) : base(logger, context)
        {
        }


        /// <inheritdoc/>
        public void Add(IEnumerable<AlarmActionOutboxMessage> messages)
        {
            ArgumentNullException.ThrowIfNull(messages);

            Context.AlarmActionOutboxMessages.AddRange(messages);
        }
    }
}

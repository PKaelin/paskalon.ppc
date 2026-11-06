// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
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


        /// <inheritdoc/>
        /// <remarks>
        /// Messages with the same creation time are ordered by id (insert order), so that e.g. the actions of one alarm keep their order.
        /// </remarks>
        public async Task<List<AlarmActionOutboxMessage>> GetPendingAsync(int take, CancellationToken cancellationToken = default)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(take, 1);

            return await Context.AlarmActionOutboxMessages
                .Where(message => message.SentOn == null)
                .OrderBy(message => message.CreatedOn)
                .ThenBy(message => message.Id)
                .Take(take)
                .ToListAsync(cancellationToken);
        }


        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await Context.SaveChangesAsync(cancellationToken);
        }
    }
}

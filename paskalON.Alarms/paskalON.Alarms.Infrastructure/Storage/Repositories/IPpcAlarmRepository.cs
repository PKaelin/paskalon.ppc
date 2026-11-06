// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Alarms.Domain.Ppcs;

namespace paskalON.Alarms.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// PPC alarm repository interface definition.
    /// </summary>
    public interface IPpcAlarmRepository
    {
        /// <summary>
        /// Gets the active (not normalized) alarms of a publisher including their configuration.
        /// </summary>
        /// <param name="key">Key of the publisher.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Tracked active alarms of the publisher, changes are saved with <see cref="SaveChangesAsync"/>.</returns>
        Task<List<PpcAlarm>> GetActiveAsync(PpcAlarmKey key, CancellationToken cancellationToken = default);


        /// <summary>
        /// Adds newly raised alarms, saved with <see cref="SaveChangesAsync"/>.
        /// </summary>
        /// <param name="alarms">Newly raised alarms.</param>
        void Add(IEnumerable<PpcAlarm> alarms);


        /// <summary>
        /// Saves the added and changed alarms in one transaction.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

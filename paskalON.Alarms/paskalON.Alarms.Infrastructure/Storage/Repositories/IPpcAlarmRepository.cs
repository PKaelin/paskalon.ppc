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
    /// <remarks>
    /// The alarms are read not tracked, so that they can be cached. Changes are saved with <see cref="Add"/> for raised
    /// alarms and <see cref="Update"/> for changed (e.g. normalized) alarms, followed by <see cref="SaveChangesAsync"/>.
    /// </remarks>
    public interface IPpcAlarmRepository
    {
        /// <summary>
        /// Gets the active (not normalized) alarms of all publishers including their configuration, not tracked.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Active alarms of all publishers.</returns>
        Task<List<PpcAlarm>> GetAllActiveAsync(CancellationToken cancellationToken = default);


        /// <summary>
        /// Gets the active (not normalized) alarms of a publisher including their configuration, not tracked.
        /// </summary>
        /// <param name="key">Key of the publisher.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Active alarms of the publisher.</returns>
        Task<List<PpcAlarm>> GetActiveAsync(PpcAlarmKey key, CancellationToken cancellationToken = default);


        /// <summary>
        /// Gets the active and normalized alarms that match the filter including their configuration without the actions, not tracked.
        /// </summary>
        /// <param name="filter">Filter of the alarms.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Matching alarms, the latest created alarm first.</returns>
        Task<List<PpcAlarm>> GetAsync(PpcAlarmFilter filter, CancellationToken cancellationToken = default);


        /// <summary>
        /// Gets the alarms with the ids (active or normalized) including their configuration with the actions, not tracked.
        /// </summary>
        /// <param name="ids">Ids of the alarms, unknown ids are ignored.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Found alarms ordered by id.</returns>
        Task<List<PpcAlarm>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default);


        /// <summary>
        /// Adds newly raised alarms, saved with <see cref="SaveChangesAsync"/>.
        /// </summary>
        /// <param name="alarms">Newly raised alarms.</param>
        void Add(IEnumerable<PpcAlarm> alarms);


        /// <summary>
        /// Updates changed existing alarms (e.g. normalized or acknowledged), saved with <see cref="SaveChangesAsync"/>.
        /// </summary>
        /// <param name="alarms">Changed existing alarms, e.g. from a cache.</param>
        /// <remarks>
        /// Must be called before adding outbox messages of the alarms, otherwise the alarms would be added again.
        /// </remarks>
        void Update(IEnumerable<PpcAlarm> alarms);


        /// <summary>
        /// Saves the added and changed alarms in one transaction.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

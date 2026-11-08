// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Evaluations;
using paskalON.Alarms.Domain.Ppcs;
using paskalON.Alarms.Dto.Summaries;
using paskalON.Alarms.Infrastructure.Storage.Repositories;

namespace paskalON.Alarms.Application.Caches
{
    /// <summary>
    /// In memory cache of the active alarms per publisher, so that the database is not queried for every received data.
    /// </summary>
    public interface IPpcAlarmActiveCache
    {
        /// <summary>
        /// Indicates whether the active alarms have been loaded.
        /// </summary>
        bool IsLoaded { get; }


        /// <summary>
        /// Loads the active alarms of all publishers and replaces the cached alarms.
        /// </summary>
        /// <param name="repository">Repository of the alarms.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of loaded active alarms.</returns>
        Task<int> LoadAsync(IPpcAlarmRepository repository, CancellationToken cancellationToken = default);


        /// <summary>
        /// Gets the active alarms of a publisher, an invalidated publisher is loaded again from the database.
        /// </summary>
        /// <param name="key">Key of the publisher.</param>
        /// <param name="repository">Repository of the alarms, used only for an invalidated publisher.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>New list of the cached active alarm instances of the publisher.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the active alarms have not been loaded.</exception>
        Task<List<PpcAlarm>> GetActiveAsync(PpcAlarmKey key, IPpcAlarmRepository repository, CancellationToken cancellationToken = default);


        /// <summary>
        /// Gets the publishers with cached active alarms and the invalidated publishers (they might have active alarms).
        /// </summary>
        /// <returns>Keys of the publishers.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the active alarms have not been loaded.</exception>
        IReadOnlyList<PpcAlarmKey> GetKeys();


        /// <summary>
        /// Counts the active alarms in the sequence state Alarm per severity level of all publishers.
        /// The invalidated publishers are loaded from the database first.
        /// </summary>
        /// <remarks>
        /// Only call it while holding the alarm write lock, like <see cref="GetActiveAsync"/>.
        /// </remarks>
        /// <param name="repository">Repository of the alarms, used only for the invalidated publishers.</param>
        /// <param name="calculatedOn">Time of the calculation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The alarm summary.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the active alarms have not been loaded.</exception>
        Task<AlarmSummaryDto> GetSummaryAsync(IPpcAlarmRepository repository, DateTimeOffset calculatedOn, CancellationToken cancellationToken = default);


        /// <summary>
        /// Applies a saved evaluation: the raised alarms are added and the normalized alarms removed.
        /// </summary>
        /// <param name="key">Key of the publisher.</param>
        /// <param name="evaluation">Evaluation that has been saved (committed).</param>
        void Apply(PpcAlarmKey key, PpcAlarmEvaluation evaluation);


        /// <summary>
        /// Invalidates the active alarms of a publisher, e.g. after a failed save of an evaluation that changed the cached alarms.
        /// </summary>
        /// <param name="key">Key of the publisher.</param>
        void Invalidate(PpcAlarmKey key);
    }
}

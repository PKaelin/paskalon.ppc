// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Evaluations;

namespace paskalON.Alarms.Application.Handlers
{
    /// <summary>
    /// Generic handler of published data with boolean, numeric and boolean dictionary properties (e.g. PcsDetailDto).
    /// </summary>
    public interface IPpcAlarmHandler
    {
        /// <summary>
        /// Handles received data: updates the stateless store, evaluates the changed values and
        /// persists the raised and normalized alarms with their alarm actions in one transaction.
        /// </summary>
        /// <param name="key">Key of the publisher of the data.</param>
        /// <param name="data">Received data.</param>
        /// <param name="receivedOn">Timestamp when the data was received.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Persisted raised and normalized alarms, empty if nothing changed.</returns>
        Task<PpcAlarmEvaluation> HandleAsync(PpcAlarmKey key, object data, DateTimeOffset receivedOn, CancellationToken cancellationToken = default);
    }
}

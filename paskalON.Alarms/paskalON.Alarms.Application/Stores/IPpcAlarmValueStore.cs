// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Evaluations;

namespace paskalON.Alarms.Application.Stores
{
    /// <summary>
    /// Stateless store of the latest published alarm relevant values per publisher.
    /// </summary>
    /// <remarks>
    /// Stateless means the values are not persisted, they are received again with the next publication.
    /// </remarks>
    public interface IPpcAlarmValueStore
    {
        /// <summary>
        /// Updates the values of a publisher.
        /// </summary>
        /// <param name="key">Key of the publisher.</param>
        /// <param name="values">Latest values by property or dictionary entry name.</param>
        /// <param name="receivedOn">Timestamp when the values were received.</param>
        /// <returns>Values that changed or were received the first time.</returns>
        List<PpcAlarmKeyValue> Update(PpcAlarmKey key, IReadOnlyDictionary<string, double> values, DateTimeOffset receivedOn);


        /// <summary>
        /// Gets the latest values of a publisher.
        /// </summary>
        /// <param name="key">Key of the publisher.</param>
        /// <returns>Latest values of the publisher, or empty if the publisher is unknown.</returns>
        List<PpcAlarmKeyValue> GetValues(PpcAlarmKey key);
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs.Localizations;

namespace paskalON.Alarms.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// Localized text repository interface definition.
    /// </summary>
    public interface ILocalizedTextRepository
    {
        /// <summary>
        /// Gets the texts of the languages, not tracked.
        /// </summary>
        /// <param name="languages">Languages of the texts, e.g. de and the default language en.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Texts of all the languages.</returns>
        Task<List<LocalizedText>> GetAsync(IReadOnlyCollection<string> languages, CancellationToken cancellationToken = default);
    }
}

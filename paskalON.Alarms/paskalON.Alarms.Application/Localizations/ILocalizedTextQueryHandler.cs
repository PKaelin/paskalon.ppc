// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Dto.Localizations;

namespace paskalON.Alarms.Application.Localizations
{
    /// <summary>
    /// Reads the texts of the localization keys of the alarm configurations, e.g. for the user interface.
    /// </summary>
    public interface ILocalizedTextQueryHandler
    {
        /// <summary>
        /// Gets the texts of all localization keys in the language, a key without a text in the language has the text of the default language.
        /// </summary>
        /// <param name="language">Language of the texts, e.g. en or de-CH.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The texts by their localization key.</returns>
        /// <exception cref="ArgumentException">Thrown when the language has no valid format.</exception>
        Task<LocalizedTextsDto> GetTextsAsync(string language, CancellationToken cancellationToken = default);
    }
}

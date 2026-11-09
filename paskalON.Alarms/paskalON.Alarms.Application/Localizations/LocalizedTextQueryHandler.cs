// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Domain.Configs.Localizations;
using paskalON.Alarms.Dto.Localizations;
using paskalON.Alarms.Infrastructure.Storage.Repositories;

namespace paskalON.Alarms.Application.Localizations
{
    /// <summary>
    /// Reads the texts of the localization keys from the database.
    /// </summary>
    /// <remarks>
    /// The texts are only changed by the client data import, they are read for every request (e.g. once per start of the user interface).
    /// </remarks>
    public class LocalizedTextQueryHandler : ILocalizedTextQueryHandler
    {
        /// <summary>
        /// Repository of the localized texts.
        /// </summary>
        private readonly ILocalizedTextRepository _textRepository;


        /// <summary>
        /// Constructor of <see cref="LocalizedTextQueryHandler"/>.
        /// </summary>
        /// <param name="textRepository">Repository of the localized texts.</param>
        public LocalizedTextQueryHandler(ILocalizedTextRepository textRepository)
        {
            ArgumentNullException.ThrowIfNull(textRepository);

            _textRepository = textRepository;
        }


        /// <inheritdoc/>
        public async Task<LocalizedTextsDto> GetTextsAsync(string language, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(language);

            if (LocalizedText.IsValidLanguage(language) is false)
            {
                throw new ArgumentException($"The language '{language}' is not valid, expected e.g. en or de-CH.", nameof(language));
            }

            bool isDefaultLanguage = language == LocalizedText.DefaultLanguage;
            string[] languages = isDefaultLanguage
                ? new string[] { LocalizedText.DefaultLanguage }
                : new string[] { language, LocalizedText.DefaultLanguage };
            List<LocalizedText> texts = await _textRepository.GetAsync(languages, cancellationToken);
            Dictionary<string, string> textsByKey = new Dictionary<string, string>(StringComparer.Ordinal);

            // The texts of the default language first, so that the texts of the requested language replace them
            foreach (LocalizedText text in texts.Where(text => text.Language == LocalizedText.DefaultLanguage))
            {
                textsByKey[text.Name] = text.Text;
            }

            foreach (LocalizedText text in texts.Where(text => isDefaultLanguage is false && text.Language == language))
            {
                textsByKey[text.Name] = text.Text;
            }

            return new LocalizedTextsDto { Language = language, Texts = textsByKey };
        }
    }
}

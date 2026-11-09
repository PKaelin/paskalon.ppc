// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Text.RegularExpressions;
using paskalON.Domains;

namespace paskalON.Alarms.Domain.Configs.Localizations
{
    /// <summary>
    /// Text of a localization key in one language, e.g. the English text of {{PCS.CommunicationError.Name}}.
    /// </summary>
    /// <remarks>
    /// - The alarm configurations store and send the localization keys unchanged, the user interface shows the text of the key.
    /// - The key and the language are unique.
    /// </remarks>
    public partial class LocalizedText : DomainBase
    {
        /// <summary>
        /// The language every localization key has a text in, used when a key has no text in the requested language.
        /// </summary>
        public const string DefaultLanguage = "en";


        /// <summary>
        /// Localization key without the braces, e.g. PCS.CommunicationError.Name for {{PCS.CommunicationError.Name}}.
        /// </summary>
        public required string Name
        {
            get;
            set
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(value);

                if (value.Contains('{') || value.Contains('}'))
                {
                    throw new ArgumentException($"The localization key '{value}' must be written without braces.", nameof(value));
                }

                field = value;
            }
        }


        /// <summary>
        /// Language of the text: ISO 639-1 language code, optionally with an ISO 3166-1 region code, e.g. en or de-CH.
        /// </summary>
        public required string Language
        {
            get;
            set
            {
                if (IsValidLanguage(value) is false)
                {
                    throw new ArgumentException($"The language '{value}' is not valid, expected e.g. en or de-CH.", nameof(value));
                }

                field = value;
            }
        }


        /// <summary>
        /// Text of the key in the language.
        /// </summary>
        public required string Text
        {
            get;
            set { ArgumentException.ThrowIfNullOrWhiteSpace(value); field = value; }
        }


        /// <summary>
        /// Checks the format of a language: ISO 639-1 language code, optionally with an ISO 3166-1 region code, e.g. en or de-CH.
        /// </summary>
        /// <param name="language">The language to check.</param>
        /// <returns>True if the language has a valid format.</returns>
        public static bool IsValidLanguage(string? language)
        {
            return language is not null && LanguagePattern().IsMatch(language);
        }


        /// <summary>
        /// A language code (lowercase) with an optional region code (uppercase).
        /// </summary>
        /// <returns>The regular expression of the language.</returns>
        [GeneratedRegex("^[a-z]{2}(-[A-Z]{2})?$")]
        private static partial Regex LanguagePattern();
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Dto.Localizations
{
    /// <summary>
    /// Texts of the localization keys of the alarm configurations in one language, e.g. for the user interface.
    /// </summary>
    /// <remarks>
    /// A key without a text in the requested language has the text of the default language (en).
    /// </remarks>
    public record LocalizedTextsDto
    {
        /// <summary>
        /// The requested language, e.g. en or de-CH.
        /// </summary>
        public required string Language { get; init; }


        /// <summary>
        /// Texts by their localization key without the braces, e.g. PCS.CommunicationError.Name for {{PCS.CommunicationError.Name}}.
        /// </summary>
        public required Dictionary<string, string> Texts { get; init; }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Text.RegularExpressions;

namespace paskalON.ClientData.Common
{
    /// <summary>
    /// Localization keys of the client data: a value written as {{Key}} is a localization key, any other value is plain text.
    /// </summary>
    public static partial class LocalizationKey
    {
        /// <summary>
        /// Gets the localization key of a value written as {{Key}}.
        /// </summary>
        /// <param name="value">The value of a property.</param>
        /// <param name="key">The localization key without the braces, or an empty string if the value is not a localization key.</param>
        /// <returns>True if the value is a localization key.</returns>
        public static bool TryParse(string? value, out string key)
        {
            key = string.Empty;

            if (value is null)
            {
                return false;
            }

            Match match = KeyPattern().Match(value);

            if (match.Success is false)
            {
                return false;
            }

            key = match.Groups["key"].Value;

            return true;
        }


        /// <summary>
        /// A value that is a localization key: {{Key}} without braces in the key.
        /// </summary>
        /// <returns>The regular expression with the group key.</returns>
        [GeneratedRegex(@"^\{\{(?<key>[^{}]+)\}\}$")]
        private static partial Regex KeyPattern();
    }
}

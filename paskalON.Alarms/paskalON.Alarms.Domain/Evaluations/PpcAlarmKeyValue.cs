// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Domain.Evaluations
{
    /// <summary>
    /// Value of a property, or dictionary entry, of the published data in the stateless store.
    /// </summary>
    /// <remarks>
    /// All values are stored as double, boolean values are represented as true = 1 and false = 0.
    /// A dictionary entry is named [Dictionary].[Key], e.g. FaultStates.OverTemperature.
    /// </remarks>
    public record PpcAlarmKeyValue
    {
        /// <summary>
        /// Separator between the dictionary name and the key of a dictionary entry.
        /// </summary>
        public const char DictionarySeparator = '.';


        /// <summary>
        /// Name of the property or dictionary entry.
        /// </summary>
        public required string Name { get; init; }


        /// <summary>
        /// Value of the property or dictionary entry.
        /// </summary>
        public required double Value { get; init; }


        /// <summary>
        /// Indicates whether the value changed with the latest update (or was received the first time).
        /// </summary>
        public required bool ValueChanged { get; init; }


        /// <summary>
        /// Timestamp when the value last changed.
        /// </summary>
        public required DateTimeOffset ValueChangedOn { get; init; }


        /// <summary>
        /// Creates the name of a dictionary entry.
        /// </summary>
        /// <param name="dictionaryName">Name of the dictionary property, e.g. FaultStates.</param>
        /// <param name="key">Key of the dictionary entry, e.g. OverTemperature.</param>
        /// <returns>Name of the dictionary entry, e.g. FaultStates.OverTemperature.</returns>
        public static string CreateDictionaryEntryName(string dictionaryName, string key)
        {
            return $"{dictionaryName}{DictionarySeparator}{key}";
        }
    }
}

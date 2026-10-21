// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Globalization;

namespace paskalON.DeviceSimulator.Equipments.Simulations
{
    /// <summary>
    /// Helpers to read simulator custom configurations (key/value pairs of the device custom configs).
    /// </summary>
    public static class SimulationCustoms
    {
        /// <summary>
        /// Empty custom configuration.
        /// </summary>
        public static IReadOnlyDictionary<string, string> Empty { get; } = new Dictionary<string, string>();


        /// <summary>
        /// Gets a double value of a custom configuration.
        /// </summary>
        /// <param name="customs">Custom configurations.</param>
        /// <param name="key">Key of the custom configuration.</param>
        /// <param name="defaultValue">Value returned when the key does not exist.</param>
        /// <returns>The configured value or the default value.</returns>
        /// <exception cref="FormatException">The configured value is not a number.</exception>
        public static double GetDouble(IReadOnlyDictionary<string, string> customs, string key, double defaultValue)
        {
            ArgumentNullException.ThrowIfNull(customs);
            ArgumentException.ThrowIfNullOrEmpty(key);

            if (customs.TryGetValue(key, out string? value) is false)
            {
                return defaultValue;
            }

            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result) is false)
            {
                throw new FormatException($"Simulator custom configuration '{key}' has the invalid number '{value}'.");
            }

            return result;
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Globalization;

namespace paskalON.Hosting
{
    /// <summary>
    /// Options of the startup retry defining how often and in which interval a failed startup operation is retried.
    /// </summary>
    public sealed class StartupRetryOptions
    {
        /// <summary>
        /// Environment variable name of the retry count.
        /// </summary>
        public const string RetryCountVariable = "STARTUP_RETRY_COUNT";


        /// <summary>
        /// Environment variable name of the retry interval in seconds.
        /// </summary>
        public const string RetryIntervalSecondsVariable = "STARTUP_RETRY_INTERVAL_SECONDS";


        /// <summary>
        /// Default number of retries after the first failed attempt.
        /// </summary>
        public const int DefaultRetryCount = 6;


        /// <summary>
        /// Default interval in seconds between two attempts.
        /// </summary>
        public const int DefaultRetryIntervalSeconds = 15;


        /// <summary>
        /// Number of retries after the first failed attempt.
        /// </summary>
        public int RetryCount { get; }


        /// <summary>
        /// Interval between two attempts.
        /// </summary>
        public TimeSpan RetryInterval { get; }


        /// <summary>
        /// Constructor of <see cref="StartupRetryOptions"/>.
        /// </summary>
        /// <param name="retryCount">Number of retries after the first failed attempt.</param>
        /// <param name="retryInterval">Interval between two attempts.</param>
        public StartupRetryOptions(int retryCount, TimeSpan retryInterval)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(retryCount);
            ArgumentOutOfRangeException.ThrowIfLessThan(retryInterval, TimeSpan.Zero);

            RetryCount = retryCount;
            RetryInterval = retryInterval;
        }


        /// <summary>
        /// Create the options from the environment variables <see cref="RetryCountVariable"/> and <see cref="RetryIntervalSecondsVariable"/>.
        /// Missing variables fall back to the defaults.
        /// </summary>
        /// <returns>The startup retry options.</returns>
        public static StartupRetryOptions FromEnvironment()
        {
            return Create(Environment.GetEnvironmentVariable(RetryCountVariable), Environment.GetEnvironmentVariable(RetryIntervalSecondsVariable));
        }


        /// <summary>
        /// Create the options from their textual representation. Missing values fall back to the defaults.
        /// </summary>
        /// <param name="retryCount">Number of retries after the first failed attempt or null for the default.</param>
        /// <param name="retryIntervalSeconds">Interval in seconds between two attempts or null for the default.</param>
        /// <returns>The startup retry options.</returns>
        /// <exception cref="FormatException">Thrown when a value is not an integer.</exception>
        public static StartupRetryOptions Create(string? retryCount, string? retryIntervalSeconds)
        {
            int count = ParseOrDefault(retryCount, DefaultRetryCount, RetryCountVariable);
            int intervalSeconds = ParseOrDefault(retryIntervalSeconds, DefaultRetryIntervalSeconds, RetryIntervalSecondsVariable);

            return new StartupRetryOptions(count, TimeSpan.FromSeconds(intervalSeconds));
        }


        /// <summary>
        /// Parse an integer value or return the default when the value is missing.
        /// </summary>
        /// <param name="value">The textual value.</param>
        /// <param name="defaultValue">The default value used when the value is missing.</param>
        /// <param name="name">The name of the value used in the error message.</param>
        /// <returns>The parsed or default value.</returns>
        private static int ParseOrDefault(string? value, int defaultValue, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) is false)
            {
                throw new FormatException($"{name} is not configured as an integer");
            }

            return result;
        }
    }
}

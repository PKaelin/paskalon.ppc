// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;

namespace paskalON.Hosting
{
    /// <summary>
    /// Retries startup operations that fail because a dependency (e.g. a service or a database) is not yet available.
    /// </summary>
    public sealed class StartupRetry
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// Time provider used to wait between two attempts.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// Options defining the retry count and interval.
        /// </summary>
        private readonly StartupRetryOptions _options;


        /// <summary>
        /// Constructor of <see cref="StartupRetry"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="timeProvider">Time provider used to wait between two attempts.</param>
        /// <param name="options">Options defining the retry count and interval.</param>
        public StartupRetry(ILogger logger, TimeProvider timeProvider, StartupRetryOptions options)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(timeProvider);
            ArgumentNullException.ThrowIfNull(options);

            _logger = logger;
            _timeProvider = timeProvider;
            _options = options;
        }


        /// <summary>
        /// Execute the operation and retry it as long as it fails with a transient exception and retries are left.
        /// </summary>
        /// <param name="operationName">Name of the operation used for logging.</param>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="isTransient">Decides whether an exception is transient and the operation shall be retried.</param>
        /// <param name="cancellationToken">Token to cancel the retries.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExecuteAsync(string operationName, Func<Task> operation, Func<Exception, bool> isTransient, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(operation);

            await ExecuteAsync(operationName, async () =>
            {
                await operation();

                return true;
            }, isTransient, cancellationToken);
        }


        /// <summary>
        /// Execute the operation and retry it as long as it fails with a transient exception and retries are left.
        /// </summary>
        /// <typeparam name="T">The result type of the operation.</typeparam>
        /// <param name="operationName">Name of the operation used for logging.</param>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="isTransient">Decides whether an exception is transient and the operation shall be retried.</param>
        /// <param name="cancellationToken">Token to cancel the retries.</param>
        /// <returns>The result of the operation.</returns>
        public async Task<T> ExecuteAsync<T>(string operationName, Func<Task<T>> operation, Func<Exception, bool> isTransient, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(operationName);
            ArgumentNullException.ThrowIfNull(operation);
            ArgumentNullException.ThrowIfNull(isTransient);

            int maximumAttempts = _options.RetryCount + 1;

            for (int attempt = 1; ; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    return await operation();
                }
                catch (Exception ex) when (attempt < maximumAttempts && cancellationToken.IsCancellationRequested is false && isTransient(ex))
                {
                    _logger.LogWarning(ex, "Startup operation {OperationName} failed on attempt {Attempt} of {MaximumAttempts}, retrying in {RetryInterval}",
                        operationName, attempt, maximumAttempts, _options.RetryInterval);
                }
                catch (Exception ex) when (attempt == maximumAttempts && isTransient(ex))
                {
                    _logger.LogError(ex, "Startup operation {OperationName} failed after {MaximumAttempts} attempts", operationName, maximumAttempts);
                    throw;
                }

                await Task.Delay(_options.RetryInterval, _timeProvider, cancellationToken);
            }
        }
    }
}

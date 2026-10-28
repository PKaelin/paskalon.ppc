// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Hosting
{
    /// <summary>
    /// Decides whether an exception of an HTTP call is transient, meaning the remote service is not reachable (yet).
    /// HTTP error responses (4xx, 5xx) are not transient because the remote service is reachable.
    /// </summary>
    public static class HttpTransientExceptionFilter
    {
        /// <summary>
        /// Check whether the exception is a connection or timeout error.
        /// </summary>
        /// <param name="exception">The exception to check.</param>
        /// <returns>True when the exception is a connection or timeout error, otherwise false.</returns>
        public static bool IsTransient(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            return exception switch
            {
                HttpRequestException httpException => (httpException.StatusCode is null
                    && httpException.HttpRequestError is (HttpRequestError.ConnectionError or HttpRequestError.NameResolutionError or HttpRequestError.ResponseEnded)),
                // The HttpClient timeout is reported as a task cancellation with an inner timeout exception
                TaskCanceledException canceledException => canceledException.InnerException is TimeoutException,
                TimeoutException => true,
                _ => false
            };
        }
    }
}

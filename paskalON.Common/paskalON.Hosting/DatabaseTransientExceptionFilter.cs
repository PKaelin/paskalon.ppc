// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Data.Common;

namespace paskalON.Hosting
{
    /// <summary>
    /// Decides whether an exception of a database call is transient, meaning the database is not reachable (yet).
    /// The database providers (e.g. Npgsql) flag connection and timeout errors with <see cref="DbException.IsTransient"/>.
    /// Entity Framework wraps those errors (e.g. in an <see cref="InvalidOperationException"/>), therefore the inner exceptions are checked as well.
    /// </summary>
    public static class DatabaseTransientExceptionFilter
    {
        /// <summary>
        /// Check whether the exception or one of its inner exceptions is a transient database error.
        /// </summary>
        /// <param name="exception">The exception to check.</param>
        /// <returns>True when the exception or one of its inner exceptions is a transient database or timeout error, otherwise false.</returns>
        public static bool IsTransient(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            for (Exception? current = exception; current is not null; current = current.InnerException)
            {
                if (current is DbException { IsTransient: true } or TimeoutException)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

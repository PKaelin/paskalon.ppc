// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Application.Handlers
{
    /// <summary>
    /// Lock of the alarm writers, so that only one writer at a time changes and saves the cached active alarms.
    /// </summary>
    public interface IPpcAlarmWriteLock
    {
        /// <summary>
        /// Waits until the lock is acquired.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Releases the lock when disposed.</returns>
        Task<IDisposable> AcquireAsync(CancellationToken cancellationToken = default);
    }
}

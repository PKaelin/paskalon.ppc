// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Application.Outboxes
{
    /// <summary>
    /// Signals that new alarm actions were saved in the outbox, so that they are sent immediately instead of with the next interval.
    /// </summary>
    public interface IAlarmActionOutboxSignal
    {
        /// <summary>
        /// Notifies that new alarm actions were saved (committed) in the outbox.
        /// </summary>
        /// <remarks>
        /// Several notifications before the next wait are combined into one, because one send sends all pending actions.
        /// </remarks>
        void Notify();


        /// <summary>
        /// Waits until new alarm actions are notified or the timeout elapsed.
        /// </summary>
        /// <param name="timeout">Maximum time to wait.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if new alarm actions were notified, false if the timeout elapsed.</returns>
        Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
    }
}

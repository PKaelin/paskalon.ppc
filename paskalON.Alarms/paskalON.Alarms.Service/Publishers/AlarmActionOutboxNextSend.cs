// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Service.Publishers
{
    /// <summary>
    /// When the outbox publisher service sends the pending alarm actions next.
    /// </summary>
    public enum AlarmActionOutboxNextSend
    {
        /// <summary>
        /// Immediately, because more pending actions than one batch exist.
        /// </summary>
        /// <remarks>
        /// Default when a new action is saved in the outbox.
        /// </remarks>
        Immediately = 0,


        /// <summary>
        /// When new actions are signaled or at the latest after the send interval.
        /// </summary>
        OnSignalOrSendInterval = 1,


        /// <summary>
        /// After the retry interval, because the send failed (e.g. broker or database not available). Signals are ignored meanwhile.
        /// </summary>
        AfterRetryInterval = 2
    }
}

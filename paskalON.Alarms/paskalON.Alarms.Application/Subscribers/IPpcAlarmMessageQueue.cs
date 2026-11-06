// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Alarms.Application.Subscribers
{
    /// <summary>
    /// Queue of the received published data between the message subscriber and the alarm handler.
    /// </summary>
    public interface IPpcAlarmMessageQueue
    {
        /// <summary>
        /// Adds received data to the queue.
        /// </summary>
        /// <param name="message">Received data.</param>
        void Enqueue(PpcAlarmMessage message);


        /// <summary>
        /// Reads the received data in receive order until cancelled.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Received data in receive order.</returns>
        IAsyncEnumerable<PpcAlarmMessage> ReadAllAsync(CancellationToken cancellationToken = default);
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Threading.Channels;

namespace paskalON.Alarms.Application.Outboxes
{
    /// <summary>
    /// Signals that new alarm actions were saved in the outbox, so that they are sent immediately instead of with the next interval.
    /// </summary>
    /// <remarks>
    /// - Singleton shared by the scoped handlers (notify) and the outbox publisher service (wait).
    /// - A channel with one slot holds at most one pending notification, further notifications are dropped
    ///   until the publisher service took it, because one send sends all pending actions.
    /// </remarks>
    public class AlarmActionOutboxSignal : IAlarmActionOutboxSignal
    {
        /// <summary>
        /// Channel with one slot for the pending notification.
        /// </summary>
        private readonly Channel<bool> _channel = Channel.CreateBounded<bool>(new BoundedChannelOptions(1)
        {
            FullMode = BoundedChannelFullMode.DropWrite,
            SingleReader = true
        });


        /// <summary>
        /// Time provider of the wait timeout.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <summary>
        /// Constructor of <see cref="AlarmActionOutboxSignal"/>.
        /// </summary>
        /// <param name="timeProvider">Time provider of the wait timeout.</param>
        public AlarmActionOutboxSignal(TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(timeProvider);

            _timeProvider = timeProvider;
        }


        /// <inheritdoc/>
        public void Notify()
        {
            _channel.Writer.TryWrite(true);
        }


        /// <inheritdoc/>
        public async Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(timeout, TimeSpan.Zero);

            using CancellationTokenSource timeoutSource = new CancellationTokenSource(timeout, _timeProvider);
            using CancellationTokenSource linkedSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, cancellationToken);

            try
            {
                await _channel.Reader.ReadAsync(linkedSource.Token);

                return true;
            }
            catch (OperationCanceledException) when (timeoutSource.IsCancellationRequested && cancellationToken.IsCancellationRequested is false)
            {
                return false;
            }
        }
    }
}

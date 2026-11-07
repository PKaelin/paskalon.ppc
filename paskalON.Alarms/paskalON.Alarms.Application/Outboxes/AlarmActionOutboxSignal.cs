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
        /// Wait for a notification that is still pending after a timed out <see cref="WaitAsync"/>, null if there is none.
        /// </summary>
        private Task<bool>? _pendingWait;


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
        /// <remarks>
        /// The timeout is the normal case (every send interval), therefore it does not cancel a token:
        /// a cancelled read would throw an <see cref="OperationCanceledException"/> every interval.
        /// Only a cancellation of <paramref name="cancellationToken"/> (e.g. shutdown) throws.
        /// Not thread-safe, there is only one waiting reader (the outbox publisher service).
        /// </remarks>
        public async Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(timeout, TimeSpan.Zero);
            cancellationToken.ThrowIfCancellationRequested();

            if (_channel.Reader.TryRead(out _))
            {
                return true;
            }

            // A timed out wait stays pending and is reused, a new wait per call would add a waiter to the channel every interval
            _pendingWait ??= _channel.Reader.WaitToReadAsync(CancellationToken.None).AsTask();
            using CancellationTokenSource delaySource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            Task delay = Task.Delay(timeout, _timeProvider, delaySource.Token);
            Task completed = await Task.WhenAny(_pendingWait, delay);

            if (completed == _pendingWait)
            {
                _pendingWait = null;
                // Stops the timer of the delay, the cancelled delay is not awaited and therefore does not throw
                await delaySource.CancelAsync();

                return _channel.Reader.TryRead(out _);
            }

            cancellationToken.ThrowIfCancellationRequested();

            return false;
        }
    }
}

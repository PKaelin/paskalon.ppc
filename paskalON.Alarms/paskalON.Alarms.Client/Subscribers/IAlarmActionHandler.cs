// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Alarms.Dto.Actions;

namespace paskalON.Alarms.Client.Subscribers
{
    /// <summary>
    /// Recipient of the alarm actions received by <see cref="AlarmActionSubscriberService"/>.
    /// </summary>
    public interface IAlarmActionHandler
    {
        /// <summary>
        /// Gets the names of the actions to receive (the routing keys), e.g. RaiseAlarm and ClearAlarm.
        /// </summary>
        IReadOnlyCollection<string> ActionNames { get; }


        /// <summary>
        /// Executes a received action. The action is delivered at least once, e.g. use the message id to ignore duplicates.
        /// </summary>
        /// <param name="action">Received action.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task.</returns>
        Task HandleAsync(AlarmActionDto action, CancellationToken cancellationToken = default);


        /// <summary>
        /// Loads the current state (e.g. the alarm summary) after the subscription was established or recovered.
        /// The actions sent while not subscribed are not received.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="Exception">Any exception (e.g. alarm service not reachable), it is retried after the retry interval.</exception>
        Task SynchronizeAsync(CancellationToken cancellationToken = default);
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Devices.Application.Subscribers
{
    /// <summary>
    /// Receives the power targets of the power conversion systems (PCS) from a message broker and applies them to the devices.
    /// </summary>
    public interface IPowerTargetSubscriber
    {
        /// <summary>
        /// Subscribes to the power target topic. If no topic is configured, a warning is logged and no power targets are received.
        /// </summary>
        /// <param name="topic">The topic to receive the power targets from.</param>
        void Subscribe(string? topic);


        /// <summary>
        /// Applies the received power targets in the order they were received, until cancellation is requested.
        /// </summary>
        /// <param name="cancellationToken">Token to stop processing.</param>
        /// <returns>Task that completes when processing has stopped.</returns>
        Task RunAsync(CancellationToken cancellationToken);
    }
}

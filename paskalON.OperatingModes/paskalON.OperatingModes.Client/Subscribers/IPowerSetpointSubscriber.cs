// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Client.Subscribers
{
    /// <summary>
    /// Receives the power setpoints from a message broker and passes them to the <see cref="IPowerSetpointHandler"/>.
    /// </summary>
    public interface IPowerSetpointSubscriber
    {
        /// <summary>
        /// Subscribes to the power setpoints topic. If no topic is configured, a warning is logged and no power setpoints are received.
        /// </summary>
        /// <param name="topic">The topic to receive the power setpoints from.</param>
        void Subscribe(string? topic);


        /// <summary>
        /// Subscribes to the DER unit power setpoints topic. If no topic is configured, a warning is logged and no DER unit power setpoints are received.
        /// </summary>
        /// <param name="topic">The topic to receive the DER unit power setpoints from.</param>
        void SubscribeDerUnit(string? topic);


        /// <summary>
        /// Handles the received power setpoints in the order they were received, until cancellation is requested.
        /// </summary>
        /// <param name="cancellationToken">Token to stop processing.</param>
        /// <returns>Task that completes when processing has stopped.</returns>
        Task RunAsync(CancellationToken cancellationToken);
    }
}

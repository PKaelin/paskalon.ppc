// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Application.Simulations
{
    /// <summary>
    /// Represents one deterministic simulated device model.
    /// </summary>
    public interface ISimulatedDevice
    {
        /// <summary>
        /// Advances the model by one simulation interval.
        /// </summary>
        /// <param name="elapsed">Time elapsed since the previous tick.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task TickAsync(TimeSpan elapsed, CancellationToken cancellationToken);
    }
}
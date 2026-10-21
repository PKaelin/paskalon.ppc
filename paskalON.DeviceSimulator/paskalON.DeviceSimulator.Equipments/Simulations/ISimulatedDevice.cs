// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.DeviceSimulator.Equipments.Simulations
{
    /// <summary>
    /// Represents one deterministic simulated device model.
    /// </summary>
    public interface ISimulatedDevice
    {
        /// <summary>
        /// Device ID of the simulated device.
        /// </summary>
        /// <remarks>
        /// The device ID is only unique within one device type (e.g. PCS or battery bank).
        /// </remarks>
        int DeviceId { get; }


        /// <summary>
        /// Name of the simulated device.
        /// </summary>
        string Name { get; }


        /// <summary>
        /// Advances the model by one simulation interval.
        /// </summary>
        /// <param name="elapsed">Time elapsed since the previous tick.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task TickAsync(TimeSpan elapsed, CancellationToken cancellationToken);
    }
}

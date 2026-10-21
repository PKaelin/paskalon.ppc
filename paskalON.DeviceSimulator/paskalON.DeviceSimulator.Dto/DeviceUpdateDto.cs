// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.DeviceSimulator.Dto.Simulations;

namespace paskalON.DeviceSimulator.Dto
{
    /// <summary>
    /// Live update of a single device pushed to the clients.
    /// </summary>
    /// <remarks>
    /// The client identifies the device by <see cref="DeviceId"/> and <see cref="Name"/>.
    /// </remarks>
    public record DeviceUpdateDto : IDevice
    {
        /// <inheritdoc/>
        public required int DeviceId { get; init; }


        /// <inheritdoc/>
        public required string Name { get; init; }


        /// <summary>
        /// Modbus or C37 endpoints of the device.
        /// </summary>
        /// <remarks>
        /// Typed as object so the endpoints are serialized with their runtime type (Modbus or C37 endpoint).
        /// </remarks>
        public required List<object> Endpoints { get; init; }


        /// <summary>
        /// Simulation state or null when the device is not a simulated PCS or battery bank.
        /// </summary>
        public SimulationDto? Simulation { get; init; }
    }
}

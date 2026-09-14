// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Protocols.C37118.Simulations;

namespace paskalON.DeviceSimulator.Equipments.Simulations
{
    /// <summary>
    /// Provides simulated C37.118 streams shared by servers and device simulations.
    /// </summary>
    public interface ISimulationStreamRegistry
    {
        /// <summary>
        /// Gets an existing stream or creates one for the endpoint and stream identifier.
        /// </summary>
        /// <param name="port">C37 server port.</param>
        /// <param name="stationName">C37 station name identifier.</param>
        /// <param name="streamId">C37 stream identifier.</param>
        /// <param name="address">C37 server address.</param>
        /// <returns>The simulated C37 stream.</returns>
        PmuDataSimulation GetOrCreate(PowerMeterBase device);


        /// <summary>
        /// All registered C37 streams.
        /// </summary>
        IReadOnlyDictionary<PmuDataSimulationKey, PmuDataSimulation> Streams { get; }
    }
}
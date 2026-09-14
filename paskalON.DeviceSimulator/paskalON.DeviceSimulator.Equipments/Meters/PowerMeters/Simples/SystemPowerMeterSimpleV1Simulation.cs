// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.C37118.Simulations;
using System.Diagnostics;

namespace paskalON.DeviceSimulator.Equipments.Meters.PowerMeters.Simples
{
    /// <summary>
    /// Simulates a simple C37 system power meter.
    /// </summary>
    public sealed class SystemPowerMeterSimpleV1Simulation : ISimulatedDevice
    {
        /// <summary>
        /// C37 stream data produced by this simulation.
        /// </summary>
        private readonly PmuDataSimulation _stream;


        /// <inheritdoc/>
        public string Name { get; init; }


        /// <summary>
        /// Constructor of <see cref="SystemPowerMeterSimpleV1Simulation"/>.
        /// </summary>
        /// <param name="stream">Backing C37 stream.</param>
        /// <param name="name">Name of the simulated device.</param>
        public SystemPowerMeterSimpleV1Simulation(PmuDataSimulation stream, string name)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(name);

            _stream = stream;
            Name = name;
        }


        /// <inheritdoc/>
        public Task TickAsync(TimeSpan elapsed, CancellationToken cancellationToken)
        {
            if (System.Diagnostics.Debugger.IsAttached == true)
            {
                Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} - {Name} - " +
                    $"StationName: {_stream.StationName} StreamId: {_stream.StreamId} " +
                    $"Active Power: {_stream.Analogs.First(dp => dp.Name == "Analog1").Measurement} " +
                    $"Reactive Power: {_stream.Analogs.First(dp => dp.Name == "Analog5").Measurement} " +
                    $"Frequency: {_stream.Frequency}");
            }

            return Task.CompletedTask;
        }
    }
}

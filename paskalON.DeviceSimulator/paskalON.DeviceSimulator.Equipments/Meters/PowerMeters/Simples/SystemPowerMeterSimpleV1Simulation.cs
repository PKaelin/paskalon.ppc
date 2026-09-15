// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Equipments.Meters.PowerMeters.Simples;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Maths.Randoms;
using paskalON.Protocols.C37118.Simulations;
using System.Diagnostics;

namespace paskalON.DeviceSimulator.Equipments.Meters.PowerMeters.Simples
{
    /// <summary>
    /// Simulates a simple C37 system power meter.
    /// </summary>
    public sealed class SystemPowerMeterSimpleV1Simulation : ISimulatedDevice
    {
        // TODO: read from configuration
        private readonly float _referenceFrequency = 60;


        /// <summary>
        /// Random walker for frequency.
        /// </summary>
        private readonly RandomWalker<float> _frequencyWalker;


        /// <summary>
        /// C37 stream data produced by this simulation.
        /// </summary>
        private readonly PmuDataSimulation _stream;


        /// <summary>
        /// Simulated device instance.
        /// </summary>
        private readonly SystemPowerMeterSimpleV1Proxy _device;


        /// <summary>
        /// List of all power conversion systems.
        /// </summary>
        private readonly ICollection<PowerConversionSystemBase> _powerConversionSystems;


        /// <inheritdoc/>
        public string Name { get => _device.Name; }


        /// <summary>
        /// Constructor of <see cref="SystemPowerMeterSimpleV1Simulation"/>.
        /// </summary>
        /// <param name="stream">Backing C37 stream.</param>
        /// <param name="name">Name of the simulated device.</param>
        public SystemPowerMeterSimpleV1Simulation(PmuDataSimulation stream, SystemPowerMeterSimpleV1Proxy device,
            ICollection<PowerConversionSystemBase> powerConversionSystems)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(device);
            ArgumentNullException.ThrowIfNull(powerConversionSystems);

            _stream = stream;
            _device = device;
            _powerConversionSystems = powerConversionSystems;
            _frequencyWalker = new RandomWalker<float>(_referenceFrequency, 0.005f, _referenceFrequency - 0.03f, _referenceFrequency + 0.03f, 1);
        }


        /// <inheritdoc/>
        public Task TickAsync(TimeSpan elapsed, CancellationToken cancellationToken)
        {
            // System meter is at the POI so it measures units that are IsInMaintenanceMode too
            double sumKiloActive = _powerConversionSystems.Where(p =>
                p.ActivePower.HasValue).Sum(s => s.ActivePower!.Value.KiloWatts);
            double sumKiloReactive = _powerConversionSystems.Where(p =>
                p.ReactivePower.HasValue).Sum(s => s.ReactivePower!.Value.KiloVoltAmperesReactive);

            _stream.Frequency = _frequencyWalker.Next();
            _stream.Analogs["Analog1"].Measurement = (float)sumKiloActive;
            _stream.Analogs["Analog5"].Measurement = (float)sumKiloReactive;

            if (System.Diagnostics.Debugger.IsAttached == true)
            {
                Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} - {Name} - " +
                    $"StationName: {_stream.StationName} StreamId: {_stream.StreamId} " +
                    $"Active Power: {_stream.Analogs["Analog1"].Measurement} " +
                    $"Reactive Power: {_stream.Analogs["Analog5"].Measurement} " +
                    $"Frequency: {_stream.Frequency}");
            }

            return Task.CompletedTask;
        }
    }
}

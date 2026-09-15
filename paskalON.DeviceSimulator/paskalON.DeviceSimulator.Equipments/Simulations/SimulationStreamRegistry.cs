// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Protocols.C37118.Simulations;

namespace paskalON.DeviceSimulator.Equipments.Simulations
{
    /// <summary>
    /// Thread-safe registry of simulator C37.118 streams.
    /// </summary>
    public sealed class SimulationStreamRegistry : ISimulationStreamRegistry
    {
        /// <summary>
        /// Registered streams keyed by endpoint and stream identifier.
        /// </summary>
        private readonly Dictionary<PmuDataSimulationKey, PmuDataSimulation> _streams = new();


        /// <summary>
        /// Data lock object.
        /// </summary>
        private readonly object _dataLock = new();


        /// <inheritdoc/>
        public IReadOnlyDictionary<PmuDataSimulationKey, PmuDataSimulation> Streams
        {
            get
            {
                lock (_dataLock)
                {
                    return new Dictionary<PmuDataSimulationKey, PmuDataSimulation>(_streams);
                }
            }
        }


        /// <inheritdoc/>
        public PmuDataSimulation GetOrCreate(PowerMeterBase device)
        {
            PmuDataSimulationKey key = new PmuDataSimulationKey
            {
                Address = device.TargetAddress,
                Port = device.TargetPort,
                StationName = device.TargetStationName,
                StreamId = device.TargetStreamId
            };

            lock (_dataLock)
            {
                if (_streams.TryGetValue(key, out PmuDataSimulation? stream) == true)
                {
                    return stream;
                }

                (Dictionary<string, AnalogMeasurement> Analogs, Dictionary<string, PhasorMeasurement> Phasors) measures = GetMeasurements(device);
                stream = new PmuDataSimulation
                {
                    StationName = device.TargetStationName,
                    StreamId = device.TargetStreamId,
                    Analogs = measures.Analogs.AsReadOnly(),
                    Phasors = measures.Phasors.AsReadOnly(),
                };
                _streams.Add(key, stream);

                return stream;
            }
        }


        private (Dictionary<string, AnalogMeasurement> Analogs, Dictionary<string, PhasorMeasurement> Phasors) GetMeasurements(PowerMeterBase device)
        {
            Dictionary<string, AnalogMeasurement> analogs = new Dictionary<string, AnalogMeasurement>();
            Dictionary<string, PhasorMeasurement> phasors = new Dictionary<string, PhasorMeasurement>();

            if (device.C37Map != null)
            {
                AddAnalog(analogs, device.C37Map.ApparentPower);
                AddAnalog(analogs, device.C37Map.ActivePower);
                AddAnalog(analogs, device.C37Map.ActivePowerA);
                AddAnalog(analogs, device.C37Map.ActivePowerB);
                AddAnalog(analogs, device.C37Map.ActivePowerC);
                AddAnalog(analogs, device.C37Map.ReactivePower);
                AddAnalog(analogs, device.C37Map.ReactivePowerA);
                AddAnalog(analogs, device.C37Map.ReactivePowerB);
                AddAnalog(analogs, device.C37Map.ReactivePowerC);
                AddAnalog(analogs, device.C37Map.EnergyDelivered);
                AddAnalog(analogs, device.C37Map.EnergyReceived);
                AddAnalog(analogs, device.C37Map.ReactiveEnergyDelivered);
                AddAnalog(analogs, device.C37Map.ReactiveEnergyReceived);
                AddAnalog(analogs, device.C37Map.VoltageLLAvg);

                AddPhasor(phasors, device.C37Map.VoltageA, PhasorUnitTypes.Voltage);
                AddPhasor(phasors, device.C37Map.VoltageB, PhasorUnitTypes.Voltage);
                AddPhasor(phasors, device.C37Map.VoltageC, PhasorUnitTypes.Voltage);
                AddPhasor(phasors, device.C37Map.VoltageAB, PhasorUnitTypes.Voltage);
                AddPhasor(phasors, device.C37Map.VoltageBC, PhasorUnitTypes.Voltage);
                AddPhasor(phasors, device.C37Map.VoltageCA, PhasorUnitTypes.Voltage);
                AddPhasor(phasors, device.C37Map.VoltagePositiveSequence, PhasorUnitTypes.Voltage);
                AddPhasor(phasors, device.C37Map.CurrentA, PhasorUnitTypes.Current);
                AddPhasor(phasors, device.C37Map.CurrentB, PhasorUnitTypes.Current);
                AddPhasor(phasors, device.C37Map.CurrentC, PhasorUnitTypes.Current);
            }

            return (analogs, phasors);
        }


        private void AddAnalog(Dictionary<string, AnalogMeasurement> analogs, string? name)
        {
            if (string.IsNullOrEmpty(name) == false)
            {
                analogs.Add(name, new AnalogMeasurement(name, 0));
            }
        }


        private void AddPhasor(Dictionary<string, PhasorMeasurement> phasors, string? name, PhasorUnitTypes type)
        {
            if (string.IsNullOrEmpty(name) == false)
            {
                phasors.Add(name, new PhasorMeasurement(name, 0, 0, type));
            }
        }
    }
}
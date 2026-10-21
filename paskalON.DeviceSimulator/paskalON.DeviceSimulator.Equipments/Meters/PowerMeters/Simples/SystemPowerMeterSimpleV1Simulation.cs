// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Equipments.Meters.PowerMeters.Simples;
using paskalON.DeviceSimulator.Equipments.Grids;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.C37118.Simulations;

namespace paskalON.DeviceSimulator.Equipments.Meters.PowerMeters.Simples
{
    /// <summary>
    /// Simulates a simple C37 system power meter at the point of interconnection.
    /// </summary>
    /// <remarks>
    /// The meter measures the sum of all simulated PCS active and reactive powers (including units in maintenance mode),
    /// split evenly over the three phases. Voltages are the nominal grid voltages and currents follow from
    /// I = S / (√3 · V). Phasor angles are in radians.
    /// </remarks>
    public sealed class SystemPowerMeterSimpleV1Simulation : ISimulatedDevice
    {
        /// <summary>
        /// Phase shift between two phases in radians (120°).
        /// </summary>
        private const double PhaseShift = (2 * Math.PI) / 3;


        /// <summary>
        /// C37 stream data produced by this simulation.
        /// </summary>
        private readonly PmuDataSimulation _stream;


        /// <summary>
        /// Simulated device instance.
        /// </summary>
        private readonly SystemPowerMeterSimpleV1Proxy _device;


        /// <summary>
        /// All simulated power conversion systems measured by this meter.
        /// </summary>
        private readonly IReadOnlyCollection<PowerConversionSystemSimulationBase> _powerConversionSystems;


        /// <summary>
        /// Shared simulated grid.
        /// </summary>
        private readonly SimulatedGrid _grid;


        /// <inheritdoc/>
        public int DeviceId { get => _device.DeviceId; }


        /// <inheritdoc/>
        public string Name { get => _device.Name; }


        /// <summary>
        /// Constructor of <see cref="SystemPowerMeterSimpleV1Simulation"/>.
        /// </summary>
        /// <param name="stream">Backing C37 stream.</param>
        /// <param name="device">Simulated device.</param>
        /// <param name="powerConversionSystems">All simulated power conversion systems measured by this meter.</param>
        /// <param name="grid">Shared simulated grid.</param>
        public SystemPowerMeterSimpleV1Simulation(PmuDataSimulation stream, SystemPowerMeterSimpleV1Proxy device,
            IReadOnlyCollection<PowerConversionSystemSimulationBase> powerConversionSystems, SimulatedGrid grid)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(device);
            ArgumentNullException.ThrowIfNull(powerConversionSystems);
            ArgumentNullException.ThrowIfNull(grid);

            _stream = stream;
            _device = device;
            _powerConversionSystems = powerConversionSystems;
            _grid = grid;
        }


        /// <inheritdoc/>
        public Task TickAsync(TimeSpan elapsed, CancellationToken cancellationToken)
        {
            double direction = _device.IsReversePowerFlow ? -1 : 1;
            double activePower = _powerConversionSystems.Sum(pcs => pcs.ActivePower) * direction;
            double reactivePower = _powerConversionSystems.Sum(pcs => pcs.ReactivePower) * direction;
            double apparentPower = Math.Sqrt(activePower * activePower + reactivePower * reactivePower);
            double voltageLineToLine = _grid.NominalVoltageLineToLine;
            double voltagePhase = voltageLineToLine / Math.Sqrt(3);
            double current = voltageLineToLine > 0 ? apparentPower / (Math.Sqrt(3) * voltageLineToLine) : 0;
            // Current lags the voltage by the power factor angle
            double powerFactorAngle = apparentPower > 0 ? Math.Atan2(reactivePower, activePower) : 0;

            _stream.Frequency = (float)_grid.Frequency;

            PowerMeterMapC37Config? map = _device.C37Map;

            if (map is null)
            {
                return Task.CompletedTask;
            }

            // Power
            SetAnalog(map.ActivePower, activePower);
            SetAnalog(map.ActivePowerA, activePower / 3);
            SetAnalog(map.ActivePowerB, activePower / 3);
            SetAnalog(map.ActivePowerC, activePower / 3);
            SetAnalog(map.ReactivePower, reactivePower);
            SetAnalog(map.ReactivePowerA, reactivePower / 3);
            SetAnalog(map.ReactivePowerB, reactivePower / 3);
            SetAnalog(map.ReactivePowerC, reactivePower / 3);
            SetAnalog(map.ApparentPower, apparentPower);
            SetAnalog(map.VoltageLLAvg, voltageLineToLine);
            // Voltage phase to neutral and positive sequence
            SetPhasor(map.VoltageA, voltagePhase, 0);
            SetPhasor(map.VoltageB, voltagePhase, -PhaseShift);
            SetPhasor(map.VoltageC, voltagePhase, PhaseShift);
            SetPhasor(map.VoltagePositiveSequence, voltagePhase, 0);
            // Voltage line to line leads the phase voltage by 30°
            SetPhasor(map.VoltageAB, voltageLineToLine, Math.PI / 6);
            SetPhasor(map.VoltageBC, voltageLineToLine, Math.PI / 6 - PhaseShift);
            SetPhasor(map.VoltageCA, voltageLineToLine, Math.PI / 6 + PhaseShift);
            // Current
            SetPhasor(map.CurrentA, current, -powerFactorAngle);
            SetPhasor(map.CurrentB, current, -PhaseShift - powerFactorAngle);
            SetPhasor(map.CurrentC, current, PhaseShift - powerFactorAngle);

            return Task.CompletedTask;
        }


        /// <summary>
        /// Sets an analog measurement when it is mapped.
        /// </summary>
        private void SetAnalog(string? endpoint, double value)
        {
            if (string.IsNullOrEmpty(endpoint) == false && _stream.Analogs.TryGetValue(endpoint, out AnalogMeasurement? analog))
            {
                analog.Measurement = (float)value;
            }
        }


        /// <summary>
        /// Sets a phasor measurement when it is mapped.
        /// </summary>
        private void SetPhasor(string? endpoint, double magnitude, double angle)
        {
            if (string.IsNullOrEmpty(endpoint) == false && _stream.Phasors.TryGetValue(endpoint, out PhasorMeasurement? phasor))
            {
                phasor.Magnitude = (float)magnitude;
                phasor.Angle = (float)NormalizeAngle(angle);
            }
        }


        /// <summary>
        /// Normalizes an angle in radians to the range -π..π.
        /// </summary>
        private static double NormalizeAngle(double angle)
        {
            return Math.Atan2(Math.Sin(angle), Math.Cos(angle));
        }
    }
}

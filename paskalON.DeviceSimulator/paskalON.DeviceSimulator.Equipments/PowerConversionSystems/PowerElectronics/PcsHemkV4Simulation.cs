// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Equipments.PowerConversionSystems.PowerElectronics;
using paskalON.DeviceSimulator.Application.Simulations;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Equipments.PowerConversionSystems.PowerElectronics
{
    /// <summary>
    /// Simulates the command and telemetry registers of a HEMK V4 PCS.
    /// </summary>
    public class PcsHemkV4Simulation : ISimulatedDevice
    {
        /// <summary>
        /// Modbus data store for this device simulation.
        /// </summary>
        private readonly IModbusDataStore _store;


        /// <inheritdoc/>
        public string Name { get; init; }


        /// <summary>
        /// Constructor of <see cref="PcsHemkV4Simulation"/>.
        /// </summary>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="name">Name of the simulated device.</param>
        public PcsHemkV4Simulation(IModbusDataStore store, string name)
        {
            ArgumentNullException.ThrowIfNull(store);
            ArgumentNullException.ThrowIfNull(name);

            _store = store;
            Name = name;
        }


        /// <inheritdoc/>
        public Task TickAsync(TimeSpan elapsed, CancellationToken cancellationToken)
        {
            ushort selector = RegisterSimulation.Read(_store, (int)PcsPcskV4Description.Register.SelectorState);
            ushort reactiveTarget = RegisterSimulation.Read(_store, (int)PcsPcskV4Description.Register.QReference);
            ushort activeTarget = RegisterSimulation.Read(_store, (int)PcsPcskV4Description.Register.PReference);

            RegisterSimulation.Write(_store, (int)PcsPcskV4Description.Register.CurrentState,
                selector == 1 ? (ushort)PcsPcskV4Description.State.On : (ushort)PcsPcskV4Description.State.Off);
            RegisterSimulation.Write(_store, (int)PcsPcskV4Description.Register.P, activeTarget);
            RegisterSimulation.Write(_store, (int)PcsPcskV4Description.Register.Q, reactiveTarget);
            RegisterSimulation.Write(_store, (int)PcsPcskV4Description.Register.PCapability, 60000);
            RegisterSimulation.Write(_store, (int)PcsPcskV4Description.Register.QCapability, 60000);

            return Task.CompletedTask;
        }
    }
}
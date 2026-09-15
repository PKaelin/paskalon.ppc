// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Equipments.PowerConversionSystems.PowerElectronics;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Equipments.PowerConversionSystems.PowerElectronics
{
    /// <summary>
    /// Simulates the command and telemetry registers of a HEMK V4 PCS.
    /// </summary>
    public sealed class PcsHemkV4Simulation : ISimulatedDevice
    {
        /// <summary>
        /// Modbus data store for this device simulation.
        /// </summary>
        private readonly IModbusDataStore _store;


        /// <summary>
        /// Simulated device instance.
        /// </summary>
        private readonly PcsHemkV4Proxy _device;


        /// <summary>
        /// Battery storage unit of the device instance.
        /// </summary>
        private readonly DerBatteryStorageUnit _unit;


        /// <inheritdoc/>
        public string Name { get => _device.Name; }


        /// <summary>
        /// Constructor of <see cref="PcsHemkV4Simulation"/>.
        /// </summary>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="device">Simulated device.</param>
        public PcsHemkV4Simulation(IModbusDataStore store, PcsHemkV4Proxy device)
        {
            ArgumentNullException.ThrowIfNull(store);
            ArgumentNullException.ThrowIfNull(device);

            _store = store;
            _device = device;
            _unit = (DerBatteryStorageUnit)device.DerUnit;
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
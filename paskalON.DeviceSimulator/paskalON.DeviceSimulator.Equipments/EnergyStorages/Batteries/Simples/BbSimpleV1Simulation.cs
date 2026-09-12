// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.Modbus.Stores;
using System.Diagnostics;

namespace paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries.Simples
{
    /// <summary>
    /// Simulates the register behavior of a Battery Bank Simple V1.
    /// </summary>
    public sealed class BbSimpleV1Simulation : ISimulatedDevice
    {
        /// <summary>
        /// Modbus data store for this device simulation.
        /// </summary>
        private readonly IModbusDataStore _store;


        /// <inheritdoc/>
        public string Name { get; init; }


        /// <summary>
        /// Constructor of <see cref="BbSimpleV1Simulation"/>.
        /// </summary>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="name">Name of the simulated device.</param>
        public BbSimpleV1Simulation(IModbusDataStore store, string name)
        {
            _store = store;
            Name = name;
        }


        /// <inheritdoc/>
        public Task TickAsync(TimeSpan elapsed, CancellationToken cancellationToken)
        {
            ushort heartbeat = RegisterSimulation.Read(_store, (int)BbSimpleV1Description.Register.Heartbeat);
            ushort selector = RegisterSimulation.Read(_store, (int)BbSimpleV1Description.Register.SelectorState);

            ushort? currentState = null;

            switch (selector)
            {
                case 0: currentState = (ushort)BbSimpleV1Description.State.Disconnected; break;
                case 1: currentState = (ushort)BbSimpleV1Description.State.Connected; break;
            }

            if (currentState != null)
            {
                RegisterSimulation.Write(_store, (int)BbSimpleV1Description.Register.CurrentState, (ushort)currentState);
            }

            if (System.Diagnostics.Debugger.IsAttached == true)
            {
                Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} - {Name} - Heartbeat: {heartbeat} " +
                    $"Selector: {selector}");
            }

            return Task.CompletedTask;
        }
    }
}

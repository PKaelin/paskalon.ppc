// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.Modbus.Stores;
using System.Diagnostics;

namespace paskalON.DeviceSimulator.Equipments.PowerConversionSystems.Simples
{
    /// <summary>
    /// Simulates the register behavior of a simple V1 PCS.
    /// </summary>
    public sealed class PcsSimpleV1Simulation : ISimulatedDevice
    {
        /// <summary>
        /// Modbus data store for this device simulation.
        /// </summary>
        private readonly IModbusDataStore _store;


        /// <inheritdoc/>
        public string Name { get; init; }


        /// <summary>
        /// Constructor of <see cref="PcsSimpleV1Simulation"/>.
        /// </summary>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="name">Name of the simulated device.</param>
        public PcsSimpleV1Simulation(IModbusDataStore store, string name)
        {
            ArgumentNullException.ThrowIfNull(store);
            ArgumentNullException.ThrowIfNull(name);

            _store = store;
            Name = name;
        }


        /// <inheritdoc/>
        public Task TickAsync(TimeSpan elapsed, CancellationToken cancellationToken)
        {
            ushort heartbeat = RegisterSimulation.Read(_store, (int)PcsSimpleV1Description.Register.Heartbeat);
            ushort selector = RegisterSimulation.Read(_store, (int)PcsSimpleV1Description.Register.SelectorState);
            ushort reactiveTarget = RegisterSimulation.Read(_store, (int)PcsSimpleV1Description.Register.QReference);
            ushort activeTarget = RegisterSimulation.Read(_store, (int)PcsSimpleV1Description.Register.PReference);

            ushort? currentState = null;

            switch (selector)
            {
                case 0: currentState = (ushort)PcsSimpleV1Description.State.Off; break;
                case 1: currentState = (ushort)PcsSimpleV1Description.State.On; break;
                case 3: currentState = (ushort)PcsSimpleV1Description.State.Standby; break;
            }

            if (currentState != null)
            {
                RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.CurrentState, (ushort)currentState);
            }

            RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.P, activeTarget);
            RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.Q, reactiveTarget);
            RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.PAvailable, 60000);
            RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.QAvailable, 60000);
            RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.ACBreaker, 1);
            RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.DcContactor, 1);

            if (System.Diagnostics.Debugger.IsAttached == true)
            {
                Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} - {Name} - Heartbeat: {heartbeat} " +
                    $"Selector: {selector} ActiveTarget: {activeTarget} ReactiveTarget: {reactiveTarget}");
            }

            return Task.CompletedTask;
        }
    }
}
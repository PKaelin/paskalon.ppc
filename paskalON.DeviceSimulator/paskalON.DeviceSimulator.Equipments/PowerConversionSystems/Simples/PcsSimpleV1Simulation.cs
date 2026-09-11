// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.DeviceSimulator.Application.Simulations;
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


        /// <summary>
        /// Constructor of <see cref="PcsSimpleV1Simulation"/>.
        /// </summary>
        /// <param name="store">Backing Modbus store.</param>
        public PcsSimpleV1Simulation(IModbusDataStore store)
        {
            ArgumentNullException.ThrowIfNull(store);
            _store = store;
        }


        /// <inheritdoc/>
        public Task TickAsync(TimeSpan elapsed, CancellationToken cancellationToken)
        {
            ushort heartbeat = RegisterSimulation.Read(_store, (int)PcsSimpleV1Description.Register.Heartbeat);
            ushort selector = RegisterSimulation.Read(_store, (int)PcsSimpleV1Description.Register.SelectorState);
            ushort reactiveTarget = RegisterSimulation.Read(_store, (int)PcsSimpleV1Description.Register.QReference);
            ushort activeTarget = RegisterSimulation.Read(_store, (int)PcsSimpleV1Description.Register.PReference);

            RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.CurrentState,
                selector == 1 ? (ushort)PcsSimpleV1Description.State.On : (ushort)PcsSimpleV1Description.State.Off);
            RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.P, activeTarget);
            RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.Q, reactiveTarget);
            RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.PAvailable, 60000);
            RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.QAvailable, 60000);
            RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.ACBreaker, 1);
            RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.DcContactor, 1);

            if (System.Diagnostics.Debugger.IsAttached == true)
            {
                Debug.WriteLine($"PCS Sim Time: {DateTime.Now.ToString("HH:mm:ss")}");
                Debug.WriteLine($"Heartbeat: {heartbeat}");
                Debug.WriteLine($"Selector: {selector}");
                Debug.WriteLine($"ActiveTarget: {activeTarget}");
                Debug.WriteLine($"ReactiveTarget: {reactiveTarget}");
            }

            return Task.CompletedTask;
        }
    }
}
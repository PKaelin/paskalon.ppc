// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Ders;
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


        /// <summary>
        /// Simulated device instance.
        /// </summary>
        private readonly PcsSimpleV1Proxy _device;


        /// <summary>
        /// Battery storage unit of the device instance.
        /// </summary>
        private readonly DerBatteryStorageUnit _unit;


        /// <inheritdoc/>
        public string Name { get => _device.Name; }


        /// <summary>
        /// Constructor of <see cref="PcsSimpleV1Simulation"/>.
        /// </summary>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="device">Simulated device.</param>
        public PcsSimpleV1Simulation(IModbusDataStore store, PcsSimpleV1Proxy device)
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
            ushort heartbeat = RegisterSimulation.Read(_store, (int)PcsSimpleV1Description.Register.Heartbeat);
            ushort selector = RegisterSimulation.Read(_store, (int)PcsSimpleV1Description.Register.SelectorState);
            double reactiveTarget = RegisterSimulation.Read<double>(_store, (int)PcsSimpleV1Description.Register.QReference, ModbusDataType.MbInt16, ModbusScale.Upscale1000);
            double activeTarget = RegisterSimulation.Read<double>(_store, (int)PcsSimpleV1Description.Register.PReference, ModbusDataType.MbInt16, ModbusScale.Upscale1000);

            ushort? currentState = null;

            switch (selector)
            {
                case 2: currentState = (ushort)PcsSimpleV1Description.State.Off; break;
                case 3: currentState = (ushort)PcsSimpleV1Description.State.On; break;
                case 6: currentState = (ushort)PcsSimpleV1Description.State.Standby; break;
            }

            if (currentState != null)
            {
                RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.CurrentState, (ushort)currentState);
            }

            // Whatever the reference is also the actual output in a perfect system
            RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.P, activeTarget, ModbusDataType.MbInt16, ModbusScale.Downscale1000);
            RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.Q, reactiveTarget, ModbusDataType.MbInt16, ModbusScale.Downscale1000);
            // For now we just set the nameplate
            RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.PAvailable, _device.NameplateMaximumActivePower.KiloWatts, ModbusDataType.MbInt16);
            RegisterSimulation.Write(_store, (int)PcsSimpleV1Description.Register.QAvailable, _device.NameplateMaximumReactivePower.KiloVoltAmperesReactive, ModbusDataType.MbInt16);
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
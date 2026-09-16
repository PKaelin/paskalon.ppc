// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Dataface.Modbus;
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


        /// <summary>
        /// Simulated device instance.
        /// </summary>
        private readonly BbSimpleV1Proxy _device;


        /// <inheritdoc/>
        public string Name { get => _device.Name; }


        /// <summary>
        /// Useable state of charge of the battery bank.
        /// </summary>
        public double UsableStateOfCharge { get; private set; }


        /// <summary>
        /// Useable capacity of the battery bank.
        /// </summary>
        public double UsableCapacity { get; private set; }


        /// <summary>
        /// Constructor of <see cref="BbSimpleV1Simulation"/>.
        /// </summary>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="device">Simulated device.</param>
        public BbSimpleV1Simulation(IModbusDataStore store, BbSimpleV1Proxy device)
        {
            ArgumentNullException.ThrowIfNull(store);
            ArgumentNullException.ThrowIfNull(device);

            _store = store;
            _device = device;
            // Start of with 100% preferred maximum
            double preferredStateOfCharge = 100;
            double absoluteStateOfCharge = device.PreferredMinimumStateOfCharge + (preferredStateOfCharge / device.AbsoluteMaximumStateOfCharge) *
                    (device.PreferredMaximumStateOfCharge - device.PreferredMinimumStateOfCharge);
            UsableStateOfCharge = (absoluteStateOfCharge - device.UsableMinimumStateOfCharge) /
                (device.UsableMaximumStateOfCharge - device.UsableMinimumStateOfCharge) * device.AbsoluteMaximumStateOfCharge;
            UsableCapacity = device.NameplateCapacity * (device.UsableMaximumStateOfCharge - device.UsableMinimumStateOfCharge) /
                (device.AbsoluteMaximumStateOfCharge - device.AbsoluteMinimumStateOfCharge);
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

            // AllocatedActivePowerValue in watts, elapsed.TotalHours in hours, NameplateCapacity in watt-hours
            // E.g. NameplateCapacity=5MWh / AbsoluteSoc=100 / UsableSoc=90  => PreferredCapacity=5MWh*0.9=4.5MWh
            // To reach 0% Usable SoC (90% down to 10% Absolute): Time = PreferredCapacity/AllocatedActivePowerValue=0.9h
            // To reach 0% Absolute SoC (100 % down to 0 % Absolute): Time = NameplateCapacity/AllocatedActivePowerValue=1h
            double socDelta = ((_device.AllocatedActivePowerValue ?? 0) * elapsed.TotalHours / UsableCapacity) * 100;


            // Device simulation does not consider preferred SOC but cuts of on 0 and 100 percent usable SOC
            if ((_device.AllocatedActivePowerValue ?? 0) > 0 && UsableStateOfCharge > 0)
            {
                UsableStateOfCharge -= socDelta;

                if (UsableStateOfCharge < 0)
                {
                    UsableStateOfCharge = 0;
                }
            }
            else if ((_device.AllocatedActivePowerValue ?? 0) < 0 && UsableStateOfCharge < 100)
            {
                UsableStateOfCharge += socDelta;

                if (UsableStateOfCharge > 100)
                {
                    UsableStateOfCharge = 100;
                }
            }

            RegisterSimulation.Write(_store, (int)BbSimpleV1Description.Register.TotalStateOfCharge, (ushort)UsableStateOfCharge,
                ModbusDataType.MbUint16, ModbusScale.Upscale100);

            if (System.Diagnostics.Debugger.IsAttached == true)
            {
                Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} - {Name} - Heartbeat: {heartbeat} " +
                    $"Selector: {selector} SOC: {Math.Round(UsableStateOfCharge, 3)} " +
                    $"Allocated Power: {Math.Round(_device.AllocatedActivePowerValue ?? 0, 2)}");
            }

            return Task.CompletedTask;
        }
    }
}

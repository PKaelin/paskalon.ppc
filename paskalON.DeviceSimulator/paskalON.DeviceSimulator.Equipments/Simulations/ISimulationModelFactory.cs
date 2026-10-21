// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries;
using paskalON.DeviceSimulator.Equipments.Grids;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems;
using paskalON.Protocols.C37118.Simulations;

namespace paskalON.DeviceSimulator.Equipments.Simulations
{
    /// <summary>
    /// Creates a simulator model for a production device.
    /// </summary>
    public interface ISimulationModelFactory
    {
        /// <summary>
        /// Creates a PCS simulation model when the device type is supported.
        /// </summary>
        /// <param name="device">Production device proxy.</param>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="grid">Shared simulated grid.</param>
        /// <returns>A simulation model, or null when unsupported.</returns>
        PowerConversionSystemSimulationBase? Create(PowerConversionSystemBase device, SimulationModbusDataStore store, SimulatedGrid grid);


        /// <summary>
        /// Creates a battery bank simulation model when the device type is supported.
        /// </summary>
        /// <param name="device">Production device proxy.</param>
        /// <param name="store">Backing Modbus store.</param>
        /// <param name="customs">Simulator custom configurations of the device.</param>
        /// <returns>A simulation model, or null when unsupported.</returns>
        BatteryBankSimulationBase? Create(BatteryBankBase device, SimulationModbusDataStore store, IReadOnlyDictionary<string, string> customs);


        /// <summary>
        /// Creates a simulation model when the power meter type is supported.
        /// </summary>
        /// <param name="device">Production power meter.</param>
        /// <param name="stream">Backing C37 stream.</param>
        /// <param name="powerConversionSystems">All simulated power conversion systems.</param>
        /// <param name="grid">Shared simulated grid.</param>
        /// <returns>A simulation model, or null when unsupported.</returns>
        ISimulatedDevice? Create(PowerMeterBase device, PmuDataSimulation stream, IReadOnlyCollection<PowerConversionSystemSimulationBase> powerConversionSystems,
            SimulatedGrid grid);
    }
}

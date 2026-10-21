// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.Devices.Equipments.Meters.PowerMeters.Simples;
using paskalON.Devices.Equipments.PowerConversionSystems.PowerElectronics;
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries;
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.DeviceSimulator.Equipments.Grids;
using paskalON.DeviceSimulator.Equipments.Meters.PowerMeters.Simples;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems.PowerElectronics;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems.Simples;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.C37118.Simulations;

namespace paskalON.DeviceSimulator.Application.Factories
{
    /// <summary>
    /// Creates simulator models for supported production device proxies.
    /// </summary>
    public sealed class PowerConversionSystemSimulationModelFactory : ISimulationModelFactory
    {
        /// <inheritdoc/>
        public PowerConversionSystemSimulationBase? Create(PowerConversionSystemBase device, SimulationModbusDataStore store, SimulatedGrid grid)
        {
            ArgumentNullException.ThrowIfNull(device);
            ArgumentNullException.ThrowIfNull(store);
            ArgumentNullException.ThrowIfNull(grid);

            return device switch
            {
                PcsSimpleV1Proxy proxy => new PcsSimpleV1Simulation(store, proxy, grid),
                PcsPcskV4Proxy proxy => new PcsPcskV4Simulation(store, proxy, grid),
                PcsHemkV4Proxy proxy => new PcsHemkV4Simulation(store, proxy, grid),
                _ => null
            };
        }


        /// <inheritdoc/>
        public BatteryBankSimulationBase? Create(BatteryBankBase device, SimulationModbusDataStore store, IReadOnlyDictionary<string, string> customs)
        {
            ArgumentNullException.ThrowIfNull(device);
            ArgumentNullException.ThrowIfNull(store);
            ArgumentNullException.ThrowIfNull(customs);

            return device switch
            {
                BbSimpleV1Proxy proxy => new BbSimpleV1Simulation(store, proxy, customs),
                _ => null
            };
        }


        /// <inheritdoc/>
        public ISimulatedDevice? Create(PowerMeterBase device, PmuDataSimulation stream, IReadOnlyCollection<PowerConversionSystemSimulationBase> powerConversionSystems,
            SimulatedGrid grid)
        {
            ArgumentNullException.ThrowIfNull(device);
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(powerConversionSystems);
            ArgumentNullException.ThrowIfNull(grid);

            return device switch
            {
                SystemPowerMeterSimpleV1Proxy proxy => new SystemPowerMeterSimpleV1Simulation(stream, proxy, powerConversionSystems, grid),
                _ => null
            };
        }
    }
}

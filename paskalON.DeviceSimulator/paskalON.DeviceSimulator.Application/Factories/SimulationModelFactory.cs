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
using paskalON.DeviceSimulator.Equipments.EnergyStorages.Batteries.Simples;
using paskalON.DeviceSimulator.Equipments.Meters.PowerMeters.Simples;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems.PowerElectronics;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems.Simples;
using paskalON.DeviceSimulator.Equipments.Simulations;
using paskalON.Protocols.C37118.Simulations;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Application.Factories
{
    /// <summary>
    /// Creates simulator models for supported production PCS proxies.
    /// </summary>
    public sealed class PowerConversionSystemSimulationModelFactory : ISimulationModelFactory
    {
        /// <inheritdoc/>
        public ISimulatedDevice? Create(PowerConversionSystemBase device, IModbusDataStore store)
        {
            ArgumentNullException.ThrowIfNull(device);
            ArgumentNullException.ThrowIfNull(store);

            return device switch
            {
                PcsSimpleV1Proxy => new PcsSimpleV1Simulation(store, (PcsSimpleV1Proxy)device),
                PcsPcskV4Proxy => new PcsPcskV4Simulation(store, (PcsPcskV4Proxy)device),
                PcsHemkV4Proxy => new PcsHemkV4Simulation(store, (PcsHemkV4Proxy)device),
                _ => null
            };
        }


        /// <inheritdoc/>
        public ISimulatedDevice? Create(BatteryBankBase device, IModbusDataStore store)
        {
            ArgumentNullException.ThrowIfNull(device);
            ArgumentNullException.ThrowIfNull(store);

            return device switch
            {
                BbSimpleV1Proxy => new BbSimpleV1Simulation(store, (BbSimpleV1Proxy)device),
                _ => null
            };
        }


        /// <inheritdoc/>
        public ISimulatedDevice? Create(PowerMeterBase device, PmuDataSimulation stream, ICollection<PowerConversionSystemBase> powerConversionSystems)
        {
            ArgumentNullException.ThrowIfNull(device);
            ArgumentNullException.ThrowIfNull(stream);


            return device switch
            {
                SystemPowerMeterSimpleV1Proxy => new SystemPowerMeterSimpleV1Simulation(stream, (SystemPowerMeterSimpleV1Proxy)device, powerConversionSystems),
                _ => null
            };
        }
    }
}
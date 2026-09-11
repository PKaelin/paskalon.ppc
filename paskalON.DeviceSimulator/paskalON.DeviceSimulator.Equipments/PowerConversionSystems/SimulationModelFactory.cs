// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Devices.Equipments.PowerConversionSystems.PowerElectronics;
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.DeviceSimulator.Application.Simulations;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems.PowerElectronics;
using paskalON.DeviceSimulator.Equipments.PowerConversionSystems.Simples;
using paskalON.Protocols.Modbus.Stores;

namespace paskalON.DeviceSimulator.Equipments.PowerConversionSystems
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
                PcsSimpleV1Proxy => new PcsSimpleV1Simulation(store),
                PcsPcskV4Proxy => new PcsPcskV4Simulation(store),
                PcsHemkV4Proxy => new PcsHemkV4Simulation(store),
                _ => null
            };
        }
    }
}
// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Infrastructure.Storage;

namespace paskalON.DemoSuperSimpleBattery.Devices.Data
{
    static class ServiceSimulatorData
    {
        /// <summary>
        /// Create simulation data for power conversion system device.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="config">The device configuration the simulations are added to.</param>
        public static void CreatePowerConversionSystemDeviceSim(IDeviceServiceContext context, PowerConversionSystemDeviceConfig config)
        {
            PowerConversionSystemDeviceCustomConfig sim1 = new PowerConversionSystemDeviceCustomConfig
            {
                ChangedBy = config.ChangedBy,
                Key = "PcsSimulationKey",
                Value = "Value",
                Description = "Sample entry for simulation"
            };

            config.Customs.Add(sim1);
            context.PowerConversionSystemDeviceCustomConfigs.Add(sim1);
        }


        /// <summary>
        /// Create simulation data for battery bank device.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="config">The device configuration the simulations are added to.</param>
        public static void CreateBatteryBankDeviceSim(IDeviceServiceContext context, BatteryBankDeviceConfig config)
        {
            BatteryBankDeviceCustomConfig sim1 = new BatteryBankDeviceCustomConfig
            {
                ChangedBy = config.ChangedBy,
                Key = "BbSimulationKey",
                Value = "Value",
                Description = "Sample entry for simulation"
            };

            config.Customs.Add(sim1);
            context.BatteryBankDeviceCustomConfigs.Add(sim1);
        }
    }
}

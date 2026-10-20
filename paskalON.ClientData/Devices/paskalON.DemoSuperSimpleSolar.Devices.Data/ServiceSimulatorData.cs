// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Infrastructure.Storage;

namespace paskalON.DemoSuperSimpleSolar.Devices.Data
{
    static class ServiceSimulatorData
    {
        public static void CreatePowerConversionSystemDeviceSim(IDeviceServiceContext context, PowerConversionSystemDeviceConfig config)
        {
        }


        /// <summary>
        /// Create simulation data for power meter device.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="config">The device configuration the simulations are added to.</param>
        public static void CreatePowerMeterDeviceSim(IDeviceServiceContext context, PowerMeterDeviceConfig config)
        {
            PowerMeterDeviceCustomConfig frequency = new PowerMeterDeviceCustomConfig
            {
                ChangedBy = config.ChangedBy,
                Key = "SimGridNominalFrequency",
                Value = "60",
                Description = "Nominal frequency in hertz of the simulated grid"
            };

            PowerMeterDeviceCustomConfig voltage = new PowerMeterDeviceCustomConfig
            {
                ChangedBy = config.ChangedBy,
                Key = "SimGridNominalVoltageLineToLine",
                Value = "34500",
                Description = "Nominal line to line voltage in volts of the simulated grid at the system power meter"
            };

            config.Customs.Add(frequency);
            config.Customs.Add(voltage);
            context.PowerMeterDeviceCustomConfigs.Add(frequency);
            context.PowerMeterDeviceCustomConfigs.Add(voltage);
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Infrastructure.Storage;
using paskalON.Domains;
using paskalON.Infrastructure.Repositories;

namespace paskalON.DeviceSimulator.Application.Repositories
{
    /// <summary>
    /// Repository of the simulator custom configurations of the devices.
    /// </summary>
    public class SimulationCustomRepository : RepositoryBase<DeviceServiceContext>, ISimulationCustomRepository
    {
        /// <summary>
        /// Constructor of <see cref="SimulationCustomRepository"/>.
        /// </summary>
        /// <param name="logger">The logger interface for application logging and diagnostics.</param>
        /// <param name="context">The device service database context.</param>
        public SimulationCustomRepository(ILogger<SimulationCustomRepository> logger, DeviceServiceContext context) : base(logger, context)
        {
        }


        /// <inheritdoc/>
        public async Task<SimulationCustomConfigs> GetCustomConfigsAsync()
        {
            List<PowerConversionSystemConfig> pcsConfigs = await Context.PowerConversionSystemConfigs
                .AsNoTracking()
                .Where(config => config.IsActive)
                .Include(config => config.PowerConversionSystemDeviceConfig)
                .ThenInclude(device => device.Customs)
                .ToListAsync();

            List<BatteryBankConfig> bbConfigs = await Context.BatteryBankConfigs
                .AsNoTracking()
                .Where(config => config.IsActive)
                .Include(config => config.BatteryBankDeviceConfig)
                .ThenInclude(device => device.Customs)
                .ToListAsync();

            List<SystemPowerMeterConfig> meterConfigs = await Context.SystemPowerMeterConfigs
                .AsNoTracking()
                .Where(config => config.IsActive)
                .Include(config => config.PowerMeterDeviceConfig)
                .ThenInclude(device => device.Customs)
                .ToListAsync();

            return new SimulationCustomConfigs
            {
                PowerConversionSystems = ToDictionary(pcsConfigs.Select(config => (config.DeviceId, config.PowerConversionSystemDeviceConfig.Customs))),
                BatteryBanks = ToDictionary(bbConfigs.Select(config => (config.DeviceId, config.BatteryBankDeviceConfig.Customs))),
                SystemPowerMeters = ToDictionary(meterConfigs.Select(config => (config.DeviceId, config.PowerMeterDeviceConfig.Customs)))
            };
        }


        /// <summary>
        /// Converts the custom configurations per device to key/value dictionaries.
        /// </summary>
        private IReadOnlyDictionary<int, IReadOnlyDictionary<string, string>> ToDictionary<T>(IEnumerable<(int DeviceId, ICollection<T> Customs)> devices)
            where T : ConfigurationBase
        {
            Dictionary<int, IReadOnlyDictionary<string, string>> result = new Dictionary<int, IReadOnlyDictionary<string, string>>();

            foreach ((int deviceId, ICollection<T> customs) in devices)
            {
                result[deviceId] = customs.ToDictionary(custom => custom.Key, custom => custom.Value);
            }

            return result;
        }
    }
}

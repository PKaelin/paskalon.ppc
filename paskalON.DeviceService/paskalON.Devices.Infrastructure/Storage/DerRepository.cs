// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Ders;
using paskalON.Devices.Infrastructure.Storage.Repositories;

namespace paskalON.Devices.Infrastructure.Storage
{
    /// <summary>
    /// Distributed Energy Resources (DER) repository.
    /// </summary>
    public class DerRepository : RepositoryBase<DeviceServiceContext>, IDerRepository
    {
        /// <summary>
        /// Constructor of <see cref="DerRepository"/>,
        /// </summary>
        /// <param name="logger">The logger interface for application logging and diagnostics.</param>
        /// <param name="context">The device service database context.</param>
        public DerRepository(ILogger<DerRepository> logger, DeviceServiceContext context) : base(logger, context)
        {
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task<DerConfig> GetDer(bool isActive = true)
        {
            List<DerConfig> ders = await Context.DerConfigs
                .Include(d => d.DerGroupConfigs)
                .ThenInclude(c => c.DerCircuits)
                .ThenInclude(u => u.DerUnitConfigs)
                .ToListAsync();

            if (ders.Count != 1)
            {
                throw new InvalidOperationException("There should be one and only DER configuration");
            }

            // Load communications 
            await Context.ModbusConfigs.Include(mc => mc.ModbusConnectionConfig).ToListAsync();
            await Context.C37Configs.ToListAsync();
            // Load DER devices
            await Context.PowerConversionSystemConfigs.Where(a => a.IsActive == isActive).Include(pcs => pcs.PowerConversionSystemDeviceConfig).ToListAsync();
            await Context.BatteryBankConfigs.Where(a => a.IsActive == isActive).Include(bb => bb.BatteryBankDeviceConfig).ToListAsync();
            await Context.SolarPanelConfigs.Where(a => a.IsActive == isActive).Include(sp => sp.SolarPanelDeviceConfig).ToListAsync();
            // Load meters and maps
            await Context.PowerMeterMapC37Configs.ToListAsync();
            await Context.PowerMeterMapModbusConfigs.ToListAsync();
            await Context.PowerMeterDeviceConfigs.ToListAsync();
            await Context.SystemPowerMeterConfigs.Where(a => a.IsActive == isActive).ToListAsync();
            await Context.CircuitPowerMeterConfigs.Where(a => a.IsActive == isActive).ToListAsync();
            await Context.AuxiliaryPowerMeterConfigs.Where(a => a.IsActive == isActive).ToListAsync();
            await Context.ExternalPowerMeterConfigs.Where(a => a.IsActive == isActive).ToListAsync();
            // Load GDM devices
            await Context.GenericModbusConfigs.Where(a => a.IsActive == isActive)
                .Include(meter => meter.GenericModbusDeviceConfig)
                .ThenInclude(map => map.GenericModbusMapConfig).ToListAsync();
            await Context.CircuitBreakerConfigs.Where(a => a.IsActive == isActive)
                .Include(meter => meter.CircuitBreakerDeviceConfig)
                .ThenInclude(map => map.GenericModbusMapConfig).ToListAsync();
            await Context.AutomaticTransferSwitchConfigs.Where(a => a.IsActive == isActive)
                .Include(meter => meter.AutomaticTransferSwitchDeviceConfig)
                .ThenInclude(map => map.GenericModbusMapConfig).ToListAsync();

            return ders.First();
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain.Configs;
using paskalON.Infrastructure.Repositories;
using paskalON.PowerControls.Domain.Configs.Ders;
using paskalON.PowerControls.Domain.Configs.Systems;

namespace paskalON.PowerControls.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// Power control repository for getting power control and constraint configurations.
    /// </summary>
    public class PowerControlRepository : RepositoryBase<PowerControlContext>, IPowerControlRepository
    {
        /// <summary>
        /// Constructor of <see cref="PowerControlRepository"/>.
        /// </summary>
        /// <param name="logger">The logger interface for application logging and diagnostics.</param>
        /// <param name="context">The power control database context.</param>
        public PowerControlRepository(ILogger<PowerControlRepository> logger, PowerControlContext context) : base(logger, context)
        {
        }


        /// <inheritdoc/>
        public async Task<List<SystemPowerControlConfig>> GetSystemPowerControlConfigs()
        {
            return await Context.SystemPowerControlConfigs.Where(s => s.IsActive)
                .AsNoTracking().Include(c => c.Constraints).ToListAsync();
        }


        /// <inheritdoc/>
        public async Task<List<DerUnitPowerControlConfig>> GetDerUnitPowerControlConfigs()
        {
            return await Context.DerUnitPowerControlConfigs.Where(s => s.IsActive)
                .AsNoTracking().Include(c => c.Constraints).ToListAsync();
        }


        /// <inheritdoc/>
        public async Task<List<DerUnitEnergyStoragePowerControlConfig>> GetDerUnitEnergyStoragePowerControlConfigs()
        {
            return await Context.DerUnitEnergyStoragePowerControlConfigs.Where(s => s.IsActive)
                .AsNoTracking().Include(c => c.Constraints).ToListAsync();
        }


        /// <inheritdoc/>
        public async Task<List<ConstraintBaseConfig>> GetConstraintConfigs()
        {
            List<SystemPowerControlConfig> systemConfigs = await Context.SystemPowerControlConfigs.Where(s => s.IsActive)
                .AsNoTracking().Include(c => c.Constraints).ToListAsync();
            List<DerUnitPowerControlConfig> unitConfigs = await Context.DerUnitPowerControlConfigs.Where(s => s.IsActive)
                .AsNoTracking().Include(c => c.Constraints).ToListAsync();
            List<DerUnitEnergyStoragePowerControlConfig> storageConfigs = await Context.DerUnitEnergyStoragePowerControlConfigs.Where(s => s.IsActive)
                .AsNoTracking().Include(c => c.Constraints).ToListAsync();

            return systemConfigs.SelectMany(c => c.Constraints)
                .Concat(unitConfigs.SelectMany(c => c.Constraints))
                .Concat(storageConfigs.SelectMany(c => c.Constraints))
                .DistinctBy(c => c.Name)
                .ToList();
        }
    }
}

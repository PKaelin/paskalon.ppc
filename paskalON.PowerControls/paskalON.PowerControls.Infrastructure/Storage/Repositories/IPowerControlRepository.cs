// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.ConstraintEngine.Domain.Configs;
using paskalON.PowerControls.Domain.Configs.Ders;
using paskalON.PowerControls.Domain.Configs.Systems;

namespace paskalON.PowerControls.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// Power control repository interface definition
    /// </summary>
    public interface IPowerControlRepository
    {
        /// <summary>
        /// Gets the list of active system power control configurations from the database, including their associated constraints.
        /// </summary>
        /// <returns>A list of active <see cref="SystemPowerControlConfig"/> objects.</returns>
        Task<List<SystemPowerControlConfig>> GetSystemPowerControlConfigs();


        /// <summary>
        /// Gets the list of active DER unit power control configurations from the database, including their associated constraints.
        /// </summary>
        /// <returns>A list of active <see cref="DerUnitPowerControlConfig"/> objects.</returns>
        Task<List<DerUnitPowerControlConfig>> GetDerUnitPowerControlConfigs();


        /// <summary>
        /// Gets the list of active DER unit energy storage power control configurations from the database, including their associated constraints.
        /// </summary>
        /// <returns>A list of active <see cref="DerUnitEnergyStoragePowerControlConfig"/> objects.</returns>   
        Task<List<DerUnitEnergyStoragePowerControlConfig>> GetDerUnitEnergyStoragePowerControlConfigs();


        /// <summary>
        /// Gets the list of active constraint configurations from the database, including their associated constraints.
        /// </summary>
        /// <returns>A list of active <see cref="ConstraintBaseConfig"/> objects.</returns>
        Task<List<ConstraintBaseConfig>> GetConstraintConfigs();
    }
}

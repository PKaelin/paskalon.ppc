// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Configs;

namespace paskalON.OperatingModes.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// Operating mode repository interface definition.
    /// </summary>
    public interface IOperatingModeRepository
    {
        Task<List<OperatingModeBaseConfig>> GetAllOperatingModes(bool isActive = true);
    }
}

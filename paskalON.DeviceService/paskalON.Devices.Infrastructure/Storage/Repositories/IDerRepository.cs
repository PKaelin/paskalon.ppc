// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Infrastructure.Storage.Repositories
{
    /// <summary>
    /// Distributed Energy Resources (DER) repository interface definition.
    /// </summary>
    public interface IDerRepository
    {
        /// <summary>
        /// Gets the Distributed Energy Resources (DER) root object with all it's configurations.
        /// </summary>
        /// <param name="isActive">Return the ones that are either active when true or inactive when false.</param>
        /// <returns></returns>
        Task<DerConfig> GetDer(bool isActive = true);
    }
}

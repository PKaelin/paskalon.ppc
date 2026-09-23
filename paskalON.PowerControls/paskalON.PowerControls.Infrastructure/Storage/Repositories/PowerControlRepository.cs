// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;

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
    }
}

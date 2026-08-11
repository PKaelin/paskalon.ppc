// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain.Configs;

namespace paskalON.ConstraintEngine.Domain
{
    /// <summary>
    /// Base class for all power constraints.
    /// </summary>
    public abstract class PowerConstraintBase : ConstraintBase
    {
        /// <summary>
        /// Constructor of <see cref="PowerConstraintBase"/>.
        /// </summary>
        /// <param name="logger">ILogger for handling application logging and diagnostics.</param>
        /// <param name="config">Constraint base configuration.</param>
        /// <param name="map">Constraint base map.</param>
        public PowerConstraintBase(ILogger logger, ConstraintBaseConfig config, PowerConstraintBaseMap map)
            : base(logger, config, map)
        {
        }
    }
}

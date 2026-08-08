// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain.Configs;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.ConstraintEngine.Domain
{
    /// <summary>
    /// Base class for all constraints.
    /// </summary>
    public abstract class ConstraintBase : IConstraint
    {
        /// <summary>
        /// ILogger for handling application logging and diagnostics.
        /// </summary>
        protected readonly ILogger _logger;
        private readonly ConstraintBaseConfig _config;
        private readonly ConstraintBaseMap _map;


        public string Name { get => _config.Name; }

        public bool IsEnabled { get => _config.IsEnabled; }


        public ConstraintBase(ILogger logger, ConstraintBaseConfig config, ConstraintBaseMap map)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

            _logger = logger;
            _config = config;
            _map = map;
        }

        public abstract void ApplyLimits(ref ActivePower activePower, ref ReactivePower reactivePower);
    }
}

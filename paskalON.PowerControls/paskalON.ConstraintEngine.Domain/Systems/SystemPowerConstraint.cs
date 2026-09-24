// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain.Configs.Systems;

namespace paskalON.ConstraintEngine.Domain.Systems
{
    /// <summary>
    /// System power constraint that constraints the active and reactive power to configured limits.
    /// </summary>
    public class SystemPowerConstraint : PowerConstraintBase, ISystemConstraint
    {
        /// <summary>
        /// System power constraint configuration.
        /// </summary>
        private readonly SystemPowerConstraintConfig _config;


        /// <summary>
        /// System power constraint map.
        /// </summary>
        private readonly SystemPowerConstraintMap _map;


        /// <summary>
        /// Indicates whether the system should derate per unit stopped.
        /// </summary>
        public bool DeratePerUnitStopped { get => _config.DeratePerUnitStopped; }


        /// <summary>
        /// Indicates whether the system should derate per unit in maintenance.
        /// </summary>
        public bool DeratePerUnitInMaintenance { get => _config.DeratePerUnitInMaintenance; }


        /// <summary>
        /// Constructor of <see cref="SystemPowerConstraint"/>.
        /// </summary>
        /// <param name="logger">ILogger for handling application logging and diagnostics.</param>
        /// <param name="config">System power constraint configuration.</param>
        /// <param name="map">System power constraint map.</param>
        public SystemPowerConstraint(ILogger logger, SystemPowerConstraintConfig config, SystemPowerConstraintMap map)
            : base(logger, config, map)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

            _config = config;
            _map = map;
        }
    }
}

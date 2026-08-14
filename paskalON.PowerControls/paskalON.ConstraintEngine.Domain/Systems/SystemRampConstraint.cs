// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain.Configs.Systems;

namespace paskalON.ConstraintEngine.Domain.Systems
{
    public class SystemRampConstraint : PowerRampConstraintBase, ISystemConstraint
    {
        /// <summary>
        /// System ramp constraint configuration.
        /// </summary>
        private readonly SystemRampConstraintConfig _config;


        /// <summary>
        /// System ramp constraint map.
        /// </summary>
        private readonly SystemRampConstraintMap _map;


        /// <summary>
        /// Constructor of <see cref="SystemRampConstraint"/>.
        /// </summary>
        /// <param name="logger">ILogger for handling application logging and diagnostics.</param>
        /// <param name="config">System ramp constraint configuration.</param>
        /// <param name="map">System ramp constraint map.</param>
        /// <param name="timeProvider">Time provider for system time abstraction.</param>
        public SystemRampConstraint(ILogger logger, SystemRampConstraintConfig config, SystemRampConstraintMap map, TimeProvider timeProvider)
            : base(logger, config, map, timeProvider)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

            _config = config;
            _map = map;
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain.Configs.Ders;

namespace paskalON.ConstraintEngine.Domain.Ders
{
    /// <summary>
    /// DER unit ramp constraint that constraints the active and reactive power to configured limits.
    /// </summary>
    public class DerUnitRampConstraint : PowerRampConstraintBase, IDerUnitConstraint
    {
        /// <summary>
        /// System ramp constraint configuration.
        /// </summary>
        private readonly DerUnitRampConstraintConfig _config;


        /// <summary>
        /// System ramp constraint map.
        /// </summary>
        private readonly DerUnitRampConstraintMap _map;


        /// <summary>
        /// Constructor of <see cref="DerUnitRampConstraint"/>.
        /// </summary>
        /// <param name="logger">ILogger for handling application logging and diagnostics.</param>
        /// <param name="config">System ramp constraint configuration.</param>
        /// <param name="map">System ramp constraint map.</param>
        /// <param name="timeProvider">Time provider for system time abstraction.</param>
        public DerUnitRampConstraint(ILogger logger, DerUnitRampConstraintConfig config, DerUnitRampConstraintMap map, TimeProvider timeProvider)
            : base(logger, config, map, timeProvider)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

            _config = config;
            _map = map;
        }
    }
}

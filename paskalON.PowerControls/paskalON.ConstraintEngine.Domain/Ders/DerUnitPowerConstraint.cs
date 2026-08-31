// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain.Configs.Ders;

namespace paskalON.ConstraintEngine.Domain.Ders
{
    /// <summary>
    /// DER unit power constraint that constraints the active and reactive power to configured limits.
    /// </summary>
    public class DerUnitPowerConstraint : PowerConstraintBase, IDerUnitConstraint
    {
        /// <summary>
        /// DER unit power constraint configuration.
        /// </summary>
        private readonly DerUnitPowerConstraintConfig _config;


        /// <summary>
        /// DER unit power constraint map.
        /// </summary>
        private readonly DerUnitPowerConstraintMap _map;


        /// <summary>
        /// Constructor of <see cref="DerUnitPowerConstraint"/>.
        /// </summary>
        /// <param name="logger">ILogger for handling application logging and diagnostics.</param>
        /// <param name="config">DER unit power constraint configuration.</param>
        /// <param name="map">DER unit power constraint map.</param>
        public DerUnitPowerConstraint(ILogger logger, DerUnitPowerConstraintConfig config, DerUnitPowerConstraintMap map)
            : base(logger, config, map)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

            _config = config;
            _map = map;
        }
    }
}

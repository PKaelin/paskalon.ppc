// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain.Configs;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.ConstraintEngine.Domain
{
    /// <summary>
    /// Base class for all constraints.
    /// </summary>
    /// <remarks>
    /// The constraint is implemented once and applied to many power controllers.
    /// </remarks>
    public abstract class ConstraintBase : IConstraint
    {
        /// <summary>
        /// ILogger for handling application logging and diagnostics.
        /// </summary>
        protected readonly ILogger _logger;


        /// <summary>
        /// Constraint base configuration.
        /// </summary>
        private readonly ConstraintBaseConfig _config;


        /// <summary>
        /// Constraint base map.
        /// </summary>
        /// <remarks>
        /// Some constraint require inputs like power, voltage, current, etc.
        /// This is the base mapping class for those inputs.
        /// </remarks>
        private readonly ConstraintBaseMap _map;


        /// <summary>
        /// Lock object to protect the active violations.
        /// </summary>
        private readonly object _dataLock = new object();


        /// <summary>
        /// Names of the violations that are currently active.
        /// </summary>
        private readonly HashSet<string> _activeViolations = new HashSet<string>(StringComparer.Ordinal);


        /// <inheritdoc/>
        public string Name { get => _config.Name; }


        /// <summary>
        /// Constructor of <see cref="ConstraintBase"/>.
        /// </summary>
        /// <param name="logger">ILogger for handling application logging and diagnostics.</param>
        /// <param name="config">Constraint base configuration.</param>
        /// <param name="map">Constraint base map.</param>
        public ConstraintBase(ILogger logger, ConstraintBaseConfig config, ConstraintBaseMap map)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

            _logger = logger;
            _config = config;
            _map = map;
        }


        /// <inheritdoc/>
        public abstract void ApplyConstraints(ref ActivePower activePower, ref ReactivePower reactivePower, bool shallLogViolations);


        /// <summary>
        /// Updates whether a violation is active, so that a violation is logged once when it starts and once when it ends
        /// instead of on every applied setpoint.
        /// </summary>
        /// <param name="violation">Name of the violation, unique within the constraint.</param>
        /// <param name="isViolated">Whether the violation is active for the current setpoint.</param>
        /// <returns>True if the violation started or ended with this call.</returns>
        protected bool UpdateViolation(string violation, bool isViolated)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(violation);

            lock (_dataLock)
            {
                if (isViolated)
                {
                    return _activeViolations.Add(violation);
                }

                return _activeViolations.Remove(violation);
            }
        }
    }
}

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
    /// Base class for all power constraints.
    /// </summary>
    public abstract class PowerConstraintBase : ConstraintBase
    {
        /// <summary>
        /// Power constraint base configuration.
        /// </summary>
        private readonly PowerConstraintConfig _config;


        /// <summary>
        /// Constructor of <see cref="PowerConstraintBase"/>.
        /// </summary>
        /// <param name="logger">ILogger for handling application logging and diagnostics.</param>
        /// <param name="config">Power constraint base configuration.</param>
        /// <param name="map">Power constraint base map.</param>
        public PowerConstraintBase(ILogger logger, PowerConstraintConfig config, PowerConstraintBaseMap map)
            : base(logger, config, map)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void ApplyConstraints(ref ActivePower activePower, ref ReactivePower reactivePower, bool shallLogViolations = true)
        {
            if (_config.MaximumActivePowerWatt.HasValue && activePower.Watts > _config.MaximumActivePowerWatt)
            {
                if (shallLogViolations == true)
                {
                    _logger.LogWarning("{Name} active power {ActivePower} exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, activePower.Watts, _config.MaximumActivePowerWatt);
                }
                activePower.Watts = _config.MaximumActivePowerWatt.Value;
            }
            else if (_config.MinimumActivePowerWatt.HasValue && activePower.Watts < _config.MinimumActivePowerWatt)
            {
                if (shallLogViolations == true)
                {
                    _logger.LogWarning("{Name} active power {ActivePower} below minimum limit {MinLimit}. Clamping to minimum.", Name, activePower.Watts, _config.MinimumActivePowerWatt);
                }
                activePower.Watts = _config.MinimumActivePowerWatt.Value;
            }
            if (_config.MaximumReactivePowerVars.HasValue && reactivePower.VoltAmperesReactive > _config.MaximumReactivePowerVars)
            {
                if (shallLogViolations == true)
                {
                    _logger.LogWarning("{Name} reactive power {ReactivePower} exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, reactivePower.VoltAmperesReactive, _config.MaximumReactivePowerVars);
                }
                reactivePower.VoltAmperesReactive = _config.MaximumReactivePowerVars.Value;
            }
            else if (_config.MinimumReactivePowerVars.HasValue && reactivePower.VoltAmperesReactive < _config.MinimumReactivePowerVars)
            {
                if (shallLogViolations == true)
                {
                    _logger.LogWarning("{Name} reactive power {ReactivePower} below minimum limit {MinLimit}. Clamping to minimum.", Name, reactivePower.VoltAmperesReactive, _config.MinimumReactivePowerVars);
                }
                reactivePower.VoltAmperesReactive = _config.MinimumReactivePowerVars.Value;
            }
        }
    }
}

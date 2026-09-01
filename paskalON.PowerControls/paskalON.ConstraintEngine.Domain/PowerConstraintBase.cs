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
            if (_config.MaximumActivePowerKiloWatt.HasValue && activePower.KiloWatts > _config.MaximumActivePowerKiloWatt)
            {
                if (shallLogViolations == true)
                {
                    _logger.LogWarning("{Name} active power {ActivePower} exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, activePower.KiloWatts, _config.MaximumActivePowerKiloWatt);
                }
                activePower.KiloWatts = _config.MaximumActivePowerKiloWatt.Value;
            }
            else if (_config.MinimumActivePowerKiloWatt.HasValue && activePower.KiloWatts < _config.MinimumActivePowerKiloWatt)
            {
                if (shallLogViolations == true)
                {
                    _logger.LogWarning("{Name} active power {ActivePower} below minimum limit {MinLimit}. Clamping to minimum.", Name, activePower.KiloWatts, _config.MinimumActivePowerKiloWatt);
                }
                activePower.KiloWatts = _config.MinimumActivePowerKiloWatt.Value;
            }
            if (_config.MaximumReactivePowerKiloVars.HasValue && reactivePower.KiloVoltAmperesReactive > _config.MaximumReactivePowerKiloVars)
            {
                if (shallLogViolations == true)
                {
                    _logger.LogWarning("{Name} reactive power {ReactivePower} exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, reactivePower.KiloVoltAmperesReactive, _config.MaximumReactivePowerKiloVars);
                }
                reactivePower.KiloVoltAmperesReactive = _config.MaximumReactivePowerKiloVars.Value;
            }
            else if (_config.MinimumReactivePowerKiloVars.HasValue && reactivePower.KiloVoltAmperesReactive < _config.MinimumReactivePowerKiloVars)
            {
                if (shallLogViolations == true)
                {
                    _logger.LogWarning("{Name} reactive power {ReactivePower} below minimum limit {MinLimit}. Clamping to minimum.", Name, reactivePower.KiloVoltAmperesReactive, _config.MinimumReactivePowerKiloVars);
                }
                reactivePower.KiloVoltAmperesReactive = _config.MinimumReactivePowerKiloVars.Value;
            }
        }
    }
}

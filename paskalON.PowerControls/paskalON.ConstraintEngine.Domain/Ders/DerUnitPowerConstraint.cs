// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain.Configs.Ders;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.ConstraintEngine.Domain.Ders
{
    public class DerUnitPowerConstraint : PowerConstraintBase
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


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void ApplyLimits(ref ActivePower activePower, ref ReactivePower reactivePower)
        {
            if (_config.IsEnabled)
            {
                if (_config.MaximumActivePowerKiloWatt.HasValue && activePower.KiloWatts > _config.MaximumActivePowerKiloWatt)
                {
                    _logger.LogWarning("{Name} active power {ActivePower} exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, activePower.KiloWatts, _config.MaximumActivePowerKiloWatt);
                    activePower.KiloWatts = _config.MaximumActivePowerKiloWatt.Value;
                }
                else if (_config.MinimumActivePowerKiloWatt.HasValue && activePower.KiloWatts < _config.MinimumActivePowerKiloWatt)
                {
                    _logger.LogWarning("{Name} active power {ActivePower} below minimum limit {MinLimit}. Clamping to minimum.", Name, activePower.KiloWatts, _config.MinimumActivePowerKiloWatt);
                    activePower.KiloWatts = _config.MinimumActivePowerKiloWatt.Value;
                }
                if (_config.MaximumReactivePowerKiloVars.HasValue && reactivePower.KiloVoltAmperesReactive > _config.MaximumReactivePowerKiloVars)
                {
                    _logger.LogWarning("{Name} reactive power {ReactivePower} exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, reactivePower.KiloVoltAmperesReactive, _config.MaximumReactivePowerKiloVars);
                    reactivePower.KiloVoltAmperesReactive = _config.MaximumReactivePowerKiloVars.Value;
                }
                else if (_config.MinimumReactivePowerKiloVars.HasValue && reactivePower.KiloVoltAmperesReactive < _config.MinimumReactivePowerKiloVars)
                {
                    _logger.LogWarning("{Name} reactive power {ReactivePower} below minimum limit {MinLimit}. Clamping to minimum.", Name, reactivePower.KiloVoltAmperesReactive, _config.MinimumReactivePowerKiloVars);
                    reactivePower.KiloVoltAmperesReactive = _config.MinimumReactivePowerKiloVars.Value;
                }
            }
        }
    }
}

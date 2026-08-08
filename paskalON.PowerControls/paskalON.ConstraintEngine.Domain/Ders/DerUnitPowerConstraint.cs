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
        private readonly DerUnitPowerConstraintConfig _config;
        private readonly DerUnitPowerConstraintMap _map;


        public DerUnitPowerConstraint(ILogger logger, DerUnitPowerConstraintConfig config, DerUnitPowerConstraintMap map)
            : base(logger, config, map)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(map);

            _config = config;
            _map = map;
        }


        public override void ApplyLimits(ref ActivePower activePower, ref ReactivePower reactivePower)
        {
            if (_config.IsEnabled)
            {
                if (_config.MaximumActivePowerKiloWatt.HasValue && activePower.KiloWatts > _config.MaximumActivePowerKiloWatt)
                {
                    _logger.LogWarning("Active power {ActivePower} exceeds maximum limit {MaxLimit}. Clamping to maximum.", activePower.KiloWatts, _config.MaximumActivePowerKiloWatt);
                    activePower.KiloWatts = _config.MaximumActivePowerKiloWatt.Value;
                }
                else if (_config.MinimumActivePowerKiloWatt.HasValue && activePower.KiloWatts < _config.MinimumActivePowerKiloWatt)
                {
                    _logger.LogWarning("Active power {ActivePower} below minimum limit {MinLimit}. Clamping to minimum.", activePower.KiloWatts, _config.MinimumActivePowerKiloWatt);
                    activePower.KiloWatts = _config.MinimumActivePowerKiloWatt.Value;
                }
                if (_config.MaximumReactivePowerKiloVars.HasValue && reactivePower.KiloVoltAmperesReactive > _config.MaximumReactivePowerKiloVars)
                {
                    _logger.LogWarning("Reactive power {ReactivePower} exceeds maximum limit {MaxLimit}. Clamping to maximum.", reactivePower.KiloVoltAmperesReactive, _config.MaximumReactivePowerKiloVars);
                    reactivePower.KiloVoltAmperesReactive = _config.MaximumReactivePowerKiloVars.Value;
                }
                else if (_config.MinimumReactivePowerKiloVars.HasValue && reactivePower.KiloVoltAmperesReactive < _config.MinimumReactivePowerKiloVars)
                {
                    _logger.LogWarning("Reactive power {ReactivePower} below minimum limit {MinLimit}. Clamping to minimum.", reactivePower.KiloVoltAmperesReactive, _config.MinimumReactivePowerKiloVars);
                    reactivePower.KiloVoltAmperesReactive = _config.MinimumReactivePowerKiloVars.Value;
                }
            }
        }
    }
}

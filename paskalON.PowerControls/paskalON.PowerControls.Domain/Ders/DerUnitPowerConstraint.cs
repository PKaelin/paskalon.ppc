// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain.Configs.Ders;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.PowerControls.Domain.Ders
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
            throw new NotImplementedException();
        }
    }
}

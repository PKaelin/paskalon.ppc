// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain.Configs.Systems;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.PowerControls.Domain.Systems
{
    public class SystemRampConstraint : ConstraintBase
    {
        private readonly SystemRampConstraintConfig _config;
        private readonly SystemRampConstraintMap _map;


        public SystemRampConstraint(ILogger logger, SystemRampConstraintConfig config, SystemRampConstraintMap map) : base(logger, config, map)
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

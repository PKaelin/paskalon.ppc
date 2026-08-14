// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Ders;

namespace paskalON.PowerControls.Domain.Strategies
{
    public class ProportionalDistributionStrategy : DistributionStrategyBase, IDistributionStrategy
    {
        public ProportionalDistributionStrategy(ILogger logger) : base(logger)
        {
        }

        public void Distribute(ActivePower systemActivePower, ReactivePower systemReactivePower, IEnumerable<DerUnitPowerControl> alUnits)
        {
            throw new NotImplementedException();
        }
    }
}

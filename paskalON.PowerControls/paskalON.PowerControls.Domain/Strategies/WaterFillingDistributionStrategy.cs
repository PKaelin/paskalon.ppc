// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Ders;

namespace paskalON.PowerControls.Domain.Strategies
{
    public class WaterFillingDistributionStrategy : DistributionStrategyBase, IDistributionStrategy
    {
        public WaterFillingDistributionStrategy(ILogger logger) : base(logger)
        {
        }

        public void Distribute(ActivePower systemActivePower, ReactivePower systemReactivePower, IEnumerable<DerUnitPowerControl> allUnits)
        {
            throw new NotImplementedException();
        }
    }
}

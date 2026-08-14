// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Ders;

namespace paskalON.PowerControls.Domain.Strategies
{
    /// <summary>
    /// Distributes regarding a configured weight and applies constraints.
    /// </summary>
    public class WeightDistributionStrategy : DistributionStrategyBase, IDistributionStrategy
    {
        /// <summary>
        /// Constructor of <see cref="WeightDistributionStrategy"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        public WeightDistributionStrategy(ILogger logger) : base(logger)
        {
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>        
        public void Distribute(ActivePower systemActivePower, ReactivePower systemReactivePower, IEnumerable<DerUnitPowerControl> alUnits)
        {
            throw new NotImplementedException();
        }
    }
}

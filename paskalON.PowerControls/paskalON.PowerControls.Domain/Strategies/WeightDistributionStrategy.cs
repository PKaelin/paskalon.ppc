// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain;
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
        public void Distribute(ActivePower systemActivePower, ReactivePower systemReactivePower, IEnumerable<DerUnitPowerControl> allUnits)
        {
            IEnumerable<DerUnitPowerControl> units = allUnits.Where(u => u.IsEnabled && u.State == DerState.Started);
            double totalWeight = units.Sum(u => u.Weight);

            if (totalWeight > 0)
            {
                foreach (DerUnitPowerControl unit in units)
                {
                    double unitTargetActivePower = systemActivePower.Watts / totalWeight * unit.Weight;
                    double unitTargetReactivePower = systemReactivePower.VoltAmperesReactive / totalWeight * unit.Weight;
                    unit.TargetActivePower.Watts = unitTargetActivePower;
                    unit.TargetReactivePower.VoltAmperesReactive = unitTargetReactivePower;

                    foreach (IConstraint constraint in unit.Constraints)
                    {
                        constraint.ApplyConstraints(ref unit.TargetActivePower, ref unit.TargetReactivePower, false);
                    }
                }
            }

            _logger.LogDebug("Active power requested: {SystemActivePower}. Active power achieved: {UnitsActivePower}",
                systemActivePower.KiloWatts, units.Sum(t => t.TargetActivePower.KiloWatts));
            _logger.LogDebug("Reactive power requested: {SystemReactivePower}. Reactive power achieved: {UnitsReactivePower}",
                systemReactivePower.KiloVoltAmperesReactive, units.Sum(t => t.TargetReactivePower.KiloVoltAmperesReactive));
        }
    }
}

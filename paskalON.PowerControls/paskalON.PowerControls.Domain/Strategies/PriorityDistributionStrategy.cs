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
    /// Distributes regarding a configured priority and applies constraints.
    /// </summary>
    public class PriorityDistributionStrategy : DistributionStrategyBase, IDistributionStrategy
    {
        /// <summary>
        /// Constructor of <see cref="PriorityDistributionStrategy"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        public PriorityDistributionStrategy(ILogger logger) : base(logger)
        {
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>        
        public void Distribute(ActivePower systemActivePower, ReactivePower systemReactivePower, IEnumerable<DerUnitPowerControl> allUnits)
        {
            IEnumerable<DerUnitPowerControl> units = allUnits.Where(u => u.IsEnabled && u.State == DerState.Started).OrderBy(o => o.Priority);
            int unitCount = units.Count();

            double unitTargetActivePower = systemActivePower.Watts;
            double unitTargetReactivePower = systemReactivePower.VoltAmperesReactive;

            foreach (DerUnitPowerControl unit in units)
            {
                unit.TargetActivePower.Watts = unitTargetActivePower;
                unit.TargetReactivePower.VoltAmperesReactive = unitTargetReactivePower;

                foreach (IConstraint constraint in unit.Constraints)
                {
                    constraint.ApplyConstraints(ref unit.TargetActivePower, ref unit.TargetReactivePower, false);
                }

                if (Math.Round(unitTargetActivePower, 0) != 0)
                {
                    unitTargetActivePower -= unit.TargetActivePower.Watts;
                }
                else
                {
                    unitTargetActivePower = 0;
                }

                if (Math.Round(unitTargetReactivePower, 0) != 0)
                {
                    unitTargetReactivePower -= unit.TargetReactivePower.VoltAmperesReactive;
                }
                else
                {
                    unitTargetReactivePower = 0;
                }
            }

            _logger.LogDebug("Active power requested: {SystemActivePower}. Active power achieved: {UnitsActivePower}",
                systemActivePower.KiloWatts, units.Sum(t => t.TargetActivePower.KiloWatts));
            _logger.LogDebug("Reactive power requested: {SystemReactivePower}. Reactive power achieved: {UnitsReactivePower}",
                systemReactivePower.KiloVoltAmperesReactive, units.Sum(t => t.TargetReactivePower.KiloVoltAmperesReactive));
        }
    }
}

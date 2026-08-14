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
    /// Distributes equally regardless the units constraints but constraints are still applied.
    /// </summary>
    public class EqualDistributionStrategy : DistributionStrategyBase, IDistributionStrategy
    {
        /// <summary>
        /// Constructor of <see cref="EqualDistributionStrategy"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        public EqualDistributionStrategy(ILogger logger) : base(logger)
        {
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>        
        public void Distribute(ActivePower systemActivePower, ReactivePower systemReactivePower, IEnumerable<DerUnitPowerControl> allUnits)
        {
            IEnumerable<DerUnitPowerControl> units = allUnits.Where(u => u.IsEnabled && u.State == DerState.Started);
            int unitCount = units.Count();

            if (unitCount > 0)
            {
                double unitTargetActivePower = systemActivePower.Watts / unitCount;
                double unitTargetReactivePower = systemReactivePower.VoltAmperesReactive / unitCount;

                foreach (DerUnitPowerControl unit in units)
                {
                    unit.TargetActivePower.Watts = unitTargetActivePower;
                    unit.TargetReactivePower.VoltAmperesReactive = unitTargetReactivePower;
                    unit.UpdatePower(unit.TargetActivePower, unit.TargetReactivePower);

                    foreach (IConstraint constraint in unit.Constraints)
                    {
                        constraint.ApplyConstraints(ref unit.TargetActivePower, ref unit.TargetReactivePower, false);
                    }
                    // If unit.Target != unitTarget then constraints have been applied
                }
            }

            _logger.LogDebug("Active power requested: {SystemActivePower}. Active power achieved: {UnitsActivePower}",
                systemActivePower.KiloWatts, units.Sum(t => t.TargetActivePower.KiloWatts));
            _logger.LogDebug("Reactive power requested: {SystemReactivePower}. Reactive power achieved: {UnitsReactivePower}",
                systemReactivePower.KiloVoltAmperesReactive, units.Sum(t => t.TargetReactivePower.KiloVoltAmperesReactive));
        }
    }
}

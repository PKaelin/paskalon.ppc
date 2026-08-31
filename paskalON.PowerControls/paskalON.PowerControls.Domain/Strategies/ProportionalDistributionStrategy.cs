// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Ders;

namespace paskalON.PowerControls.Domain.Strategies
{
    /// <summary>
    /// Distributes regarding a proportional strategy and applies constraints.
    /// </summary>
    public class ProportionalDistributionStrategy : DistributionStrategyBase, IDistributionStrategy
    {
        /// <summary>
        /// Constructor of <see cref="ProportionalDistributionStrategy"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>

        public ProportionalDistributionStrategy(ILogger logger) : base(logger)
        {
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Distribute(ActivePower systemActivePower, ReactivePower systemReactivePower, IEnumerable<DerUnitPowerControl> allUnits)
        {
            IEnumerable<DerUnitPowerControl> units = allUnits.Where(u => u.IsEnabled && u.State == DerState.Started);

            double totalMaxActive = units.Sum(u => u.MaximumActivePower.Watts);
            double totalMinActive = units.Sum(u => u.MinimumActivePower.Watts);
            double totalMaxReactive = units.Sum(u => u.MaximumReactivePower.VoltAmperesReactive);
            double totalMinReactive = units.Sum(u => u.MinimumReactivePower.VoltAmperesReactive);

            foreach (DerUnitPowerControl unit in units)
            {
                unit.TargetActivePower.Watts = Calculate(systemActivePower.Watts, unit.MinimumActivePower.Watts, unit.MaximumActivePower.Watts,
                    totalMinActive, totalMaxActive);

                unit.TargetReactivePower.VoltAmperesReactive = Calculate(systemReactivePower.VoltAmperesReactive, unit.MinimumReactivePower.VoltAmperesReactive,
                    unit.MaximumReactivePower.VoltAmperesReactive, totalMinReactive, totalMaxReactive);

                foreach (IConstraint constraint in unit.Constraints)
                {
                    constraint.ApplyConstraints(ref unit.TargetActivePower, ref unit.TargetReactivePower, false);
                }
            }

            _logger.LogDebug("Active power requested: {SystemActivePower}. Active power achieved: {UnitsActivePower}",
                systemActivePower.KiloWatts, units.Sum(t => t.TargetActivePower.KiloWatts));
            _logger.LogDebug("Reactive power requested: {SystemReactivePower}. Reactive power achieved: {UnitsReactivePower}",
                systemReactivePower.KiloVoltAmperesReactive, units.Sum(t => t.TargetReactivePower.KiloVoltAmperesReactive));
        }


        /// <summary>
        /// Calculate the distributed value.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="min">The units minimum.</param>
        /// <param name="max">The units maximum.</param>
        /// <param name="totalMin">The units total minimum.</param>
        /// <param name="totalMax">The units total maximum.</param>
        /// <returns></returns>
        private double Calculate(double target, double min, double max, double totalMin, double totalMax)
        {
            if (target < 0)
            {
                return totalMin == 0 ? 0 : min / totalMin * target;
            }

            return totalMax == 0 ? 0 : max / totalMax * target;
        }
    }
}


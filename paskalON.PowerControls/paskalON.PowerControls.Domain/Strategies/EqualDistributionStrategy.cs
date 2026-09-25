// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
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
        public void Distribute(ActivePower systemActivePower, ReactivePower systemReactivePower, IEnumerable<IDerUnitPowerControl> allUnits)
        {
            IEnumerable<IDerUnitPowerControl> units = allUnits.Where(u => u.IsEnabled && u.State == DerState.Started);
            int unitCount = units.Count();

            if (unitCount > 0)
            {
                double unitTargetActivePower = systemActivePower.Watts / unitCount;
                double unitTargetReactivePower = systemReactivePower.VoltAmperesReactive / unitCount;

                foreach (IDerUnitPowerControl unit in units)
                {
                    // Create local for thread safety
                    ActivePower targetActivePower = new ActivePower(unitTargetActivePower);
                    ReactivePower targetReactivePower = new ReactivePower(unitTargetReactivePower);
                    // Updates the unit's power settings and considers possible contraints and limits
                    unit.UpdatePower(targetActivePower, targetReactivePower);
                    // If unit.Target != unitTarget then constraints have been applied
                }
            }

            _logger.LogDebug("Active power requested: {SystemActivePower}. Active power achieved: {UnitsActivePower}",
                systemActivePower.Watts, units.Sum(t => t.TargetActivePower.Watts));
            _logger.LogDebug("Reactive power requested: {SystemReactivePower}. Reactive power achieved: {UnitsReactivePower}",
                systemReactivePower.VoltAmperesReactive, units.Sum(t => t.TargetReactivePower.VoltAmperesReactive));
        }
    }
}

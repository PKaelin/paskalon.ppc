// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
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
        public void Distribute(ActivePower systemActivePower, ReactivePower systemReactivePower, IEnumerable<DerUnitPowerControl> alUnits)
        {
            IEnumerable<DerUnitPowerControl> units = alUnits.Where(u => u.IsEnabled && u.State == DerState.Started);
            int unitCount = units.Count();

            if (unitCount > 0)
            {

                double unitTargetActivePower = systemActivePower.Watts;
                double unitTargetReactivePower = systemReactivePower.VoltAmperesReactive;

                foreach (DerUnitPowerControl unit in units)
                {
                    if (Math.Round(unitTargetActivePower, 0) != 0)
                    {
                        unit.SetActivePowerTarget(unitTargetActivePower);
                    }
                    else
                    {
                        unit.SetActivePowerTarget(0);
                    }

                    if (Math.Round(unitTargetReactivePower, 0) != 0)
                    {
                        unit.SetReactivePowerTarget(unitTargetReactivePower);
                    }
                    else
                    {
                        unit.SetReactivePowerTarget(0);
                    }

                    unit.UpdatePower(unit.TargetActivePower, unit.TargetReactivePower);
                    unitTargetActivePower -= unit.TargetActivePower.Watts;
                    unitTargetReactivePower -= unit.TargetReactivePower.VoltAmperesReactive;
                }
            }

            _logger.LogDebug("Active power requested: {SystemActivePower}. Active power achieved: {UnitsActivePower}",
                systemActivePower.KiloWatts, units.Sum(t => t.TargetActivePower.KiloWatts));
            _logger.LogDebug("Reactive power requested: {SystemReactivePower}. Reactive power achieved: {UnitsReactivePower}",
                systemReactivePower.KiloVoltAmperesReactive, units.Sum(t => t.TargetReactivePower.KiloVoltAmperesReactive));
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Ders;

namespace paskalON.PowerControls.Domain.Strategies
{
    /// <summary>
    /// Distribution strategy interface.
    /// </summary>
    public interface IDistributionStrategy
    {
        /// <summary>
        /// Distributes according to its algorithm.
        /// </summary>
        /// <param name="systemActivePower">The systems constraint active power target.</param>
        /// <param name="systemReactivePower">The systems constraint reactive power target.</param>
        /// <param name="alUnits">All DER unit power controls.</param>
        void Distribute(ActivePower systemActivePower, ReactivePower systemReactivePower, IEnumerable<DerUnitPowerControl> alUnits);
    }
}

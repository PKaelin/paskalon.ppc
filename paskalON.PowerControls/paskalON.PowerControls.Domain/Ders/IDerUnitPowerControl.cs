// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.ConstraintEngine.Domain;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.PowerControls.Domain.Configs.Strategies;

namespace paskalON.PowerControls.Domain.Ders
{
    /// <summary>
    /// Common contract for all DER unit power controls.
    /// </summary>
    public interface IDerUnitPowerControl : IPowerControl
    {
        /// <summary>
        /// Current DER state.
        /// </summary>
        DerState State { get; }


        /// <summary>
        /// DER unit name.
        /// </summary>
        string DerUnitName { get; }


        /// <summary>
        /// Device identifier of the power conversion system (PCS) that controls this DER unit.
        /// </summary>
        int PcsDeviceId { get; }


        /// <summary>
        /// DER unit constraints.
        /// </summary>
        IEnumerable<IDerUnitConstraint> Constraints { get; }


        /// <summary>
        /// Distribution strategy type.
        /// </summary>
        DistributionStrategyType DistributionStrategyType { get; }


        /// <summary>
        /// Distribution priority.
        /// </summary>
        int Priority { get; }


        /// <summary>
        /// Distribution weight.
        /// </summary>
        double Weight { get; }


        /// <summary>
        /// Maximum active power.
        /// </summary>
        ActivePower MaximumActivePower { get; }


        /// <summary>
        /// Minimum active power.
        /// </summary>
        ActivePower MinimumActivePower { get; }


        /// <summary>
        /// Maximum reactive power.
        /// </summary>
        ReactivePower MaximumReactivePower { get; }


        /// <summary>
        /// Minimum reactive power.
        /// </summary>
        ReactivePower MinimumReactivePower { get; }
    }
}

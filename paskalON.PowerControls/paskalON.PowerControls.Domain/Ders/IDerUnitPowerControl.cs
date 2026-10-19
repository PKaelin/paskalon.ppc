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


        /// <summary>
        /// Active power target set while the unit is in maintenance. Only valid if <see cref="IsMaintenanceTargetSet"/> is true.
        /// </summary>
        /// <remarks>
        /// Kept separate from the system targets, so the targets of the system distribution and of the maintenance never mix.
        /// </remarks>
        ActivePower MaintenanceTargetActivePower { get; }


        /// <summary>
        /// Reactive power target set while the unit is in maintenance. Only valid if <see cref="IsMaintenanceTargetSet"/> is true.
        /// </summary>
        /// <remarks>
        /// Kept separate from the system targets, so the targets of the system distribution and of the maintenance never mix.
        /// </remarks>
        ReactivePower MaintenanceTargetReactivePower { get; }


        /// <summary>
        /// True if a maintenance target is set.
        /// </summary>
        /// <remarks>
        /// It is reset as soon as the unit receives a target from the system distribution again.
        /// </remarks>
        bool IsMaintenanceTargetSet { get; }


        /// <summary>
        /// Raised when the maintenance target changed.
        /// </summary>
        event EventHandler? MaintenanceTargetChanged;


        /// <summary>
        /// Sets the maintenance target. The caller is responsible to apply the unit constraints before.
        /// </summary>
        /// <param name="activePower">
        /// The constrained active power target.
        /// </param>
        /// <param name="reactivePower">
        /// The constrained reactive power target.
        /// </param>
        void SetMaintenanceTarget(ActivePower activePower, ReactivePower reactivePower);
    }
}

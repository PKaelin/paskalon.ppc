// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.PowerControls.Domain
{
    /// <summary>
    /// Interface for power controls.
    /// </summary>
    public interface IPowerControl
    {
        /// <summary>
        /// Is active means it is available for selection.
        /// </summary>
        public bool IsActive { get; }


        /// <summary>
        /// Is enabled means the constraint is active and will be applied.
        /// </summary>
        public bool IsEnabled { get; }


        /// <summary>
        /// Actual active power target of the power control after applying constraints and derating.
        /// </summary>
        ActivePower TargetActivePower { get; }


        /// <summary>
        /// Actual reactive power target of the power control after applying constraints and derating.
        /// </summary>
        ReactivePower TargetReactivePower { get; }


        /// <summary>
        /// Sets the active and reactive power targets.
        /// </summary>
        /// <param name="activePower">Active power target.</param>
        /// <param name="reactivePower">Reactive power target.</param>
        void SetTargetPower(ActivePower activePower, ReactivePower reactivePower);


        /// <summary>
        /// Updates the active and reactive power for the power control.
        /// </summary>
        /// <param name="activePower">Active power target for the power control.</param>
        /// <param name="reactivePower">Reactive power target for the power control.</param>
        void UpdatePower(ActivePower activePower, ReactivePower reactivePower);
    }
}

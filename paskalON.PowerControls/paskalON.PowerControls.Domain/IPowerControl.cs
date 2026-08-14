// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
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
        /// Active power target for the power control.
        /// </summary>
        ref ActivePower TargetActivePower { get; }


        /// <summary>
        /// Reactive power target for the power control.
        /// </summary>
        ref ReactivePower TargetReactivePower { get; }


        /// <summary>
        /// Updates the active and reactive power for the power control.
        /// </summary>
        /// <param name="activePower">Active power target for the power control.</param>
        /// <param name="reactivePower">Reactive power target for the power control.</param>
        void UpdatePower(ActivePower activePower, ReactivePower reactivePower);
    }
}

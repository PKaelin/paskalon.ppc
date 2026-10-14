// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Controls;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Domain.Abstractions
{
    /// <summary>
    /// Interface definition for all closed loop operating modes.
    /// </summary>
    public interface IOperatingClosedMode : IOperatingMode
    {
        /// <summary>
        /// Gets the active power feedback control.
        /// </summary>
        /// <remarks>
        /// Null if the operating mode does not correct active power with a feedback signal.
        /// </remarks>
        FeedbackControl<ActivePower>? ActiveFeedback { get; }


        /// <summary>
        /// Gets the reactive power feedback control.
        /// </summary>
        /// <remarks>
        /// Null if the operating mode does not correct reactive power with a feedback signal.
        /// </remarks>
        FeedbackControl<ReactivePower>? ReactiveFeedback { get; }
    }
}

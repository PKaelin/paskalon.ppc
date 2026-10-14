// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Ramps;

namespace paskalON.OperatingModes.Domain.Transitions
{
    /// <summary>
    /// Request to transition the step of one changed operating mode on a power axis.
    /// </summary>
    /// <param name="OperatingModeName">The name of the operating mode causing the change.</param>
    /// <param name="Kind">The kind of transition.</param>
    /// <param name="Offset">The step of the operating mode in base unit (previous contribution - current contribution).</param>
    /// <param name="RampController">The ramp controller of the operating mode causing the change (own copy).</param>
    /// <param name="OwnsAxis">Whether the operating mode owns the power axis (base, exclusive or override).</param>
    /// <remarks>
    /// OwnsAxis = true for:
    /// Active:	Override, BaseActive, ExclusiveActive
    /// Reactive: Override, BaseReactive, ExclusiveReactive
    /// </remarks>
    public sealed record TransitionRequest(string OperatingModeName, TransitionKind Kind, double Offset, IRampController RampController, bool OwnsAxis);
}

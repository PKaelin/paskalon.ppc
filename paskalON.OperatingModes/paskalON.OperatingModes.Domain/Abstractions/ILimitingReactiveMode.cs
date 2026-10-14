// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Abstractions
{
    /// <summary>
    /// Classification: Limiting Reactive
    /// Position: After the additives must have a Base Reactive before this mode
    /// Cardinality: 0..n
    /// Description: Imposes a boundary on the combined target. It does not necessarily replace the underlying control mode.
    /// </summary>
    /// <remarks>
    /// Reads <see cref="OperatingModeSignals.FinalReactivePower"/> and writes the limited value to <see cref="OperatingModeSignals.LimitedReactivePower"/>. Multiple limiting modes chain because the final value returns the limited value once it is set.
    /// </remarks>
    public interface ILimitingReactiveMode : IStackableMode
    {
    }
}

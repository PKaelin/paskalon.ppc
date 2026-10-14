// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Classifications of an operating mode resolved from the implemented classification interfaces.
    /// </summary>
    /// <remarks>
    /// An operating mode can have one classification per power axis, therefore this is a flag enumeration.
    /// </remarks>
    [Flags]
    public enum OperatingModeClassification
    {
        /// <summary>
        /// No classification.
        /// </summary>
        None = 0,

        /// <summary>
        /// Forces active and or reactive power targets of a unit or the plant. See <see cref="Abstractions.IOverrideMode"/>.
        /// </summary>
        Override = 1 << 0,

        /// <summary>
        /// The only active power mode. See <see cref="Abstractions.IExclusiveActiveMode"/>.
        /// </summary>
        ExclusiveActive = 1 << 1,

        /// <summary>
        /// The only reactive power mode. See <see cref="Abstractions.IExclusiveReactiveMode"/>.
        /// </summary>
        ExclusiveReactive = 1 << 2,

        /// <summary>
        /// The primary active power mode. See <see cref="Abstractions.IBaseActiveMode"/>.
        /// </summary>
        BaseActive = 1 << 3,

        /// <summary>
        /// The primary reactive power mode. See <see cref="Abstractions.IBaseReactiveMode"/>.
        /// </summary>
        BaseReactive = 1 << 4,

        /// <summary>
        /// Contributes an active power correction. See <see cref="Abstractions.IAdditiveActiveMode"/>.
        /// </summary>
        AdditiveActive = 1 << 5,

        /// <summary>
        /// Contributes a reactive power correction. See <see cref="Abstractions.IAdditiveReactiveMode"/>.
        /// </summary>
        AdditiveReactive = 1 << 6,

        /// <summary>
        /// Imposes a boundary on the active power. See <see cref="Abstractions.ILimitingActiveMode"/>.
        /// </summary>
        LimitingActive = 1 << 7,

        /// <summary>
        /// Imposes a boundary on the reactive power. See <see cref="Abstractions.ILimitingReactiveMode"/>.
        /// </summary>
        LimitingReactive = 1 << 8,
    }
}

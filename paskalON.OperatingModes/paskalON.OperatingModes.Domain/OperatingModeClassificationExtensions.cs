// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Abstractions;

namespace paskalON.OperatingModes.Domain
{
    /// <summary>
    /// Extension and resolver methods for <see cref="OperatingModeClassification"/>.
    /// </summary>
    public static class OperatingModeClassificationExtensions
    {
        /// <summary>
        /// All classifications that control active power.
        /// </summary>
        private const OperatingModeClassification ActiveClassifications = OperatingModeClassification.Override | OperatingModeClassification.ExclusiveActive |
            OperatingModeClassification.BaseActive | OperatingModeClassification.AdditiveActive | OperatingModeClassification.LimitingActive;


        /// <summary>
        /// All classifications that control reactive power.
        /// </summary>
        private const OperatingModeClassification ReactiveClassifications = OperatingModeClassification.Override | OperatingModeClassification.ExclusiveReactive |
            OperatingModeClassification.BaseReactive | OperatingModeClassification.AdditiveReactive | OperatingModeClassification.LimitingReactive;


        /// <summary>
        /// Resolves the classifications of an operating mode from its implemented classification interfaces.
        /// </summary>
        /// <param name="operatingMode">The operating mode to resolve.</param>
        /// <returns>The resolved classifications.</returns>
        public static OperatingModeClassification Resolve(IOperatingMode operatingMode)
        {
            ArgumentNullException.ThrowIfNull(operatingMode);
            // Use compound bitwise OR assignment (preserves any existing flags)
            OperatingModeClassification classification = OperatingModeClassification.None;
            classification |= operatingMode is IOverrideMode ? OperatingModeClassification.Override : OperatingModeClassification.None;
            classification |= operatingMode is IExclusiveActiveMode ? OperatingModeClassification.ExclusiveActive : OperatingModeClassification.None;
            classification |= operatingMode is IExclusiveReactiveMode ? OperatingModeClassification.ExclusiveReactive : OperatingModeClassification.None;
            classification |= operatingMode is IBaseActiveMode ? OperatingModeClassification.BaseActive : OperatingModeClassification.None;
            classification |= operatingMode is IBaseReactiveMode ? OperatingModeClassification.BaseReactive : OperatingModeClassification.None;
            classification |= operatingMode is IAdditiveActiveMode ? OperatingModeClassification.AdditiveActive : OperatingModeClassification.None;
            classification |= operatingMode is IAdditiveReactiveMode ? OperatingModeClassification.AdditiveReactive : OperatingModeClassification.None;
            classification |= operatingMode is ILimitingActiveMode ? OperatingModeClassification.LimitingActive : OperatingModeClassification.None;
            classification |= operatingMode is ILimitingReactiveMode ? OperatingModeClassification.LimitingReactive : OperatingModeClassification.None;

            return classification;
        }


        /// <summary>
        /// Gets whether the classification controls active power.
        /// </summary>
        /// <param name="classification">The classification.</param>
        /// <returns>True if at least one active power classification is set.</returns>
        public static bool HasActive(this OperatingModeClassification classification)
        {
            return (classification & ActiveClassifications) != OperatingModeClassification.None;
        }


        /// <summary>
        /// Gets whether the classification controls reactive power.
        /// </summary>
        /// <param name="classification">The classification.</param>
        /// <returns>True if at least one reactive power classification is set.</returns>
        public static bool HasReactive(this OperatingModeClassification classification)
        {
            return (classification & ReactiveClassifications) != OperatingModeClassification.None;
        }
    }
}

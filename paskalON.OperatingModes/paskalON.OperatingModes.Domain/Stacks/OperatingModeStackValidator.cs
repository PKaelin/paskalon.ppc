// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Abstractions;

namespace paskalON.OperatingModes.Domain.Stacks
{
    /// <summary>
    /// Validates the stack of operating modes to ensure they adhere to the expected rules and constraints.
    /// </summary>
    public static class OperatingModeStackValidator
    {
        /// <summary>
        /// Validates the name, classification and ordering rules of the operating modes.
        /// </summary>
        /// <param name="modes">The operating modes ordered by ascending priority.</param>
        /// <exception cref="InvalidOperationException">The operating modes violate a classification rule.</exception>
        public static void Validate(IReadOnlyList<IStackableMode> modes)
        {
            ArgumentNullException.ThrowIfNull(modes);

            if (modes.Count == 0)
            {
                return;
            }

            ValidateUnique(modes);
            ValidateClassification(modes);

            // Check exclusive active modes
            int exclusiveActive = modes.OfType<IExclusiveActiveMode>().Count();

            if (exclusiveActive > 0)
            {
                if (exclusiveActive > 1)
                {
                    throw new InvalidOperationException("Only one exclusive active mode is allowed.");
                }

                if (modes.Any(m => m is IAdditiveActiveMode or ILimitingActiveMode or IBaseActiveMode))
                {
                    throw new InvalidOperationException("Exclusive active modes cannot be combined with base, additive, or limiting active modes.");
                }
            }

            // Check exclusive reactive modes
            int exclusiveReactive = modes.OfType<IExclusiveReactiveMode>().Count();

            if (exclusiveReactive > 0)
            {
                if (exclusiveReactive > 1)
                {
                    throw new InvalidOperationException("Only one exclusive reactive mode is allowed.");
                }

                if (modes.Any(m => m is IAdditiveReactiveMode or ILimitingReactiveMode or IBaseReactiveMode))
                {
                    throw new InvalidOperationException("Exclusive reactive modes cannot be combined with base, additive, or limiting reactive modes.");
                }
            }

            // Check base active
            int activeBases = modes.OfType<IBaseActiveMode>().Count();

            if (activeBases > 1)
            {
                throw new InvalidOperationException("Only one active-power base mode is allowed.");
            }

            if (modes.Any(mode => mode is IAdditiveActiveMode or ILimitingActiveMode) && activeBases != 1)
            {
                throw new InvalidOperationException("Active-power additives and limits require a base active mode.");
            }

            // Check base reactive
            int reactiveBases = modes.OfType<IBaseReactiveMode>().Count();

            if (reactiveBases > 1)
            {
                throw new InvalidOperationException("Only one reactive-power base mode is allowed.");
            }

            if (modes.Any(mode => mode is IAdditiveReactiveMode or ILimitingReactiveMode) && reactiveBases != 1)
            {
                throw new InvalidOperationException("Reactive-power additives and limits require a base reactive mode.");
            }

            ValidateOrdering(modes);
        }


        /// <summary>
        /// Validates that each operating mode has at most one "classification" per power axis.
        /// </summary>
        /// <param name="modes">The operating modes.</param>
        /// <remarks>
        /// A mode cannot be both an exclusive and a base mode or both an additive and a limiting mode for the same power axis (active or reactive).
        /// </remarks>
        private static void ValidateClassification(IReadOnlyList<IStackableMode> modes)
        {
            foreach (IStackableMode mode in modes)
            {
                int activeClassifications = (mode is IExclusiveActiveMode ? 1 : 0) +
                    (mode is IBaseActiveMode ? 1 : 0) +
                    (mode is IAdditiveActiveMode ? 1 : 0) +
                    (mode is ILimitingActiveMode ? 1 : 0);

                if (activeClassifications > 1)
                {
                    throw new InvalidOperationException($"Operating mode '{mode.Name}' has multiple active-power classifications.");
                }

                int reactiveClassifications = (mode is IExclusiveReactiveMode ? 1 : 0) +
                    (mode is IBaseReactiveMode ? 1 : 0) +
                    (mode is IAdditiveReactiveMode ? 1 : 0) +
                    (mode is ILimitingReactiveMode ? 1 : 0);

                if (reactiveClassifications > 1)
                {
                    throw new InvalidOperationException($"Operating mode '{mode.Name}' has multiple reactive-power classifications.");
                }
            }
        }


        /// <summary>
        /// Validates that the operating mode names are unique.
        /// </summary>
        /// <param name="modes">The operating modes.</param>
        private static void ValidateUnique(IReadOnlyList<IStackableMode> modes)
        {
            if (modes.Select(mode => mode.Name).Distinct(StringComparer.Ordinal).Count() != modes.Count)
            {
                throw new InvalidOperationException("The stack contains duplicate operating mode names.");
            }
        }


        /// <summary>
        /// Validates that base modes come first and additive modes come before limiting modes per power axis.
        /// </summary>
        /// <param name="modes">The operating modes ordered by ascending priority.</param>
        private static void ValidateOrdering(IReadOnlyList<IStackableMode> modes)
        {
            // Validate already checked for exclusive modes, so we only need to check the ordering of base, additive, and limiting modes.
            // Validate already checked whether there is a base mode present when additive or limiting modes are present.
            // Check active modes ordering
            List<IStackableMode> activeModes = modes.Where(m => m is IBaseActiveMode or IAdditiveActiveMode or ILimitingActiveMode).ToList();
            bool? hasActiveLimiting = null;

            // No active power modes are configured
            if (activeModes.Count > 0)
            {
                // Enforce the first element rule explicitly
                if (activeModes[0] is not IBaseActiveMode)
                {
                    throw new InvalidOperationException("The first active-power mode in the stack must be a base mode.");
                }
                // Validate the ordering all IAdditive before all ILimiting no mixture
                for (int i = 1; i < activeModes.Count; i++)
                {
                    IStackableMode currentMode = activeModes[i];

                    if (currentMode is ILimitingActiveMode)
                    {
                        hasActiveLimiting = true;
                    }
                    else if (currentMode is IAdditiveActiveMode && hasActiveLimiting == true)
                    {
                        throw new InvalidOperationException("An additive active mode cannot follow a limiting active mode in the stack.");
                    }
                }
            }

            // Check reactive modes ordering
            List<IStackableMode> reactiveModes = modes.Where(m => m is IBaseReactiveMode or IAdditiveReactiveMode or ILimitingReactiveMode).ToList();
            bool? hasReactiveLimiting = null;

            // No reactive power modes are configured
            if (reactiveModes.Count > 0)
            {
                // Enforce the first element rule explicitly
                if (reactiveModes[0] is not IBaseReactiveMode)
                {
                    throw new InvalidOperationException("The first reactive-power mode in the stack must be a base mode.");
                }
                // Validate the ordering all IAdditive before all ILimiting no mixture
                for (int i = 1; i < reactiveModes.Count; i++)
                {
                    IStackableMode currentMode = reactiveModes[i];

                    if (currentMode is ILimitingReactiveMode)
                    {
                        hasReactiveLimiting = true;
                    }
                    else if (currentMode is IAdditiveReactiveMode && hasReactiveLimiting == true)
                    {
                        throw new InvalidOperationException("An additive reactive mode cannot follow a limiting reactive mode in the stack.");
                    }
                }
            }
        }
    }
}

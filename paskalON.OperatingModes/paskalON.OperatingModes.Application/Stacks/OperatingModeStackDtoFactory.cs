// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Stacks;
using paskalON.OperatingModes.Dto.Stacks;

namespace paskalON.OperatingModes.Application.Stacks
{
    /// <summary>
    /// Factory that creates the stack DTOs of the stackable operating modes.
    /// </summary>
    /// <remarks>
    /// Specialized operating modes are matched first (e.g. <see cref="ActivePowerLimitMode"/>), then the classification
    /// defaults (e.g. all limiting modes) and finally <see cref="OperatingModeStackDto"/> for all other stackable modes.
    /// A new DTO must also be registered as derived type on <see cref="OperatingModeStackDto"/> to be serialized with its properties.
    /// </remarks>
    public class OperatingModeStackDtoFactory : IOperatingModeStackDtoFactory
    {
        /// <summary>
        /// Stack types of the domain classifications. The override classification has no stack type.
        /// </summary>
        private static readonly (OperatingModeClassification Classification, OperatingModeStackTypeDto Type)[] _stackTypes = new (OperatingModeClassification, OperatingModeStackTypeDto)[]
        {
            (OperatingModeClassification.ExclusiveActive, OperatingModeStackTypeDto.ExclusiveActive),
            (OperatingModeClassification.ExclusiveReactive, OperatingModeStackTypeDto.ExclusiveReactive),
            (OperatingModeClassification.BaseActive, OperatingModeStackTypeDto.BaseActive),
            (OperatingModeClassification.BaseReactive, OperatingModeStackTypeDto.BaseReactive),
            (OperatingModeClassification.AdditiveActive, OperatingModeStackTypeDto.AdditiveActive),
            (OperatingModeClassification.AdditiveReactive, OperatingModeStackTypeDto.AdditiveReactive),
            (OperatingModeClassification.LimitingActive, OperatingModeStackTypeDto.LimitingActive),
            (OperatingModeClassification.LimitingReactive, OperatingModeStackTypeDto.LimitingReactive)
        };


        /// <inheritdoc/>
        public OperatingModeStackDto Create(IStackableMode operatingMode, int? priority = null)
        {
            ArgumentNullException.ThrowIfNull(operatingMode);

            OperatingModeStackTypeDto type = CreateStackType(operatingMode.Classification);

            switch (operatingMode)
            {
                // Specialized operating modes
                case ActivePowerLimitMode activePowerLimitMode:
                    return new OperatingModeLimitingStackDto
                    {
                        OperatingModeName = operatingMode.Name,
                        Type = type,
                        Priority = priority,
                        MaximumLimit = activePowerLimitMode.MaximumActivePower.Watts,
                        MinimumLimit = activePowerLimitMode.MinimumActivePower.Watts
                    };

                case ReactivePowerLimitMode reactivePowerLimitMode:
                    return new OperatingModeLimitingStackDto
                    {
                        OperatingModeName = operatingMode.Name,
                        Type = type,
                        Priority = priority,
                        MaximumLimit = reactivePowerLimitMode.MaximumReactivePower.VoltAmperesReactive,
                        MinimumLimit = reactivePowerLimitMode.MinimumReactivePower.VoltAmperesReactive
                    };

                // Classification defaults
                case ILimitingActiveMode:
                    return new OperatingModeLimitingStackDto
                    {
                        OperatingModeName = operatingMode.Name,
                        Type = type,
                        Priority = priority,
                        MaximumLimit = operatingMode.ActiveControl?.Target.Watts ?? 0
                    };

                case ILimitingReactiveMode:
                    return new OperatingModeLimitingStackDto
                    {
                        OperatingModeName = operatingMode.Name,
                        Type = type,
                        Priority = priority,
                        MaximumLimit = operatingMode.ReactiveControl?.Target.VoltAmperesReactive ?? 0
                    };

                default:
                    return new OperatingModeStackDto
                    {
                        OperatingModeName = operatingMode.Name,
                        Type = type,
                        Priority = priority
                    };
            }
        }


        /// <inheritdoc/>
        public IReadOnlyList<OperatingModeStackDto> Create(IEnumerable<IStackableMode> operatingModes)
        {
            ArgumentNullException.ThrowIfNull(operatingModes);

            return operatingModes.Select(operatingMode => Create(operatingMode)).ToList();
        }


        /// <inheritdoc/>
        public IReadOnlyList<OperatingModeStackDto> Create(IEnumerable<OperatingModeStackEntry> entries)
        {
            ArgumentNullException.ThrowIfNull(entries);

            return entries.Select(entry => Create(entry.OperatingMode, entry.Priority)).ToList();
        }


        /// <summary>
        /// Creates the stack type flags of the classification flags.
        /// </summary>
        /// <param name="classification">The classification of the operating mode.</param>
        /// <returns>The stack type flags.</returns>
        private static OperatingModeStackTypeDto CreateStackType(OperatingModeClassification classification)
        {
            OperatingModeStackTypeDto type = OperatingModeStackTypeDto.Unknown;

            foreach ((OperatingModeClassification Classification, OperatingModeStackTypeDto Type) stackType in _stackTypes)
            {
                if (classification.HasFlag(stackType.Classification))
                {
                    type |= stackType.Type;
                }
            }

            return type;
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.ConstraintEngine.Domain.Configs;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.ConstraintEngine.Domain
{
    /// <summary>
    /// Base class for all power constraints.
    /// </summary>
    public abstract class PowerConstraintBase : ConstraintBase
    {
        /// <summary>
        /// Violation name of an active power above the maximum limit.
        /// </summary>
        private const string ActiveAboveMaximum = nameof(ActiveAboveMaximum);


        /// <summary>
        /// Violation name of an active power below the minimum limit.
        /// </summary>
        private const string ActiveBelowMinimum = nameof(ActiveBelowMinimum);


        /// <summary>
        /// Violation name of a reactive power above the maximum limit.
        /// </summary>
        private const string ReactiveAboveMaximum = nameof(ReactiveAboveMaximum);


        /// <summary>
        /// Violation name of a reactive power below the minimum limit.
        /// </summary>
        private const string ReactiveBelowMinimum = nameof(ReactiveBelowMinimum);


        /// <summary>
        /// Power constraint base configuration.
        /// </summary>
        private readonly PowerConstraintConfig _config;


        /// <summary>
        /// Constructor of <see cref="PowerConstraintBase"/>.
        /// </summary>
        /// <param name="logger">ILogger for handling application logging and diagnostics.</param>
        /// <param name="config">Power constraint base configuration.</param>
        /// <param name="map">Power constraint base map.</param>
        public PowerConstraintBase(ILogger logger, PowerConstraintConfig config, PowerConstraintBaseMap map)
            : base(logger, config, map)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }


        /// <inheritdoc/>
        /// <remarks>
        /// The setpoints are applied periodically, so a violation is logged once when it starts and once when it ends.
        /// </remarks>
        public override void ApplyConstraints(ref ActivePower activePower, ref ReactivePower reactivePower, bool shallLogViolations = true)
        {
            bool isActiveAboveMaximum = _config.MaximumActivePowerWatt.HasValue && activePower.Watts > _config.MaximumActivePowerWatt;
            bool isActiveBelowMinimum = isActiveAboveMaximum is false && _config.MinimumActivePowerWatt.HasValue
                && activePower.Watts < _config.MinimumActivePowerWatt;
            bool isReactiveAboveMaximum = _config.MaximumReactivePowerVars.HasValue && reactivePower.VoltAmperesReactive > _config.MaximumReactivePowerVars;
            bool isReactiveBelowMinimum = isReactiveAboveMaximum is false && _config.MinimumReactivePowerVars.HasValue
                && reactivePower.VoltAmperesReactive < _config.MinimumReactivePowerVars;

            if (shallLogViolations == true)
            {
                LogActivePowerViolations(activePower, isActiveAboveMaximum, isActiveBelowMinimum);
                LogReactivePowerViolations(reactivePower, isReactiveAboveMaximum, isReactiveBelowMinimum);
            }

            if (isActiveAboveMaximum)
            {
                activePower.Watts = _config.MaximumActivePowerWatt!.Value;
            }
            else if (isActiveBelowMinimum)
            {
                activePower.Watts = _config.MinimumActivePowerWatt!.Value;
            }

            if (isReactiveAboveMaximum)
            {
                reactivePower.VoltAmperesReactive = _config.MaximumReactivePowerVars!.Value;
            }
            else if (isReactiveBelowMinimum)
            {
                reactivePower.VoltAmperesReactive = _config.MinimumReactivePowerVars!.Value;
            }
        }


        /// <summary>
        /// Logs a started or ended violation of the active power limits.
        /// </summary>
        /// <param name="activePower">The requested active power.</param>
        /// <param name="isAboveMaximum">Whether the active power is above the maximum limit.</param>
        /// <param name="isBelowMinimum">Whether the active power is below the minimum limit.</param>
        private void LogActivePowerViolations(ActivePower activePower, bool isAboveMaximum, bool isBelowMinimum)
        {
            bool isMaximumChanged = UpdateViolation(ActiveAboveMaximum, isAboveMaximum);
            bool isMinimumChanged = UpdateViolation(ActiveBelowMinimum, isBelowMinimum);

            if (isMaximumChanged && isAboveMaximum)
            {
                _logger.LogWarning("{Name} active power {ActivePower} exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, activePower.Watts,
                    _config.MaximumActivePowerWatt);
            }
            else if (isMinimumChanged && isBelowMinimum)
            {
                _logger.LogWarning("{Name} active power {ActivePower} below minimum limit {MinLimit}. Clamping to minimum.", Name, activePower.Watts,
                    _config.MinimumActivePowerWatt);
            }
            else if (isMaximumChanged || isMinimumChanged)
            {
                _logger.LogInformation("{Name} active power {ActivePower} is within the limits again.", Name, activePower.Watts);
            }
        }


        /// <summary>
        /// Logs a started or ended violation of the reactive power limits.
        /// </summary>
        /// <param name="reactivePower">The requested reactive power.</param>
        /// <param name="isAboveMaximum">Whether the reactive power is above the maximum limit.</param>
        /// <param name="isBelowMinimum">Whether the reactive power is below the minimum limit.</param>
        private void LogReactivePowerViolations(ReactivePower reactivePower, bool isAboveMaximum, bool isBelowMinimum)
        {
            bool isMaximumChanged = UpdateViolation(ReactiveAboveMaximum, isAboveMaximum);
            bool isMinimumChanged = UpdateViolation(ReactiveBelowMinimum, isBelowMinimum);

            if (isMaximumChanged && isAboveMaximum)
            {
                _logger.LogWarning("{Name} reactive power {ReactivePower} exceeds maximum limit {MaxLimit}. Clamping to maximum.", Name, reactivePower.VoltAmperesReactive,
                    _config.MaximumReactivePowerVars);
            }
            else if (isMinimumChanged && isBelowMinimum)
            {
                _logger.LogWarning("{Name} reactive power {ReactivePower} below minimum limit {MinLimit}. Clamping to minimum.", Name, reactivePower.VoltAmperesReactive,
                    _config.MinimumReactivePowerVars);
            }
            else if (isMaximumChanged || isMinimumChanged)
            {
                _logger.LogInformation("{Name} reactive power {ReactivePower} is within the limits again.", Name, reactivePower.VoltAmperesReactive);
            }
        }
    }
}

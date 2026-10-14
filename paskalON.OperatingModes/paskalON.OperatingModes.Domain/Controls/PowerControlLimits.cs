// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain.Configs;

namespace paskalON.OperatingModes.Domain.Controls
{
    /// <summary>
    /// Resolved limits and deadbands of a power control in the base unit (Watts or Vars).
    /// </summary>
    /// <param name="Minimum">The minimum allowed power.</param>
    /// <param name="Maximum">The maximum allowed power.</param>
    /// <param name="DeadbandSetpoint">Deadband for setpoint changes and for reaching the target setpoint.</param>
    /// <param name="DeadbandAvailable">Deadband for available power changes.</param>
    public sealed record PowerControlLimits(double Minimum, double Maximum, double DeadbandSetpoint, double DeadbandAvailable)
    {
        /// <summary>
        /// Creates the active power limits. The operating mode configuration overwrites the nameplate.
        /// </summary>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <returns>The resolved active power limits.</returns>
        public static PowerControlLimits CreateActive(OperatingModeBaseConfig config, SystemConfig systemConfig)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(systemConfig);

            return new PowerControlLimits(config.MinimumActivePowerLimitWatt ?? systemConfig.NameplateMinimumActivePowerWatt,
                config.MaximumActivePowerLimitWatt ?? systemConfig.NameplateMaximumActivePowerWatt, config.DeadbandSetpoint, config.DeadbandAvailable);
        }


        /// <summary>
        /// Creates the reactive power limits. The operating mode configuration overwrites the nameplate.
        /// </summary>
        /// <param name="config">The operating mode configuration.</param>
        /// <param name="systemConfig">The system configuration.</param>
        /// <returns>The resolved reactive power limits.</returns>
        public static PowerControlLimits CreateReactive(OperatingModeBaseConfig config, SystemConfig systemConfig)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(systemConfig);

            return new PowerControlLimits(config.MinimumReactivePowerLimitVars ?? systemConfig.NameplateMinimumReactivePowerVars,
                config.MaximumReactivePowerLimitVars ?? systemConfig.NameplateMaximumReactivePowerVars, config.DeadbandSetpoint, config.DeadbandAvailable);
        }


        /// <summary>
        /// Clamps a value into the minimum and maximum limits.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <returns>The clamped value.</returns>
        public double Clamp(double value)
        {
            if (value > Maximum)
            {
                return Maximum;
            }

            if (value < Minimum)
            {
                return Minimum;
            }

            return value;
        }
    }
}

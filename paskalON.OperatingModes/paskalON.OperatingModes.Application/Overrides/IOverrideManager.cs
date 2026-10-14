// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Application.Overrides
{
    /// <summary>
    /// Interface for managing the override slot.
    /// </summary>
    public interface IOverrideManager
    {
        /// <summary>
        /// Gets the override slot.
        /// </summary>
        OperatingModeOverrides Overrides { get; }


        /// <summary>
        /// Override modes available for activation.
        /// </summary>
        IReadOnlyCollection<IOverrideMode> AvailableOperatingModes { get; }


        /// <summary>
        /// Active override modes.
        /// </summary>
        IReadOnlyCollection<IOverrideMode> ActiveOperatingModes { get; }


        /// <summary>
        /// Initializes the override manager with the configured operating modes. Only override modes are available for activation.
        /// </summary>
        /// <param name="operatingModes">A dictionary of configured operating modes keyed by their names.</param>
        void InitializeOverrides(Dictionary<string, OperatingModeBase> operatingModes);


        /// <summary>
        /// Activates an override mode. It is activated by the next calculation cycle.
        /// </summary>
        /// <param name="operatingModeName">The name of the override mode.</param>
        void ActivateOperatingMode(string operatingModeName);


        /// <summary>
        /// Deactivates an override mode. It is deactivated by the next calculation cycle.
        /// </summary>
        /// <param name="operatingModeName">The name of the override mode.</param>
        void DeactivateOperatingMode(string operatingModeName);


        /// <summary>
        /// Sets the operator setpoints of a maintenance override mode.
        /// </summary>
        /// <param name="operatingModeName">The name of the maintenance mode.</param>
        /// <param name="activePower">The active power setpoint.</param>
        /// <param name="reactivePower">The reactive power setpoint.</param>
        void SetSetpoint(string operatingModeName, ActivePower activePower, ReactivePower reactivePower);
    }
}

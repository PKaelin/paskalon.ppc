// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.Abstractions;
using paskalON.OperatingModes.Domain.OpenModes;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.OperatingModes.Application.Overrides
{
    /// <summary>
    /// Manages the override slot, allowing for activating and deactivating override modes.
    /// </summary>
    /// <remarks>
    /// The override slot validates the cardinality (0..1 per DER unit) and records the changes. The system operating mode control
    /// activates and deactivates the override modes in its next calculation cycle.
    /// </remarks>
    public class OverrideManager : IOverrideManager
    {
        /// <summary>
        /// Override modes available for activation.
        /// </summary>
        private Dictionary<string, IOverrideMode> _availableOperatingModes = new Dictionary<string, IOverrideMode>(StringComparer.Ordinal);


        /// <inheritdoc/>
        public OperatingModeOverrides Overrides { get; }


        /// <inheritdoc/>
        public IReadOnlyCollection<IOverrideMode> AvailableOperatingModes { get => _availableOperatingModes.Values; }


        /// <inheritdoc/>
        public IReadOnlyCollection<IOverrideMode> ActiveOperatingModes { get => Overrides.OperatingModes; }


        /// <summary>
        /// Constructor of <see cref="OverrideManager"/>.
        /// </summary>
        /// <param name="overrides">The override slot.</param>
        public OverrideManager(OperatingModeOverrides overrides)
        {
            ArgumentNullException.ThrowIfNull(overrides);

            Overrides = overrides;
        }


        /// <inheritdoc/>
        public void InitializeOverrides(Dictionary<string, OperatingModeBase> operatingModes)
        {
            ArgumentNullException.ThrowIfNull(operatingModes);

            _availableOperatingModes = operatingModes.Values
                .OfType<IOverrideMode>()
                .ToDictionary(operatingMode => operatingMode.Name, StringComparer.Ordinal);
            Overrides.Clear();
        }


        /// <inheritdoc/>
        public void ActivateOperatingMode(string operatingModeName)
        {
            IOverrideMode operatingMode = GetAvailableOperatingMode(operatingModeName);
            Overrides.Add(operatingMode);
        }


        /// <inheritdoc/>
        public void DeactivateOperatingMode(string operatingModeName)
        {
            IOverrideMode operatingMode = GetAvailableOperatingMode(operatingModeName);
            Overrides.Remove(operatingMode);
        }


        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">The override mode does not accept operator setpoints.</exception>
        public void SetSetpoint(string operatingModeName, ActivePower activePower, ReactivePower reactivePower)
        {
            IOverrideMode operatingMode = GetAvailableOperatingMode(operatingModeName);

            if (operatingMode is not MaintenanceMode maintenanceMode)
            {
                throw new InvalidOperationException($"Override mode '{operatingModeName}' does not accept operator setpoints.");
            }

            maintenanceMode.SetSetpoint(activePower, reactivePower);
        }


        /// <summary>
        /// Gets an available override mode by its name.
        /// </summary>
        /// <param name="operatingModeName">The name of the override mode.</param>
        /// <returns>The available override mode.</returns>
        /// <exception cref="KeyNotFoundException">The override mode is not configured.</exception>
        private IOverrideMode GetAvailableOperatingMode(string operatingModeName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(operatingModeName);

            if (_availableOperatingModes.TryGetValue(operatingModeName, out IOverrideMode? operatingMode) is false)
            {
                throw new KeyNotFoundException($"Override mode '{operatingModeName}' is not configured.");
            }

            return operatingMode;
        }
    }
}

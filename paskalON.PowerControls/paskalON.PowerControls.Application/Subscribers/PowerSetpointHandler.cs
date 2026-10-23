// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.OperatingModes.Client.Subscribers;
using paskalON.OperatingModes.Dto.Powers;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.PowerControls.Application.Subscribers
{
    /// <summary>
    /// Applies the received system and DER unit power setpoints to the power control manager.
    /// </summary>
    public sealed class PowerSetpointHandler : IPowerSetpointHandler
    {
        /// <summary>
        /// Power control manager to apply the setpoint to the control manager.
        /// </summary>
        private readonly IPowerControlManager _powerControlManager;


        /// <summary>
        /// Constructor of <see cref="PowerSetpointHandler"/>.
        /// </summary>
        /// <param name="powerControlManager">Power control manager to apply the setpoint to the control manager.</param>
        public PowerSetpointHandler(IPowerControlManager powerControlManager)
        {
            ArgumentNullException.ThrowIfNull(powerControlManager);

            _powerControlManager = powerControlManager;
        }


        /// <inheritdoc/>
        public async Task HandleSetpointsAsync(CurrentSetpointsDto setpoints)
        {
            ArgumentNullException.ThrowIfNull(setpoints);

            await _powerControlManager.SetSystemPowerTarget(new ActivePower(setpoints.SetpointActivePowerWatts),
                new ReactivePower(setpoints.SetpointReactivePowerVars));
        }


        /// <inheritdoc/>
        public async Task HandleDerUnitSetpointsAsync(CurrentDerUnitSetpointsDto setpoints)
        {
            ArgumentNullException.ThrowIfNull(setpoints);

            await _powerControlManager.SetDerUnitPowerTarget(setpoints.DerUnitName, new ActivePower(setpoints.SetpointActivePowerWatts),
                new ReactivePower(setpoints.SetpointReactivePowerVars));
        }
    }
}

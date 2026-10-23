// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.PowerControls.Client.Subscribers;
using paskalON.PowerControls.Dto.Powers;

namespace paskalON.Devices.Application.Subscribers
{
    /// <summary>
    /// Applies the received power targets to the power conversion systems (PCS) of the device manager.
    /// </summary>
    public sealed class PowerTargetHandler : IPowerTargetHandler
    {
        /// <summary>
        /// Device manager to apply the power targets to the devices.
        /// </summary>
        private readonly IDeviceManager _deviceManager;


        /// <summary>
        /// Constructor of <see cref="PowerTargetHandler"/>.
        /// </summary>
        /// <param name="deviceManager">Device manager to apply the power targets to the devices.</param>
        public PowerTargetHandler(IDeviceManager deviceManager)
        {
            ArgumentNullException.ThrowIfNull(deviceManager);

            _deviceManager = deviceManager;
        }


        /// <inheritdoc/>
        public async Task HandleAsync(CurrentTargetsDto targets)
        {
            ArgumentNullException.ThrowIfNull(targets);

            await _deviceManager.SetPcsPowerTarget(targets.DeviceId, targets.ActivePowerWatt, targets.ReactivePowerVar);
        }
    }
}

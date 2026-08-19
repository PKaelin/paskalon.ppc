// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Dto;
using paskalON.Telemetry.Entries;
using System.Collections.Concurrent;

namespace paskalON.Devices.Client.Registers
{
    public sealed class DeviceRegister<TDevice, TDefinition, TCore, TDetail> : IDeviceRegister<TDevice, TDefinition, TCore, TDetail>
        where TDevice : DeviceBase<TDefinition, TCore, TDetail>
        where TDefinition : class, IDeviceDefinition
        where TCore : class, IDevice
        where TDetail : class, IDevice
    {

        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger _logger;


        /// <summary>
        /// Threat safe dictionary.
        /// </summary>
        private readonly ConcurrentDictionary<int, TDevice> _devices = new();


        /// <summary>
        /// List of registered devices.
        /// </summary>
        public ICollection<TDevice> Devices => _devices.Values;



        /// <summary>
        /// Constructor of <see cref="DeviceRegister{TDevice, TDefinition, TCore, TDetail}"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        public DeviceRegister(ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Add(TDevice device)
        {
            if (_devices.TryAdd(device.DeviceId, device) == false)
            {
                throw new InvalidOperationException($"Device {device.DeviceId} already exists.");
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool TryGet(int deviceId, out TDevice? device)
        {
            return _devices.TryGetValue(deviceId, out device);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void UpdateDefinition(TDefinition message)
        {
            if (!_devices.TryGetValue(message.DeviceId, out var device))
            {
                _logger.LogError("Update definition messages device id {DeviceID} was not registered in devices.", message.DeviceId);
                return;
            }

            device.UpdateDefinition(message);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void UpdateCore(TCore message)
        {
            if (!_devices.TryGetValue(message.DeviceId, out var device))
            {
                _logger.LogError("Update core messages device id {DeviceID} was not registered in devices.", message.DeviceId);
                return;
            }

            device.UpdateCore(message);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void UpdateDetail(TDetail message)
        {
            if (!_devices.TryGetValue(message.DeviceId, out var device))
            {
                _logger.LogError("Update detail messages device id {DeviceID} was not registered in devices.", message.DeviceId);
                return;
            }

            device.UpdateDetail(message);
        }
    }
}

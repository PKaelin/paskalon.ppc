// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.SignalR;
using paskalON.Luna.Application;
using paskalON.Luna.Service.WebApi.Hubs;

namespace paskalON.Luna.Service.Publishers
{
    /// <summary>
    /// Periodically pushes the values of the subscribed devices to the clients of the <see cref="DeviceHub"/>.
    /// </summary>
    /// <remarks>
    /// Each client only receives the device it subscribed to. Nothing is published while no client subscribed.
    /// </remarks>
    public class DeviceSubscriptionPublisher : BackgroundService
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<DeviceSubscriptionPublisher> _logger;


        /// <summary>
        /// Device manager to get the devices.
        /// </summary>
        private readonly ILunaDeviceManager _deviceManager;


        /// <summary>
        /// Hub context to send the device values to the clients.
        /// </summary>
        private readonly IHubContext<DeviceHub, IDeviceHubClient> _hubContext;


        /// <summary>
        /// Register of the subscribed devices.
        /// </summary>
        private readonly DeviceSubscriptions _subscriptions;


        /// <summary>
        /// Time based interval for publishing.
        /// </summary>
        private readonly TimeSpan _interval;


        /// <summary>
        /// Constructor of <see cref="DeviceSubscriptionPublisher"/>.
        /// </summary>
        /// <param name="logger">Logger for application logging and diagnostics.</param>
        /// <param name="deviceManager">Device manager to get the devices.</param>
        /// <param name="hubContext">Hub context to send the device values to the clients.</param>
        /// <param name="subscriptions">Register of the subscribed devices.</param>
        /// <param name="interval">Time based interval for publishing.</param>
        public DeviceSubscriptionPublisher(ILogger<DeviceSubscriptionPublisher> logger, ILunaDeviceManager deviceManager,
            IHubContext<DeviceHub, IDeviceHubClient> hubContext, DeviceSubscriptions subscriptions, TimeSpan interval)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceManager);
            ArgumentNullException.ThrowIfNull(hubContext);
            ArgumentNullException.ThrowIfNull(subscriptions);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(interval, TimeSpan.Zero);

            _logger = logger;
            _deviceManager = deviceManager;
            _hubContext = hubContext;
            _subscriptions = subscriptions;
            _interval = interval;
        }


        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            PeriodicTimer timer = new PeriodicTimer(_interval);

            using (timer)
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    if (_subscriptions.IsEmpty)
                    {
                        continue;
                    }

                    foreach (KeyValuePair<string, DeviceSubscription> entry in _subscriptions.GetAll())
                    {
                        await PublishAsync(entry.Key, entry.Value);
                    }
                }
            }
        }


        /// <summary>
        /// Sends the values of the subscribed device to the connection. A failure is logged, so the other connections still receive their values.
        /// </summary>
        /// <param name="connectionId">ID of the connection.</param>
        /// <param name="subscription">The device the connection subscribed to.</param>
        /// <returns>Task</returns>
        private async Task PublishAsync(string connectionId, DeviceSubscription subscription)
        {
            IDeviceHubClient client = _hubContext.Clients.Client(connectionId);

            try
            {
                switch (subscription.Type)
                {
                    case DeviceSubscriptionType.SystemPowerMeter:
                        await client.SystemPowerMeterUpdated(subscription.DeviceId, _deviceManager.GetSystemPowerMeter(subscription.DeviceId));
                        break;
                    case DeviceSubscriptionType.AuxiliaryPowerMeter:
                        await client.AuxiliaryPowerMeterUpdated(subscription.DeviceId, _deviceManager.GetAuxiliaryPowerMeter(subscription.DeviceId));
                        break;
                    case DeviceSubscriptionType.ExternalPowerMeter:
                        await client.ExternalPowerMeterUpdated(subscription.DeviceId, _deviceManager.GetExternalPowerMeter(subscription.DeviceId));
                        break;
                    case DeviceSubscriptionType.PowerConversionSystem:
                        await client.PcsUpdated(subscription.DeviceId, _deviceManager.GetPcs(subscription.DeviceId));
                        break;
                    default:
                        _logger.LogWarning("Device subscription type {Type} is not supported.", subscription.Type);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Publishing device {DeviceId} of type {Type} failed.", subscription.DeviceId, subscription.Type);
            }
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.SignalR;
using paskalON.Luna.Application;

namespace paskalON.Luna.Service.WebApi.Hubs
{
    /// <summary>
    /// SignalR hub that pushes the live values of a device to the Luna UI.
    /// </summary>
    /// <remarks>
    /// The UI subscribes to the device shown in the content area of the system tab. A subscribing client receives
    /// the current values immediately and the <see cref="Publishers.DeviceSubscriptionPublisher"/> sends the following
    /// ones periodically, until the client unsubscribes or disconnects.
    /// </remarks>
    public class DeviceHub : Hub<IDeviceHubClient>
    {
        /// <summary>
        /// Route of the hub.
        /// </summary>
        public const string Route = "/hubs/device";


        /// <summary>
        /// Device manager to get the devices.
        /// </summary>
        private readonly ILunaDeviceManager _deviceManager;


        /// <summary>
        /// Register of the subscribed devices.
        /// </summary>
        private readonly DeviceSubscriptions _subscriptions;


        /// <summary>
        /// Constructor of <see cref="DeviceHub"/>.
        /// </summary>
        /// <param name="deviceManager">Device manager to get the devices.</param>
        /// <param name="subscriptions">Register of the subscribed devices.</param>
        public DeviceHub(ILunaDeviceManager deviceManager, DeviceSubscriptions subscriptions)
        {
            ArgumentNullException.ThrowIfNull(deviceManager);
            ArgumentNullException.ThrowIfNull(subscriptions);

            _deviceManager = deviceManager;
            _subscriptions = subscriptions;
        }


        /// <summary>
        /// Subscribes the caller to a system power meter, replacing its previous subscription.
        /// </summary>
        /// <param name="deviceId">Device ID of the system power meter.</param>
        /// <returns>Task</returns>
        public async Task SubscribeSystemPowerMeter(int deviceId)
        {
            _subscriptions.Subscribe(Context.ConnectionId, new DeviceSubscription(DeviceSubscriptionType.SystemPowerMeter, deviceId));
            await Clients.Caller.SystemPowerMeterUpdated(deviceId, _deviceManager.GetSystemPowerMeter(deviceId));
        }


        /// <summary>
        /// Removes the subscription of the caller.
        /// </summary>
        public void Unsubscribe()
        {
            _subscriptions.Unsubscribe(Context.ConnectionId);
        }


        /// <inheritdoc/>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _subscriptions.Unsubscribe(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}

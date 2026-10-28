// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.SignalR;
using paskalON.Luna.Application;

namespace paskalON.Luna.Service.WebApi.Hubs
{
    /// <summary>
    /// SignalR hub that pushes the live system values to the Luna UI.
    /// </summary>
    /// <remarks>
    /// The hub has no client callable methods. The UI connects while the dashboard is shown, a connecting client
    /// receives the current summary immediately and the <see cref="Publishers.SystemMeterSummaryPublisher"/>
    /// sends the following ones periodically.
    /// </remarks>
    public class SystemHub : Hub<ISystemClient>
    {
        /// <summary>
        /// Route of the hub.
        /// </summary>
        public const string Route = "/hubs/system";


        /// <summary>
        /// Device manager to get the system meter summary.
        /// </summary>
        private readonly ILunaDeviceManager _deviceManager;


        /// <summary>
        /// Counter of the connected clients.
        /// </summary>
        private readonly HubConnectionCounter _connectionCounter;


        /// <summary>
        /// Constructor of <see cref="SystemHub"/>.
        /// </summary>
        /// <param name="deviceManager">Device manager to get the system meter summary.</param>
        /// <param name="connectionCounter">Counter of the connected clients.</param>
        public SystemHub(ILunaDeviceManager deviceManager, HubConnectionCounter connectionCounter)
        {
            ArgumentNullException.ThrowIfNull(deviceManager);
            ArgumentNullException.ThrowIfNull(connectionCounter);

            _deviceManager = deviceManager;
            _connectionCounter = connectionCounter;
        }


        /// <inheritdoc/>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SystemMeterSummaryUpdated(_deviceManager.GetSystemMeterSummary());
            _connectionCounter.Add();
        }


        /// <inheritdoc/>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _connectionCounter.Remove();
            await base.OnDisconnectedAsync(exception);
        }
    }
}

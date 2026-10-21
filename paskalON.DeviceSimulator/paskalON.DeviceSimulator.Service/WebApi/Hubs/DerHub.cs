// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.SignalR;

namespace paskalON.DeviceSimulator.Service.WebApi.Hubs
{
    /// <summary>
    /// SignalR hub that pushes live DER values to the device simulator UI.
    /// </summary>
    /// <remarks>
    /// The hub has no client callable methods. The <see cref="Publishers.SimulationPublisher"/> sends the updates
    /// of the units and devices the UI reported as expanded (POST v1/der/setexpanded).
    /// </remarks>
    public class DerHub : Hub<IDerClient>
    {
        /// <summary>
        /// Route of the hub.
        /// </summary>
        public const string Route = "/hubs/der";
    }
}

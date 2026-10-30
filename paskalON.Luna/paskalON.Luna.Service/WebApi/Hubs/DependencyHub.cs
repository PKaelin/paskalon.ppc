// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.SignalR;
using paskalON.Luna.Application;

namespace paskalON.Luna.Service.WebApi.Hubs
{
    /// <summary>
    /// SignalR hub that pushes the availability of the dependencies to the Luna UI.
    /// </summary>
    /// <remarks>
    /// The hub has no client callable methods. It is separate from the <see cref="SystemHub"/>, because the UI header
    /// is connected all the time and the system meter summary shall only be published while the dashboard is shown.
    /// A connecting client receives the current status immediately and the
    /// <see cref="Publishers.DependencyStatusPublisher"/> sends the following ones periodically.
    /// </remarks>
    public class DependencyHub : Hub<IDependencyClient>
    {
        /// <summary>
        /// Route of the hub.
        /// </summary>
        public const string Route = "/hubs/dependency";


        /// <summary>
        /// Dependency manager to get the availability of the dependencies.
        /// </summary>
        private readonly ILunaDependencyManager _dependencyManager;


        /// <summary>
        /// Constructor of <see cref="DependencyHub"/>.
        /// </summary>
        /// <param name="dependencyManager">Dependency manager to get the availability of the dependencies.</param>
        public DependencyHub(ILunaDependencyManager dependencyManager)
        {
            ArgumentNullException.ThrowIfNull(dependencyManager);

            _dependencyManager = dependencyManager;
        }


        /// <inheritdoc/>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.DependencyStatusUpdated(_dependencyManager.GetDependencyStatus());
        }
    }
}

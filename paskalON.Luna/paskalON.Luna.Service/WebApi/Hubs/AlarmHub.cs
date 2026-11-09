// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.AspNetCore.SignalR;
using paskalON.Luna.Application;

namespace paskalON.Luna.Service.WebApi.Hubs
{
    /// <summary>
    /// Pushes the alarm summary to the UI clients, e.g. for the alarm shapes in the header.
    /// </summary>
    /// <remarks>
    /// A client receives the current summary when it connects, then every change (see AlarmSummaryPublisher).
    /// </remarks>
    public class AlarmHub : Hub<IAlarmClient>
    {
        /// <summary>
        /// Route of the hub.
        /// </summary>
        public const string Route = "/hubs/alarm";


        /// <summary>
        /// Manager of the alarm summary.
        /// </summary>
        private readonly ILunaAlarmManager _alarmManager;


        /// <summary>
        /// Constructor of <see cref="AlarmHub"/>.
        /// </summary>
        /// <param name="alarmManager">Manager of the alarm summary.</param>
        public AlarmHub(ILunaAlarmManager alarmManager)
        {
            ArgumentNullException.ThrowIfNull(alarmManager);

            _alarmManager = alarmManager;
        }


        /// <summary>
        /// Sends the current alarm summary to the connected client.
        /// </summary>
        /// <returns>Task.</returns>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.AlarmSummaryUpdated(_alarmManager.GetSummary());
        }
    }
}

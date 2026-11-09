// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using paskalON.Alarms.Dto.Summaries;
using paskalON.Luna.Application;
using paskalON.Luna.Service.WebApi.Hubs;

namespace paskalON.Luna.Service.Publishers
{
    /// <summary>
    /// Pushes the alarm summary to all UI clients connected to the <see cref="AlarmHub"/> every time it changed.
    /// </summary>
    /// <remarks>
    /// The change event only requests a push, the push runs in the loop of the service so that a slow client does not
    /// block the receiving of the alarm actions. Changes during a push are combined into one push of the latest summary.
    /// </remarks>
    public class AlarmSummaryPublisher : BackgroundService
    {
        /// <summary>
        /// Logger instance.
        /// </summary>
        private readonly ILogger<AlarmSummaryPublisher> _logger;


        /// <summary>
        /// Manager of the alarm summary.
        /// </summary>
        private readonly ILunaAlarmManager _alarmManager;


        /// <summary>
        /// Context of the alarm hub.
        /// </summary>
        private readonly IHubContext<AlarmHub, IAlarmClient> _hubContext;


        /// <summary>
        /// Requests a push, holds at most one pending request.
        /// </summary>
        private readonly Channel<bool> _pushRequests = Channel.CreateBounded<bool>(new BoundedChannelOptions(1)
        {
            FullMode = BoundedChannelFullMode.DropWrite,
            SingleReader = true
        });


        /// <summary>
        /// Constructor of <see cref="AlarmSummaryPublisher"/>.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="alarmManager">Manager of the alarm summary.</param>
        /// <param name="hubContext">Context of the alarm hub.</param>
        public AlarmSummaryPublisher(ILogger<AlarmSummaryPublisher> logger, ILunaAlarmManager alarmManager, IHubContext<AlarmHub, IAlarmClient> hubContext)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(alarmManager);
            ArgumentNullException.ThrowIfNull(hubContext);

            _logger = logger;
            _alarmManager = alarmManager;
            _hubContext = hubContext;
        }


        /// <summary>
        /// Pushes the latest summary after every change until the service stops.
        /// </summary>
        /// <param name="stoppingToken">Cancelled when the service stops.</param>
        /// <returns>Task.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _alarmManager.SummaryChanged += OnSummaryChanged;

            try
            {
                while (await _pushRequests.Reader.WaitToReadAsync(stoppingToken))
                {
                    _pushRequests.Reader.TryRead(out _);

                    try
                    {
                        await _hubContext.Clients.All.AlarmSummaryUpdated(_alarmManager.GetSummary());
                    }
                    catch (Exception exception) when (exception is not OperationCanceledException)
                    {
                        _logger.LogError(exception, "Pushing the alarm summary failed.");
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Expected when normal shutdown.
            }
            finally
            {
                _alarmManager.SummaryChanged -= OnSummaryChanged;
            }
        }


        /// <summary>
        /// Requests a push of the changed summary.
        /// </summary>
        private void OnSummaryChanged(object? sender, AlarmSummaryDto summary)
        {
            _pushRequests.Writer.TryWrite(true);
        }
    }
}

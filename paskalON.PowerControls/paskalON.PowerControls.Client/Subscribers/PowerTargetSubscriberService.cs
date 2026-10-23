// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Hosting;

namespace paskalON.PowerControls.Client.Subscribers
{
    /// <summary>
    /// Background service that handles the power targets received by the <see cref="IPowerTargetSubscriber"/>.
    /// </summary>
    public class PowerTargetSubscriberService : BackgroundService
    {
        /// <summary>
        /// Power target subscriber that receives and handles the power targets.
        /// </summary>
        private readonly IPowerTargetSubscriber _powerTargetSubscriber;


        /// <summary>
        /// Constructor of <see cref="PowerTargetSubscriberService"/>.
        /// </summary>
        /// <param name="powerTargetSubscriber">Power target subscriber that receives and handles the power targets.</param>
        public PowerTargetSubscriberService(IPowerTargetSubscriber powerTargetSubscriber)
        {
            ArgumentNullException.ThrowIfNull(powerTargetSubscriber);

            _powerTargetSubscriber = powerTargetSubscriber;
        }


        /// <inheritdoc/>
        /// <remarks>
        /// ExecuteAsync is called only after the application starts running (app.Run).
        /// </remarks>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _powerTargetSubscriber.RunAsync(stoppingToken);
        }
    }
}

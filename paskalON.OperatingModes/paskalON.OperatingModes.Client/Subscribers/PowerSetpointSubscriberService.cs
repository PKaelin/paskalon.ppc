// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Hosting;

namespace paskalON.OperatingModes.Client.Subscribers
{
    /// <summary>
    /// Background service that handles the power setpoints received by the <see cref="IPowerSetpointSubscriber"/>.
    /// </summary>
    public class PowerSetpointSubscriberService : BackgroundService
    {
        /// <summary>
        /// Subscriber that receives and handles power setpoint updates for this component.
        /// </summary>
        /// <remarks>Assigned during initialization and not modified thereafter.</remarks>
        private readonly IPowerSetpointSubscriber _powerSetpointSubscriber;


        /// <summary>
        /// Constructor of <see cref="PowerSetpointSubscriberService"/>.
        /// </summary>
        /// <param name="powerSetpointSubscriber">Subscriber that receives and handles power setpoint updates for this component.</param>
        public PowerSetpointSubscriberService(IPowerSetpointSubscriber powerSetpointSubscriber)
        {
            ArgumentNullException.ThrowIfNull(powerSetpointSubscriber);

            _powerSetpointSubscriber = powerSetpointSubscriber;
        }


        /// <inheritdoc/>
        /// <remarks>
        /// ExecuteAsync is called only after the application starts running (app.Run).
        /// </remarks>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _powerSetpointSubscriber.RunAsync(stoppingToken);
        }
    }
}

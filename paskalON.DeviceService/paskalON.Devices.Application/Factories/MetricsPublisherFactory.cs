// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.DependencyInjection;
using paskalON.Telemetry;

namespace paskalON.Devices.Application.Factories
{
    /// <summary>
    /// Metrics publisher factory.
    /// </summary>
    public class MetricsPublisherFactory : IMetricsPublisherFactory
    {
        /// <summary>
        /// Service provider interface.
        /// </summary>
        private readonly IServiceProvider _services;


        /// <summary>
        /// Constructor of <see cref="MetricsPublisherFactory"/>.
        /// </summary>
        /// <param name="services">Service provider interface.</param>
        public MetricsPublisherFactory(IServiceProvider services)
        {
            ArgumentNullException.ThrowIfNull(services);

            _services = services;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>        
        public IMetricsPublisher Create()
        {
            IMetricsPublisher publisher = _services.GetRequiredService<IMetricsPublisher>();

            return publisher;
        }
    }
}

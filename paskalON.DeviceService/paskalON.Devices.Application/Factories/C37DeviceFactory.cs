// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using paskalON.Dataface.C37s;
using paskalON.Devices.Domain.Configs;
using paskalON.Protocols.C37118;

namespace paskalON.Devices.Application.Factories
{
    /// <summary>
    /// C37 device factory.
    /// </summary>
    public class C37DeviceFactory : IC37DeviceFactory
    {
        /// <summary>
        /// Service provider interface.
        /// </summary>
        private readonly IServiceProvider _services;


        /// <summary>
        /// Constructor of <see cref="C37DeviceFactory"/>.
        /// </summary>
        /// <param name="services">Service provider interface.</param>
        public C37DeviceFactory(IServiceProvider services)
        {
            ArgumentNullException.ThrowIfNull(services);

            _services = services;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public (IC37Dataface Dataface, IC37Client Client) Create(C37Config config)
        {
            ArgumentNullException.ThrowIfNull(config);

            IC37Dataface dataface = new C37Register(config.Name);
            ILogger<C37Client> logger = _services.GetRequiredService<ILogger<C37Client>>();
            IC37Client client = new C37Client(logger, config.Address, config.Port);

            return (dataface, client);
        }
    }
}

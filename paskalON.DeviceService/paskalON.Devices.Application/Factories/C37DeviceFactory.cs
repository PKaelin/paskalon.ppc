// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using paskalON.Dataface.C37s;
using paskalON.Devices.Domain.Configs;
using paskalON.Protocols.C37118;
using paskalON.Protocols.C37118.Configs;
using System.Net.Sockets;

namespace paskalON.Devices.Application.Factories
{
    /// <summary>
    /// C37 device factory.
    /// </summary>
    public class C37DeviceFactory : IC37DeviceFactory
    {
        private readonly record struct C37ClientKey(AddressFamily AddressFamily, string Address, ushort Port);

        /// <summary>
        /// Service provider interface.
        /// </summary>
        private readonly IServiceProvider _services;


        /// <summary>
        /// Shared clients keyed by their C37 destination.
        /// </summary>
        private readonly Dictionary<C37ClientKey, IC37Client> _clients = new Dictionary<C37ClientKey, IC37Client>();


        /// <summary>
        /// Synchronizes shared client creation.
        /// </summary>
        private readonly object _clientLock = new object();


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
            C37ClientKey key = new C37ClientKey(config.AddressFamily, config.Address.ToUpperInvariant(), config.Port);
            IC37Client client;

            lock (_clientLock)
            {
                if (_clients.TryGetValue(key, out IC37Client? existingClient))
                {
                    client = existingClient;
                }
                else
                {
                    ILogger<C37Client> logger = _services.GetRequiredService<ILogger<C37Client>>();
                    ClientConnectionConfig connectionConfig = new ClientConnectionConfig
                    {
                        ServerAddress = config.Address,
                        ServerPort = config.Port,
                        AddressFamily = config.AddressFamily,
                        ConnectionTimeoutMilliseconds = config.C37ConnectionConfig.ConnectionTimeoutMilliseconds,
                        DisconnectionTimeoutMilliseconds = config.C37ConnectionConfig.DisconnectionTimeoutMilliseconds,
                        ConnectRetryCount = config.C37ConnectionConfig.ConnectRetryCount,
                        ConnectRetryIntervalMilliseconds = config.C37ConnectionConfig.ConnectRetryIntervalMilliseconds,
                        OperationTimeoutMilliseconds = config.C37ConnectionConfig.OperationTimeoutMilliseconds
                    };

                    client = new C37Client(logger, connectionConfig);
                    _clients.Add(key, client);
                }
            }

            return (dataface, client);
        }
    }
}

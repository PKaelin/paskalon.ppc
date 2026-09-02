// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.Protocols.Modbus;
using paskalON.Protocols.Modbus.Configs;
using paskalON.Protocols.Modbus.NModbus;

namespace paskalON.Devices.Application.Factories
{
    /// <summary>
    /// Modbus device factory.
    /// </summary>
    public sealed class ModbusDeviceFactory : IModbusDeviceFactory
    {
        /// <summary>
        /// Service provider interface.
        /// </summary>
        private readonly IServiceProvider _services;


        /// <summary>
        /// Constructor of <see cref="ModbusDeviceFactory"/>.
        /// </summary>
        /// <param name="services">Service provider interface.</param>
        public ModbusDeviceFactory(IServiceProvider services)
        {
            ArgumentNullException.ThrowIfNull(services);

            _services = services;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public (IModbusDataface Dataface, IModbusClient Client) Create(ModbusConfig config)
        {
            ArgumentNullException.ThrowIfNull(config);

            IModbusDataface dataface = new ModbusRegister(config.Name);
            ILogger<NModbusClient> logger = _services.GetRequiredService<ILogger<NModbusClient>>();
            ClientConnectionConfig connectionConfig = new ClientConnectionConfig
            {
                ServerAddress = config.Address,
                ServerPort = config.Port,
                AddressFamily = config.AddressFamily,
                ConnectionTimeoutMilliseconds = config.ModbusConnectionConfig.ConnectionTimeoutMilliseconds,
                DisconnectionTimeoutMilliseconds = config.ModbusConnectionConfig.DisconnectionTimeoutMilliseconds,
                ConnectRetryCount = config.ModbusConnectionConfig.ConnectRetryCount,
                ConnectRetryIntervalMilliseconds = config.ModbusConnectionConfig.ConnectRetryIntervalMilliseconds,
                OperationTimeoutMilliseconds = config.ModbusConnectionConfig.OperationTimeoutMilliseconds,
                SendRetryCount = config.ModbusConnectionConfig.SendRetryCount,
                SendRetryIntervalMilliseconds = config.ModbusConnectionConfig.SendRetryIntervalMilliseconds,
            };

            IModbusClient client = new NModbusClient(logger, connectionConfig, config.UnitId);

            return (dataface, client);
        }
    }
}

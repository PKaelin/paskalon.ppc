// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs;
using paskalON.Protocols.Modbus;

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
            ILogger<ModbusClient> logger = _services.GetRequiredService<ILogger<ModbusClient>>();
            IModbusClient client = new ModbusClient(logger, config.Address, config.Port);

            return (dataface, client);
        }
    }
}

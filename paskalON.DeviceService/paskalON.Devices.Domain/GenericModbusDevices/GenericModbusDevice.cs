// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.GenericModbusDevices.Entries;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.GenericModbusDevices
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Generic Modbus Device that communicates using the universal Modbus Protocol.
    /// </summary>
    public abstract class GenericModbusDevice : GenericModbusDeviceBase
    {
        /// <summary>
        /// Generic Modbus configuration.
        /// </summary>
        private readonly GenericModbusConfig _config;


        /// <summary>
        /// Constructor of <see cref="GenericModbusDevice"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The generic Modbus configuration.</param>
        /// <param name="genericModbusEntries">List of generic Modbus entries.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="dataface">The dataface interface.</param>
        public GenericModbusDevice(ILogger logger, GenericModbusConfig config, List<GenericModbusEntryBase> genericModbusEntries,
            IMetricsPublisher publisher, IDataface dataface) : base(logger, config, genericModbusEntries, publisher, dataface)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }
    }
}

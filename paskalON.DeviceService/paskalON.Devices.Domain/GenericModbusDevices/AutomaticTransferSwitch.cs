// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.GenericModbusDevices.Entries;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.GenericModbusDevices
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// An Automatic Transfer Switch (ATS) is an intelligent, self-acting device that shifts an electrical
    /// load between two power sources without requiring human intervention.
    /// </summary>
    public class AutomaticTransferSwitch : GenericModbusDeviceBase
    {
        /// <summary>
        /// Automatic transfer switch configuration.
        /// </summary>
        private readonly AutomaticTransferSwitchConfig _config;


        /// <summary>
        /// Constructor of <see cref="AutomaticTransferSwitch"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The automatic transfer switch configuration.</param>
        /// <param name="genericModbusEntries">List of generic Modbus entries.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="device">The device interface.</param>
        public AutomaticTransferSwitch(ILogger logger, AutomaticTransferSwitchConfig config, List<GenericModbusEntryBase> genericModbusEntries,
            IMetricsPublisher<AutomaticTransferSwitch> publisher, IGenericModbusDevice<AutomaticTransferSwitch> device)
            : base(logger, config, genericModbusEntries, (IMetricsPublisher<GenericModbusDeviceBase>)publisher, (IGenericModbusDevice<GenericModbusDeviceBase>)device)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }
    }
}

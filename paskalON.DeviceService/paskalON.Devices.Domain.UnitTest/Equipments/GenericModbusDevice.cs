// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface;
using paskalON.Devices.Domain.Configs.GenericModbusDevices;
using paskalON.Devices.Domain.GenericModbusDevices;
using paskalON.Devices.Domain.GenericModbusDevices.Entries;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.UnitTest.Equipments
{
    /// <summary>
    /// Test class for GenericModbusDevice tests.
    /// </summary>
    public class GenericModbusDevice : GenericModbusDeviceBase
    {
        public GenericModbusDevice(ILogger logger, GenericModbusBaseConfig config, List<GenericModbusEntryBase> genericModbusEntries, IMetricsPublisher publisher, IDataface dataface)
            : base(logger, config, genericModbusEntries, publisher, dataface)
        {
        }
    }
}

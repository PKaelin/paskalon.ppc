// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Equipments.PowerConversionSystems.Simples;
using paskalON.Protocols.Modbus;
using paskalON.Telemetry;

namespace paskalON.DeviceSimulator.Equipments.PowerConversionSystems.Simples
{
    public class PcsSimpleV1ProxySim : PcsSimpleV1Proxy
    {
        /// <summary>
        /// Modbus server for the simulation.
        /// </summary>
        public IModbusServer? ModbusServer { get; set; }

        public PcsSimpleV1ProxySim(ILogger logger, PowerConversionSystemConfig config, DerUnit derUnit, IMetricsPublisher publisher,
            IModbusDataface dataface, IModbusClient client) : base(logger, config, derUnit, publisher, dataface, client)
        {
        }

        protected override void RegisterMetrics()
        {
            // Do not register metrics
        }
    }
}

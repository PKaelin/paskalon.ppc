// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.PowerConversionSystems;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.UnitTest.Equipments
{
    /// <summary>
    /// Test class for PCS tests.
    /// </summary>
    public class Pcs : PowerConversionSystemBase
    {
        public int PcsDeviceTest { get; set; }


        // Do not test any communication in this tests. Do them in the Equipment tests.
        public Pcs(ILogger logger, PowerConversionSystemConfig config, DerUnit derUnit, IMetricsPublisher publisher, IModbusDataface dataface)
            : base(logger, config, derUnit, publisher, dataface)
        {
        }


        protected override void RegisterMetrics()
        {
            base.RegisterMetrics();
            MetricsPublisher.Register<Pcs, int>(this, nameof(PcsDeviceTest), MetricType.Gauge, x => x.PcsDeviceTest, _config.MetricsFactorClass1);
        }


        protected override void RegisterDataface()
        {
            Dataface.Register<Pcs, IModbusRegister>(r => r.Register<Pcs, double?>(this, nameof(ActivePower), (x, v) => x.ActivePowerValue = v, 1002, 1, ModbusDataType.MbInt16));
            Dataface.Register<Pcs, IModbusRegister>(r => r.Register<Pcs, double?>(this, nameof(ReactivePower), (x, v) => x.ReactivePowerValue = v, 1003, 1, ModbusDataType.MbInt16));
            Dataface.Register<Pcs, IModbusRegister>(r => r.Register<Pcs, double?>(this, nameof(DCCurrent), (x, v) => x.DCCurrent = v, 1004, 1, ModbusDataType.MbInt16));
            Dataface.Register<Pcs, IModbusRegister>(r => r.Register<Pcs, double?>(this, nameof(DCVoltage), (x, v) => x.DCVoltage = v, 1005, 1, ModbusDataType.MbInt16));
        }
    }
}
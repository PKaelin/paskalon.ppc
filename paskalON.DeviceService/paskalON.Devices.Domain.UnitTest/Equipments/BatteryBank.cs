// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyStorages.Batteries;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.UnitTest.Equipments
{
    /// <summary>
    /// Test class for BatteryBank tests.
    /// </summary>
    public class BatteryBank : BatteryBankBase
    {
        public int BatteryBankDeviceTest { get; set; }


        // Do not test any communication in this tests. Do them in the Equipment tests.
        public BatteryBank(ILogger logger, BatteryBankConfig config, DerBatteryStorageUnit derUnit, IMetricsPublisher publisher, IModbusDataface dataface)
            : base(logger, config, derUnit, publisher, dataface)
        {
        }


        protected override void RegisterMetrics()
        {
            base.RegisterMetrics();
            MetricsPublisher.Register<BatteryBank, int>(this, nameof(BatteryBankDeviceTest), MetricType.Gauge, x => x.BatteryBankDeviceTest, _config.MetricsFactorClass1);
        }


        protected override void RegisterDataface()
        {
            Dataface.Register<BatteryBank, IModbusRegister>(r => r.Register<BatteryBank, double?>(this, nameof(StateOfCharge), (x, v) => x.StateOfCharge = v, 1002, 1, ModbusDataType.MbInt16));
            Dataface.Register<BatteryBank, IModbusRegister>(r => r.Register<BatteryBank, double?>(this, nameof(StateOfHealth), (x, v) => x.StateOfHealth = v, 1003, 1, ModbusDataType.MbInt16));
            Dataface.Register<BatteryBank, IModbusRegister>(r => r.Register<BatteryBank, double?>(this, nameof(TotalDCVoltage), (x, v) => x.TotalDCVoltage = v, 1004, 1, ModbusDataType.MbInt16));
            Dataface.Register<BatteryBank, IModbusRegister>(r => r.Register<BatteryBank, double?>(this, nameof(TotalDCCurrent), (x, v) => x.TotalDCCurrent = v, 1005, 1, ModbusDataType.MbInt16));
        }
    }
}
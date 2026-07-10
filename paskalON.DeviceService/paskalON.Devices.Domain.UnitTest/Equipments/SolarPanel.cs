// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface.Modbus;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;
using paskalON.Devices.Domain.Ders;
using paskalON.Devices.Domain.EnergyResources.Solars;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.UnitTest.Equipments
{
    /// <summary>
    /// Test class for SolarPanel tests.
    /// </summary>
    public class SolarPanel : SolarPanelBase
    {
        public int SolarPanelDeviceTest { get; set; }


        // Do not test any communication in this tests. Do them in the Equipment tests.
        public SolarPanel(ILogger logger, SolarPanelConfig config, DerSolarUnit derUnit, IMetricsPublisher publisher, IModbusDataface dataface)
            : base(logger, config, derUnit, publisher, dataface)
        {
        }


        protected override void RegisterMetrics()
        {
            base.RegisterMetrics();
            MetricsPublisher.Register<SolarPanel, int>(this, nameof(SolarPanelDeviceTest), MetricType.Gauge, x => x.SolarPanelDeviceTest, _config.MetricsFactorClass1);
        }


        protected override void RegisterDataface()
        {
            Dataface.Register<SolarPanel, IModbusRegister>(r => r.Register<SolarPanel, int>(this, nameof(SolarPanelDeviceTest), (x, v) => x.SolarPanelDeviceTest = v, 1002, 1, ModbusDataType.MbInt16));
        }
    }
}
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


        /// <summary>
        /// Completed with the requested value when an active power target is set, e.g. by the SOC safety check in the background.
        /// </summary>
        public TaskCompletionSource<double?> ActivePowerTargetSet { get; set; } = new TaskCompletionSource<double?>(TaskCreationOptions.RunContinuationsAsynchronously);


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

        /// <inheritdoc/>
        public override async Task SetActivePowerTargetAsync(double? value)
        {
            await base.SetActivePowerTargetAsync(value);
            ActivePowerTargetSet.TrySetResult(value);
        }


        public override Task HeartbeatAsync(CancellationToken cancellationToken)
        {
            // Not required for unit tests.
            return Task.CompletedTask;
        }
    }
}
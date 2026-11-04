// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Meters.PowerMeters;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.UnitTest.Equipments
{
    /// <summary>
    /// Test class for PowerMeter tests.
    /// </summary>
    public class PowerMeter : PowerMeterBase
    {
        public PowerMeter(ILogger logger, PowerMeterBaseConfig config, IMetricsPublisher publisher, IDataface dataface)
            : base(logger, config, publisher, dataface)
        {
        }
    }
}

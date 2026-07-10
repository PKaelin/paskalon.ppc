// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Telemetry;

namespace paskalON.Devices.Domain.Meters.PowerMeters
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// System power meter measures the electrical power output at the point of interconnection (POI).
    /// </summary>
    /// <remarks>
    /// POI is where the plant connects to to the local electrical grid.
    /// </remarks>
    public abstract class SystemPowerMeter : PowerMeterBase
    {
        /// <summary>
        /// System power meter configuration.
        /// </summary>
        private readonly SystemPowerMeterConfig _config;


        /// <summary>
        /// Constructor of <see cref="SystemPowerMeter"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The system power meter config.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="dataface">The dataface interface.</param>
        public SystemPowerMeter(ILogger logger, SystemPowerMeterConfig config, IMetricsPublisher publisher, IDataface dataface)
            : base(logger, config, publisher, dataface)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }
    }
}

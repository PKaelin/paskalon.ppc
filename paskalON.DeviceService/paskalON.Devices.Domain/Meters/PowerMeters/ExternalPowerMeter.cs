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
    /// External power meter measures the electrical power output outside the POI.
    /// </summary>
    public abstract class ExternalPowerMeter : PowerMeterBase
    {
        /// <summary>
        /// External power meter configuration.
        /// </summary>
        private readonly ExternalPowerMeterConfig _config;


        /// <summary>
        /// Constructor of <see cref="ExternalPowerMeter"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The external power meter configuration.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="dataface">The dataface interface.</param>
        public ExternalPowerMeter(ILogger logger, ExternalPowerMeterConfig config, IMetricsPublisher publisher, IDataface dataface)
            : base(logger, config, publisher, dataface)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }
    }
}

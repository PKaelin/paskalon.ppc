// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
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
    /// Auxiliary power meter measures the electrical energy consumed by the plant's own supporting infrastructure.
    /// </summary>
    /// <remarks>
    /// Pumps & Fans: Boiler feed pumps, cooling water pumps, and forced-draft fans.
    /// Facility Utilities: Control room HVAC, lighting, and fire protection systems.
    /// Handling Systems: Coal conveyors, ash removal, and scrubbers.
    /// </remarks>
    public abstract class AuxiliaryPowerMeter : PowerMeterBase
    {
        /// <summary>
        /// Auxiliary power meter configuration.
        /// </summary>
        private readonly AuxiliaryPowerMeterConfig _config;


        /// <summary>
        /// Constructor of <see cref="AuxiliaryPowerMeter"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The auxiliary power meter configuration.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="dataface">The dataface interface.</param>
        public AuxiliaryPowerMeter(ILogger logger, AuxiliaryPowerMeterConfig config, IMetricsPublisher publisher, IDataface dataface)
            : base(logger, config, publisher, dataface)
        {
            ArgumentNullException.ThrowIfNull(config);

            _config = config;
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Dto.Ders;
using paskalON.OperatingModes.Application.Curves;
using paskalON.OperatingModes.Application.Ramps;
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.OpenModes;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.OpenModes;
using paskalON.OperatingModes.Domain.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Application.Factories
{
    /// <summary>
    /// Factory for creating operating mode instances based on the provided configuration and DER information.
    /// </summary>
    public class OperatingModeFactory : IOperatingModeFactory
    {
        /// <summary>
        /// The factory for creating operating mode maps based on the provided configuration and DER information.
        /// </summary>
        private readonly IOperatingModeMapFactory _mapFactory;


        /// <summary>
        /// Constructor for <see cref="OperatingModeFactory"/>.
        /// </summary>
        /// <param name="mapFactory">The factory for creating operating mode maps.</param>
        public OperatingModeFactory(IOperatingModeMapFactory mapFactory)
        {
            ArgumentNullException.ThrowIfNull(mapFactory);

            _mapFactory = mapFactory;
        }


        /// <inheritdoc/>
        public OperatingModeBase Create(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig,
            OperatingModeBaseConfig config, DerDto der)
        {
            OperatingModeBaseMap? map = _mapFactory.Create(config, der);
            IRampController rampController = new RampController(logger, timeProvider, config.RampConfig);
            ICurveController? curveController = null;

            if (config.CurveConfig != null)
            {
                // TODO: Implement
                curveController = new CurveController();
            }

            if (config is MaintenanceModeConfig)
            {
                MaintenanceModeMap maintenanceMap = map as MaintenanceModeMap ?? throw new ArgumentException("Operating mode map is not of type MaintenanceModeMap.");

                return new MaintenanceMode(logger, timeProvider, publisher, systemConfig, (MaintenanceModeConfig)config,
                    maintenanceMap, rampController, curveController);
            }

            if (config is ActivePowerFixedModeConfig)
            {
                ActivePowerFixedModeMap activePowerFixedMap = map as ActivePowerFixedModeMap ?? throw new ArgumentException("Operating mode map is not of type ActivePowerFixedModeMap.");

                return new ActivePowerFixedMode(logger, timeProvider, publisher, systemConfig, (ActivePowerFixedModeConfig)config, activePowerFixedMap, rampController, curveController);
            }

            if (config is ReactivePowerFixedModeConfig)
            {
                ReactivePowerFixedModeMap reactivePowerFixedMap = map as ReactivePowerFixedModeMap ?? throw new ArgumentException("Operating mode map is not of type ReactivePowerFixedModeMap.");

                return new ReactivePowerFixedMode(logger, timeProvider, publisher, systemConfig, (ReactivePowerFixedModeConfig)config, reactivePowerFixedMap, rampController, curveController);
            }
            else
            {
                throw new ArgumentException("Operating mode configuration not implemented yet.");
            }
        }
    }
}

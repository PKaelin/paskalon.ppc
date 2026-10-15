// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Dto.Ders;
using paskalON.OperatingModes.Application.Curves;
using paskalON.OperatingModes.Application.Ramps;
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Configs.OpenModes;
using paskalON.OperatingModes.Domain.Configs.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.Configs.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Curves;
using paskalON.OperatingModes.Domain.OpenModes;
using paskalON.OperatingModes.Domain.OpenModes.EnergyResources;
using paskalON.OperatingModes.Domain.OpenModes.FrequencyActives;
using paskalON.OperatingModes.Domain.OpenModes.VoltageReactives;
using paskalON.OperatingModes.Domain.Overrides;
using paskalON.OperatingModes.Domain.Ramps;
using paskalON.Telemetry;

namespace paskalON.OperatingModes.Application.Factories
{
    /// <summary>
    /// Factory that creates operating mode instances based on their configuration.
    /// </summary>
    public class OperatingModeFactory : IOperatingModeFactory
    {
        /// <summary>
        /// Factory that creates the input maps of the operating modes.
        /// </summary>
        private readonly IOperatingModeMapFactory _mapFactory;


        /// <summary>
        /// Constructor of <see cref="OperatingModeFactory"/>.
        /// </summary>
        /// <param name="mapFactory">Factory that creates the input maps of the operating modes.</param>
        public OperatingModeFactory(IOperatingModeMapFactory mapFactory)
        {
            ArgumentNullException.ThrowIfNull(mapFactory);

            _mapFactory = mapFactory;
        }


        /// <inheritdoc/>
        public OperatingModeBase Create(ILogger logger, TimeProvider timeProvider, IMetricsPublisher publisher, SystemConfig systemConfig,
            OperatingModeBaseConfig config, DerDto der, IUnitOverrideLookup overrides)
        {
            ArgumentNullException.ThrowIfNull(config);

            OperatingModeBaseMap? map = _mapFactory.Create(config, der, overrides);
            IRampController rampController = new RampController(logger, timeProvider, config.RampConfig);
            ICurveController? curveController = null;

            if (config.CurveConfig != null)
            {
                // TODO: Implement
                curveController = new CurveController();
            }

            switch (config)
            {
                case MaintenanceModeConfig maintenanceConfig:
                    return new MaintenanceMode(logger, timeProvider, publisher, systemConfig, maintenanceConfig, GetMap<MaintenanceModeMap>(map), rampController, curveController);

                case ActivePowerFixedModeConfig activePowerFixedConfig:
                    return new ActivePowerFixedMode(logger, timeProvider, publisher, systemConfig, activePowerFixedConfig, GetMap<ActivePowerFixedModeMap>(map), rampController, curveController);

                case ReactivePowerFixedModeConfig reactivePowerFixedConfig:
                    return new ReactivePowerFixedMode(logger, timeProvider, publisher, systemConfig, reactivePowerFixedConfig, GetMap<ReactivePowerFixedModeMap>(map), rampController, curveController);

                case MaximumPowerPointTrackingModeConfig maximumPowerPointTrackingConfig:
                    return new MaximumPowerPointTrackingMode(logger, timeProvider, publisher, systemConfig, maximumPowerPointTrackingConfig, GetMap<MaximumPowerPointTrackingModeMap>(map),
                        rampController, curveController);

                case ActivePowerModeConfig activePowerConfig:
                    return new ActivePowerMode(logger, timeProvider, publisher, systemConfig, activePowerConfig, GetMap<ActivePowerModeMap>(map), rampController, curveController);

                case MaximumActivePowerLimitModeConfig maximumActivePowerLimitConfig:
                    return new MaximumActivePowerLimitMode(logger, timeProvider, publisher, systemConfig, maximumActivePowerLimitConfig, GetMap<MaximumActivePowerLimitModeMap>(map),
                        rampController, curveController);

                case ReactivePowerModeConfig reactivePowerConfig:
                    return new ReactivePowerMode(logger, timeProvider, publisher, systemConfig, reactivePowerConfig, GetMap<ReactivePowerModeMap>(map), rampController, curveController);

                default:
                    throw new ArgumentException($"Operating mode configuration '{config.GetType().Name}' not implemented yet.");
            }
        }


        /// <summary>
        /// Gets the map as the expected map type.
        /// </summary>
        /// <typeparam name="TMap">The expected map type.</typeparam>
        /// <param name="map">The created map.</param>
        /// <returns>The typed map.</returns>
        /// <exception cref="ArgumentException">The map is not of the expected type.</exception>
        private static TMap GetMap<TMap>(OperatingModeBaseMap? map) where TMap : OperatingModeBaseMap
        {
            return map as TMap ?? throw new ArgumentException($"Operating mode map is not of type {typeof(TMap).Name}.");
        }
    }
}

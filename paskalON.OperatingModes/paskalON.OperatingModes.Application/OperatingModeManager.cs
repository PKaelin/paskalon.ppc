// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Devices.Client;
using paskalON.OperatingModes.Application.Factories;
using paskalON.OperatingModes.Application.Stacks;
using paskalON.OperatingModes.Domain;
using paskalON.OperatingModes.Domain.Configs;
using paskalON.OperatingModes.Domain.Systems;
using paskalON.OperatingModes.Infrastructure.Storage.Repositories;
using paskalON.Telemetry;
using paskalON.Telemetry.Factories;

namespace paskalON.OperatingModes.Application
{
    /// <summary>
    /// Operating mode manager responsible for managing systems operating modes and configurations.
    /// </summary>
    public class OperatingModeManager : IOperatingModeManager
    {
        /// <summary>
        /// Logger for application logging and diagnostics.
        /// </summary>
        private readonly ILogger<OperatingModeManager> _logger;


        /// <summary>
        /// Metrics publisher factory to create metrics publishers for telemetry.
        /// </summary>
        private readonly IMetricsPublisherFactory _metricsPublisherFactory;


        /// <summary>
        /// Device client to receive device DTOs from pub/sub.
        /// </summary>
        private readonly IDeviceClient _deviceClient;


        /// <summary>
        /// System operating mode control instance.
        /// </summary>
        private SystemOperatingModeControl? _systemOperatingModeControl;


        /// <summary>
        /// Operating mode factory to create operating mode instances based on configurations.
        /// </summary>
        private readonly IOperatingModeFactory _operatingModeFactory;


        /// <summary>
        /// Time provider to get the current time for operating mode operations.
        /// </summary>
        private readonly TimeProvider _timeProvider;


        /// <inheritdoc/>
        public ICollection<IMetricsPublisher> MetricsPublishers { get; } = new List<IMetricsPublisher>();


        /// <inheritdoc/>
        public IStackManager StackManager { get; } = new StackManager();


        public OperatingModeManager(ILogger<OperatingModeManager> logger, IDeviceClient deviceClient, IMetricsPublisherFactory metricsPublisherFactory,
            TimeProvider timeProvider, IOperatingModeFactory operatingModeFactory)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(deviceClient);
            ArgumentNullException.ThrowIfNull(metricsPublisherFactory);
            ArgumentNullException.ThrowIfNull(timeProvider);
            ArgumentNullException.ThrowIfNull(operatingModeFactory);

            _logger = logger;
            _deviceClient = deviceClient;
            _metricsPublisherFactory = metricsPublisherFactory;
            _operatingModeFactory = operatingModeFactory;
            _timeProvider = timeProvider;
        }



        public virtual async Task Initialize(IOperatingModeRepository repository, SystemConfig systemConfig)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(systemConfig);

            _logger.LogInformation("Initialize operating mode during startup");
            List<OperatingModeBaseConfig> operatingModeConfigs = await repository.GetAllOperatingModes();

            List<OperatingModeBase> operatingModes = new List<OperatingModeBase>();

            foreach (OperatingModeBaseConfig config in operatingModeConfigs)
            {
                IMetricsPublisher operatingModeMetrics = _metricsPublisherFactory.Create();
                OperatingModeBase operatingMode = _operatingModeFactory.Create(_logger, _timeProvider, operatingModeMetrics, systemConfig, config, _deviceClient.Der);
                operatingModes.Add(operatingMode);
                MetricsPublishers.Add(operatingModeMetrics);
            }

            StackManager.InitializeStack(operatingModes.ToDictionary(operatingMode => operatingMode.Name, StringComparer.Ordinal));
            IMetricsPublisher systemModeMetrics = _metricsPublisherFactory.Create();
            _systemOperatingModeControl = new SystemOperatingModeControl(_logger, operatingModes, systemModeMetrics);
            MetricsPublishers.Add(systemModeMetrics);
        }
    }
}

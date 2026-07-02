using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;
using paskalON.Devices.Domain.Ders;
using paskalON.Domains.Contracts;
using paskalON.Domains.Telemetry;

namespace paskalON.Devices.Domain.EnergyResources.Solars
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Solar panel base for the physical panel.
    /// </summary>
    /// <remarks>
    /// At the moment we dont integrate with solar panels.
    /// We still use a list in solar unit as we could have different brands and or types of solar panels.
    /// </remarks>
    public abstract class SolarPanelBase : DerDeviceBase<SolarPanelBase>
    {
        /// <summary>
        /// Solar panel configuration.
        /// </summary>
        private readonly SolarPanelConfig _config;


        /// <summary>
        /// Parent solar unit.
        /// </summary>
        public DerSolarUnit SolarUnit { get; private set; }


        /// <summary>
        /// Returns true if a communication error has occurred.
        /// </summary>
        /// <remarks>
        /// This is currently always false as we dont integrate with solar panels at this point.
        /// </remarks>
        public bool CommunicationError { get => false; }


        /// <summary>
        /// Flag whether this instance is in maintenance mode this is when the DER Unit is in maintenance mode.
        /// </summary>
        public bool IsInMaintenanceMode { get => SolarUnit.IsInMaintenanceMode; }


        /// <summary>
        /// Number of solar panels.
        /// </summary>
        public int NumberOfPanels { get => _config.NumberOfPanels; }


        /// <summary>
        /// Simulate list of solar panels by multiplying the solar device configured MinimumVoltageSum.
        /// </summary>
        public double MinimumVoltageSum { get => _config.MinimumVoltageSum; }


        /// <summary>
        /// Simulate list of solar panels by multiplying the solar device configured MaximumVoltageSum.
        /// </summary>
        public double MaximumVoltageSum { get => _config.MaximumVoltageSum; }


        /// <summary>
        /// Simulate list of solar panels by multiplying the solar device configured MinimumCurrentSum.
        /// </summary>
        public double MinimumCurrentSum { get => _config.MinimumCurrentSum; }


        /// <summary>
        /// Simulate list of solar panels by multiplying the solar device configured MaximumCurrentSum.
        /// </summary>
        public double MaximumCurrentSum { get => _config.MaximumCurrentSum; }



        /// <summary>
        /// Constructor of <see cref="SolarPanelBase"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The solar panel configuration.</param>
        /// <param name="derSolarUnit">The parent solar unit.</param>
        /// <param name="device">The device interface.</param>
        protected SolarPanelBase(ILogger logger, SolarPanelConfig config, DerSolarUnit derSolarUnit, ISolarPanel<SolarPanelBase> device)
            : base(logger, config, device)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(derSolarUnit);
            ArgumentNullException.ThrowIfNull(device);

            _config = config;
            SolarUnit = derSolarUnit;
            RegisterMetrics(device.MetricsPublisher);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void RegisterMetrics(IMetricsPublisher<SolarPanelBase> metricsPublisher)
        {
            metricsPublisher.Register<bool>(nameof(CommunicationError), x => x.CommunicationError, _config.MetricsFactorClass4);
            metricsPublisher.Register<bool>(nameof(IsInMaintenanceMode), x => x.IsInMaintenanceMode, _config.MetricsFactorClass4);
            metricsPublisher.Register<int>(nameof(NumberOfPanels), x => x.NumberOfPanels, _config.MetricsFactorClass4);
            metricsPublisher.Register<double>(nameof(MinimumVoltageSum), x => x.MinimumVoltageSum, _config.MetricsFactorClass4);
            metricsPublisher.Register<double>(nameof(MaximumVoltageSum), x => x.MaximumVoltageSum, _config.MetricsFactorClass4);
            metricsPublisher.Register<double>(nameof(MinimumCurrentSum), x => x.MinimumCurrentSum, _config.MetricsFactorClass4);
            metricsPublisher.Register<double>(nameof(MaximumCurrentSum), x => x.MaximumCurrentSum, _config.MetricsFactorClass4);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="dataface"></param>
        protected override void RegisterDataface(IDataface<SolarPanelBase> dataface)
        {
            // We dont communicate with solar panels at this point so we dont register any dataface properties.
        }

    }
}

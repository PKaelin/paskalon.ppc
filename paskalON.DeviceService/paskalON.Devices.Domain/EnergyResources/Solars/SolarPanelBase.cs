using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;
using paskalON.Devices.Domain.Ders;
using paskalON.Domains.Telemetry;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
    public abstract class SolarPanelBase : DerDeviceBase<SolarPanelBase>, ISolarPanel<SolarPanelBase>, INotifyPropertyChanged
    {
        /// <summary>
        /// Solar panel configuration.
        /// </summary>
        private readonly SolarPanelConfig _config;


        /// <summary>
        /// Solar panel device instance that communicates with the device.
        /// </summary>
        private readonly ISolarPanel<SolarPanelBase> _device;


        /// <summary>
        /// Event when the solar panel state <see cref="SolarPanelStateChangedEventArgs"/> changes.
        /// </summary>
        public event EventHandler<SolarPanelStateChangedEventArgs>? StateChanged;


        /// <summary>
        /// Event when the communication error state changed.
        /// </summary>
        public event EventHandler<CommunicationErrorChangedEventArgs>? CommunicationErrorChanged;


        /// <summary>
        /// Event when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;


        /// <summary>
        /// Parent solar unit.
        /// </summary>
        public DerSolarUnit SolarUnit { get; private set; }


        /// <summary>
        /// State of the solar panel.
        /// Specialized solar panel has to map its states to the these states.
        /// </summary>
        public SolarPanelState State
        {
            // At this point there is no communication with solar panels but we might with smart solar panels in the future.
            // Default value will always be false.
            get;
            set { if (field != value) { field = value; SetState(value); } else field = value; }
        }


        /// <summary>
        /// Returns true if a communication error has occurred.
        /// </summary>
        /// <remarks>
        /// This is currently always false as we dont integrate with solar panels at this point.
        /// </remarks>
        public bool CommunicationError
        {
            // At this point there is no communication with solar panels but we might with smart solar panels in the future.
            // Default value will always be false.
            get;
            set { if (field != value) { field = value; SetCommunicationError(value); } else field = value; }
        }


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
            _device = device;
            SolarUnit = derSolarUnit;
            RegisterMetrics(device.MetricsPublisher);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Connect()
        {
            _logger.LogInformation("{Name} connect requested.", Name);
            _device.Connect();
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Disconnect()
        {
            _logger.LogInformation("{Name} disconnect requested.", Name);
            _device.Disconnect();
        }


        /// <summary>
        /// Trigger GenericModbusDeviceState change events
        /// </summary>
        /// <param name="state">The GenericModbusDeviceState state.</param>
        protected void SetState(SolarPanelState state)
        {
            _logger.LogInformation("{Name} - SolarPanelState state changed to: {State}", Name, State);
            StateChanged?.Invoke(this, new SolarPanelStateChangedEventArgs(state));
        }


        /// <summary>
        /// Trigger CommunicationError change events.
        /// </summary>
        /// <param name="state">The communication error state.</param>
        protected void SetCommunicationError(bool state)
        {
            if (state == true)
            {
                _logger.LogError("{Name} - CommunicationError state changed to: {State}", Name, CommunicationError);
            }
            else
            {
                _logger.LogInformation("{Name} - CommunicationError state changed to: {State}", Name, CommunicationError);
            }

            CommunicationErrorChanged?.Invoke(this, new CommunicationErrorChangedEventArgs(state));
        }


        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property that changed. An empty value or null indicates that all of the
        /// properties have changed.
        /// </param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
    }
}

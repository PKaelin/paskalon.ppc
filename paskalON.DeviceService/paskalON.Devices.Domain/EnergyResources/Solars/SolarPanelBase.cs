// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;
using paskalON.Devices.Domain.Ders;
using paskalON.Telemetry;
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
    public abstract class SolarPanelBase : DerDeviceBase, ISolarPanel, INotifyPropertyChanged
    {
        /// <summary>
        /// Solar panel configuration.
        /// </summary>
        private readonly SolarPanelConfig _config;


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
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="dataface">The dataface interface.</param>
        protected SolarPanelBase(ILogger logger, SolarPanelConfig config, DerSolarUnit derSolarUnit, IMetricsPublisher publisher,
            IDataface dataface) : base(logger, config, publisher, dataface)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(derSolarUnit);
            ArgumentNullException.ThrowIfNull(publisher);

            _config = config;
            SolarUnit = derSolarUnit;

            RegisterMetrics();
            RegisterDataface();
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual async Task ConnectAsync()
        {
            _logger.LogInformation("{Name} connect requested.", Name);
            // We dont communicate at this point.
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual async Task DisconnectAsync()
        {
            _logger.LogInformation("{Name} disconnect requested.", Name);
            // We dont communicate at this point.
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void RegisterMetrics()
        {
            IEnumerable<KeyValuePair<string, object?>> tags = new Dictionary<string, object?>
            {
                { "Name", _config.Name },
                { "DeviceId", _config.DeviceId }
            };

            // Initialize metrics
            MetricsPublisher.Initialize("Solar", tags);
            // Solar
            MetricsPublisher.Register<SolarPanelBase, bool>(this, nameof(CommunicationError), MetricType.Gauge, x => x.CommunicationError, _config.MetricsFactorClass4);
            MetricsPublisher.Register<SolarPanelBase, bool>(this, nameof(IsInMaintenanceMode), MetricType.Gauge, x => x.IsInMaintenanceMode, _config.MetricsFactorClass4);
            MetricsPublisher.Register<SolarPanelBase, int>(this, nameof(NumberOfPanels), MetricType.Gauge, x => x.NumberOfPanels, _config.MetricsFactorClass4);
            MetricsPublisher.Register<SolarPanelBase, double>(this, nameof(MinimumVoltageSum), MetricType.Gauge, x => x.MinimumVoltageSum, _config.MetricsFactorClass4);
            MetricsPublisher.Register<SolarPanelBase, double>(this, nameof(MaximumVoltageSum), MetricType.Gauge, x => x.MaximumVoltageSum, _config.MetricsFactorClass4);
            MetricsPublisher.Register<SolarPanelBase, double>(this, nameof(MinimumCurrentSum), MetricType.Gauge, x => x.MinimumCurrentSum, _config.MetricsFactorClass4);
            MetricsPublisher.Register<SolarPanelBase, double>(this, nameof(MaximumCurrentSum), MetricType.Gauge, x => x.MaximumCurrentSum, _config.MetricsFactorClass4);
        }


        /// <summary>
        /// Trigger GenericModbusDeviceState change events
        /// </summary>
        /// <param name="state">The GenericModbusDeviceState state.</param>
        private void SetState(SolarPanelState state)
        {
            _logger.LogInformation("{Name} - SolarPanelState state changed to: {State}", Name, State);
            StateChanged?.Invoke(this, new SolarPanelStateChangedEventArgs(state));
        }


        /// <summary>
        /// Trigger CommunicationError change events.
        /// </summary>
        /// <param name="state">The communication error state.</param>
        private void SetCommunicationError(bool state)
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

    }
}

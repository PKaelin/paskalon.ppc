// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface;
using paskalON.Devices.Domain.Configs.EnergyStorages.Batteries;
using paskalON.Devices.Domain.Ders;
using paskalON.Telemetry;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace paskalON.Devices.Domain.EnergyStorages.Batteries
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Battery bank base for the physical battery bank.
    /// </summary>
    /// <remarks>
    /// Battery bank is a collection of individual batteries wired together in series or parallel to function as a single large-scale energy storage.
    /// </remarks>
    public abstract class BatteryBankBase : DerDeviceBase, IBatteryBank, INotifyPropertyChanged
    {
        /// <summary>
        /// Battery bank configuration.
        /// </summary>
        protected readonly BatteryBankConfig _config;


        /// <summary>
        /// This lock object needs to be used by this class and derived classes.
        /// The classes need to use the same lock for thread safety.
        /// </summary>
        protected object dataLock = new();


        /// <summary>
        /// Event when the battery bank state <see cref="BatteryBankState"/> changes.
        /// </summary>
        public event EventHandler<BatteryBankStateChangedEventArgs>? StateChanged;


        /// <summary>
        /// Event when the communication error state changed.
        /// </summary>
        public event EventHandler<CommunicationErrorChangedEventArgs>? CommunicationErrorChanged;


        /// <summary>
        /// Event when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;


        /// <summary>
        /// Parent battery storage unit.
        /// </summary>
        public DerBatteryStorageUnit BatteryStorageUnit { get; private set; }


        /// <summary>
        /// Battery bank state.
        /// Specialized battery bank has to map its states to the these states.
        /// </summary>
        public BatteryBankState State
        {
            get;
            set { if (field != value) { field = value; SetState(value); } else field = value; }
        }


        /// <summary>
        /// Returns true if a communication error has occurred.
        /// </summary>
        public bool CommunicationError
        {
            get;
            set { if (field != value) { field = value; SetCommunicationError(value); } else field = value; }
        }


        /// <summary>
        /// Flag whether this instance is in maintenance mode this is when the DER Unit is in maintenance mode.
        /// </summary>
        public bool IsInMaintenanceMode { get => BatteryStorageUnit.IsInMaintenanceMode; }


        /// <summary>
        /// Indicates whether the battery bank is initially connected or not.
        /// </summary>
        public bool InitiallyConnected { get => _config.InitiallyConnected; }


        /// <summary>
        /// NameplateCapacity in watt hours.
        /// </summary>
        public double NameplateCapacity { get => _config.BatteryBankDeviceConfig.NameplateCapacity; }


        /// <summary>
        /// NameplateMaximumChargeRate in watts.
        /// </summary>
        public double NameplateMaximumChargeRate { get => _config.BatteryBankDeviceConfig.NameplateMaximumChargeRate; }


        /// <summary>
        /// NameplateMaximumDischargeRate in watts.
        /// </summary>
        public double NameplateMaximumDischargeRate { get => _config.BatteryBankDeviceConfig.NameplateMaximumDischargeRate; }


        /// <summary>
        /// Count of racks also know as segments of the battery.
        /// </summary>
        public int RackCount { get => _config.BatteryBankDeviceConfig.RackCount; }


        /// <summary>
        /// Count of strings per rack.
        /// </summary>
        public int StringsPerRackCount { get => _config.BatteryBankDeviceConfig.StringsPerRackCount; }


        /// <summary>
        /// Which inverter BUS the battery is connected to.
        /// This is used to write the batteries maximum and minimum currents that are allowed to the PCS.
        /// </summary>
        public int InverterBusNumber { get => _config.BatteryBankDeviceConfig.InverterBusNumber; }


        /// <summary>
        /// A strict lower bound on how far the IC is allowed to discharge the battery.
        /// Expressed as a percentage of actual capacity, not usable capacity.
        /// </summary>
        public double AbsoluteMinimumStateOfCharge { get => _config.BatteryBankDeviceConfig.AbsoluteMinimumStateOfCharge; }


        /// <summary>
        /// A strict upper bound on how far the IC is allowed to discharge the battery.
        /// Expressed as a percentage of actual capacity, not usable capacity.
        /// </summary>
        public double AbsoluteMaximumStateOfCharge { get => _config.BatteryBankDeviceConfig.AbsoluteMaximumStateOfCharge; }


        /// <summary>
        /// The absolute minimum temperature the battery can operate at.
        /// </summary>
        public double AbsoluteMinimumTemperature { get => _config.BatteryBankDeviceConfig.AbsoluteMinimumTemperature; }


        /// <summary>
        /// The absolute maximum temperature the battery can operate at.
        /// </summary>
        public double AbsoluteMaximumTemperature { get => _config.BatteryBankDeviceConfig.AbsoluteMaximumTemperature; }


        /// <summary>
        /// The preferred minimum state of charge, as a percentage of usable capacity.
        /// </summary>
        public double PreferredMinimumStateOfCharge { get => _config.BatteryBankDeviceConfig.PreferredMinimumStateOfCharge; }


        /// <summary>
        /// The preferred maximum state of charge, as a percentage of usable capacity.
        /// </summary>
        public double PreferredMaximumStateOfCharge { get => _config.BatteryBankDeviceConfig.PreferredMaximumStateOfCharge; }


        /// <summary>
        /// The preferred minimum temperature the battery can operate at.
        /// </summary>
        public double PreferredMinimumTemperature { get => _config.BatteryBankDeviceConfig.PreferredMinimumTemperature; }


        /// <summary>
        /// The preferred maximum temperature the battery can operate at.
        /// </summary>
        public double PreferredMaximumTemperature { get => _config.BatteryBankDeviceConfig.PreferredMaximumTemperature; }


        /// <summary>
        /// Expected maximum current (i.e. the absolute physical limit) the battery could produce.
        /// </summary>
        public double AbsoluteMaxDischargeCurrentAmps { get => _config.BatteryBankDeviceConfig.AbsoluteMaxDischargeCurrentAmps; }


        /// <summary>
        /// Expected minimum current (i.e. the absolute physical limit) the battery could produce.
        /// </summary>
        public double AbsoluteMaxChargeCurrentAmps { get => _config.BatteryBankDeviceConfig.AbsoluteMaxChargeCurrentAmps; }


        /// <summary>
        /// The minimum DC voltage the battery can operate at.
        /// </summary>
        public double MinimumDcVoltage { get => _config.BatteryBankDeviceConfig.MinimumDcVoltage; }


        /// <summary>
        /// The maximum DC voltage the battery can operate at.
        /// </summary>
        public double MaximumDcVoltage { get => _config.BatteryBankDeviceConfig.MaximumDcVoltage; }


        /// <summary>
        /// Configured value determining whether the proxy should report 0 capability in the event of communication loss.
        /// </summary>
        public bool ZeroCapacityOnCommLoss { get => _config.BatteryBankDeviceConfig.ZeroCapacityOnCommLoss; }


        /// <summary>
        /// The flow direction of the battery bank, indicating whether it is charging, discharging, or idle.
        /// </summary>
        /// <remarks>
        /// Calculated by the direction of current and a deadband.
        /// </remarks>
        public BatteryBankFlowDirection? BatteryBankFlowDirection
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        // TODO: Return the usable state of charge if configured else the actual state of charge.
        /// <summary>
        /// Usable state of charge as a percentage of the battery's capacity.        
        /// </summary>
        /// <remarks>StateOfCharge is as a floating point value between 0 and 100.</remarks>
        public double? StateOfCharge
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Actual state of charge as a percentage of the battery's actual capacity rather than its usable capacity.
        /// </summary>
        /// <remarks>ActualStateOfCharge is as a floating point value between 0 and 100.</remarks>
        public double? ActualStateOfCharge
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// State of health of the battery bank as a percentage of capacity / nameplate capacity.
        /// </summary>
        public double? StateOfHealth
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// DC bus voltage of the battery bank.
        /// </summary>
        public double? TotalDCVoltage
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// DC bus current of the battery bank.
        /// </summary>
        public double? TotalDCCurrent
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Minimum measured cell voltage of the battery bank.
        /// </summary>
        public double? MinimumCellVoltage
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Maximum measured cell voltage of the battery bank.
        /// </summary>
        public double? MaximumCellVoltage
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }

        /// <summary>
        /// Minimum rack temperature of the battery bank.
        /// </summary>
        public double? MinimumRackTemperature
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Maximum rack temperature of the battery bank.
        /// </summary>
        public double? MaximumRackTemperature
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Average rack temperature.
        /// </summary>
        public double? AverageRackTemperature
        {
            get => MinimumRackTemperature.HasValue && MaximumRackTemperature.HasValue ? (MinimumRackTemperature + MaximumRackTemperature) / 2 : null;
        }


        /// <summary>
        /// Minimum string temperature of the battery bank.
        /// </summary>
        public double? MinimumStringTemperature
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Maximum string temperature of the battery bank.
        /// </summary>
        public double? MaximumStringTemperature
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Average string temperature.
        /// </summary>
        public double? AverageStringTemperature
        {
            get => MinimumStringTemperature.HasValue && MaximumStringTemperature.HasValue ? (MinimumStringTemperature + MaximumStringTemperature) / 2 : null;
        }



        /// <summary>
        /// Constructor of <see cref="BatteryBankBase"/>.
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The battery bank configuration.</param>
        /// <param name="batteryStorageUnit">The paren battery storage unit.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="dataface">The dataface interface.</param>
        protected BatteryBankBase(ILogger logger, BatteryBankConfig config, DerBatteryStorageUnit batteryStorageUnit, IMetricsPublisher publisher,
            IDataface dataface) : base(logger, config, publisher, dataface)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(batteryStorageUnit);
            ArgumentNullException.ThrowIfNull(publisher);

            _config = config;
            BatteryStorageUnit = batteryStorageUnit;

            RegisterMetrics();
            RegisterDataface();
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual async Task ConnectAsync()
        {
            _logger.LogInformation("{Name} connect requested.", Name);
            State = BatteryBankState.Connecting;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual async Task DisconnectAsync()
        {
            _logger.LogInformation("{Name} disconnect requested.", Name);
            State = BatteryBankState.Disconnecting;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async virtual Task CheckHealthAsync()
        {
            // TODO: Implement state check, data received check and com error update if necessary.
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
            MetricsPublisher.Initialize("BMS", tags);
            // MetricsFactorClass1
            MetricsPublisher.Register<BatteryBankBase, bool>(this, nameof(CommunicationError), MetricType.Gauge, x => x.CommunicationError, _config.MetricsFactorClass1);
            MetricsPublisher.Register<BatteryBankBase, double>(this, nameof(TotalDCVoltage), MetricType.Gauge, x => x.TotalDCVoltage, _config.MetricsFactorClass1);
            MetricsPublisher.Register<BatteryBankBase, double>(this, nameof(TotalDCCurrent), MetricType.Gauge, x => x.TotalDCCurrent, _config.MetricsFactorClass1);
            MetricsPublisher.Register<BatteryBankBase, double>(this, nameof(StateOfCharge), MetricType.Gauge, x => x.StateOfCharge, _config.MetricsFactorClass1);
            MetricsPublisher.Register<BatteryBankBase, double>(this, nameof(ActualStateOfCharge), MetricType.Gauge, x => x.ActualStateOfCharge, _config.MetricsFactorClass1);
            MetricsPublisher.Register<BatteryBankBase, double>(this, nameof(StateOfHealth), MetricType.Gauge, x => x.StateOfHealth, _config.MetricsFactorClass1);
            // MetricsFactorClass2
            MetricsPublisher.Register<BatteryBankBase, BatteryBankState>(this, nameof(State), MetricType.Gauge, x => x.State, _config.MetricsFactorClass2);
            MetricsPublisher.Register<BatteryBankBase, double>(this, nameof(MinimumCellVoltage), MetricType.Gauge, x => x.MinimumCellVoltage, _config.MetricsFactorClass1);
            MetricsPublisher.Register<BatteryBankBase, double>(this, nameof(MaximumCellVoltage), MetricType.Gauge, x => x.MaximumCellVoltage, _config.MetricsFactorClass1);
            MetricsPublisher.Register<BatteryBankBase, double>(this, nameof(MinimumRackTemperature), MetricType.Gauge, x => x.MinimumRackTemperature, _config.MetricsFactorClass1);
            MetricsPublisher.Register<BatteryBankBase, double>(this, nameof(MaximumRackTemperature), MetricType.Gauge, x => x.MaximumRackTemperature, _config.MetricsFactorClass1);
            MetricsPublisher.Register<BatteryBankBase, double>(this, nameof(MinimumStringTemperature), MetricType.Gauge, x => x.MinimumStringTemperature, _config.MetricsFactorClass1);
            MetricsPublisher.Register<BatteryBankBase, double>(this, nameof(MaximumStringTemperature), MetricType.Gauge, x => x.MaximumStringTemperature, _config.MetricsFactorClass1);
            // MetricsFactorClass3
            MetricsPublisher.Register<BatteryBankBase, bool>(this, nameof(IsInMaintenanceMode), MetricType.Gauge, x => x.IsInMaintenanceMode, _config.MetricsFactorClass3);
            // MetricsFactorClass4
        }


        /// <summary>
        /// Triggers battery banks state changed event.
        /// </summary>
        /// <param name="state">The new battery bank state.</param>
        private void SetState(BatteryBankState state)
        {
            _logger.LogInformation("{Name} - BatteryBank state changed to {State}", Name, State);
            StateChanged?.Invoke(this, new BatteryBankStateChangedEventArgs(state));
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

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Units = paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.Devices.Domain.PowerConversionSystems
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Power Conversion System (PCS) base class for all PCSs.
    /// </summary>
    public abstract class PowerConversionSystemBase : DerDeviceBase, IPowerConversionSystem, INotifyPropertyChanged
    {
        /// <summary>
        /// Power conversion system configuration of this instance.
        /// </summary>
        protected readonly PowerConversionSystemConfig _config;


        /// <summary>
        /// This lock object needs to be used by this class and derived classes.
        /// The classes need to use the same lock for thread safety.
        /// </summary>
        protected object dataLock = new();


        /// <summary>
        /// Event when the Power Conversion System state <see cref="PcsState"/> changes.
        /// </summary>
        public event EventHandler<PcsStateChangedEventArgs>? StateChanged;


        /// <summary>
        /// Event when the communication error state changed.
        /// </summary>
        public event EventHandler<CommunicationErrorChangedEventArgs>? CommunicationErrorChanged;


        /// <summary>
        /// Event when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;


        /// <summary>
        /// Parent Distributed Energy Resource unit (DER-Unit).
        /// </summary>
        public DerUnit DerUnit { get; set; }


        /// <summary>
        /// State of the Power Conversion System (PCS).
        /// Specialized PCS has to map its states to the these states.
        /// </summary>
        public PcsState State
        {
            get;
            set { if (field != value) { field = value; SetState(value); } }
        }


        /// <summary>
        /// Returns true if a communication error has occurred.
        /// </summary>
        public bool CommunicationError
        {
            get;
            set { if (field != value) { field = value; _ = SetCommunicationError(value); } }
        }


        /// <summary>
        /// Flag whether this PCS is initially started or not.
        /// </summary>
        public bool InitiallyStarted { get => _config.InitiallyStarted; }


        /// <summary>
        /// Nameplate maximum active power rating.
        /// </summary>
        public ActivePower NameplateMaximumActivePower { get => new ActivePower(_config.PowerConversionSystemDeviceConfig.NameplateMaximumActivePower); }


        /// <summary>
        /// Nameplate maximum reactive power rating.
        /// </summary>
        public ReactivePower NameplateMaximumReactivePower { get => new ReactivePower(_config.PowerConversionSystemDeviceConfig.NameplateMaximumActivePower); }


        /// <summary>
        /// Nameplate maximum apparent power rating.
        /// </summary>
        public ApparentPower NameplateMaximumApparentPower { get => new ApparentPower(_config.PowerConversionSystemDeviceConfig.NameplateMaximumApparentPower); }


        /// <summary>
        /// Theoretical maximum AC current output.
        /// </summary>
        public double NameplateMaximumACCurrent { get => _config.PowerConversionSystemDeviceConfig.NameplateMaximumACCurrent; }


        /// <summary>
        /// Theoretical minimum DC voltage output.
        /// </summary>
        public double MinimumDCVoltage { get => _config.PowerConversionSystemDeviceConfig.MinimumDCVoltage; }


        /// <summary>
        /// Theoretical maximum DC voltage output.
        /// </summary>
        public double MaximumDCVoltage { get => _config.PowerConversionSystemDeviceConfig.MaximumDCVoltage; }


        /// <summary>
        /// Configured value determining whether the proxy should report 0 real and reactive power in the event of communication loss.
        /// </summary>        
        public bool ZeroOutputOnCommLoss { get => _config.PowerConversionSystemDeviceConfig.ZeroOutputOnCommLoss; }


        /// <summary>
        /// Configured minimum active power that the PCS should output when in standby mode.
        /// </summary>
        public double StandbyActivePowerKiloWatts { get => _config.PowerConversionSystemDeviceConfig.StandbyActivePowerKiloWatts; }


        /// <summary>
        /// Flag whether this instance is in maintenance mode this is when the DER Unit is in maintenance mode.
        /// </summary>
        public bool IsInMaintenanceMode { get => DerUnit.IsInMaintenanceMode; }


        private double? _activePowerTarget;
        /// <summary>
        /// Current active power target in Watts
        /// </summary>
        public ActivePower? ActivePowerTarget
        {
            get { lock (dataLock) { return (_activePowerTarget is null) ? null : new ActivePower((double)_activePowerTarget); } }
        }


        /// <summary>
        /// Current active power output value in Watts
        /// </summary>
        public double? ActivePowerValue
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Current active power output in Watts
        /// </summary>
        public ActivePower? ActivePower
        {
            get { return (ActivePowerValue is null) ? null : new ActivePower((double)ActivePowerValue); }
        }


        private double? _reactivePowerTarget;
        /// <summary>
        /// Current reactive power target in Vars
        /// </summary>
        public ReactivePower? ReactivePowerTarget
        {
            get { lock (dataLock) { return (_reactivePowerTarget is null) ? null : new ReactivePower((double)_reactivePowerTarget); } }
        }


        /// <summary>
        /// Current reactive power output value in Vars
        /// </summary>
        public double? ReactivePowerValue
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Current reactive power output in Vars
        /// </summary>
        public ReactivePower? ReactivePower
        {
            get { lock (dataLock) { return (ReactivePowerValue is null) ? null : new ReactivePower((double)ReactivePowerValue); } }
        }


        /// <summary>
        /// Apparent power output.
        /// </summary>
        public ApparentPower? ApparentPower
        {
            get
            {
                lock (dataLock) { return Units.ApparentPower.OrthogonalSum(ActivePower, ReactivePower); }
            }
        }


        /// <summary>
        /// Frequency in hertz.
        /// </summary>
        public double? Frequency
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// DC Current or calculated DC Current
        /// </summary>
        /// </remarks>
        public double? DCCurrent
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// DC Voltage or calculated DC Voltage.
        /// </summary>      
        public double? DCVoltage
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// AC Current or calculated AC Current
        /// </summary>
        public double? ACCurrent
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// AC Voltage or calculated AC Voltage
        /// </summary>
        public double? ACVoltage
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Flag whether the AC breaker is closed.
        /// An AC breaker will open on overcurrent (usually settable) or when voltages are down a minimum.
        /// </summary>
        public bool? IsACBreakerClosed
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Array of flags whether the DC contactors are closed DC contactor will open/close via external command.
        /// </summary>
        public bool[]? IsDcContactorClosed
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Contains fault definitions and their states.
        /// </summary>
        /// <remarks>
        /// Fault states indicate a critical failure that requires the system to shut down immediately to prevent damage.
        /// Do not set these states directly us <see cref="SetFault(string, bool)"/> method instead.
        /// </remarks>
        public Dictionary<string, bool> FaultStates { get; } = new Dictionary<string, bool>();


        /// <summary>
        /// Indicates whether there are any active alarms.
        /// </summary>
        public bool HasActiveFaults { get => FaultStates.Any(a => a.Value == true); }


        /// <summary>
        /// Contains warning definitions and their states.
        /// </summary>
        /// <remarks>
        /// Warning states indicate that the system is operating outside its optimal range but can continue running.
        /// Do not set these states directly us <see cref="SetWarning(string, bool)"/> method instead.
        /// </remarks>
        public Dictionary<string, bool> WarningStates { get; } = new Dictionary<string, bool>();


        /// <summary>
        /// Indicates whether there are any active warnings.
        /// </summary>
        public bool HasActiveWarnings { get => WarningStates.Any(a => a.Value == true); }


        /// <summary>
        /// Contains vendors event definitions and their states.
        /// </summary>
        /// <remarks>
        /// Vendor events can are vendor specific events like maintenance due etc.
        /// Do not set these states directly us <see cref="SetVendorEvent(string, bool)"/> method instead.
        /// </remarks>
        public Dictionary<string, bool> VendorEvents { get; } = new Dictionary<string, bool>();


        /// <summary>
        /// Indicates whether there are any vendor events.
        /// </summary>
        public bool HasVendorEvents { get => WarningStates.Any(a => a.Value == true); }


        /// <summary>
        /// Constructor of <see cref="PowerConversionSystemBase"/>
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The power conversion system configuration.</param>
        /// <param name="derUnit">The parent DER unit.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="dataface">The dataface interface.</param>
        public PowerConversionSystemBase(ILogger logger, PowerConversionSystemConfig config, DerUnit derUnit, IMetricsPublisher publisher,
            IDataface dataface) : base(logger, config, publisher, dataface)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(derUnit);

            _config = config;
            DerUnit = derUnit;
            RegisterMetrics();
            RegisterDataface();
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual async Task StartAsync()
        {
            _logger.LogInformation("{Name} start requested.", Name);
            State = PcsState.Starting;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual async Task StopAsync()
        {
            _logger.LogInformation("{Name} stop requested.", Name);
            State = PcsState.Stopping;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual async Task StandbyAsync(double? standbyActivePower = null)
        {
            _logger.LogInformation("{Name} standby requested with standby active power: {StandbyActivePower}.", Name, standbyActivePower ?? StandbyActivePowerKiloWatts);
            State = PcsState.EnteringStandby;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual async Task SetActivePowerTargetAsync(double? value)
        {
            lock (dataLock)
            {
                if (_activePowerTarget != value)
                {
                    _activePowerTarget = value;
                    _logger.LogInformation("{Name} - Set active power target to: {activePowerTarget}", Name, _activePowerTarget);
                }
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value">Reactive power value (VArs).</param>
        public virtual async Task SetReactivePowerTargetAsync(double? value)
        {
            lock (dataLock)
            {
                if (_reactivePowerTarget != value)
                {
                    _reactivePowerTarget = value;
                    _logger.LogInformation("{Name} - Set reactive power target to: {reactivePowerTarget}", Name, _reactivePowerTarget);
                }
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async virtual Task CheckHealthAsync()
        {
            // TODO: Implement state check, data received check and com error update if necessary.
        }


        /// <summary>
        /// Trigger PCS state change events.
        /// </summary>
        /// <param name="state">The PCS state.</param>
        private void SetState(PcsState state)
        {
            _logger.LogInformation("{Name} - PCS state changed to: {State}", Name, State);
            StateChanged?.Invoke(this, new PcsStateChangedEventArgs(state));
        }


        /// <summary>
        /// Trigger CommunicationError change events.
        /// </summary>
        /// <param name="state">The communication error state.</param>
        private async Task SetCommunicationError(bool state)
        {
            if (state == true)
            {
                if (ZeroOutputOnCommLoss == true)
                {
                    ActivePowerValue = 0;
                    ReactivePowerValue = 0;
                    lock (dataLock)
                    {
                        _activePowerTarget = 0;
                        _reactivePowerTarget = 0;
                    }
                    _logger.LogInformation("{Name} - Set power targets to 0 due as ZeroOutputOnCommLoss is true on CommunicationError", Name);
                    // Desperate attempt in case there is still a connection.
                    await SetActivePowerTargetAsync(0);
                    await SetReactivePowerTargetAsync(0);
                }

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
        /// Sets a fault.
        /// </summary>
        /// <param name="name">Name of the alarm.</param>
        /// <param name="state">State of the alarm.</param>
        protected void SetFault(string name, bool state)
        {
            // TODO: clean out if state is false and havent been updated for a while.
            if (FaultStates.ContainsKey(name) == false || FaultStates[name] != state)
            {
                if (state == true)
                {
                    _logger.LogError("{Name} Fault state occurred. Fault: {Fault}", Name, name);
                }
                else
                {
                    _logger.LogInformation("{Name} Fault state changed to normal. Fault: {Fault}", Name, name);
                }
            }

            FaultStates[name] = state;
        }


        /// <summary>
        /// Sets a warning.
        /// </summary>
        /// <param name="name">Name of the warning.</param>
        /// <param name="state">State of the warning.</param>
        protected void SetWarning(string name, bool state)
        {
            // TODO: clean out if state is false and havent been updated for a while.
            if (WarningStates.ContainsKey(name) == false || WarningStates[name] != state)
            {
                if (state == true)
                {
                    _logger.LogWarning("{Name} Warning state occurred. Warning: {Warning}", Name, name);
                }
                else
                {
                    _logger.LogInformation("{Name} Warning state changed to normal. Warning: {Warning}", Name, name);
                }
            }

            WarningStates[name] = state;
        }


        /// <summary>
        /// Sets a vendor event.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="vendorEvents">Vendor event.</param>
        protected void SetVendorEvent(string name, bool vendorEvents)
        {
            // TODO: clean out if state is false and havent been updated for a while.
            if (VendorEvents.ContainsKey(name) == false || VendorEvents[name] != vendorEvents)
            {
                if (vendorEvents == true)
                {
                    _logger.LogWarning("{Name} Vendor event occurred. Event: {Event}", Name, name);
                }
                else
                {
                    _logger.LogInformation("{Name} Vendor event changed to normal. Event: {Event}", Name, name);
                }
            }

            VendorEvents[name] = vendorEvents;
        }


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
            MetricsPublisher.Initialize("PCS", tags);
            // MetricsFactorClass1
            MetricsPublisher.Register<PowerConversionSystemBase, bool>(this, nameof(CommunicationError), MetricType.Gauge, x => x.CommunicationError, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerConversionSystemBase, double>(this, nameof(ActivePowerTarget), MetricType.Gauge, x => x.ActivePowerTarget?.Watts, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerConversionSystemBase, double>(this, nameof(ReactivePowerTarget), MetricType.Gauge, x => x.ReactivePowerTarget?.VoltAmperesReactive, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerConversionSystemBase, double>(this, nameof(ActivePower), MetricType.Gauge, x => x.ActivePower?.Watts, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerConversionSystemBase, double>(this, nameof(ReactivePower), MetricType.Gauge, x => x.ReactivePower?.VoltAmperesReactive, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerConversionSystemBase, double>(this, nameof(DCCurrent), MetricType.Gauge, x => x.DCCurrent, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerConversionSystemBase, double>(this, nameof(DCVoltage), MetricType.Gauge, x => x.DCVoltage, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerConversionSystemBase, double>(this, nameof(ACCurrent), MetricType.Gauge, x => x.ACCurrent, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerConversionSystemBase, double>(this, nameof(ACVoltage), MetricType.Gauge, x => x.ACVoltage, _config.MetricsFactorClass1);
            // MetricsFactorClass2
            MetricsPublisher.Register<PowerConversionSystemBase, PcsState>(this, nameof(State), MetricType.Gauge, x => x.State, _config.MetricsFactorClass2);
            MetricsPublisher.Register<PowerConversionSystemBase, bool>(this, nameof(HasActiveFaults), MetricType.Gauge, x => x.HasActiveFaults, _config.MetricsFactorClass2);
            MetricsPublisher.Register<PowerConversionSystemBase, bool>(this, nameof(HasActiveWarnings), MetricType.Gauge, x => x.HasActiveWarnings, _config.MetricsFactorClass2);
            // MetricsFactorClass3
            MetricsPublisher.Register<PowerConversionSystemBase, bool>(this, nameof(IsInMaintenanceMode), MetricType.Gauge, x => x.IsInMaintenanceMode, _config.MetricsFactorClass3);
            // MetricsFactorClass4
            MetricsPublisher.Register<PowerConversionSystemBase, double>(this, nameof(Frequency), MetricType.Gauge, x => x.Frequency, _config.MetricsFactorClass4);
            MetricsPublisher.Register<PowerConversionSystemBase, double>(this, nameof(StandbyActivePowerKiloWatts), MetricType.Gauge, x => x.StandbyActivePowerKiloWatts, _config.MetricsFactorClass4);
        }
    }
}

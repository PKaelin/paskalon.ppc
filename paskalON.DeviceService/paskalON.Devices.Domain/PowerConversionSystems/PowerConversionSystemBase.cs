using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;
using paskalON.Devices.Domain.Ders;
using paskalON.Domains.Telemetry;
using paskalON.PhysicalUnits.Electricals.Powers;
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
    public abstract class PowerConversionSystemBase : DerDeviceBase<PowerConversionSystemBase>, INotifyPropertyChanged
    {
        /// <summary>
        /// Power conversion system configuration of this instance.
        /// </summary>
        private readonly PowerConversionSystemConfig _config;


        /// <summary>
        /// This lock object needs to be used by this class and derived classes.
        /// The classes need to use the same lock for thread safety.
        /// </summary>
        protected object dataLock = new();


        /// <summary>
        /// Event when the Power Conversion System state <see cref="PcsStateChanged"/> changes.
        /// </summary>
        public event EventHandler<PcsStateChangedEventArgs>? StateChanged;


        /// <summary>
        /// Event when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;


        /// <summary>
        /// Parent Distributed Energy Resource unit (DER-Unit).
        /// </summary>
        public DerUnit DerUnit { get; set; }


        /// <summary>
        /// Returns true if a communication error has occurred.
        /// </summary>
        public bool CommunicationError { get; set; }


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
        /// State of the Power Conversion System (PCS).
        /// Specialized PCS has to map its states to the these states.
        /// </summary>
        public PcsState State
        {
            get;
            set { field = value; SetState(value); }
        }


        /// <summary>
        /// Flag whether this instance is in maintenance mode this is when the DER Unit is in maintenance mode.
        /// </summary>
        public bool IsInMaintenanceMode { get => DerUnit.IsInMaintenanceMode; }



        private double? activePowerTarget;
        /// <summary>
        /// Current active power target in Watts
        /// </summary>
        public ActivePower? ActivePowerTarget
        {
            get { lock (dataLock) { return (activePowerTarget is null) ? null : new ActivePower((double)activePowerTarget); } }
        }


        private double? activePower;
        /// <summary>
        /// Current active power output in Watts
        /// </summary>
        public ActivePower? ActivePower
        {
            get { lock (dataLock) { return (activePower is null) ? null : new ActivePower((double)activePower); } }
        }


        private double? reactivePowerTarget;
        /// <summary>
        /// Current reactive power target in Vars
        /// </summary>
        public ReactivePower? ReactivePowerTarget
        {
            get { lock (dataLock) { return (reactivePowerTarget is null) ? null : new ReactivePower((double)reactivePowerTarget); } }
        }


        private double? reactivePower;
        /// <summary>
        /// Current reactive power output in Vars
        /// </summary>
        public ReactivePower? ReactivePower
        {
            get { lock (dataLock) { return (reactivePower is null) ? null : new ReactivePower((double)reactivePower); } }
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
        /// Line frequency in hertz.
        /// </summary>
        public double? LineFrequency
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// AC Current or AC Current sum in case of current phase A, B, C
        /// </summary>
        public double? ACCurrentSum
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// DC Current
        /// </summary>
        public double? DCCurrent
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// DC Voltage.
        /// </summary>
        public double? DCVoltage
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// AC Phase B to A voltage in volts.
        /// </summary>
        public double? VoltagePhaseAToB
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } SetAcBreakerStatuses(); }
        }


        /// <summary>
        /// AC Phase C to B voltage in volts.
        /// </summary>
        public double? VoltagePhaseBToC
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; SetAcBreakerStatuses(); } }
        }


        /// <summary>
        /// AC Phase A to C voltage in volts.
        /// </summary>
        public double? VoltagePhaseCToA
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; SetAcBreakerStatuses(); } }
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
        /// Contains alarm definitions and their states.
        /// </summary>
        public Dictionary<string, bool> AlarmStates { get; } = new Dictionary<string, bool>();


        /// <summary>
        /// Indicates whether there are any active alarms.
        /// </summary>
        public bool HasActiveAlarms { get => AlarmStates.Any(a => a.Value == true); }


        /// <summary>
        /// Contains warning definitions and their states.
        /// </summary>
        public Dictionary<string, bool> WarningStates { get; } = new Dictionary<string, bool>();


        /// <summary>
        /// Indicates whether there are any active warnings.
        /// </summary>
        public bool HasActiveWarnings { get => WarningStates.Any(a => a.Value == true); }


        /// <summary>
        /// Contains vendors event definitions and their states.
        /// </summary>
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
        /// <param name="metricsPublisher">Metrics publisher interface.</param>
        public PowerConversionSystemBase(ILogger logger, PowerConversionSystemConfig config, DerUnit derUnit, IMetricsPublisher<PowerConversionSystemBase> metricsPublisher)
            : base(logger, config, metricsPublisher)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(DerUnit);
            ArgumentNullException.ThrowIfNull(metricsPublisher);

            _config = config;
            DerUnit = derUnit;
            RegisterMetrics();
        }


        /// <summary>
        /// Sets the active power.
        /// </summary>
        /// <param name="value">Active power value (Watts).</param>
        protected void SetActivePower(double? value)
        {
            lock (dataLock)
            {
                activePower = value;
            }
        }


        /// <summary>
        /// Sets the reactive power.
        /// </summary>
        /// <param name="value">Reactive power value (VArs).</param>
        protected void SetReactivePower(double? value)
        {
            lock (dataLock)
            {
                reactivePower = value;
            }
        }


        /// <summary>
        /// Sets the active power target.
        /// </summary>
        /// <param name="value">Active power value (Watts).</param>
        protected void SetActivePowerTarget(double? value)
        {
            lock (dataLock)
            {
                if (activePowerTarget != value)
                {
                    activePowerTarget = value;
                    _logger.LogInformation("{Name} - Set active power target to: {activePowerTarget}", Name, activePowerTarget);
                }
            }
        }


        /// <summary>
        /// Sets the reactive power target.
        /// </summary>
        /// <param name="value">Reactive power value (VArs).</param>
        protected void SetReactivePowerTarget(double? value)
        {
            lock (dataLock)
            {
                if (reactivePowerTarget != value)
                {
                    reactivePowerTarget = value;
                    _logger.LogInformation("{Name} - Set reactive power target to: {reactivePowerTarget}", Name, reactivePowerTarget);
                }
            }
        }


        /// <summary>
        /// Trigger PCS state change events
        /// </summary>
        /// <param name="state">The PCS state.</param>
        protected void SetState(PcsState state)
        {
            _logger.LogInformation("{Name} - PCS state changed to: {State}", Name, State);
            StateChanged?.Invoke(this, new PcsStateChangedEventArgs(state));
        }


        /// <summary>
        /// Set the AC breaker status according to the voltage phases
        /// </summary>
        protected void SetAcBreakerStatuses()
        {
            IsACBreakerClosed = (VoltagePhaseAToB != 0.0 || VoltagePhaseBToC != 0.0 || VoltagePhaseCToA != 0.0);
        }


        /// <summary>
        /// Sets an alarm.
        /// </summary>
        /// <param name="name">Name of the alarm.</param>
        /// <param name="state">State of the alarm.</param>
        protected void SetAlarm(string name, bool state)
        {
            AlarmStates[name] = state;
        }


        /// <summary>
        /// Sets a warning.
        /// </summary>
        /// <param name="name">Name of the warning.</param>
        /// <param name="state">State of the warning.</param>
        protected void SetWarning(string name, bool state)
        {
            WarningStates[name] = state;
        }


        /// <summary>
        /// Sets a vendor event.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="state">State of the event.</param>
        protected void SetVendorEvent(string name, bool state)
        {
            VendorEvents[name] = state;
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
        /// Register base metrics with the metrics publisher.
        /// </summary>
        private void RegisterMetrics()
        {
            // MetricsFactorClass1
            _metricsPublisher.Register<bool>(nameof(CommunicationError), x => x.CommunicationError, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(ActivePowerTarget), x => x.ActivePowerTarget?.Watts, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(ReactivePowerTarget), x => x.ReactivePowerTarget?.VoltAmperesReactive, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(ActivePower), x => x.ActivePower?.Watts, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(ReactivePower), x => x.ReactivePower?.VoltAmperesReactive, _config.MetricsFactorClass1);
            // MetricsFactorClass2
            _metricsPublisher.Register<PcsState>(nameof(State), x => x.State, _config.MetricsFactorClass2);
            _metricsPublisher.Register<bool>(nameof(HasActiveAlarms), x => x.HasActiveAlarms, _config.MetricsFactorClass2);
            _metricsPublisher.Register<bool>(nameof(HasActiveWarnings), x => x.HasActiveWarnings, _config.MetricsFactorClass2);
            _metricsPublisher.Register<double?>(nameof(VoltagePhaseAToB), x => x.VoltagePhaseAToB, _config.MetricsFactorClass2);
            _metricsPublisher.Register<double?>(nameof(VoltagePhaseBToC), x => x.VoltagePhaseBToC, _config.MetricsFactorClass2);
            _metricsPublisher.Register<double?>(nameof(VoltagePhaseCToA), x => x.VoltagePhaseCToA, _config.MetricsFactorClass2);
            _metricsPublisher.Register<double?>(nameof(ACCurrentSum), x => x.ACCurrentSum, _config.MetricsFactorClass2);
            _metricsPublisher.Register<double?>(nameof(DCCurrent), x => x.DCCurrent, _config.MetricsFactorClass2);
            _metricsPublisher.Register<double?>(nameof(DCVoltage), x => x.DCVoltage, _config.MetricsFactorClass2);
            // MetricsFactorClass3
            _metricsPublisher.Register<bool>(nameof(IsInMaintenanceMode), x => x.IsInMaintenanceMode, _config.MetricsFactorClass3);
            // MetricsFactorClass4
            _metricsPublisher.Register<double?>(nameof(LineFrequency), x => x.LineFrequency, _config.MetricsFactorClass4);
        }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.Extensions.Logging;
using paskalON.Dataface;
using paskalON.Dataface.C37s;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Ders;
using paskalON.PhysicalUnits.Electricals.Energies;
using paskalON.PhysicalUnits.Electricals.Powers;
using paskalON.Telemetry;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace paskalON.Devices.Domain.Meters.PowerMeters
{
    //---------------------------------------------------------------
    // Do not modify this class without consulting the Lead Engineer.
    //---------------------------------------------------------------
    /// <summary>
    /// Power meter base call for all power meters.
    /// </summary>
    /// <remarks>
    /// A power meter measures electrical values (e.g., active/reactive power in watts/vars, voltage, frequency, and power factor).
    /// </remarks>
    public abstract class PowerMeterBase : DerDeviceBase, IPowerMeter, INotifyPropertyChanged
    {
        /// <summary>
        /// Power meter configuration.
        /// </summary>
        private readonly PowerMeterBaseConfig _config;


        /// <summary>
        /// This lock object needs to be used by this class and derived classes.
        /// The classes need to use the same lock for thread safety.
        /// </summary>
        protected object dataLock = new();


        /// <summary>
        /// Event when the Power Meter state <see cref="PowerMeterStateChangedEventArgs"/> changes.
        /// </summary>
        public event EventHandler<PowerMeterStateChangedEventArgs>? StateChanged;


        /// <summary>
        /// Event when the communication error state changed.
        /// </summary>
        public event EventHandler<CommunicationErrorChangedEventArgs>? CommunicationErrorChanged;


        /// <summary>
        /// Event when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;



        /// <summary>
        /// State of the Power Meter.
        /// Specialized Power Meter has to map its states to the these states.
        /// </summary>
        public PowerMeterState State
        {
            get;
            set { if (field != value) { field = value; SetState(value); } }
        }


        /// <summary>
        /// Communication error.
        /// </summary>
        /// <remarks>
        /// Returns true if a communication error has occurred.
        /// </remarks>
        public bool CommunicationError
        {
            get;
            set { if (field != value) { field = value; SetCommunicationError(value); } }
        }


        /// <summary>
        /// Is reverse power flow from configuration.
        /// </summary>        
        /// </remarks>
        public bool IsReversePowerFlow { get => _config.PowerMeterDeviceConfig.IsReversePowerFlow; }


        /// <summary>
        /// Is current signed from configuration.
        /// </summary>
        public bool IsCurrentSigned { get => _config.PowerMeterDeviceConfig.IsCurrentSigned; }


        /// <summary>
        /// Power factor standard used for this meter.
        /// </summary>
        public PowerFactorStandard PowerFactorStandard { get => _config.PowerFactorStandard; }


        /// <summary>
        /// Active power value.
        /// </summary>
        public double? ActivePowerValue
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = IsReversePowerFlow ? -value : value; } }
        }


        /// <summary>
        /// Active power.
        /// </summary>
        public ActivePower? ActivePower { get => ActivePowerValue == null ? null : new ActivePower((double)ActivePowerValue); }


        /// <summary>
        /// Reactive power value.
        /// </summary>
        public double? ReactivePowerValue
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = IsReversePowerFlow ? -value : value; } }
        }


        /// <summary>
        /// Reactive power.
        /// </summary>
        public ReactivePower? ReactivePower { get => ReactivePowerValue == null ? null : new ReactivePower((double)ReactivePowerValue); }


        /// <summary>
        /// Apparent power value.
        /// </summary>
        public double? ApparentPowerValue
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Apparent power.
        /// </summary>
        public ApparentPower? ApparentPower { get => ApparentPowerValue == null ? null : new ApparentPower((double)ApparentPowerValue); }


        /// <summary>
        /// Calculated power factor according to power factor standard configuration.
        /// </summary>
        public double? PowerFactor
        {
            get
            {
                return PowerFactorStandard == PowerFactorStandard.IEEE ? IeeePowerFactor.Calculate(ActivePowerValue, ReactivePowerValue)?.PowerFactor
                    : IecPowerFactor.Calculate(ActivePowerValue, ReactivePowerValue)?.PowerFactor;
            }
        }


        /// <summary>
        /// Frequency. 
        /// </summary>
        public double? Frequency
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Voltage phasor value, phase A.
        /// </summary>
        public ulong? VoltageA
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Voltage magnitude, phase A.
        /// </summary>
        public double? VoltageAMagnitude { get => GetMagnitudeFromPhasorValue(VoltageA); }


        /// <summary>
        /// Voltage angle, phase A.
        /// </summary>
        public double? VoltageAAngle { get => GetAngleFromPhasorValue(VoltageA); }


        /// <summary>
        /// Voltage phasor value, phase B.
        /// </summary>
        public ulong? VoltageB
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Voltage magnitude, phase B.
        /// </summary>
        public double? VoltageBMagnitude { get => GetMagnitudeFromPhasorValue(VoltageB); }


        /// <summary>
        /// Voltage angle, phase B.
        /// </summary>
        public double? VoltageBAngle { get => GetAngleFromPhasorValue(VoltageB); }


        /// <summary>
        /// Voltage phasor value, phase C.
        /// </summary>
        public ulong? VoltageC
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }

        /// <summary>
        /// Voltage magnitude, phase C.
        /// </summary>
        public double? VoltageCMagnitude { get => GetMagnitudeFromPhasorValue(VoltageC); }


        /// <summary>
        /// Voltage angle, phase C.
        /// </summary>
        public double? VoltageCAngle { get => GetAngleFromPhasorValue(VoltageC); }


        /// <summary>
        /// Voltage phasor value, line-to-line AB.
        /// </summary>
        public ulong? VoltageAB
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }

        /// <summary>
        /// Voltage magnitude, line-to-line AB.
        /// </summary>
        public double? VoltageABMagnitude { get => GetMagnitudeFromPhasorValue(VoltageAB); }


        /// <summary>
        /// Voltage angle, line-to-line AB.
        /// </summary>
        public double? VoltageABAngle { get => GetAngleFromPhasorValue(VoltageAB); }


        /// <summary>
        /// Voltage phasor value, line-to-line BC.
        /// </summary>
        public ulong? VoltageBC
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Voltage magnitude, line-to-line BC.
        /// </summary>
        public double? VoltageBCMagnitude { get => GetMagnitudeFromPhasorValue(VoltageBC); }


        /// <summary>
        /// Voltage angle, line-to-line BC.
        /// </summary>
        public double? VoltageBCAngle { get => GetAngleFromPhasorValue(VoltageBC); }


        /// <summary>
        /// Voltage phasor value, line-to-line CA.
        /// </summary>
        public ulong? VoltageCA
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Voltage magnitude, line-to-line CA.
        /// </summary>
        public double? VoltageCAMagnitude { get => GetMagnitudeFromPhasorValue(VoltageCA); }


        /// <summary>
        /// Voltage angle, line-to-line BC.
        /// </summary>
        public double? VoltageCAAngle { get => GetAngleFromPhasorValue(VoltageCA); }


        /// <summary>
        /// Voltage positive sequence phasor value.
        /// </summary>
        public ulong? VoltagePositiveSequence
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Voltage positive sequence magnitude.
        /// </summary>
        public double? VoltagePositiveSequenceMagnitude { get => GetMagnitudeFromPhasorValue(VoltagePositiveSequence); }


        /// <summary>
        /// Voltage positive sequence angle.
        /// </summary>
        public double? VoltagePositiveSequenceAngle { get => GetAngleFromPhasorValue(VoltagePositiveSequence); }


        /// <summary>
        /// Current phasor value, phase A.
        /// </summary>
        public ulong? CurrentA
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Current magnitude, phase A.
        /// </summary>
        public double? CurrentAMagnitude { get => GetMagnitudeFromPhasorValue(CurrentA) * ((IsReversePowerFlow && IsCurrentSigned) ? -1 : 1); }


        /// <summary>
        /// Current angle, phase A.
        /// </summary>
        public double? CurrentAAngle { get => GetAngleFromPhasorValue(CurrentA); }


        /// <summary>
        /// Current phasor value, phase B.
        /// </summary>
        public ulong? CurrentB
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Current magnitude, phase B.
        /// </summary>
        public double? CurrentBMagnitude { get => GetMagnitudeFromPhasorValue(CurrentB) * ((IsReversePowerFlow && IsCurrentSigned) ? -1 : 1); }


        /// <summary>
        /// Current angle, phase B.
        /// </summary>
        public double? CurrentBAngle { get => GetAngleFromPhasorValue(CurrentB); }


        /// <summary>
        /// Current phasor value, phase C.
        /// </summary>
        public ulong? CurrentC
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Current magnitude, phase C.
        /// </summary>
        public double? CurrentCMagnitude { get => GetMagnitudeFromPhasorValue(CurrentC) * ((IsReversePowerFlow && IsCurrentSigned) ? -1 : 1); }


        /// <summary>
        /// Current angle, phase C.
        /// </summary>
        public double? CurrentCAngle { get => GetAngleFromPhasorValue(CurrentC); }


        /// <summary>
        /// Voltage ll average value.
        /// </summary>
        public double? VoltageLLAvg
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Active power A value.
        /// </summary>
        public double? ActivePowerAValue
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = IsReversePowerFlow ? -value : value; } }
        }


        /// <summary>
        /// Active power A.
        /// </summary>
        public ActivePower? ActivePowerA { get => ActivePowerAValue == null ? null : new ActivePower((double)ActivePowerAValue); }


        /// <summary>
        /// Active power B value.
        /// </summary>
        public double? ActivePowerBValue
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = IsReversePowerFlow ? -value : value; } }
        }


        /// <summary>
        /// Active power B.
        /// </summary>
        public ActivePower? ActivePowerB { get => ActivePowerBValue == null ? null : new ActivePower((double)ActivePowerBValue); }


        /// <summary>
        /// Active power C value.
        /// </summary>
        public double? ActivePowerCValue
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = IsReversePowerFlow ? -value : value; } }
        }


        /// <summary>
        /// Active power C.
        /// </summary>
        public ActivePower? ActivePowerC { get => ActivePowerCValue == null ? null : new ActivePower((double)ActivePowerCValue); }


        /// <summary>
        /// Reactive power A value.
        /// </summary>
        public double? ReactivePowerAValue
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = IsReversePowerFlow ? -value : value; } }
        }


        /// <summary>
        /// Reactive power A.
        /// </summary>
        public ReactivePower? ReactivePowerA { get => ReactivePowerAValue == null ? null : new ReactivePower((double)ReactivePowerAValue); }


        /// <summary>
        /// Reactive power B value.
        /// </summary>
        public double? ReactivePowerBValue
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = IsReversePowerFlow ? -value : value; } }
        }


        /// <summary>
        /// Reactive power B.
        /// </summary>
        public ReactivePower? ReactivePowerB { get => ReactivePowerBValue == null ? null : new ReactivePower((double)ReactivePowerBValue); }


        /// <summary>
        /// Reactive power C value.
        /// </summary>
        public double? ReactivePowerCValue
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = IsReversePowerFlow ? -value : value; } }
        }

        /// <summary>
        /// Reactive power C.
        /// </summary>
        public ReactivePower? ReactivePowerC { get => ReactivePowerCValue == null ? null : new ReactivePower((double)ReactivePowerCValue); }


        /// <summary>
        /// Energy delivered value.
        /// </summary>
        public double? EnergyDeliveredValue
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Energy delivered.
        /// </summary>
        public Energy? EnergyDelivered { get => EnergyDeliveredValue == null ? null : new Energy((double)EnergyDeliveredValue); }


        /// <summary>
        /// Energy received value.
        /// </summary>
        public double? EnergyReceivedValue
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Energy received.
        /// </summary>
        public Energy? EnergyReceived { get => EnergyReceivedValue == null ? null : new Energy((double)EnergyReceivedValue); }


        /// <summary>
        /// Reactive energy delivered value.
        /// </summary>
        public double? ReactiveEnergyDeliveredValue
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Reactive energy delivered.
        /// </summary>
        public ReactiveEnergy? ReactiveEnergyDelivered { get => ReactiveEnergyDeliveredValue == null ? null : new ReactiveEnergy((double)ReactiveEnergyDeliveredValue); }


        /// <summary>
        /// Reactive energy received value.
        /// </summary>
        public double? ReactiveEnergyReceivedValue
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Reactive energy received.
        /// </summary>
        public ReactiveEnergy? ReactiveEnergyReceived { get => ReactiveEnergyReceivedValue == null ? null : new ReactiveEnergy((double)ReactiveEnergyReceivedValue); }



        /// <summary>
        /// Constructor of <see cref="PowerMeterBase"/>
        /// </summary>
        /// <param name="logger">The logging instance.</param>
        /// <param name="config">The power meter configuration.</param>
        /// <param name="publisher">The publisher interface.</param>
        /// <param name="dataface">The dataface interface.</param>
        public PowerMeterBase(ILogger logger, PowerMeterBaseConfig config, IMetricsPublisher publisher, IDataface dataface)
            : base(logger, config, publisher, dataface)
        {
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(publisher);

            _config = config;

            RegisterMetrics();
            RegisterDataface();
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual async Task ConnectAsync()
        {
            _logger.LogInformation("{Name} connect requested.", Name);
            State = PowerMeterState.Connecting;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual async Task DisconnectAsync()
        {
            _logger.LogInformation("{Name} disconnect requested.", Name);
            State = PowerMeterState.Disconnecting;
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
            MetricsPublisher.Initialize("PowerMeter", tags);
            // MetricsFactorClass1
            MetricsPublisher.Register<PowerMeterBase, bool>(this, nameof(CommunicationError), MetricType.Gauge, x => x.CommunicationError, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(Frequency), MetricType.Gauge, x => x.Frequency, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(PowerFactor), MetricType.Gauge, x => x.PowerFactor, _config.MetricsFactorClass1);
            // Power A-C
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(ActivePower), MetricType.Gauge, x => x.ActivePowerValue, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(ReactivePower), MetricType.Gauge, x => x.ReactivePowerValue, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(ApparentPower), MetricType.Gauge, x => x.ApparentPowerValue, _config.MetricsFactorClass1);
            // Voltage
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(VoltageAMagnitude), MetricType.Gauge, x => x.VoltageAMagnitude, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(VoltageAAngle), MetricType.Gauge, x => x.VoltageAAngle, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(VoltageBMagnitude), MetricType.Gauge, x => x.VoltageBMagnitude, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(VoltageBAngle), MetricType.Gauge, x => x.VoltageBAngle, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(VoltageCMagnitude), MetricType.Gauge, x => x.VoltageCMagnitude, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(VoltageCAngle), MetricType.Gauge, x => x.VoltageCAngle, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(VoltageABMagnitude), MetricType.Gauge, x => x.VoltageABMagnitude, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(VoltageABAngle), MetricType.Gauge, x => x.VoltageABAngle, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(VoltageBCMagnitude), MetricType.Gauge, x => x.VoltageBCMagnitude, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(VoltageBCAngle), MetricType.Gauge, x => x.VoltageBCAngle, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(VoltageCAMagnitude), MetricType.Gauge, x => x.VoltageCAMagnitude, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(VoltageCAAngle), MetricType.Gauge, x => x.VoltageCAAngle, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(VoltagePositiveSequenceMagnitude), MetricType.Gauge, x => x.VoltagePositiveSequenceMagnitude, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(VoltagePositiveSequenceAngle), MetricType.Gauge, x => x.VoltagePositiveSequenceAngle, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(VoltageLLAvg), MetricType.Gauge, x => x.VoltageLLAvg, _config.MetricsFactorClass1);
            // Current
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(CurrentAMagnitude), MetricType.Gauge, x => x.CurrentAMagnitude, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(CurrentAAngle), MetricType.Gauge, x => x.CurrentAAngle, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(CurrentBMagnitude), MetricType.Gauge, x => x.CurrentBMagnitude, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(CurrentBAngle), MetricType.Gauge, x => x.CurrentBAngle, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(CurrentCMagnitude), MetricType.Gauge, x => x.CurrentCMagnitude, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(CurrentCAngle), MetricType.Gauge, x => x.CurrentCAngle, _config.MetricsFactorClass1);
            // Power A-C
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(ActivePowerA), MetricType.Gauge, x => x.ActivePowerAValue, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(ActivePowerB), MetricType.Gauge, x => x.ActivePowerBValue, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(ActivePowerC), MetricType.Gauge, x => x.ActivePowerCValue, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(ReactivePowerA), MetricType.Gauge, x => x.ReactivePowerAValue, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(ReactivePowerB), MetricType.Gauge, x => x.ReactivePowerBValue, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(ReactivePowerC), MetricType.Gauge, x => x.ReactivePowerCValue, _config.MetricsFactorClass1);
            // Energy
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(EnergyDelivered), MetricType.Gauge, x => x.EnergyDeliveredValue, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(EnergyReceived), MetricType.Gauge, x => x.EnergyReceivedValue, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(ReactiveEnergyDelivered), MetricType.Gauge, x => x.ReactiveEnergyDeliveredValue, _config.MetricsFactorClass1);
            MetricsPublisher.Register<PowerMeterBase, double>(this, nameof(ReactiveEnergyReceived), MetricType.Gauge, x => x.ReactiveEnergyReceivedValue, _config.MetricsFactorClass1);
            // MetricsFactorClass2
            // MetricsFactorClass3
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void RegisterDataface()
        {
            PowerMeterMapC37Config? c37Config = _config.PowerMeterDeviceConfig.PowerMeterMapC37Config;

            // C37 data interface is only using names which we have in this base class.
            // Modbus data interface uses register numbers, scaling, etc. and therefore should be registered in a manufacturer class.
            if (c37Config != null)
            {
                // Active power
                if (string.IsNullOrEmpty(c37Config.ActivePower) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, double?>(this, c37Config.ActivePower, C37SignalType.Analog, (x, v) => x.ActivePowerValue = v));
                if (string.IsNullOrEmpty(c37Config.ActivePowerA) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, double?>(this, c37Config.ActivePowerA, C37SignalType.Analog, (x, v) => x.ActivePowerAValue = v));
                if (string.IsNullOrEmpty(c37Config.ActivePowerB) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, double?>(this, c37Config.ActivePowerB, C37SignalType.Analog, (x, v) => x.ActivePowerBValue = v));
                if (string.IsNullOrEmpty(c37Config.ActivePowerC) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, double?>(this, c37Config.ActivePowerC, C37SignalType.Analog, (x, v) => x.ActivePowerCValue = v));
                // Reactive power
                if (string.IsNullOrEmpty(c37Config.ReactivePower) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, double?>(this, c37Config.ReactivePower, C37SignalType.Analog, (x, v) => x.ReactivePowerValue = v));
                if (string.IsNullOrEmpty(c37Config.ReactivePowerA) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, double?>(this, c37Config.ReactivePowerA, C37SignalType.Analog, (x, v) => x.ReactivePowerAValue = v));
                if (string.IsNullOrEmpty(c37Config.ReactivePowerB) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, double?>(this, c37Config.ReactivePowerB, C37SignalType.Analog, (x, v) => x.ReactivePowerBValue = v));
                if (string.IsNullOrEmpty(c37Config.ReactivePowerC) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, double?>(this, c37Config.ReactivePowerC, C37SignalType.Analog, (x, v) => x.ReactivePowerCValue = v));
                // Apparent power
                if (string.IsNullOrEmpty(c37Config.ApparentPower) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, double?>(this, c37Config.ApparentPower, C37SignalType.Analog, (x, v) => x.ApparentPowerValue = v));
                // Voltage
                if (string.IsNullOrEmpty(c37Config.VoltageA) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, ulong?>(this, c37Config.VoltageA, C37SignalType.Phasor, (x, v) => x.VoltageA = v));
                if (string.IsNullOrEmpty(c37Config.VoltageB) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, ulong?>(this, c37Config.VoltageB, C37SignalType.Phasor, (x, v) => x.VoltageB = v));
                if (string.IsNullOrEmpty(c37Config.VoltageC) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, ulong?>(this, c37Config.VoltageC, C37SignalType.Phasor, (x, v) => x.VoltageC = v));
                if (string.IsNullOrEmpty(c37Config.VoltageAB) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, ulong?>(this, c37Config.VoltageAB, C37SignalType.Phasor, (x, v) => x.VoltageAB = v));
                if (string.IsNullOrEmpty(c37Config.VoltageBC) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, ulong?>(this, c37Config.VoltageBC, C37SignalType.Phasor, (x, v) => x.VoltageBC = v));
                if (string.IsNullOrEmpty(c37Config.VoltageCA) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, ulong?>(this, c37Config.VoltageCA, C37SignalType.Phasor, (x, v) => x.VoltageCA = v));
                if (string.IsNullOrEmpty(c37Config.VoltagePositiveSequence) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, ulong?>(this, c37Config.VoltagePositiveSequence, C37SignalType.Phasor, (x, v) => x.VoltagePositiveSequence = v));
                if (string.IsNullOrEmpty(c37Config.VoltageLLAvg) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, double?>(this, c37Config.VoltageLLAvg, C37SignalType.Analog, (x, v) => x.VoltageLLAvg = v));
                // Current
                if (string.IsNullOrEmpty(c37Config.CurrentA) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, ulong?>(this, c37Config.CurrentA, C37SignalType.Phasor, (x, v) => x.CurrentA = v));
                if (string.IsNullOrEmpty(c37Config.CurrentB) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, ulong?>(this, c37Config.CurrentB, C37SignalType.Phasor, (x, v) => x.CurrentB = v));
                if (string.IsNullOrEmpty(c37Config.CurrentC) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, ulong?>(this, c37Config.CurrentC, C37SignalType.Phasor, (x, v) => x.CurrentC = v));
                // Energy
                if (string.IsNullOrEmpty(c37Config.EnergyDelivered) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, double?>(this, c37Config.EnergyDelivered, C37SignalType.Analog, (x, v) => x.EnergyDeliveredValue = v));
                if (string.IsNullOrEmpty(c37Config.EnergyReceived) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, double?>(this, c37Config.EnergyReceived, C37SignalType.Analog, (x, v) => x.EnergyReceivedValue = v));
                if (string.IsNullOrEmpty(c37Config.ReactiveEnergyDelivered) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, double?>(this, c37Config.ReactiveEnergyDelivered, C37SignalType.Analog, (x, v) => x.ReactiveEnergyDeliveredValue = v));
                if (string.IsNullOrEmpty(c37Config.ReactiveEnergyReceived) == false)
                    Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, double?>(this, c37Config.ReactiveEnergyReceived, C37SignalType.Analog, (x, v) => x.ReactiveEnergyReceivedValue = v));
                // Misc
                // Frequency doesn't have a configurable name and should be fix "FREQUENCY" in the registrations.
                Dataface.Register<PowerMeterBase, IC37Register>(r => r.Register<PowerMeterBase, double?>(this, "FREQUENCY", C37SignalType.Frequency, (x, v) => x.Frequency = v));
            }
        }


        /// <summary>
        /// Get the magnitude value from phasor endpoint.
        /// </summary>
        /// <param name="value">The full phasor value containing magnitude and angle.</param>
        /// <returns>The magnitude value.</returns>
        protected double? GetMagnitudeFromPhasorValue(ulong? value)
        {
            if (value.HasValue == false)
            {
                return null;
            }

            return BitConverter.UInt32BitsToSingle((uint)(value >> 32));
        }


        /// <summary>
        /// Get the angle value from phasor endpoint.
        /// </summary>
        /// <param name="value">The full phasor value containing magnitude and angle.</param>
        /// <returns>The angle value in radiant or degrees.</returns>
        protected double? GetAngleFromPhasorValue(ulong? value)
        {
            if (value.HasValue == false)
            {
                return null;
            }

            return BitConverter.UInt32BitsToSingle((uint)(value & 0xFFFFFFFF));
        }


        /// <summary>
        /// Trigger Power Meter state change events.
        /// </summary>
        /// <param name="state">The Power Meter state.</param>
        private void SetState(PowerMeterState state)
        {
            _logger.LogInformation("{Name} - Power Meter state changed to: {State}", Name, State);
            StateChanged?.Invoke(this, new PowerMeterStateChangedEventArgs(state));
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
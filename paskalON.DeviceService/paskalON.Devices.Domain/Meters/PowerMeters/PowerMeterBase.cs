using Microsoft.Extensions.Logging;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;
using paskalON.Devices.Domain.Ders;
using paskalON.Domains.Telemetry;
using paskalON.PhysicalUnits.Electricals.Energies;
using paskalON.PhysicalUnits.Electricals.Powers;

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
    public abstract class PowerMeterBase : DerDeviceBase<PowerMeterBase>
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
        /// Communication error.
        /// </summary>
        /// <remarks>
        /// Returns true if a communication error has occurred.
        /// </remarks>
        public bool CommunicationError { get; set; }


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
        /// Voltage magnitude, phase A.
        /// </summary>
        public double? VoltageA
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Voltage angle, phase A.
        /// </summary>
        public double? VoltageAngleA
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }

        /// <summary>
        /// Voltage magnitude, phase B.
        /// </summary>
        public double? VoltageB
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }



        /// <summary>
        /// Voltage angle, phase B.
        /// </summary>
        public double? VoltageAngleB
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Voltage magnitude, phase C.
        /// </summary>
        public double? VoltageC
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Voltage angle, phase C.
        /// </summary>
        public double? VoltageAngleC
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }



        /// <summary>
        /// Voltage positive sequence.
        /// </summary>
        public double? VoltagePositiveSequence
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Voltage positive sequence angle.
        /// </summary>
        public double? VoltagePositiveSequenceAngle
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Voltage magnitude, line-to-line AB.
        /// </summary>
        public double? VoltageAB
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Voltage magnitude, line-to-line BC.
        /// </summary>
        public double? VoltageBC
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Voltage magnitude, line-to-line CA.
        /// </summary>
        public double? VoltageCA
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Current magnitude, phase A.
        /// </summary>
        public double? CurrentA
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = (IsReversePowerFlow && IsCurrentSigned) ? -value : value; } }
        }


        /// <summary>
        /// Current angle, phase A.
        /// </summary>
        public double? CurrentAngleA
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Current magnitude, phase B.
        /// </summary>
        public double? CurrentB
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = (IsReversePowerFlow && IsCurrentSigned) ? -value : value; } }
        }


        /// <summary>
        /// Current angle, phase B.
        /// </summary>
        public double? CurrentAngleB
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Current magnitude, phase C.
        /// </summary>
        public double? CurrentC
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = (IsReversePowerFlow && IsCurrentSigned) ? -value : value; } }
        }


        /// <summary>
        /// Current angle, phase C.
        /// </summary>
        public double? CurrentAngleC
        {
            get { lock (dataLock) { return field; } }
            set { lock (dataLock) { field = value; } }
        }


        /// <summary>
        /// Voltage ll average.
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
        /// <param name="powerMeterConfig">The power meter configuration.</param>
        /// <param name="metricsPublisher">The metrics publisher instance.</param>
        public PowerMeterBase(ILogger logger, PowerMeterBaseConfig powerMeterConfig, IMetricsPublisher<PowerMeterBase> metricsPublisher) : base(logger, powerMeterConfig, metricsPublisher)
        {
            _config = powerMeterConfig;
            RegisterMetrics();
        }


        private void RegisterMetrics()
        {
            // MetricsFactorClass1
            _metricsPublisher.Register<bool>(nameof(CommunicationError), x => x.CommunicationError, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(Frequency), x => x.Frequency, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(PowerFactor), x => x.PowerFactor, _config.MetricsFactorClass1);
            // Power A-C
            _metricsPublisher.Register<double?>(nameof(ActivePower), x => x.ActivePowerValue, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(ReactivePower), x => x.ReactivePowerValue, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(ApparentPower), x => x.ApparentPowerValue, _config.MetricsFactorClass1);
            // Voltage
            _metricsPublisher.Register<double?>(nameof(VoltageA), x => x.VoltageA, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(VoltageAngleA), x => x.VoltageAngleA, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(VoltageB), x => x.VoltageB, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(VoltageAngleB), x => x.VoltageAngleB, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(VoltageC), x => x.VoltageC, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(VoltageAngleC), x => x.VoltageAngleC, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(VoltagePositiveSequence), x => x.VoltagePositiveSequence, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(VoltagePositiveSequenceAngle), x => x.VoltagePositiveSequenceAngle, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(VoltageAB), x => x.VoltageAB, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(VoltageBC), x => x.VoltageBC, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(VoltageCA), x => x.VoltageCA, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(VoltageLLAvg), x => x.VoltageLLAvg, _config.MetricsFactorClass1);
            // Current
            _metricsPublisher.Register<double?>(nameof(CurrentA), x => x.CurrentA, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(CurrentAngleA), x => x.CurrentAngleA, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(CurrentB), x => x.CurrentB, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(CurrentAngleB), x => x.CurrentAngleB, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(CurrentC), x => x.CurrentC, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(CurrentAngleC), x => x.CurrentAngleC, _config.MetricsFactorClass1);
            // Power A-C
            _metricsPublisher.Register<double?>(nameof(ActivePowerA), x => x.ActivePowerAValue, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(ActivePowerB), x => x.ActivePowerBValue, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(ActivePowerC), x => x.ActivePowerCValue, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(ReactivePowerA), x => x.ReactivePowerAValue, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(ReactivePowerB), x => x.ReactivePowerBValue, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(ReactivePowerC), x => x.ReactivePowerCValue, _config.MetricsFactorClass1);
            // Energy
            _metricsPublisher.Register<double?>(nameof(EnergyDelivered), x => x.EnergyDeliveredValue, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(EnergyReceived), x => x.EnergyReceivedValue, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(ReactiveEnergyDelivered), x => x.ReactiveEnergyDeliveredValue, _config.MetricsFactorClass1);
            _metricsPublisher.Register<double?>(nameof(ReactiveEnergyReceived), x => x.ReactiveEnergyReceivedValue, _config.MetricsFactorClass1);
            // MetricsFactorClass2
            // MetricsFactorClass3
        }
    }
}

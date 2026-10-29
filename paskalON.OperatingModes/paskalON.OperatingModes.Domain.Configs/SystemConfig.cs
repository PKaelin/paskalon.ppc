// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;
using paskalON.Domains.Configs;

namespace paskalON.OperatingModes.Domain.Configs
{
    /// <summary>
    /// Configuration class for the system.
    /// </summary>
    public class SystemConfig : DomainBase
    {
        /// <summary>
        /// Indicates the minimum valid metrics publishing interval value.
        /// If this value is less than this constant it will cause an exception.
        /// </summary>
        private const long MinimumMetricsIntervalMilliseconds = 200;


        /// <summary>
        /// Indicates the minimum interval, in milliseconds, between operating mode calculations.
        /// If this value is less than this constant it will cause an exception.
        /// </summary>
        private const long MinimumOperatingModeCalculationIntervalMilliseconds = 200;


        /// <summary>
        /// Indicates the minimum interval, in milliseconds, between two heartbeats.
        /// If this value is less than this constant it will cause an exception.
        /// </summary>
        private const long MinimumDependencyHeartbeatIntervalMilliseconds = 100;


        /// <summary>
        /// Indicates the minimum time, in milliseconds, after which a dependency is lost.
        /// If this value is less than this constant it will cause an exception.
        /// </summary>
        /// <remarks>
        /// Must be longer than the primary secondary failover (500 ms), so that a failover does not cause a loss.
        /// </remarks>
        private const long MinimumDependencyLossTimeoutMilliseconds = 1000;


        /// <summary>
        /// Indicates the minimum interval, in milliseconds, between two republished setpoints.
        /// If this value is less than this constant it will cause an exception.
        /// </summary>
        private const long MinimumSetpointRepublishIntervalMilliseconds = 200;


        /// <summary>
        /// Operating mode type.
        /// </summary>
        /// <remarks>
        /// Though this is a flag this operating mode system should be configured to only serve one type.
        /// </remarks>
        public required PowerControlType Type
        {
            get;
            set
            {
                int v = (int)value;
                if (Enum.IsDefined(typeof(PowerControlType), value) == false) throw new ArgumentException("Only one type per operating mode system is allowed.");
                field = value;
            }
        }


        /// <summary>
        /// System reference frequency in Hertz.
        /// </summary>
        /// <remarks>
        /// 60 Hz Regions (Cycles per Second)
        /// E.g. United States, Canada, Mexico, Brazil
        /// 50 Hz Regions (Cycles per Second)
        /// E.g. Europe, UK, Ireland, Australia, India, China, Switzerland
        /// </remarks>
        public required double ReferenceFrequency
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// System reference voltage in volts.
        /// </summary>
        public double ReferenceVoltage
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// Systems maximum voltage nameplate.
        /// </summary>
        /// <remarks>
        /// Maximum voltage nameplate refers to the highest operating voltage a system can continuously and safely operate.
        /// Most equipment are designed to operate safely with a voltage variation of +/- 10% from the rated nameplate.
        /// </remarks>
        public double NameplateMaximumVoltage
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// Systems minimum voltage nameplate.
        /// </summary>
        public double NameplateMinimumVoltage
        {
            get { return field; }
            set { field = value; }
        }


        /// <summary>
        /// Systems maximum current nameplate.
        /// </summary>
        /// <remarks>
        public double NameplateMaximumCurrent
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// Systems minimum current nameplate.
        /// </summary>
        public double NameplateMinimumCurrent
        {
            get { return field; }
            set { field = value; }
        }


        /// <summary>
        /// Systems maximum active power nameplate in watt.
        /// </summary>
        /// <remarks>
        /// Maximum active power nameplate refers to the highest operating active power a system can continuously and safely operate.
        /// Most equipment are designed to operate safely with an active power variation of +/- 10% from the rated nameplate.
        /// </remarks>
        public double NameplateMaximumActivePowerWatt
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// Systems minimum active power nameplate in Watt.
        /// </summary>
        /// <remarks>
        /// Minimum active power nameplate refers to the lowest operating active power a system can continuously and safely operate.
        /// Most equipment are designed to operate safely with an active power variation of +/- 10% from the rated nameplate.
        /// </remarks>
        public double NameplateMinimumActivePowerWatt
        {
            get { return field; }
            set { field = value; }
        }


        /// <summary>
        /// Systems maximum reactive power nameplate in vars.
        /// </summary>
        /// <remarks>
        /// Maximum reactive power nameplate refers to the highest operating reactive power a system can continuously and safely operate.
        /// Most equipment are designed to operate safely with a reactive power variation of +/- 10% from the rated nameplate.
        /// </remarks>
        public double NameplateMaximumReactivePowerVars
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// Systems minimum reactive power nameplate in vars.
        /// </summary>
        /// <remarks>
        /// Minimum reactive power nameplate refers to the lowest operating reactive power a system can continuously and safely operate.
        /// Most equipment are designed to operate safely with a reactive power variation of +/- 10% from the rated nameplate.
        /// </remarks>
        public double NameplateMinimumReactivePowerVars
        {
            get { return field; }
            set { field = value; }
        }


        /// <summary>
        /// The interval, in milliseconds, between operating mode calculations.
        /// </summary>
        public int OperatingModeCalculationIntervalMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumOperatingModeCalculationIntervalMilliseconds); field = value; }
        } = 1000;


        /// <summary>
        /// Metrics publishing interval in milliseconds.
        /// </summary>
        /// <remarks>
        /// Used in combination with the MetricsFactorClassX to determine the publishing interval for each class.
        /// Defined in: <see cref="DeviceIdNameBase"/>.
        /// </remarks>
        public int MetricsIntervalMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumMetricsIntervalMilliseconds); field = value; }
        } = 1000;


        /// <summary>
        /// The interval, in milliseconds, between two heartbeats of this service.
        /// </summary>
        public int DependencyHeartbeatIntervalMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumDependencyHeartbeatIntervalMilliseconds); field = value; }
        } = 500;


        /// <summary>
        /// The time, in milliseconds, the message broker (Redis) must be disconnected after which it is lost.
        /// </summary>
        /// <remarks>
        /// While a dependency is lost the operating mode control holds the last setpoint.
        /// </remarks>
        public int RedisLossTimeoutMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumDependencyLossTimeoutMilliseconds); field = value; }
        } = 1500;


        /// <summary>
        /// The time, in milliseconds, without a power control heartbeat after which the power control is lost.
        /// </summary>
        /// <remarks>
        /// Must be a multiple of the power control heartbeat interval. While a dependency is lost the operating mode control holds the last setpoint.
        /// </remarks>
        public int PowerControlLossTimeoutMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumDependencyLossTimeoutMilliseconds); field = value; }
        } = 1500;


        /// <summary>
        /// The time, in milliseconds, without device data after which the device service is lost.
        /// </summary>
        /// <remarks>
        /// Must be a multiple of the device publishing interval of the device service. While a dependency is lost the operating mode control holds the last setpoint.
        /// </remarks>
        public int DeviceServiceLossTimeoutMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumDependencyLossTimeoutMilliseconds); field = value; }
        } = 3000;


        /// <summary>
        /// The time, in milliseconds, all dependencies must be available after a loss before the operating mode control resumes.
        /// </summary>
        public int ResumeSettleMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfNegative(value); field = value; }
        } = 2000;


        /// <summary>
        /// The interval, in milliseconds, in which the current setpoints are republished even if they did not change.
        /// </summary>
        /// <remarks>
        /// Ensures that a restarted subscriber, e.g. the power control, receives the current setpoints.
        /// </remarks>
        public int SetpointRepublishIntervalMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumSetpointRepublishIntervalMilliseconds); field = value; }
        } = 1000;


        /// <summary>
        /// Subscriber topic for power conversion system core used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicPcsCore { get; set; }


        /// <summary>
        /// Subscriber topic for power conversion system detail used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicPcsDetail { get; set; }


        /// <summary>
        /// Subscriber topic for battery bank core used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicBatteryBankCore { get; set; }


        /// <summary>
        /// Subscriber topic for battery bank detail used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicBatteryBankDetail { get; set; }


        /// <summary>
        /// Subscriber topic for solar panel core used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicSolarPanelCore { get; set; }


        /// <summary>
        /// Subscriber topic for solar panel detail used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicSolarPanelDetail { get; set; }


        /// <summary>
        /// Subscriber topic for external power meter core used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicExternalPowerMeterCore { get; set; }


        /// <summary>
        /// Subscriber topic for external power meter detail used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicExternalPowerMeterDetail { get; set; }


        /// <summary>
        /// Subscriber topic for auxiliary power meter core used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicAuxiliaryPowerMeterCore { get; set; }


        /// <summary>
        /// Subscriber topic for auxiliary power meter detail used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicAuxiliaryPowerMeterDetail { get; set; }


        /// <summary>
        /// Subscriber topic for circuit power meter core used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicCircuitPowerMeterCore { get; set; }


        /// <summary>
        /// Subscriber topic for circuit power meter detail used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicCircuitPowerMeterDetail { get; set; }


        /// <summary>
        /// Subscriber topic for system power meter core used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicSystemPowerMeterCore { get; set; }


        /// <summary>
        /// Subscriber topic for system power meter detail used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicSystemPowerMeterDetail { get; set; }


        /// <summary>
        /// Publisher topic for current system setpoints used for the message publisher.
        /// </summary>
        public string? PublisherTopicCurrentSetpoint { get; set; }


        /// <summary>
        /// Publisher topic for current DER unit setpoints used for the message publisher.
        /// </summary>
        public string? PublisherTopicCurrentDerUnitSetpoint { get; set; }


        /// <summary>
        /// Publisher topic for the heartbeat of this service used for the message publisher.
        /// </summary>
        public string? PublisherTopicHeartbeat { get; set; }


        /// <summary>
        /// Subscriber topic for the power control heartbeat used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicPowerControlHeartbeat { get; set; }


        /// <summary>
        /// Startup delay so that the operating mode is properly initialized.
        /// </summary>
        public int StartupDelay { get; set; } = 5000;
    }
}

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
        /// If this value is less than 100 milliseconds it will cause an exception.
        /// </summary>
        private const long MinimumMetricsIntervalMilliseconds = 200;


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
        public double NameplateMaximumActivePowerWatt
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// Systems minimum active power nameplate in Watt.
        /// </summary>
        /// <remarks>
        public double NameplateMinimumActivePowerWatt
        {
            get { return field; }
            set { field = value; }
        }


        /// <summary>
        /// Systems maximum reactive power nameplate in vars.
        /// </summary>
        /// <remarks>
        public double NameplateMaximumReactivePowerVars
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfNegative(field); field = value; }
        }


        /// <summary>
        /// Systems minimum reactive power nameplate in vars.
        /// </summary>
        /// <remarks>
        public double NameplateMinimumReactivePowerVars
        {
            get { return field; }
            set { field = value; }
        }

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
        /// Startup delay so that the operating mode is properly initialized.
        /// </summary>
        public int StartupDelay { get; set; } = 5000;
    }
}

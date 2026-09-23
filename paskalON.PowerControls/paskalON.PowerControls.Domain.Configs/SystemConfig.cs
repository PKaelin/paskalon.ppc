// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;
using paskalON.Domains.Configs;

namespace paskalON.PowerControls.Domain.Configs
{
    public class SystemConfig : DomainBase
    {
        /// <summary>
        /// Indicates the minimum valid metrics publishing interval value.
        /// If this value is less than 100 milliseconds it will cause an exception.
        /// </summary>
        private const long MinimumMetricsIntervalMilliseconds = 200;


        /// <summary>
        /// Power control type.
        /// </summary>
        /// <remarks>
        /// Though this is a flag this power control system should be configured to only serve one type.
        /// </remarks>
        public required PowerControlType Type
        {
            get;
            set
            {
                int v = (int)value;
                if (Enum.IsDefined(typeof(PowerControlType), value) == false) throw new ArgumentException("Only one type per power control system is allowed.");
                field = value;
            }
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
        /// Startup delay so that the power control is properly initialized.
        /// </summary>
        public int StartupDelayForDevices { get; set; } = 5000;
    }
}

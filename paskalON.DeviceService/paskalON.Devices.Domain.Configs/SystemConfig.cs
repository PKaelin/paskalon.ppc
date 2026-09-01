// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.Devices.Domain.Configs
{
    /// <summary>
    /// Configuration class for the system.
    /// </summary>
    public class SystemConfig : DomainBase
    {
        /// <summary>
        /// Indicates the minimum valid polling interval value.
        /// If this value is less than 100 milliseconds it will cause an exception.
        /// </summary>
        private const long MinimumDataLoggingIntervalMilliseconds = 100;


        /// <summary>
        /// Metrics publishing interval in milliseconds.
        /// </summary>
        public int MetricsIntervalMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumDataLoggingIntervalMilliseconds); field = value; }
        } = 1000;


        /// <summary>
        /// Device publishing interval in milliseconds.
        /// </summary>
        public int DeviceIntervalMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumDataLoggingIntervalMilliseconds); field = value; }
        } = 1000;


        /// <summary>
        /// Device publishing factor for core data.
        /// </summary>
        /// <remarks>
        /// The factor is multiplied by the DeviceIntervalMilliseconds.
        /// </remarks>
        /// <example>
        /// DeviceIntervalMilliseconds = 1000, DeviceFactorClassCore = 1 means every 1 second core data get published.
        /// </example>
        public int DeviceFactorCore { get; set; } = 1;


        /// <summary>
        /// Device publishing factor for detail data.
        /// </summary>
        /// <remarks>
        /// The factor is multiplied by the DeviceIntervalMilliseconds.
        /// </remarks>
        /// <example>
        /// DeviceIntervalMilliseconds = 1000, DeviceFactorClassDetail = 5 means every 5 second detail data get published.
        /// </example>
        public int DeviceFactorDetail { get; set; } = 5;


        /// <summary>
        /// Publisher topic for power conversion system core used for the message publisher.
        /// </summary>
        public string? PublisherTopicPcsCore { get; set; }


        /// <summary>
        /// Publisher topic for power conversion system detail used for the message publisher.
        /// </summary>
        public string? PublisherTopicPcsDetail { get; set; }


        /// <summary>
        /// Publisher topic for battery bank core used for the message publisher.
        /// </summary>
        public string? PublisherTopicBatteryBankCore { get; set; }


        /// <summary>
        /// Publisher topic for battery bank detail used for the message publisher.
        /// </summary>
        public string? PublisherTopicBatteryBankDetail { get; set; }


        /// <summary>
        /// Publisher topic for solar panel core used for the message publisher.
        /// </summary>
        public string? PublisherTopicSolarPanelCore { get; set; }


        /// <summary>
        /// Publisher topic for solar panel detail used for the message publisher.
        /// </summary>
        public string? PublisherTopicSolarPanelDetail { get; set; }


        /// <summary>
        /// Publisher topic for external power meter core used for the message publisher.
        /// </summary>
        public string? PublisherTopicExternalPowerMeterCore { get; set; }


        /// <summary>
        /// Publisher topic for external power meter detail used for the message publisher.
        /// </summary>
        public string? PublisherTopicExternalPowerMeterDetail { get; set; }


        /// <summary>
        /// Publisher topic for auxiliary power meter core used for the message publisher.
        /// </summary>
        public string? PublisherTopicAuxiliaryPowerMeterCore { get; set; }


        /// <summary>
        /// Publisher topic for auxiliary power meter detail used for the message publisher.
        /// </summary>
        public string? PublisherTopicAuxiliaryPowerMeterDetail { get; set; }


        /// <summary>
        /// Publisher topic for circuit power meter core used for the message publisher.
        /// </summary>
        public string? PublisherTopicCircuitPowerMeterCore { get; set; }


        /// <summary>
        /// Publisher topic for circuit power meter detail used for the message publisher.
        /// </summary>
        public string? PublisherTopicCircuitPowerMeterDetail { get; set; }


        /// <summary>
        /// Publisher topic for system power meter core used for the message publisher.
        /// </summary>
        public string? PublisherTopicSystemPowerMeterCore { get; set; }


        /// <summary>
        /// Publisher topic for system power meter detail used for the message publisher.
        /// </summary>
        public string? PublisherTopicSystemPowerMeterDetail { get; set; }


        /// <summary>
        /// Startup delay so that the devices have some time to connect.
        /// </summary>
        public int StartupDelayForDevices { get; set; } = 5000;
    }
}

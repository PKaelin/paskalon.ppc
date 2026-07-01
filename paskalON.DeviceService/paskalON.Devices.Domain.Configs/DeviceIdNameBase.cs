namespace paskalON.Devices.Domain.Configs
{
    /// <summary>
    /// Base class for all classes that require a device ID and name.
    /// </summary>
    public abstract class DeviceIdNameBase : NameBase
    {
        /// <summary>
        /// Indicates the minimum valid polling interval value.
        /// If this value is less than 100 milliseconds it will cause an exception.
        /// </summary>
        private const long MinimumDataLoggingIntervalMilliseconds = 100;


        /// <summary>
        /// Id of the device.
        /// </summary>
        /// <remarks>
        /// Each device subclass should have its own unique DeviceID.
        /// </remarks>
        public abstract required int DeviceId { get; set; }


        /// <summary>
        /// Whether this device is active meaning whether it should be loaded into configuration.
        /// </summary>
        public required bool IsActive { get; set; }


        private long metricsIntervalMilliseconds = 1000;
        /// <summary>
        /// Metrics publishing interval in milliseconds.
        /// </summary>
        public long MetricsIntervalMilliseconds
        {
            get { return metricsIntervalMilliseconds; }
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumDataLoggingIntervalMilliseconds); metricsIntervalMilliseconds = value; }
        }

        /// <summary>
        /// Metrics publishing factor for class 1 metrics.
        /// </summary>
        /// <remarks>
        /// The factor is multiplied by the MetricsIntervalMilliseconds.
        /// </remarks>
        /// <example>
        /// MetricsIntervalMilliseconds = 1000, MetricsFactorClass1 = 1 means every 1 second class 1 metrics get published.
        /// </example>
        public int MetricsFactorClass1 { get; set; } = 1;


        /// <summary>
        /// Metrics publishing factor for class 2 metrics.
        /// </summary>
        /// <remarks>
        /// The factor is multiplied by the MetricsIntervalMilliseconds.
        /// </remarks>
        /// <example>
        /// MetricsIntervalMilliseconds = 1000, MetricsFactorClass2 = 5 means every 5 seconds class 2 metrics get published.
        /// </example>
        public int MetricsFactorClass2 { get; set; } = 5;


        /// <summary>
        /// Metrics publishing factor for class 3 metrics.
        /// </summary>
        /// <remarks>
        /// The factor is multiplied by the MetricsIntervalMilliseconds.
        /// </remarks>
        /// <example>
        /// MetricsIntervalMilliseconds = 1000, MetricsFactorClass3 = 30 means every 30 seconds class 3 metrics get published.
        /// </example>
        public int MetricsFactorClass3 { get; set; } = 30;


        /// <summary>
        /// Metrics publishing factor for class 4 metrics.
        /// </summary>
        /// <remarks>
        /// The factor is multiplied by the MetricsIntervalMilliseconds.
        /// </remarks>
        /// <example>
        /// MetricsIntervalMilliseconds = 1000, MetricsFactorClass4 = 300 means every 5 minutes class 4 metrics get published.
        /// </example>
        public int MetricsFactorClass4 { get; set; } = 300;
    }
}

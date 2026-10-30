// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Luna.Domain.Configs
{
    /// <summary>
    /// Configuration class for the system.
    /// </summary>
    public class SystemConfig
    {
        /// <summary>
        /// Indicates the minimum time, in milliseconds, after which a dependency is lost.
        /// If this value is less than this constant it will cause an exception.
        /// </summary>
        /// <remarks>
        /// Must be longer than the primary secondary failover (500 ms), so that a failover does not cause a loss.
        /// </remarks>
        private const long MinimumDependencyLossTimeoutMilliseconds = 1000;


        /// <summary>
        /// Subscriber topic for current system setpoints used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicCurrentSetpoint { get; set; }


        /// <summary>
        /// Subscriber topic for DER unit setpoints used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicCurrentDerUnitSetpoint { get; set; }


        /// <summary>
        /// Subscriber topic for power control targets used for the message subscriber.
        /// </summary>
        public string? SubscriberTopicPowerTarget { get; set; }


        /// <summary>
        /// Subscriber topic for the power control heartbeat used to monitor the power control service.
        /// </summary>
        public string? SubscriberTopicPowerControlHeartbeat { get; set; }


        /// <summary>
        /// Subscriber topic for the operating mode heartbeat used to monitor the operating mode service.
        /// </summary>
        public string? SubscriberTopicOperatingModeHeartbeat { get; set; }


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
        /// Name of the shutdown override operating mode configured in the operating mode service.
        /// </summary>
        public string? ShutdownOperatingModeName { get; set; }


        /// <summary>
        /// Gets or sets the system nameplate rating for maximum active power.
        /// </summary>
        public double MaximumActivePowerSystemNameplateWatts { get; set; }


        /// <summary>
        /// Gets or sets the minimum active power at the system nameplate rating.
        /// </summary>
        public double MinimumActivePowerSystemNameplateWatts { get; set; }


        /// <summary>
        /// Gets or sets the maximum reactive power specified on the system nameplate.
        /// </summary>
        public double MaximumReactivePowerSystemNameplateVars { get; set; }


        /// <summary>
        /// Gets or sets the minimum reactive power at the system nameplate rating.
        /// </summary>
        public double MinimumReactivePowerSystemNameplateVars { get; set; }


        /// <summary>
        /// The time, in milliseconds, the message broker (Redis) must be disconnected after which it is lost.
        /// </summary>
        /// <remarks>
        /// Shown in the UI as not available while it is lost.
        /// </remarks>
        public int RedisLossTimeoutMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumDependencyLossTimeoutMilliseconds); field = value; }
        } = 1500;


        /// <summary>
        /// The time, in milliseconds, without device data after which the device service is lost.
        /// </summary>
        /// <remarks>
        /// Must be a multiple of the device publishing interval of the device service.
        /// </remarks>
        public int DeviceServiceLossTimeoutMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumDependencyLossTimeoutMilliseconds); field = value; }
        } = 3000;


        /// <summary>
        /// The time, in milliseconds, without a power control heartbeat after which the power control is lost.
        /// </summary>
        /// <remarks>
        /// Must be a multiple of the power control heartbeat interval.
        /// </remarks>
        public int PowerControlLossTimeoutMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumDependencyLossTimeoutMilliseconds); field = value; }
        } = 3000;


        /// <summary>
        /// The time, in milliseconds, without an operating mode heartbeat after which the operating mode is lost.
        /// </summary>
        /// <remarks>
        /// Must be a multiple of the operating mode heartbeat interval.
        /// </remarks>
        public int OperatingModeLossTimeoutMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumDependencyLossTimeoutMilliseconds); field = value; }
        } = 3000;
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.Alarms.Domain.Configs
{
    /// <summary>
    /// Configuration class for the system.
    /// </summary>
    public class SystemConfig : DomainBase
    {
        /// <summary>
        /// Indicates the minimum interval, in milliseconds, between two sends of the pending alarm actions.
        /// If this value is less than this constant it will cause an exception.
        /// </summary>
        private const int MinimumAlarmActionSendIntervalMilliseconds = 100;


        /// <summary>
        /// Indicates the minimum time, in milliseconds, before a failed send of the alarm actions is retried.
        /// If this value is less than this constant it will cause an exception.
        /// </summary>
        private const int MinimumAlarmActionRetryIntervalMilliseconds = 1000;


        /// <summary>
        /// Indicates the minimum interval, in milliseconds, between two checks of the delayed alarm raises and normalizations.
        /// If this value is less than this constant it will cause an exception.
        /// </summary>
        private const int MinimumAlarmDelayCheckIntervalMilliseconds = 100;


        /// <summary>
        /// Indicates the minimum interval, in milliseconds, between two heartbeats.
        /// If this value is less than this constant it will cause an exception.
        /// </summary>
        private const int MinimumDependencyHeartbeatIntervalMilliseconds = 100;


        /// <summary>
        /// The interval, in milliseconds, between two sends of the pending alarm actions in the outbox.
        /// </summary>
        public int AlarmActionSendIntervalMilliseconds
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumAlarmActionSendIntervalMilliseconds);
                field = value;
            }
        } = 1000;


        /// <summary>
        /// The time, in milliseconds, before a failed send of the alarm actions is retried.
        /// </summary>
        /// <remarks>
        /// A send fails e.g. when the message broker is not reachable or no recipient queue is bound for the action.
        /// </remarks>
        public int AlarmActionRetryIntervalMilliseconds
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumAlarmActionRetryIntervalMilliseconds);
                field = value;
            }
        } = 5000;


        /// <summary>
        /// The interval, in milliseconds, between two checks of the delayed alarm raises and normalizations
        /// (TimeBeforeRaisingAlarmMilliseconds and TimeBeforeNormalizingAlarmMilliseconds of the alarm configurations).
        /// </summary>
        /// <remarks>
        /// A delayed alarm is raised or normalized at most this interval after its delay elapsed.
        /// </remarks>
        public int AlarmDelayCheckIntervalMilliseconds
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumAlarmDelayCheckIntervalMilliseconds);
                field = value;
            }
        } = 500;


        /// <summary>
        /// The interval, in milliseconds, between two heartbeats of this service.
        /// </summary>
        /// <remarks>
        /// Every instance of the alarm service publishes its heartbeat, the dependency is available while at least one instance is alive.
        /// </remarks>
        public int DependencyHeartbeatIntervalMilliseconds
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumDependencyHeartbeatIntervalMilliseconds);
                field = value;
            }
        } = 500;


        /// <summary>
        /// Publisher topic for the heartbeat of this service used for the message publisher, e.g. ppc:alarms:heartbeat.
        /// </summary>
        /// <remarks>
        /// Null means the heartbeat is not published and the alarm service is reported as lost by its dependents.
        /// </remarks>
        public string? PublisherTopicHeartbeat { get; set; }


        /// <summary>
        /// Publisher exchange for the alarm actions used for the RabbitMQ publisher, e.g. ppc:alarms:actions.
        /// </summary>
        /// <remarks>
        /// The exchange is a topic exchange and the routing key is the action name (e.g. ReducePower),
        /// recipients bind their queues to this exchange with the actions they execute.
        /// </remarks>
        public string? PublisherExchangeAlarmActions { get; set; }


        /// <summary>
        /// Subscriber topic for power conversion system detail used for the message subscriber, e.g. ppc:pcs:detail.
        /// </summary>
        /// <remarks>
        /// The fault states, warning states and vendor events of the detail data raise the PCS alarms.
        /// Null means the PCS detail data is not evaluated, the same applies to all subscriber topics.
        /// </remarks>
        public string? SubscriberTopicPcsDetail { get; set; }


        /// <summary>
        /// Subscriber topic for power conversion system core used for the message subscriber, e.g. ppc:pcs:core.
        /// </summary>
        public string? SubscriberTopicPcsCore { get; set; }


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
        /// Subscriber topic for the power targets of the power conversion systems published by the power control, e.g. ppc:pc:targets.
        /// </summary>
        public string? SubscriberTopicPowerTarget { get; set; }


        /// <summary>
        /// Subscriber topic for the current system setpoint published by the operating mode.
        /// </summary>
        public string? SubscriberTopicCurrentSetpoint { get; set; }
    }
}

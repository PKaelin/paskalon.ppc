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
        /// Null means the PCS alarms are not evaluated.
        /// </remarks>
        public string? SubscriberTopicPcsDetail { get; set; }
    }
}

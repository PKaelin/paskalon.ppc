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
        /// Publisher exchange for the alarm actions used for the RabbitMQ publisher, e.g. ppc:alarms:actions.
        /// </summary>
        /// <remarks>
        /// The exchange is a topic exchange and the routing key is the action name (e.g. ReducePower),
        /// recipients bind their queues to this exchange with the actions they execute.
        /// </remarks>
        public string? PublisherExchangeAlarmActions { get; set; }
    }
}

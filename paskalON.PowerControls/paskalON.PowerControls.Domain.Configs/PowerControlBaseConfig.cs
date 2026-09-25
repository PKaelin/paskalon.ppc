// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.ConstraintEngine.Domain.Configs;
using paskalON.Domains;

namespace paskalON.PowerControls.Domain.Configs
{
    public abstract class PowerControlBaseConfig : NameBase
    {
        /// <summary>
        /// Is active means it is available for selection.
        /// </summary>
        /// <remarks>
        /// Not active means it is configured but can not be used.
        /// Consider RBAC for this.
        /// </remarks>
        public required bool IsActive { get; set; }


        /// <summary>
        /// Is enabled means the constraint is active and will be applied.
        /// </summary>
        public required bool IsEnabled { get; set; }


        /// <summary>
        /// Collection of constraints.
        /// </summary>
        public ICollection<ConstraintBaseConfig> Constraints { get; set; } = [];


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

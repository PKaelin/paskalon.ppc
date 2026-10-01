// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;
using paskalON.Domains.Configs;
using paskalON.OperatingModes.Domain.Configs.Curves;
using paskalON.OperatingModes.Domain.Configs.Ramps;

namespace paskalON.OperatingModes.Domain.Configs
{
    /// <summary>
    /// Base class for all operating mode configurations.
    /// </summary>
    /// <remarks>
    /// Some operating modes just need basic ramp and curve configuration.
    /// Inherit from this base class for specific configurations like:
    /// class FrequencyWattCurveModeConfig : OperatingModeConfig
    /// </remarks>
    public abstract class OperatingModeBaseConfig : NameBase
    {
        /// <summary>
        /// Indicates the minimum valid polling interval value.
        /// If this value is less than 100 milliseconds it will cause an exception.
        /// </summary>
        private const long MinimumDataLoggingIntervalMilliseconds = 100;


        /// <summary>
        /// Is active means it is available for selection.
        /// </summary>
        /// <remarks>
        /// Not active means it is configured but can not be used.
        /// Consider RBAC for this.
        /// </remarks>
        public required bool IsActive { get; set; }


        /// <summary>
        /// Operating mode type as a flag representation.
        /// </summary>
        /// <remarks>
        /// As they are flags they can be used like Bess|Solar to define that they can be
        /// used for both BESS and Solar systems.
        /// </remarks>
        public required PowerControlType Type { get; set; }


        /// <summary>
        /// Timeout period (in seconds) between enabling the operating mode and the automatic disablement of the mode.
        /// </summary>
        /// <remarks>
        /// Value of 0 means it will never be automatically disabled.
        /// </remarks>
        public int TimeoutSeconds
        {
            get { return field; }
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, 0); field = value; }
        }


        /// <summary>
        /// Ramp configuration Id of this operating mode.
        /// </summary>
        public int RampConfigId { get; set; }


        /// <summary>
        /// Ramp configuration of this operating mode.
        /// </summary>
        public required RampBaseConfig RampConfig { get; set; }


        /// <summary>
        /// Curve configuration Id of this operating mode.
        /// </summary>
        public virtual int? CurveConfigId { get; set; }


        /// <summary>
        /// Curve configuration of this operating mode.
        /// </summary>
        public virtual CurveBaseConfig? CurveConfig { get; set; }


        /// <summary>
        /// Deadband in threshold used to filter minor setpoint noise signals.
        /// </summary>
        /// <remarks>
        /// This value is either in Watt or Vars.
        /// </remarks>
        public double DeadbandSetpoint
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, 0); field = value; }
        } = 100000;


        /// <summary>
        /// Deadband in threshold used to filter minor available noise signals.
        /// </summary>
        /// <remarks>
        /// This value is either in Watt or Vars.
        /// </remarks>
        public double DeadbandAvailable
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, 0); field = value; }
        } = 100000;


        /// <summary>
        /// Configurable maximum active power limit in watt.
        /// </summary>
        /// <remarks>
        /// This value should not exceed the nameplate.
        /// If this value is not set the systems nameplate for active power is used.
        /// </remarks>
        public double? MaximumActivePowerLimitWatt
        {
            get;
            set
            {
                if (value != null && MinimumActivePowerLimitWatt.HasValue && MinimumActivePowerLimitWatt.Value > value)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MaximumActivePowerLimitWatt)} has to be bigger than {nameof(MinimumActivePowerLimitWatt)}");
                }

                field = value;
            }
        }


        /// <summary>
        /// Configurable minimum active power limit in watt.
        /// </summary>
        /// <remarks>
        /// This value should not exceed the nameplate if it is negative.
        /// If this value is not set the systems nameplate for active power is used.
        /// </remarks>
        public double? MinimumActivePowerLimitWatt
        {
            get;
            set
            {
                if (value != null && MaximumActivePowerLimitWatt.HasValue && MaximumActivePowerLimitWatt.Value < value)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MinimumActivePowerLimitWatt)} has to be smaller than {nameof(MaximumActivePowerLimitWatt)}");
                }

                field = value;
            }
        }

        /// <summary>
        /// Configurable maximum reactive power limit in vars.
        /// </summary>
        /// <remarks>
        /// This value should not exceed the nameplate.
        /// If this value is not set the systems nameplate for reactive power is used.
        /// </remarks>
        public double? MaximumReactivePowerLimitVars
        {
            get;
            set
            {
                if (value != null && MinimumReactivePowerLimitVars.HasValue && MinimumReactivePowerLimitVars.Value > value)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MaximumReactivePowerLimitVars)} has to be bigger than {nameof(MinimumReactivePowerLimitVars)}");
                }

                field = value;
            }
        }


        /// <summary>
        /// Configurable minimum reactive power limit in vars.
        /// </summary>
        /// <remarks>
        /// This value should not exceed the nameplate.
        /// If this value is not set the systems nameplate for reactive power is used.
        /// </remarks>
        public double? MinimumReactivePowerLimitVars
        {
            get;
            set
            {
                if (value != null && MaximumReactivePowerLimitVars.HasValue && MaximumReactivePowerLimitVars.Value < value)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MinimumReactivePowerLimitVars)} has to be smaller than {nameof(MaximumReactivePowerLimitVars)}");
                }

                field = value;
            }
        }


        /// <summary>
        /// Metrics publishing interval in milliseconds.
        /// </summary>
        public long MetricsIntervalMilliseconds
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, MinimumDataLoggingIntervalMilliseconds); field = value; }
        } = 1000;


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

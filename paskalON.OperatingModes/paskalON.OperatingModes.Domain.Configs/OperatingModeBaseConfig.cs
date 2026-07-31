// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
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
        public required OperatingModeType Type { get; set; }


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
        /// Ramp configuration of this operating mode.
        /// </summary>
        public required RampBaseConfig RampConfig { get; set; }


        /// <summary>
        /// Curve configuration of this operating mode.
        /// </summary>
        public virtual CurveBaseConfig? CurveConfig { get; set; }


        /// <summary>
        /// Deadband in kilo threshold used to filter minor setpoint noise signals.
        /// </summary>
        public double DeadbandSetpointKilo
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, 0); field = value; }
        } = 100;


        /// <summary>
        /// Deadband in kilo threshold used to filter minor available noise signals.
        /// </summary>
        /// <remarks>
        public double DeadbandAvailableKilo
        {
            get;
            set { ArgumentOutOfRangeException.ThrowIfLessThan(value, 0); field = value; }
        } = 100;


        /// <summary>
        /// Configurable maximum active power limit in kilo watt.
        /// </summary>
        /// <remarks>
        /// This value should not exceed the nameplate.
        /// If this value is not set the systems nameplate for active power is used.
        /// </remarks>
        public double? MaximumActivePowerLimitKiloWatt
        {
            get;
            set
            {
                if (value != null && MinimumActivePowerLimitKiloWatt.HasValue && MinimumActivePowerLimitKiloWatt.Value > value)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MaximumActivePowerLimitKiloWatt)} has to be bigger than {nameof(MinimumActivePowerLimitKiloWatt)}");
                }

                field = value;
            }
        }


        /// <summary>
        /// Configurable minimum active power limit in kilo watt.
        /// </summary>
        /// <remarks>
        /// This value should not exceed the nameplate if it is negative.
        /// If this value is not set the systems nameplate for active power is used.
        /// </remarks>
        public double? MinimumActivePowerLimitKiloWatt
        {
            get;
            set
            {
                if (value != null && MaximumActivePowerLimitKiloWatt.HasValue && MaximumActivePowerLimitKiloWatt.Value < value)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MinimumActivePowerLimitKiloWatt)} has to be smaller than {nameof(MaximumActivePowerLimitKiloWatt)}");
                }

                field = value;
            }
        }

        /// <summary>
        /// Configurable maximum reactive power limit in kilo vars.
        /// </summary>
        /// <remarks>
        /// This value should not exceed the nameplate.
        /// If this value is not set the systems nameplate for reactive power is used.
        /// </remarks>
        public double? MaximumReactivePowerLimitKiloVars
        {
            get;
            set
            {
                if (value != null && MinimumReactivePowerLimitKiloVars.HasValue && MinimumReactivePowerLimitKiloVars.Value > value)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MaximumReactivePowerLimitKiloVars)} has to be bigger than {nameof(MinimumReactivePowerLimitKiloVars)}");
                }

                field = value;
            }
        }


        /// <summary>
        /// Configurable minimum reactive power limit in kilo vars.
        /// </summary>
        /// <remarks>
        /// This value should not exceed the nameplate.
        /// If this value is not set the systems nameplate for reactive power is used.
        /// </remarks>
        public double? MinimumReactivePowerLimitKiloVars
        {
            get;
            set
            {
                if (value != null && MaximumReactivePowerLimitKiloVars.HasValue && MaximumReactivePowerLimitKiloVars.Value < value)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MinimumReactivePowerLimitKiloVars)} has to be smaller than {nameof(MaximumReactivePowerLimitKiloVars)}");
                }

                field = value;
            }
        }


        /// <summary>
        /// Class name of the specialized operating mode to be initialized.
        /// </summary>
        /// <remarks>
        /// If class name is empty the default operating mode instance is created.
        /// </remarks>
        public string? ClassName { get; set; }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.Modes.ComplexPower
{
    /// <summary>
    /// Maintenance mode configuration.
    /// </summary>
    /// <remarks>
    /// This is the open loop configuration.
    /// There is a closed loop for SOC maintenance.
    /// </remarks>
    public class MaintenanceModeConfig : OperatingModeBaseConfig
    {
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
        /// If this value is not set the value 0 for active power is used.
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
        /// If this value is not set the systems nameplate for active power is used.
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
        /// If this value is not set the systems nameplate for active power is used.
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
    }
}
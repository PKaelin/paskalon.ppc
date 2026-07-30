// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.OpenModes.VoltageReactives
{
    /// <summary>
    /// Reactive power fixed mode configuration.
    /// </summary>
    public class ReactivePowerFixedModeConfig : OperatingModeBaseConfig
    {
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

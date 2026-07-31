// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.OperatingModes.Domain.Configs.OpenModes.EnergyResources
{
    /// <summary>
    /// Configuration class for maximum power point tracking node.
    /// </summary>
    public class MaximumPowerPointTrackingModeConfig : OperatingModeBaseConfig
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
        } = 0;
    }
}

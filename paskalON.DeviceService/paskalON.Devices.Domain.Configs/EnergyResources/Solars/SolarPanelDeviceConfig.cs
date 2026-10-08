// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.Devices.Domain.Configs.EnergyResources.Solars
{
    /// <summary>
    /// Single solar panel configuration
    /// </summary>
    public class SolarPanelDeviceConfig : NameBase
    {
        /// <summary>
        /// The class name of the type to instantiate.
        /// This uniquely identifies the eventually used type of component. (e.g. a ManufacturerPcs, ManufacturerBattery etc.)
        /// </summary>
        public required string ClassName { get; set; }


        /// <summary>
        /// Minimum output voltage of the panel in volts.
        /// </summary>
        /// <remarks>
        /// Simple check but implies that initialised maximum is already bigger than minimum. If not, an exception is thrown.
        /// </remarks>
        public double MinimumVoltage
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);

                if (value > MaximumVoltage)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "MinimumVoltage cannot be greater than MaximumVoltage.");
                }

                field = value;
            }
        }


        /// <summary>
        /// Maximum output voltage of the panel in volts.
        /// </summary>
        /// <remarks>
        /// Simple check but implies that initialised minimum is already smaller than maximum. If not, an exception is thrown.
        /// </remarks>
        public double MaximumVoltage
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);

                if (value < MinimumVoltage)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "MaximumVoltage cannot be less than MinimumVoltage.");
                }

                field = value;
            }
        }


        /// <summary>
        /// Minimum output current of the panel in ampere.
        /// </summary>
        /// <remarks>
        /// Simple check but implies that initialised maximum is already bigger than minimum. If not, an exception is thrown.
        /// </remarks>
        public double MinimumCurrent
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);

                if (value > MaximumCurrent)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "MinimumCurrent cannot be greater than MaximumCurrent.");
                }

                field = value;
            }
        }


        /// <summary>
        /// Maximum output current of the panel in ampere.
        /// </summary>
        /// <remarks>
        /// Simple check but implies that initialised minimum is already smaller than maximum. If not, an exception is thrown.
        /// </remarks>
        public double MaximumCurrent
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);

                if (value < MinimumCurrent)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "MaximumCurrent cannot be less than MinimumCurrent.");
                }

                field = value;
            }
        }
    }
}

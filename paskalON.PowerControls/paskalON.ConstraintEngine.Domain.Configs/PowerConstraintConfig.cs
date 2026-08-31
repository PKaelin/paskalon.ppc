// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.ConstraintEngine.Domain.Configs
{
    /// <summary>
    /// Configuration for power constraints.
    /// </summary>
    public class PowerConstraintConfig : ConstraintBaseConfig
    {
        /// <summary>
        /// Maximum active power allowed by the constraint.
        /// </summary>
        /// <remarks>
        /// This value can be the systems or units nameplate or less.
        /// </remarks>
        public double? MaximumActivePowerKiloWatt
        {
            get;
            set
            {
                if (value != null && MinimumActivePowerKiloWatt.HasValue && MinimumActivePowerKiloWatt.Value > value)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MaximumActivePowerKiloWatt)} has to be bigger than {nameof(MinimumActivePowerKiloWatt)}");
                }

                field = value;
            }
        }


        /// <summary>
        /// Minimum active power allowed by the constraint.
        /// </summary>
        /// <remarks>
        /// This value can be the systems or units nameplate or less.
        /// </remarks>
        public double? MinimumActivePowerKiloWatt
        {
            get;
            set
            {
                if (value != null && MaximumActivePowerKiloWatt.HasValue && MaximumActivePowerKiloWatt.Value < value)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MaximumActivePowerKiloWatt)} has to be smaller than {nameof(MaximumActivePowerKiloWatt)}");
                }

                field = value;
            }
        }


        /// <summary>
        /// Maximum reactive power allowed by the constraint.
        /// </summary>
        /// <remarks>
        /// This value can be the systems or units nameplate or less.
        /// </remarks>
        public double? MaximumReactivePowerKiloVars
        {
            get;
            set
            {
                if (value != null && MinimumReactivePowerKiloVars.HasValue && MinimumReactivePowerKiloVars.Value > value)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MaximumReactivePowerKiloVars)} has to be bigger than {nameof(MinimumReactivePowerKiloVars)}");
                }

                field = value;
            }
        }


        /// <summary>
        /// Minimum reactive power allowed by the constraint.
        /// </summary>
        /// <remarks>
        /// This value can be the systems or units nameplate or less.
        /// </remarks>
        public double? MinimumReactivePowerKiloVars
        {
            get;
            set
            {
                if (value != null && MaximumReactivePowerKiloVars.HasValue && MaximumReactivePowerKiloVars.Value < value)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MinimumReactivePowerKiloVars)} has to be smaller than {nameof(MaximumReactivePowerKiloVars)}");
                }

                field = value;
            }
        }

    }
}

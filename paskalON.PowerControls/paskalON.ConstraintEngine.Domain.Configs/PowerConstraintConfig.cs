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
        public double? MaximumActivePowerWatt
        {
            get;
            set
            {
                if (value != null && MinimumActivePowerWatt.HasValue && MinimumActivePowerWatt.Value > value)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MaximumActivePowerWatt)} has to be bigger than {nameof(MinimumActivePowerWatt)}");
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
        public double? MinimumActivePowerWatt
        {
            get;
            set
            {
                if (value != null && MaximumActivePowerWatt.HasValue && MaximumActivePowerWatt.Value < value)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MaximumActivePowerWatt)} has to be smaller than {nameof(MaximumActivePowerWatt)}");
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
        public double? MaximumReactivePowerVars
        {
            get;
            set
            {
                if (value != null && MinimumReactivePowerVars.HasValue && MinimumReactivePowerVars.Value > value)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MaximumReactivePowerVars)} has to be bigger than {nameof(MinimumReactivePowerVars)}");
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
        public double? MinimumReactivePowerVars
        {
            get;
            set
            {
                if (value != null && MaximumReactivePowerVars.HasValue && MaximumReactivePowerVars.Value < value)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(MinimumReactivePowerVars)} has to be smaller than {nameof(MaximumReactivePowerVars)}");
                }

                field = value;
            }
        }

    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.ConstraintEngine.Domain.Configs;
using paskalON.PowerControls.Domain.Configs.Strategies;

namespace paskalON.PowerControls.Domain.Configs.Ders
{
    public class DerUnitPowerControlConfig : PowerControlBaseConfig
    {
        /// <summary>
        /// Corresponding DER unit name for which this power constraint is defined.
        /// </summary>
        public required string DerUnitName { get; set; }


        /// <summary>
        /// Distribution strategy type.
        /// </summary>
        public required DistributionStrategyType DistributionStrategyType { get; set; }


        /// <summary>
        /// If priority distribution strategy is used then this priority is used.
        /// </summary>
        public ushort? Priority
        {
            get;
            set
            {
                if (value != null)
                {
                    ArgumentOutOfRangeException.ThrowIfNegative((ushort)value);
                }

                field = value;
            }
        }


        /// <summary>
        /// If weighted distribution strategy is used then this weight is used.
        /// </summary>
        public double? Weight
        {
            get;
            set
            {
                if (value != null)
                {
                    ArgumentOutOfRangeException.ThrowIfNegative((double)value);
                }

                field = value;
            }
        }


        /// <summary>
        /// Collection of constraints.
        /// </summary>
        public ICollection<ConstraintBaseConfig> Constraints { get; set; } = [];
    }
}

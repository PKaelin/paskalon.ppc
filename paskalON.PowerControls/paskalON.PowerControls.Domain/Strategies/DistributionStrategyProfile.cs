// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.PowerControls.Domain.Strategies
{
    /// <summary>
    /// Distribution strategy profile.
    /// </summary>
    public class DistributionStrategyProfile
    {
        /// <summary>
        /// Interface for priority distribution.
        /// </summary>
        public IDistributionStrategy? PriorityDistribution { get; init; }


        /// <summary>
        /// Interface for equal distribution.
        /// </summary>
        public IDistributionStrategy? EqualDistribution { get; init; }


        /// <summary>
        /// Interface for weight distribution.
        /// </summary>
        public IDistributionStrategy? WeightedDistribution { get; init; }

        /// <summary>
        /// Interface for proportional distribution.
        /// </summary>
        public IDistributionStrategy? ProportionalDistribution { get; init; }


        /// <summary>
        /// Interface for water filling distribution.
        /// </summary>
        public IDistributionStrategy? WaterFillingDistribution { get; init; }
    }
}

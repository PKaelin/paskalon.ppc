// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
namespace paskalON.PowerControls.Domain.Configs.Strategies
{
    /// <summary>
    /// Distribution strategy type.
    /// </summary>
    public enum DistributionStrategyType
    {
        /// <summary>
        /// Distributes to the highest priority first.
        /// </summary>
        Priority = 0,
        /// <summary>
        /// Distributes all equal parts.
        /// </summary>
        Equal = 1,
        /// <summary>
        /// Distributes all to a weight.
        /// </summary>
        Weight = 1,
        /// <summary>
        /// Distributes using a proportions or limits.
        /// </summary>
        Proportional = 2,
        /// <summary>
        /// Distributes lowest first then uniformly across.
        /// </summary>
        WaterFilling = 3,
    }
}

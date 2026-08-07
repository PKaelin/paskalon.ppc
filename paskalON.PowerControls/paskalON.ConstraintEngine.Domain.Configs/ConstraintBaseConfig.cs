// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.ConstraintEngine.Domain.Configs
{
    /// <summary>
    /// Base class for all constraint configurations.
    /// </summary>
    public abstract class ConstraintBaseConfig : NameBase
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
        /// Is enabled means the constraint is active and will be applied.
        /// </summary>
        public required bool IsEnabled { get; set; }

    }
}

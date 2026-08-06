// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using paskalON.Domains;
using paskalON.PhysicalUnits.Electricals.Powers;

namespace paskalON.PowerControls.Domain.Configs.Constraints
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
        /// Power control type as a flag representation.
        /// </summary>
        /// <remarks>
        /// As they are flags they can be used like Bess|Solar to define that they can be
        /// used for both BESS and Solar systems.
        /// </remarks>
        public required PowerFactorType Type { get; set; }


        /// <summary>
        /// Is enabled means the constraint is active and will be applied.
        /// </summary>
        public required bool IsEnabled { get; set; }

    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.PowerControls.Domain.Configs
{
    public abstract class PowerControlBaseConfig : NameBase
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

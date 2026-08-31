// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.PowerControls.Domain.Configs
{
    public class SystemConfig : DomainBase
    {
        /// <summary>
        /// Power control type.
        /// </summary>
        /// <remarks>
        /// Though this is a flag this power control system should be configured to only serve one type.
        /// </remarks>
        public required PowerControlType Type
        {
            get;
            set
            {
                int v = (int)value;
                if (Enum.IsDefined(typeof(PowerControlType), value) == false) throw new ArgumentException("Only one type per power control system is allowed.");
                field = value;
            }
        }
    }
}

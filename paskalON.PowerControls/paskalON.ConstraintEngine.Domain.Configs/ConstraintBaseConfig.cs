// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using paskalON.Domains;

namespace paskalON.ConstraintEngine.Domain.Configs
{
    /// <summary>
    /// Base class for all constraint configurations.
    /// </summary>
    /// <remarks>
    /// The constraint can be configured just once and applied to many power controllers.
    /// </remarks>
    public abstract class ConstraintBaseConfig : NameBase
    {
    }
}

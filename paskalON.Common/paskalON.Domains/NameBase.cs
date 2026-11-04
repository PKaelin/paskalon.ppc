// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Domains
{
    /// <summary>
    /// Base class for all named domain bases.
    /// </summary>
    public abstract class NameBase : DomainBase
    {
        /// <summary>
        /// Name of the domain configuration.
        /// </summary>
        public required virtual string Name { get; set; }

    }
}

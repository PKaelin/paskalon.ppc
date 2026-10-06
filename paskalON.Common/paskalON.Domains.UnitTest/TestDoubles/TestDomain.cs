// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
namespace paskalON.Domains.UnitTest.TestDoubles
{
    /// <summary>
    /// Concrete domain used to exercise the behavior of the abstract <see cref="DomainBase"/>.
    /// </summary>
    internal sealed class TestDomain : DomainBase
    {
        /// <summary>
        /// Additional payload of the concrete domain.
        /// </summary>
        public string Payload { get; set; } = string.Empty;
    }
}

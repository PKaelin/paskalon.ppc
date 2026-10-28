// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using System.Data.Common;

namespace paskalON.Hosting.UnitTest.TestDoubles
{
    /// <summary>
    /// Database exception test double with a configurable transient flag, as raised by database providers like Npgsql.
    /// </summary>
    internal sealed class TestDbException : DbException
    {
        /// <summary>
        /// Constructor of <see cref="TestDbException"/>.
        /// </summary>
        /// <param name="isTransient">Whether the exception is transient.</param>
        public TestDbException(bool isTransient)
            : base("Failed to connect to paskalon.powercontrols.data:5432")
        {
            IsTransient = isTransient;
        }


        /// <inheritdoc/>
        public override bool IsTransient { get; }
    }
}

// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;

namespace paskalON.Infrastructure.PostgreSql.IntegrationTest.Tests.SampleData
{
    /// <summary>
    /// A sample database context with a model but without any migrations.
    /// </summary>
    public class UnmigratedContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnmigratedContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public UnmigratedContext(DbContextOptions<UnmigratedContext> options) : base(options)
        {
        }


        /// <summary>
        /// Gets the sample devices.
        /// </summary>
        public DbSet<SampleDevice> SampleDevices => Set<SampleDevice>();
    }
}

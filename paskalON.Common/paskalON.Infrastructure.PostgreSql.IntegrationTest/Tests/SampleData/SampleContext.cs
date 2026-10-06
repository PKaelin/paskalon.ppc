// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;

namespace paskalON.Infrastructure.PostgreSql.IntegrationTest.Tests.SampleData
{
    /// <summary>
    /// A sample database context with migrations used to verify the PostgreSQL test infrastructure.
    /// </summary>
    public class SampleContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public SampleContext(DbContextOptions<SampleContext> options) : base(options)
        {
        }


        /// <summary>
        /// Gets the sample devices.
        /// </summary>
        public DbSet<SampleDevice> SampleDevices => Set<SampleDevice>();


        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SampleDevice>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).IsRequired().HasMaxLength(SampleDevice.NameMaxLength);
                entity.HasIndex(x => x.Name).IsUnique();
            });
        }
    }
}

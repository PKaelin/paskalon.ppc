// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace paskalON.Domains.UnitTest.TestDoubles
{
    /// <summary>
    /// In-memory model host that wires up the domain configurations the same way the microservice contexts do.
    /// </summary>
    /// <remarks>
    /// Only the EF Core model is built. No database connection is ever opened, the connection string is a placeholder.
    /// </remarks>
    internal sealed class TestDomainContext : DbContext
    {
        /// <summary>
        /// Concrete configurations of the test model.
        /// </summary>
        public DbSet<TestConfiguration> Configurations { get; set; } = null!;


        /// <summary>
        /// Concrete named domains of the test model.
        /// </summary>
        public DbSet<TestNamedDomain> NamedDomains { get; set; } = null!;


        /// <summary>
        /// Concrete plain domains of the test model.
        /// </summary>
        public DbSet<TestDomain> Domains { get; set; } = null!;


        /// <summary>
        /// Migration history of the test model.
        /// </summary>
        public DbSet<History> Histories { get; set; } = null!;


        /// <summary>
        /// Gets the full design-time model including all relational annotations (e.g. collations).
        /// </summary>
        /// <returns>The design-time model.</returns>
        public IModel GetDesignTimeModel()
        {
            return this.GetService<IDesignTimeModel>().Model;
        }


        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=domains_unit_test");
        }


        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCollation("case_insensitive_collation", locale: "und-u-ks-level2", provider: "icu", deterministic: false);
            modelBuilder.Entity<History>().ToTable(t => t.ExcludeFromMigrations(true));
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DomainBase).Assembly);
        }
    }
}

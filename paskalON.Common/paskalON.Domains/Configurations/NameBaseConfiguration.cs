// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace paskalON.Domains.Configurations
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class NameBaseConfiguration : IEntityTypeConfiguration<NameBase>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<NameBase> builder)
        {
            // Tell EF Core to push all properties down to concrete tables.
            builder.UseTpcMappingStrategy();

            builder.Property(x => x.Name)
                .HasMaxLength(250)
                .IsRequired();
            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}

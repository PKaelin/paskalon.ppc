// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Alarms.Domain.Configs.Localizations;

namespace paskalON.Alarms.Infrastructure.Storage.Configurations
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class LocalizedTextConfiguration : IEntityTypeConfiguration<LocalizedText>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<LocalizedText> builder)
        {
            // The key has the length of the localizable alarm configuration fields minus the braces
            builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
            builder.Property(x => x.Language).HasMaxLength(10).IsRequired();
            builder.Property(x => x.Text).HasMaxLength(2000).IsRequired();

            // The key is unique per language
            builder.HasIndex(x => new { x.Name, x.Language }).IsUnique();
        }
    }
}

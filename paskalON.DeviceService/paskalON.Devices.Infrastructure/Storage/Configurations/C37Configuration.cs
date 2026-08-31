// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs;

namespace paskalON.Devices.Infrastructure.Storage.Configurations
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class C37Configuration : IEntityTypeConfiguration<C37Config>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<C37Config> builder)
        {
            builder.Property(x => x.Address).IsRequired();
            builder.Property(x => x.Port).IsRequired();
            builder.Property(x => x.TransportLayer).IsRequired();
            builder.Property(x => x.StreamId).IsRequired();
            builder.Property(x => x.StationName).IsRequired();
            // Depends on the encoding but lets assume 1 byte per character
            builder.Property(x => x.StationName).HasMaxLength(16);
        }
    }
}

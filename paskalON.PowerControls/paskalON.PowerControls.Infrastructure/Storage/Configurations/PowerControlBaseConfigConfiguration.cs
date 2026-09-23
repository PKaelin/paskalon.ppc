// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.PowerControls.Domain.Configs;

namespace paskalON.PowerControls.Infrastructure.Storage.Configurations
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class PowerControlBaseConfigConfiguration : IEntityTypeConfiguration<PowerControlBaseConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<PowerControlBaseConfig> builder)
        {
            builder.HasMany(x => x.Constraints)
                .WithOne()
                .HasForeignKey("PowerControlBaseConfigId");
        }
    }
}

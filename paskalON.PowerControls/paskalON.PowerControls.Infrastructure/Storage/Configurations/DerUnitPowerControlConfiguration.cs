// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.PowerControls.Domain.Configs.Ders;

namespace paskalON.PowerControls.Infrastructure.Storage.Configurations
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class DerUnitPowerControlConfiguration : IEntityTypeConfiguration<DerUnitPowerControlConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<DerUnitPowerControlConfig> builder)
        {
            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.IsEnabled).IsRequired();
            builder.Property(x => x.DerUnitName).HasMaxLength(250).IsRequired();
            builder.Property(x => x.DistributionStrategyType).IsRequired();
        }
    }
}

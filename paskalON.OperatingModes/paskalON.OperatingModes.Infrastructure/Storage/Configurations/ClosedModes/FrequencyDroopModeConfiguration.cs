// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.FrequencyActives;

namespace paskalON.OperatingModes.Infrastructure.Storage.Configurations.ClosedModes
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class FrequencyDroopModeConfiguration : IEntityTypeConfiguration<FrequencyDroopModeConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<FrequencyDroopModeConfig> builder)
        {
            builder.HasOne(x => x.CurveConfig)
                .WithMany()
                .HasForeignKey(x => x.CurveConfigId)
                .IsRequired();
        }
    }
}

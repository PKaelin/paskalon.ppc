// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.OperatingModes.Domain.Configs.ClosedModes.VoltageActives;

namespace paskalON.OperatingModes.Infrastructure.Storage.Configurations.ClosedModes
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class VoltageWattDroopModeConfiguration : IEntityTypeConfiguration<VoltageWattDroopModeConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<VoltageWattDroopModeConfig> builder)
        {
            builder.HasOne(x => x.CurveConfig)
                .WithMany()
                .HasForeignKey(x => x.CurveConfigId)
                .IsRequired();
        }
    }
}

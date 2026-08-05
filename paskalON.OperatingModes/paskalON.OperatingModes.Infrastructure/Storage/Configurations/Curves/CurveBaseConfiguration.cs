// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// SPDX-License-Identifier: Apache-2.0
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.OperatingModes.Domain.Configs.Curves;

namespace paskalON.OperatingModes.Infrastructure.Storage.Configurations.Curves
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class CurveBaseConfiguration : IEntityTypeConfiguration<CurveBaseConfig>
    {
        /// <summary>
        /// Allows configuration for an entity type to be factored into a separate class.
        /// </summary>
        public void Configure(EntityTypeBuilder<CurveBaseConfig> builder)
        {
            builder.HasMany(x => x.Points)
                .WithOne(x => x.CurveBaseConfig)
                .HasForeignKey(x => x.CurveBaseConfigId)
                .IsRequired();
        }
    }
}

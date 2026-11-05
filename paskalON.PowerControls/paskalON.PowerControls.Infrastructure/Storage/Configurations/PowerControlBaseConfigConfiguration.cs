// Copyright 2026 Pascal Kaelin (Operating as paskalON)
// Licensed under the paskalON Source-Available License (PSAL).
// See LICENSE for the full license terms.
//----------------------------------------‐------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.ConstraintEngine.Domain.Configs;
using paskalON.PowerControls.Domain.Configs;

namespace paskalON.PowerControls.Infrastructure.Storage.Configurations
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class PowerControlBaseConfigConfiguration : IEntityTypeConfiguration<PowerControlBaseConfig>
    {
        /// <summary>
        /// Name of the join table between power controls and their constraints.
        /// </summary>
        public const string JoinTableName = "PowerControlConstraint";


        /// <summary>
        /// Name of the join table column referencing the power control.
        /// </summary>
        public const string PowerControlForeignKey = "PowerControlBaseConfigId";


        /// <summary>
        /// Name of the join table column referencing the constraint.
        /// </summary>
        public const string ConstraintForeignKey = "ConstraintBaseConfigId";


        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<PowerControlBaseConfig> builder)
        {
            // Many-to-many: a constraint is defined once and can be shared by several power controls
            builder.HasMany(x => x.Constraints)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    JoinTableName,
                    r => r.HasOne<ConstraintBaseConfig>().WithMany().HasForeignKey(ConstraintForeignKey),
                    l => l.HasOne<PowerControlBaseConfig>().WithMany().HasForeignKey(PowerControlForeignKey),
                    j => j.HasKey(PowerControlForeignKey, ConstraintForeignKey));
        }
    }
}

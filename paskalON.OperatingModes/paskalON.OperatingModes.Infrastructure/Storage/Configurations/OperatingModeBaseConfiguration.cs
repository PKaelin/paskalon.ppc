using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.OperatingModes.Domain.Configs;

namespace paskalON.OperatingModes.Infrastructure.Storage.Configurations
{
    /// <summary>
    /// Allows configuration for an entity type to be factored into a separate class.
    /// </summary>
    public class OperatingModeBaseConfiguration : IEntityTypeConfiguration<OperatingModeBaseConfig>
    {
        /// <summary>
        /// Configures the entity of type TEntity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<OperatingModeBaseConfig> builder)
        {
            builder.HasOne(x => x.RampConfig)
                .WithMany()
                .HasForeignKey(x => x.RampConfigId)
                .IsRequired();

            builder.HasOne(x => x.CurveConfig)
                .WithMany()
                .HasForeignKey(x => x.CurveConfigId)
                .IsRequired(false);

            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.Type).IsRequired();
        }
    }
}

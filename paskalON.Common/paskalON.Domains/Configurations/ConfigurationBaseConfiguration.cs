using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace paskalON.Domains.Configurations
{
    public class ConfigurationBaseConfiguration : IEntityTypeConfiguration<ConfigurationBase>
    {
        public void Configure(EntityTypeBuilder<ConfigurationBase> builder)
        {
            // Tell EF Core to push all properties down to concrete tables
            builder.UseTpcMappingStrategy();

            builder.Property(x => x.Key)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(x => x.Value)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(800)
                .IsRequired();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs;

namespace paskalON.Devices.Infrastructure.Storage.Configurations
{
    public class NameBaseConfiguration : IEntityTypeConfiguration<NameBase>
    {
        public void Configure(EntityTypeBuilder<NameBase> builder)
        {
            // Tell EF Core to push all properties down to concrete tables.
            builder.UseTpcMappingStrategy();

            builder.Property(x => x.Name)
                .HasMaxLength(250)
                .IsRequired();
            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}

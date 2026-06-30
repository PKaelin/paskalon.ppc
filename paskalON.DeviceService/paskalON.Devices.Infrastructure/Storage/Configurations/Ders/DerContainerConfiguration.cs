using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.Ders;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.Ders
{
    public class DerContainerConfiguration : IEntityTypeConfiguration<DerContainerConfig>
    {
        public void Configure(EntityTypeBuilder<DerContainerConfig> builder)
        {
            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.DeviceId).IsRequired();
            builder.HasIndex(x => x.DeviceId).IsUnique();
        }
    }
}

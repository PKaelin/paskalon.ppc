using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.PowerConversionSystems
{
    public class PowerConversionSystemConfiguration : IEntityTypeConfiguration<PowerConversionSystemConfig>
    {
        public void Configure(EntityTypeBuilder<PowerConversionSystemConfig> builder)
        {
            builder.HasOne(x => x.PowerConversionSystemDeviceConfig)
                .WithMany()
                .HasForeignKey(x => x.PowerConversionSystemDeviceConfigId)
                .IsRequired();

            // TODO: Implement clean relationship
            builder.HasOne(x => x.ModbusConfig)
                .WithMany()
                .HasForeignKey(x => x.ModbusConfigId)
                .IsRequired();

            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.DeviceId).IsRequired();
            builder.HasIndex(x => x.DeviceId).IsUnique();
        }
    }
}

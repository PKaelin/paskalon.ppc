using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.Meters
{
    public class PowerMeterBaseConfiguration : IEntityTypeConfiguration<PowerMeterBaseConfig>
    {
        public void Configure(EntityTypeBuilder<PowerMeterBaseConfig> builder)
        {
            builder.HasOne(x => x.PowerMeterDeviceConfig)
                .WithMany()
                .HasForeignKey(x => x.PowerMeterDeviceConfigId)
                .IsRequired();

            builder.HasOne(x => x.ModbusConfig)
                .WithMany()
                .HasForeignKey(x => x.ModbusConfigId)
                .IsRequired(false);

            builder.HasOne(x => x.C37Config)
                .WithMany()
                .HasForeignKey(x => x.C37ConfigId)
                .IsRequired(false);

            builder.Property(x => x.PowerFactorStandard).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.DeviceId).IsRequired();
            builder.HasIndex(x => x.DeviceId).IsUnique();
        }
    }
}

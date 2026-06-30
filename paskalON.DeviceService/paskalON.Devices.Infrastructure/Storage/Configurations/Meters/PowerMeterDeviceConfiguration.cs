using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.Meters.PowerMeters;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.Meters
{
    public class PowerMeterDeviceConfiguration : IEntityTypeConfiguration<PowerMeterDeviceConfig>
    {
        public void Configure(EntityTypeBuilder<PowerMeterDeviceConfig> builder)
        {
            builder.HasOne(x => x.PowerMeterMapC37Config)
                .WithMany()
                .HasForeignKey(x => x.PowerMeterMapC37ConfigId)
                .IsRequired(false);

            builder.HasOne(x => x.PowerMeterMapModbusConfig)
                .WithMany()
                .HasForeignKey(x => x.PowerMeterMapModbusConfigId)
                .IsRequired(false);

            builder.Property(x => x.ClassName).IsRequired();
        }
    }
}

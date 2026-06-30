using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.EnergyResources.Solars
{
    public class SolarPanelConfiguration : IEntityTypeConfiguration<SolarPanelConfig>
    {
        public void Configure(EntityTypeBuilder<SolarPanelConfig> builder)
        {
            builder.HasOne(x => x.SolarPanelDeviceConfig)
                .WithMany()
                .HasForeignKey(x => x.SolarPanelDeviceConfigId)
                .IsRequired();

            builder.Property(x => x.IsActive).IsRequired();
            builder.Property(x => x.DeviceId).IsRequired();
            builder.HasIndex(x => x.DeviceId).IsUnique();
        }
    }
}

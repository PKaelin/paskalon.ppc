using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.EnergyResources.Solars;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.EnergyResources.Solars
{
    public class SolarPanelDeviceConfiguration : IEntityTypeConfiguration<SolarPanelDeviceConfig>
    {
        public void Configure(EntityTypeBuilder<SolarPanelDeviceConfig> builder)
        {
            builder.Property(x => x.ClassName).IsRequired();
        }
    }
}

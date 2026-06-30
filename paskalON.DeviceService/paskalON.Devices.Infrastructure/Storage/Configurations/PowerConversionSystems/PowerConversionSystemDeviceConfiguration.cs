using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using paskalON.Devices.Domain.Configs.PowerConversionSystems;

namespace paskalON.Devices.Infrastructure.Storage.Configurations.PowerConversionSystems
{
    public class PowerConversionSystemDeviceConfiguration : IEntityTypeConfiguration<PowerConversionSystemDeviceConfig>
    {
        public void Configure(EntityTypeBuilder<PowerConversionSystemDeviceConfig> builder)
        {
            builder.Property(x => x.ClassName).IsRequired();
            builder.Property(x => x.NameplateMaximumActivePower).IsRequired();
            builder.Property(x => x.NameplateMaximumReactivePower).IsRequired();
            builder.Property(x => x.NameplateMaximumApparentPower).IsRequired();
        }
    }
}
